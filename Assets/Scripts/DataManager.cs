using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public User User;
    [SerializeField] private SymbolData[] _allSymbols;

    
            
    private Dictionary<string, SymbolData> nameToSymbolData = new Dictionary<string, SymbolData>();

    public Dictionary<string, SymbolData> NameToSymbolData => nameToSymbolData;

    private Dictionary<int, SymbolData> idToSymbolData = new Dictionary<int, SymbolData>();

    public Dictionary<int, SymbolData> IdToSymbolData => idToSymbolData;

    public SymbolData[] AllSymbols
    {
        get => _allSymbols;
        set => _allSymbols = value;
    }

    [SerializeField] private int[][] _paylines;
    [SerializeField] private int _maxPaylines;  
    [SerializeField] private int[][] _reelsSymbolIDs;
    
    public int MaxPaylines
    {
        get { return _maxPaylines; }
        set { _maxPaylines = value; }
    }

    // public static DataManager Instance;

    // string[][] spinResult = {new string[] {"A", "A", "A", "B", "C"},new string[] {"B", "A", "C", "F", "G"}, new string[]{"A", "G", "B", "D", "E"}};
    //string[][] spinResult = {new string[] {"C", "H", "A", "B", "C"},new string[] {"B", "C", "C", "C", "G"}, new string[]{"A", "G", "B", "D", "C"}};

    
    
    /// <summary>
    /// User should be created from data loaded from the backend. TO simplify, datamanager will create a user on evey start;
    /// </summary>
    void Start()
    {
        User = new User();
    }

    private bool _dataLoaded = false;

    public bool DataLoaded
    {
        get { return _dataLoaded; }
        set
        {
            if (value)
            {
                BuildLookupDictionaries();
                InjectSlotData();
            }

            _dataLoaded = value;
        }
    }

    private void BuildLookupDictionaries()
    {
        for (int i = 0; i < _allSymbols.Length; i++)
        {
            nameToSymbolData.Add(_allSymbols[i].Name, _allSymbols[i]);
            idToSymbolData.Add(_allSymbols[i].Id, _allSymbols[i]);
        }
    }
    
    private void InjectSlotData()
    {
        SlotSetupData slotSetupData = new SlotSetupData();
        slotSetupData.Paylines = _paylines;
        slotSetupData.MaxPaylines = _maxPaylines;
        slotSetupData.ReelSymbolIDs = _reelsSymbolIDs;
        
        SlotController.Instance.SetupSlotMachine(slotSetupData);

    }

    public string[][] GetSpinResult()
    {
        return GenerateResult();
    }

    

    
    /*
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
    };*/

    /// <summary>
    /// gets a string json of AllPaylinesWrapper -> PaylinesWrapper[] Paylines -> int[] Payline and saves it into paylines variable
    /// </summary>
    /// <param name="data"></param>
    public void ParsePaylinesFromJSON(string data)
    {
        AllPaylinesWrapper wrapper = JsonUtility.FromJson<AllPaylinesWrapper>(data);
        _paylines = new int[wrapper.Paylines.Length][];
        for (int i = 0; i < wrapper.Paylines.Length; i++)
        {
            for (int j = 0; j < wrapper.Paylines[i].Payline.Length; j++)
            {
                _paylines[i] = new int[wrapper.Paylines[i].Payline.Length];

                _paylines[i][j] = wrapper.Paylines[i].Payline[j];
            }
        }
     
        Debug.Log("paylines parsed");   
    }

    public void ParseAllSymbolDataFromJSON(string data)
    {
        AllSymbolsWrapper wrapper = JsonUtility.FromJson<AllSymbolsWrapper>(data);

        _allSymbols = (SymbolData[]) wrapper.Symbols.ToArray().Clone();
        Debug.Log("allSymbolData parsed");   
        
    }
    
    public void ParseReelsSymbolsIDsFromJSON(string data)
    {
        AllReelsSymbolsWrapper wrapper = JsonUtility.FromJson<AllReelsSymbolsWrapper>(data);

        _reelsSymbolIDs = new int[wrapper.ReelsSymbolData.Length][]; //Only works with same length reels
        for (int i = 0; i < wrapper.ReelsSymbolData.Length; i++)
        {
            _reelsSymbolIDs[i] = new int[wrapper.ReelsSymbolData[i].SymbolDataIDs.Count];
            for (int j = 0; j < wrapper.ReelsSymbolData[i].SymbolDataIDs.Count; j++)
            {
                _reelsSymbolIDs[i][j] = wrapper.ReelsSymbolData[i].SymbolDataIDs[j];
            }
        }
        //_reelsSymbolIDs
        Debug.Log("reel symbol ids parsed");   
     
    }
    
    public int[][] GetSlotPaylines()
    {
        return _paylines;
    }


    #region BackendLogic

    /// <summary>
    /// Creates a random result
    /// </summary>
    /// <returns></returns>
    string[][] GenerateResult()
    {
        string[][] spinResult  = new string[3][];
        for (int i = 0; i < spinResult.GetLength(0); i++)
        {
            spinResult[i] = new string[5];
            for (int j = 0; j < 5; j++)
            {
                SymbolData randomSymbol = SlotController.Instance.Reels[i]
                    .ReelSymbols[Random.Range(0, SlotController.Instance.Reels[i].ReelSymbols.Length)];

                spinResult[i][j] = randomSymbol.Name;
            }
        }
        return spinResult;
    }

    #endregion

   
}
