using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AninoExam;
using UnityEngine;

#region WrapperClasses
/// <summary>
/// Wrapper classes to bypass unity json serializer limitations
/// </summary>
///
[Serializable]
public class AllSymbolsWrapper
{
    public List<SymbolData> Symbols;
}

[Serializable]
public class AllPaylinesWrapper
{
    public PaylinesWrapper[] Paylines;
}
[Serializable]
public class PaylinesWrapper
{
    public int[] Payline;
}

[Serializable]
public class AllReelsSymbolsWrapper
{
    public ReelWrapper[] ReelsSymbolData;
}

[Serializable]
public class ReelWrapper
{
    public List<int> SymbolDataIDs;
}

#endregion


public class FirebaseRemoteConfig
{
    private static FirebaseRemoteConfig _Instance = new FirebaseRemoteConfig();

    public static FirebaseRemoteConfig Instance()
    {
        return _Instance;
    }

    private FirebaseRemoteConfig()
    {
    }
}

public class RemoteConfigFetcher : MonoBehaviour
{
    #region LocalData

    private const string _local_AllSymbols =
        "{\"Symbols\":[{\"Id\":0,\"Name\":\"A\",\"Image\":\"symbols_0\",\"Payout\":[0,0,1,5,10]},{\"Id\":1,\"Name\":\"B\",\"Image\":\"symbols_1\",\"Payout\":[0,0,2,8,25]},{\"Id\":2,\"Name\":\"C\",\"Image\":\"symbols_2\",\"Payout\":[0,0,1,5,10]},{\"Id\":3,\"Name\":\"D\",\"Image\":\"symbols_3\",\"Payout\":[0,0,1,5,10]},{\"Id\":4,\"Name\":\"E\",\"Image\":\"symbols_4\",\"Payout\":[0,0,1,5,10]},{\"Id\":5,\"Name\":\"F\",\"Image\":\"symbols_5\",\"Payout\":[0,0,1,5,10]},{\"Id\":6,\"Name\":\"G\",\"Image\":\"symbols_6\",\"Payout\":[0,0,1,5,10]},{\"Id\":7,\"Name\":\"H\",\"Image\":\"symbols_7\",\"Payout\":[0,0,1,5,10]},{\"Id\":8,\"Name\":\"I\",\"Image\":\"symbols_8\",\"Payout\":[0,0,1,5,10]},{\"Id\":9,\"Name\":\"J\",\"Image\":\"symbols_9\",\"Payout\":[0,0,1,5,10]},{\"Id\":10,\"Name\":\"K\",\"Image\":\"symbols_10\",\"Payout\":[0,0,1,5,10]},{\"Id\":11,\"Name\":\"L\",\"Image\":\"symbols_11\",\"Payout\":[0,0,1,5,10]},{\"Id\":12,\"Name\":\"M\",\"Image\":\"symbols_12\",\"Payout\":[0,0,1,5,10]},{\"Id\":13,\"Name\":\"N\",\"Image\":\"symbols_13\",\"Payout\":[0,0,1,5,10]},{\"Id\":14,\"Name\":\"O\",\"Image\":\"symbols_14\",\"Payout\":[0,0,1,5,10]},{\"Id\":15,\"Name\":\"P\",\"Image\":\"symbols_15\",\"Payout\":[0,0,1,5,10]}]}";

    private const string _local_AllPaylines =
        "{\"Paylines\":[{\"Payline\":[1,1,1,1,1]},{\"Payline\":[0,0,0,0,0]},{\"Payline\":[2,2,2,2,2]},{\"Payline\":[0,1,2,1,0]},{\"Payline\":[2,1,0,1,2]},{\"Payline\":[0,0,1,2,2]},{\"Payline\":[2,2,1,0,0]},{\"Payline\":[1,0,1,2,1]},{\"Payline\":[1,2,1,0,1]},{\"Payline\":[1,0,0,1,0]},{\"Payline\":[1,2,2,1,2]},{\"Payline\":[0,1,0,0,1]},{\"Payline\":[2,1,2,2,1]},{\"Payline\":[0,2,0,2,0]},{\"Payline\":[2,0,2,0,2]},{\"Payline\":[1,0,2,0,1]},{\"Payline\":[1,2,0,2,1]},{\"Payline\":[0,1,1,1,0]},{\"Payline\":[2,1,1,1,2]},{\"Payline\":[0,2,2,2,0]}]}";

    private const int _local_MaxLines = 20;
    
    //private const string _local_ReelsIDs = "{\"ReelsSymbolData\":[{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]}]}";

   
    //easy mode local
    private const string _local_ReelsIDs = "{\"ReelsSymbolData\":[{\"SymbolDataIDs\":[0,1,2,3,4,5]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6]},{\"SymbolDataIDs\":[0,1,2,3,4,5]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6]},{\"SymbolDataIDs\":[0,1,2,3,4,5]}]}";

    #endregion

    
    [Header("SET TP TRUE TO FORCE LOCAL DATA")]
    public bool ForceLocalData = false;
    
    #region FirebaseKeys

    private const string _keyPaylines = "AllPaylines";
    private const string _keySymbols = "AllSymbols";
    private const string _keyMaxLines = "MaxLines";
    private const string _keyReelsIDs = "ReelsSymbolsIds";

    #endregion    
    // Start is called before the first frame update
    void Start()
    {
        StartFetching();
    }

    void StartFetching()
    {
        Fetch(Handler);
        SlotController.Instance.ChangeGameState(GameState.Setup);
    }

    private void Fetch(Action<bool> completionHandler = null)
    {
        var settings = Firebase.RemoteConfig.FirebaseRemoteConfig.Settings;
        settings.IsDeveloperMode = true;
        Firebase.RemoteConfig.FirebaseRemoteConfig.Settings = settings;

        System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync (new System.TimeSpan (0));

        fetchTask.ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (completionHandler != null) completionHandler(false);
                return;
            } 
            Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();

           if (completionHandler != null) completionHandler(true);
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    
    /// <summary>
    /// Handler pases the data to Datamanager, and sets data loaded to true when finished
    /// </summary>
    /// <param name="success"></param>
    public void Handler(bool success)
    {
        if (ForceLocalData)
        {
            DataManager.Instance.ParsePaylinesFromJSON(_local_AllPaylines);
            DataManager.Instance.ParseAllSymbolDataFromJSON(_local_AllSymbols);
            DataManager.Instance.MaxPaylines = _local_MaxLines;
            DataManager.Instance.ParseReelsSymbolsIDsFromJSON(_local_ReelsIDs);
            
            DataManager.Instance.DataLoaded = true;

            return;
        }
        
        if (success)
        {
            string data = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(_keyPaylines).StringValue;
            DataManager.Instance.ParsePaylinesFromJSON(data);
            DataManager.Instance.ParseAllSymbolDataFromJSON(Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(_keySymbols).StringValue);
            DataManager.Instance.MaxPaylines = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(_keyMaxLines).StringValue);
            DataManager.Instance.ParseReelsSymbolsIDsFromJSON(Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(_keyReelsIDs).StringValue);
        }
        else
        {
            DataManager.Instance.ParsePaylinesFromJSON(_local_AllPaylines);
            DataManager.Instance.ParseAllSymbolDataFromJSON(_local_AllSymbols);
            DataManager.Instance.MaxPaylines = _local_MaxLines;
            DataManager.Instance.ParseReelsSymbolsIDsFromJSON(_local_ReelsIDs);
        }

        DataManager.Instance.DataLoaded = true;
    }   
}
