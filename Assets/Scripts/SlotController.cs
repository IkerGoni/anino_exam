using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D;
using UnityEngine;

namespace AninoExam
{
    public enum GameState
    {
        None,
        Loading,
        Setup,
        Ready,
        GettingResult,
        StartSpin,
        Spinning,
        StoppingSpin,
        WaitingForSpinsToStop,
        SpinFinished,
        SpinFeedback,
    }

    public class SlotSetupData
    {
        public int[][] Paylines;
        public int MaxPaylines;
        public int[][] ReelSymbolIDs;
    }

    public class SlotController : Singleton<SlotController>
    {
        private GameState currentGameState = GameState.Loading;

        
        //References to the reels of the machine
        [SerializeField] private ReelController[] _reels;
        [HideInInspector] public ReelController[] Reels => _reels;

        //Slot game vars
        private int _betStep = 1;
        private int _selectedBet = 1;
        public int SelectedBet => _selectedBet;
        
        private int _maxPaylines = 20;
        public int MaxPaylines
        {
            set => _maxPaylines = value;
        }

        
        
        private int _selectedPaylines; //even not in use, would be nice if user could reduce the paylines in use (I know that is unusual on social casino though)
        
        
        private int _currentPrize = 0;
        public int CurrentPrize => _currentPrize;

        
        private int _spinningSlots;
        //RESULTS

        private SymbolData[][] _currentReelResult = null;
        private int[][] _paylines;


        public void ChangeGameState(GameState newGameState)
        {
            currentGameState = newGameState;
        }

        public void SetupSlotMachine(SlotSetupData setupData)
        {
            _paylines = setupData.Paylines;
            _maxPaylines = setupData.MaxPaylines;
            _selectedPaylines = _maxPaylines;
            
            for (int i = 0; i < setupData.ReelSymbolIDs.GetLength(0); i++)
            {
                _reels[i].Setup(setupData.ReelSymbolIDs[i]);
            }

            AddEventListeners();
            
            UI_Controller.Instance.HideBlackPanel();
            ChangeGameState(GameState.GettingResult);
        }

        private void AddEventListeners()
        {
            for (int i = 0; i < _reels.Length; i++)
            {
                _reels[i].OnReelStopped += ProcessReelStopped;
            }
        }

        private void RemoveEventListeners()
        {
            for (int i = 0; i < _reels.Length; i++)
            {
                _reels[i].OnReelStopped -= ProcessReelStopped;
            }
        }

        private void ProcessReelStopped()
        {
            _spinningSlots--;
            if (_spinningSlots == 0)
            {
                ChangeGameState(GameState.SpinFinished);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (currentGameState == GameState.Loading)
            {
                return;
            }
            
            if (currentGameState == GameState.GettingResult)
            {

                GetSpinResult();
            
                ChangeGameState(GameState.Ready);
            }
            else if (currentGameState == GameState.StartSpin)
            {
                
                UI_Controller.Instance.UpdateUIStartpin();
                
                for (int i = 0; i < _reels.Length; i++)
                {
                    _reels[i].StartSpinning();
                }

                _spinningSlots = _reels.Length;
                
                AudioManager.Instance.PlayAudio(Audio.SpinStart);
                ChangeGameState(GameState.Spinning);
            }      
            else if (currentGameState == GameState.StoppingSpin)
            {

                for (int i = 0; i < _reels.Length; i++)
                {
                    _reels[i].DisplayResults();
                }
                ChangeGameState(GameState.WaitingForSpinsToStop);

            }                    
            else if (currentGameState == GameState.SpinFinished)
            {   
                if(_currentPrize>0)
                    DataManager.Instance.User.AddChips(_currentPrize*_selectedBet);
                
                UI_Controller.Instance.UpdateUIPostSpin();
            }
        }


        public void SpinButtonClicked()
        {
            if (currentGameState == GameState.Ready)
            {
                if (DataManager.Instance.User.Chips >= _selectedBet * _selectedPaylines)
                {
                    DataManager.Instance.User.SubstractChips( _selectedBet * _selectedPaylines);
                    ChangeGameState(GameState.StartSpin);
                }
                else
                {
                    AudioManager.Instance.PlayAudio(Audio.Error);
                   //Chance to launch IAP shop
                }
            }
            else if (currentGameState == GameState.Spinning)
            {
                 ChangeGameState(GameState.StoppingSpin);
            } 
        }
        
        /// <summary>
        /// This method should get called as soon as the spin starts, and call apply result in a successful callback.
        /// For the exercise is requested to prepare the result before spinning.
        /// </summary>
        private void GetSpinResult()
        {
            ApplySpinResults(DataManager.Instance.GetSpinResult());
            _currentPrize = GetSelectedPaylinesPrize();
        }

        /// <summary>
        /// This method would get called on a callback of getSpinResults to the backend
        /// </summary>
        void ApplySpinResults(string[][] spinResult)
        {
            ResetCurrentResult();
            SymbolData[]
                reelResult =
                    new SymbolData[_reels[0]
                        .ReelSize]; //This is out from the next loop bc we know that all reels have same number of symbols. In other shaped slots, should be inside the loop

            for (int i = 0; i < _reels.Length; i++)
            {
                for (int j = _reels[i].ReelSize - 1; j >= 0; j--)
                {
                    reelResult[j] = DataManager.Instance.NameToSymbolData[spinResult[j][i]];
                    _currentReelResult[i][j] = reelResult[j];
                }

                _reels[i].SetResult(reelResult);
            }
        }


        
        void ResetCurrentResult()
        {
            _currentReelResult = new SymbolData[_reels.Length][];
            for (int i = 0; i < _currentReelResult.Length; i++)
            {
                _currentReelResult[i] = new SymbolData[_reels[i].ReelSize];
            }
        }
        
        /// <summary>
        /// Evaluates paylines based on selected paylines
        /// </summary>
        /// <returns>Returns the global payout prize</returns>
        private int GetSelectedPaylinesPrize()
        {
            int currentPrize = 0;
            for (int i = 0; i < _selectedPaylines; i++)
            {
                int currentPayLinePrice = GetPaylinePrize(_paylines[i]);
                if (currentPayLinePrice > 0)
                {      
                    currentPrize += currentPayLinePrice*_selectedBet;
                }
            }

            return currentPrize;
        }

        
        /// <summary>
        /// Calculates and returns paylines total payout
        /// </summary>
        /// <param name="payline"></param>
        /// <returns>payline total payout</returns>
        private int GetPaylinePrize(int[] payline)
        {
            
            //Creates a dict of symbol appareances on payline
            
            Dictionary<int, int> symbolAppareances = new Dictionary<int, int>();
            
            for (int i = 0; i < payline.Length; i++)
            {
                if (symbolAppareances.ContainsKey(_currentReelResult[i][payline[i]].Id))
                {
                    symbolAppareances[_currentReelResult[i][payline[i]].Id]++;
                }
                else
                {
                    symbolAppareances.Add(_currentReelResult[i][payline[i]].Id, 0);
                }
            }

            int paylinePrize = 0;
            
            // Calculates the total payout based on symbols apparances
            
            foreach (KeyValuePair<int, int> entry in symbolAppareances)
            {
                paylinePrize += DataManager.Instance.IdToSymbolData[entry.Key].Payout[entry.Value];
            }
            
            return paylinePrize;
        }


        #region chips and bet value

        public void IncreaseBetStep()
        {
            _selectedBet += _betStep;
        }
        
        public void DecreaseBetStep()
        {
            _selectedBet -= _betStep;
        }

    #endregion
    }
}
