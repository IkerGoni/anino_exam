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
        Start,
        Ready,
        GettingResult,
        StartSpin,
        Spinning,
        StoppingSpin,
        SpinFinished,
    }

    public class SlotSetupData
    {
        public int[][] Paylines;
        public SymbolData[] AllSymbols;
        public int MaxPaylines;
        public int[][] ReelSymbolIDs;
    }

    public class SlotController : Singleton<SlotController>
    {
        private GameState currentGameState = GameState.Loading;

        
        //References to the reels of the machine
        [SerializeField] private ReelController[] _reels;
        [HideInInspector] public ReelController[] Reels => _reels;

        //Control vars - > move to state machine
        [SerializeField] private bool _startSpinning = false;
        [SerializeField] private bool _stopSpinning = false;
        //  [SerializeField] private bool _getResult = false;

        [Header("ConfigVars Settings")] [SerializeField]
        private float _spinSpeed = 5f;

        //Slot game vars
        private int _betStep = 1;
        private int _selectedBet = 1;
        public int SelectedBet => _selectedBet;
        
        private int _maxPaylines = 20;
        public int MaxPaylines
        {
            get => _maxPaylines;
            set => _maxPaylines = value;
        }

        
        
        private int _selectedPaylines;
        
        
        private int _currentPrize = 0;
        public int CurrentPrize => _currentPrize;

        
        private int _spinningSlots;
        //RESULTS

        private SymbolData[][] currentReelResult = null;
        private int[][] _paylines;


        void Start()
        {
           /* BuildLookupDictionary();
            SetupSlotMachine();*/
        }

       

        public void ChangeGameState(GameState newGameState)
        {
            currentGameState = newGameState;
        }

        public void SetupSlotMachine(SlotSetupData setupData)
        {

            _paylines = setupData.Paylines;
            _paylines = setupData.Paylines;
            _selectedPaylines = _maxPaylines;
            
            for (int i = 0; i < setupData.ReelSymbolIDs.GetLength(0); i++)
            {
                _reels[i].Setup(setupData.ReelSymbolIDs[i]);
            }

            AddEventListeners();
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
                //wait for preparing data
                return;
            }
            else if (currentGameState == GameState.GettingResult)
            {
                UI_Controller.Instance.UpdateUIPreSpin();

                GetSpinResult();
            
                ChangeGameState(GameState.Ready);
            }
            else if (currentGameState == GameState.Ready)
            {
                
            }
            else if (currentGameState == GameState.StartSpin)
            {
                
                UI_Controller.Instance.UpdateUIStartpin();
                
                for (int i = 0; i < _reels.Length; i++)
                {
                    _reels[i].StartSpinning();
                }

                _spinningSlots = _reels.Length;
                ChangeGameState(GameState.Spinning);
            }
            else if (currentGameState == GameState.Spinning)
            {
                //reels are spining
            }
            else if (currentGameState == GameState.StoppingSpin)
            {
                //reels[0].DisplayResults();

                for (int i = 0; i < _reels.Length; i++)
                {
                    _reels[i].DisplayResults();
                }
            }
            else if (currentGameState == GameState.SpinFinished)
            {
                if(_currentPrize>0)
                    DataManager.Instance.User.AddChips(_currentPrize*_selectedBet);
                
                UI_Controller.Instance.UpdateUIPostSpin();

                ChangeGameState(GameState.GettingResult);

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
                    Debug.Log("Not enough chips"); //Chance to launch IAP shop
                
                //TODO spinStartEvent
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
            EvaluatePaylines();
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
                    currentReelResult[i][j] = reelResult[j];
                }

                _reels[i].SetResult(reelResult);
            }
        }

        void ResetCurrentResult()
        {
            currentReelResult = new SymbolData[_reels.Length][];
            for (int i = 0; i < currentReelResult.Length; i++)
            {
                currentReelResult[i] = new SymbolData[_reels[i].ReelSize];
            }
        }

        private void EvaluatePaylines()
        {
            _currentPrize = 0;
            for (int i = 0; i < _maxPaylines; i++)
            {
                int currentPayLinePrice = GetPaylinePrize(_paylines[i]);
                if (currentPayLinePrice > 0)
                {
                    Debug.Log("payline "+_paylines[i][0].ToString()+", "
                              +_paylines[i][1].ToString()+", "
                              +_paylines[i][2].ToString()+", "
                              +_paylines[i][3].ToString()+", "
                              +_paylines[i][4].ToString()+", with price: " + currentPayLinePrice);
                    _currentPrize += currentPayLinePrice*_selectedBet;
                }
            }

            Debug.Log("TOTAL PRIZE: " + _currentPrize);
            Debug.Log("---------------------------------");
        }

        //first try was with SymbolSO as key, but the are not equal (obviusly), storing the id 
        private int GetPaylinePrize(int[] payline)
        {
            Dictionary<int, int> symbolAppareances = new Dictionary<int, int>();
            int totalPaylinePrize = 0;
            for (int i = 0; i < payline.Length; i++)
            {
                if (symbolAppareances.ContainsKey(currentReelResult[i][payline[i]].Id))
                {
                    symbolAppareances[currentReelResult[i][payline[i]].Id]++;
                }
                else
                {
                    symbolAppareances.Add(currentReelResult[i][payline[i]].Id, 0);
                }
            }

            int paylinePrize = GetSymbolOcurrencePrizes(symbolAppareances);

            if (paylinePrize > 0)
            {
                string paylineS = String.Empty;

                for (int i = 0; i < payline.Length; i++)
                {
                    paylineS += payline[i] + ", ";
                }
            }

            return paylinePrize;
        }

        private int GetSymbolOcurrencePrizes(Dictionary<int, int> symbolAppareances)
        {
            int linePrize = 0;

            foreach (KeyValuePair<int, int> entry in symbolAppareances)
            {
                linePrize += DataManager.Instance.IdToSymbolData[entry.Key].Payout[entry.Value];
            }

            return linePrize;
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
