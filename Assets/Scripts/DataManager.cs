using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AninoExam;
using UnityEngine;



/// <summary>
/// DataManager takes care of all related to data.
/// Has lookup dictionaries. This could be in other class, for simplicity, Data manager might be handling more than
/// necessary, as the User.
/// </summary>
public class DataManager : Singleton<DataManager>
{

    private User _user;
    public User User => _user;

    
    //All possible symbol data
    [SerializeField] private SymbolData[] _allSymbols;
    public SymbolData[] AllSymbols
    {
        get => _allSymbols;
        set => _allSymbols = value;
    }
    
    //Lookup dictionaries
    private Dictionary<string, SymbolData> nameToSymbolData = new Dictionary<string, SymbolData>();
    public Dictionary<string, SymbolData> NameToSymbolData => nameToSymbolData;
    
    private Dictionary<int, SymbolData> idToSymbolData = new Dictionary<int, SymbolData>();
    public Dictionary<int, SymbolData> IdToSymbolData => idToSymbolData;

    private Dictionary<string, Sprite> nameToSpriteData = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> NameToSpriteData => nameToSpriteData;


    //Current slot config Data
    [SerializeField] private int[][] _paylines;
    [SerializeField] private int[][] _reelsSymbolIDs;
    
    [SerializeField] private int _maxPaylines;  
    public int MaxPaylines
    {
        get { return _maxPaylines; }
        set { _maxPaylines = value; }
    }
    private bool _dataLoaded = false;

    public bool DataLoaded
    {
        get { return _dataLoaded; }
        set
        {
            //first launch. Personally not a big fan of having logic on assignations.
            //Usually i would only have validation logic here.
            if (value && !_dataLoaded) 
            {
                BuildLookupDictionaries();
                InjectSlotData();
                UI_Controller.Instance.PaylineCanvas.SetupPaylineCanvas(_paylines); //not really clean, but trying to close the test due to out of time.
                UI_Controller.Instance.PayoutCanvas.Setup(_allSymbols);

            }

            _dataLoaded = value;
        }
    }
    
        
    /// <summary>
    /// User should be created from data loaded from the backend. TO simplify, datamanager will create a user on evey start;
    /// </summary>
    void Start()
    {
        _user = new User();
    }

    /// <summary>
    /// Creates the loookup Dictionaries
    /// </summary>
    private void BuildLookupDictionaries()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/symbols");

        for (int i = 0; i < _allSymbols.Length; i++)
        {
            nameToSymbolData.Add(_allSymbols[i].Name, _allSymbols[i]);
            idToSymbolData.Add(_allSymbols[i].Id, _allSymbols[i]);
        }
        

        for (int i = 0; i < allSprites.Length; i++)
        {
            nameToSpriteData.Add(allSprites[i].name, allSprites[i]);
        }
    }
    
    /// <summary>
    /// Creates and injects the slot setup data into the game slot controller.
    /// </summary>
    private void InjectSlotData()
    {
        SlotSetupData slotSetupData = new SlotSetupData();
        slotSetupData.Paylines = _paylines;
        slotSetupData.MaxPaylines = _maxPaylines;
        slotSetupData.ReelSymbolIDs = _reelsSymbolIDs;
        
        SlotController.Instance.SetupSlotMachine(slotSetupData);

    }

       #region Parse remote config data
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
            _paylines[i] = new int[wrapper.Paylines[i].Payline.Length];

            for (int j = 0; j < wrapper.Paylines[i].Payline.Length; j++)
            {
                _paylines[i][j] = wrapper.Paylines[i].Payline[j];
            }
        }     
    }

    public void ParseAllSymbolDataFromJSON(string data)
    {
        AllSymbolsWrapper wrapper = JsonUtility.FromJson<AllSymbolsWrapper>(data);

        _allSymbols = (SymbolData[]) wrapper.Symbols.ToArray().Clone();        
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
    }
    
    #endregion
    
    /// <summary>
    /// Gets spin results. in real life slot game, this would be a call to backend with a callback
    /// </summary>
    /// <returns></returns>
    public string[][] GetSpinResult()
    {
        return GenerateResult();
    }

    #region Logic that should be on backend

    /// <summary>
    /// Creates a random result. This function, has to be on backend.
    /// In the test is completely random, but it has to be based on weighted probabilities,
    /// The slot should have a return around 95%.
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
