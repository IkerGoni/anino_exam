using System.Collections;
using System.Collections.Generic;
using AninoExam;
using UnityEngine;


/// <summary>
/// DataManager in a real Slot project would be the class responsible of communication between the client and the Backend server
/// Would take care, f.e. of:
/// - Get/Updare user data
/// - Check if spin is doable(enoughChips) and get the spin result
/// - ...
///
/// For this test, I am going to have the data on client
/// </summary>



public class DataManager : Singleton<DataManager>
{

   // public static DataManager Instance;

    
    // Start is called before the first frame update
   /* private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }*/


   // string[][] spinResult = {new string[] {"A", "A", "A", "B", "C"},new string[] {"B", "A", "C", "F", "G"}, new string[]{"A", "G", "B", "D", "E"}};
    string[][] spinResult = {new string[] {"C", "H", "A", "B", "C"},new string[] {"B", "C", "C", "C", "G"}, new string[]{"A", "G", "B", "D", "C"}};


    
   
    
    

    public string[][] GetSpinResult()
    {
        return GenerateResult();
    }

    private static int[][] paylines =
    {
        new int[] {1, 1, 1, 1, 1},
        new int[] {0, 0, 0, 0, 0},
        new int[] {2, 2, 2, 2, 2},
        new int[] {0, 1, 2, 1, 0},
        new int[] {2, 1, 0, 1, 2},
        new int[] {0, 0, 1, 2, 2},
        new int[] {2, 2, 1, 0, 0},
        new int[] {1, 0, 1, 2, 1},
        new int[] {1, 2, 1, 0, 1},
        new int[] {1, 0, 0, 1, 0},
        new int[] {1, 2, 2, 1, 2},
        new int[] {0, 1, 0, 0, 1},
        new int[] {2, 1, 2, 2, 1},
        new int[] {0, 2, 0, 2, 0},
        new int[] {2, 0, 2, 0, 2},
        new int[] {1, 0, 2, 0, 1},
        new int[] {1, 2, 0, 2, 1},
        new int[] {0, 1, 1, 1, 0},
        new int[] {2, 1, 1, 1, 2},
        new int[] {0, 2, 2, 2, 0}
    };

    public int[][] GetSlotPaylines()
    {
        return paylines;
    }


    #region BackendLogic

    /// <summary>
    /// Creates a random result
    /// </summary>
    /// <returns></returns>
    string[][] GenerateResult()
    {
        spinResult  = new string[3][];
        for (int i = 0; i < spinResult.GetLength(0); i++)
        {
            spinResult[i] = new string[5];
            for (int j = 0; j < 5; j++)
            {
                SymbolSO randomSymbol = SlotController.Instance.Reels[i]
                    .ReelSymbols[Random.Range(0, SlotController.Instance.Reels[i].ReelSymbols.Length)];

                spinResult[i][j] = randomSymbol.Name;

            }
        }

        return spinResult;
    }

    #endregion

   
}
