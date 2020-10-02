using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

namespace AninoExam
{
    public class SlotController : Singleton<SlotController>
    {
        //public static SlotController Instance;

        private Dictionary<string, SymbolSO> nameToSymbol = new Dictionary<string, SymbolSO>();
        private Dictionary<int, SymbolSO> idToSymbol = new Dictionary<int, SymbolSO>();

        [SerializeField] private SymbolSO[] allSymbols = new SymbolSO[] { };

        [SerializeField] private ReelController[] _reels;
        [HideInInspector] public ReelController[] Reels => _reels;

        [SerializeField] private bool _startSpinning = false;

        [SerializeField] private bool _stopSpinning = false;
        //  [SerializeField] private bool _getResult = false;

        [Header("ConfigVars Settings")] [SerializeField]
        private float _spinSpeed = 5f;


        private int _betStep = 1;
        private int _currentBetValue = 1;

        //RESULTS

        private SymbolSO[][] currentReelResult = null;

        private int[][] _paylines;


        void Start()
        {
            BuildLookupDictionary();
            SetupSlotMachine();
        }

        private void BuildLookupDictionary()
        {
            for (int i = 0; i < allSymbols.Length; i++)
            {
                nameToSymbol.Add(allSymbols[i].Name, allSymbols[i]);
                idToSymbol.Add(allSymbols[i].Id, allSymbols[i]);
            }
        }

        void SetupSlotMachine()
        {
            for (int i = 0; i < _reels.Length; i++)
            {
                _reels[i].Speed = _spinSpeed;
            }

            _paylines = DataManager.Instance.GetSlotPaylines();
        }

        // Update is called once per frame
        void Update()
        {
            if (_startSpinning)
            {

                GetSpinResult();

                for (int i = 0; i < _reels.Length; i++)
                {
                    _reels[i].StartSpinning();
                }

                _startSpinning = false;
            }

            if (_stopSpinning)
            {
                //reels[0].DisplayResults();

                for (int i = 0; i < _reels.Length; i++)
                {
                    _reels[i].DisplayResults();
                }

                _stopSpinning = false;


            }

            /* if (_getResult)
             {
                 _getResult = false;
                 GetSpinResult();
             }*/

        }


        public void Spin()
        {
            _startSpinning = true;
        }
        
        /// <summary>
        /// This method should get called as soon as the spin starts, and call apply result in a successful callback.
        /// As we are not using any backend, we will call ApplyResults directly
        /// </summary>
        private void GetSpinResult()
        {
            ApplySpinResults(DataManager.Instance.GetSpinResult());
            EvaluatePaylines();
        }

        //  {{"A", "A", "A", "B", "C"}, {"B", "A", "C", "F", "G"}, {"A", "G", "B", "D", "E"}};
        /// <summary>
        /// This method would get called on a callback of getSpinResults to the backend
        /// </summary>
        void ApplySpinResults(string[][] spinResult)
        {
            ResetCurrentResult();
            SymbolSO[]
                reelResult =
                    new SymbolSO[_reels[0]
                        .ReelSize]; //This is out from the next loop bc we know that all reels have same number of symbols. In other shaped slots, should be inside the loop

            for (int i = 0; i < _reels.Length; i++)
            {
                for (int j = _reels[i].ReelSize - 1; j >= 0; j--)
                {
                    reelResult[j] = nameToSymbol[spinResult[j][i]];
                    currentReelResult[i][j] = reelResult[j];
                }

                _reels[i].SetResult(reelResult);
            }

            /*for (int i = 0; i < currentReelResult.Length; i++)
            {
                for (int j = 0; j < currentReelResult[i].Length; j++)
                {
                    Debug.Log(currentReelResult[i][j].Name);
                }
            }*/
        }

        void ResetCurrentResult()
        {
            currentReelResult = new SymbolSO[_reels.Length][];
            for (int i = 0; i < currentReelResult.Length; i++)
            {
                currentReelResult[i] = new SymbolSO[_reels[i].ReelSize];
            }
        }

        private void EvaluatePaylines()
        {
            int total = 0;
            for (int i = 0; i < _paylines.GetLength(0); i++)
            {
                int currentPayLinePrice = GetPaylinePrize(_paylines[i]);
                if (currentPayLinePrice > 0)
                    Debug.Log("payline with price: " + currentPayLinePrice);
                total += currentPayLinePrice;
            }

            Debug.Log($"Total: {total}");
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

                Debug.Log(paylineS);
            }

            return paylinePrize;
        }

        private int GetSymbolOcurrencePrizes(Dictionary<int, int> symbolAppareances)
        {
            int linePrize = 0;

            foreach (KeyValuePair<int, int> entry in symbolAppareances)
            {
                linePrize += idToSymbol[entry.Key].Payout[entry.Value];
            }

            return linePrize;
        }



        #region chips and bet value

        public void IncreaseBetStep()
        {
            _currentBetValue += _betStep;
        }
        
        public void DecreaseBetStep()
        {
            _currentBetValue += _betStep;
        }

    #endregion
    }
}
