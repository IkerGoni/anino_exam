using System;
using System.Collections;
using System.Collections.Generic;
using AninoExam;
using UnityEngine;



public class TesterClass : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private SymbolData two;
    void Start()
    {
        Invoke("DoTask",0.3f);
        
    }
    
    public List<SymbolSO> reelSymbols = new List<SymbolSO>();
    void DoTask()
    {
        AllReelsSymbolsWrapper reelsWrapper = new AllReelsSymbolsWrapper();
        reelsWrapper.ReelsSymbolData = new ReelWrapper[reelSymbols.Count];
        for (int i = 0; i < reelSymbols.Count; i++)
        {
            ReelWrapper tempWrapper = new ReelWrapper();
            tempWrapper.SymbolDataIDs = new List<int>();
            for (int j = 0; j < reelSymbols.Count; j++)
            {
                /*SymbolData tempSymbolData = new SymbolData();
                tempSymbolData.Id = reelSymbols[j].Id;
                tempSymbolData.Name = reelSymbols[j].Name;
                tempSymbolData.Image = reelSymbols[j].Image.name;
                tempSymbolData.Payout = reelSymbols[j].Payout;*/
                tempWrapper.SymbolDataIDs.Add(reelSymbols[j].Id);
            }

            reelsWrapper.ReelsSymbolData[i] = tempWrapper;
        }

        string data = JsonUtility.ToJson(reelsWrapper);
        Debug.Log(data);

        /*  List<SymbolData> temp = new List<SymbolData>();
          for (int i = 0; i < SlotController.Instance.AllSymbols.Length; i++)
          {
              SymbolData tempSymbolData = new SymbolData();
              tempSymbolData.Id = SlotController.Instance.AllSymbols[i].Id;
              tempSymbolData.Name = SlotController.Instance.AllSymbols[i].Name;
              tempSymbolData.Image = SlotController.Instance.AllSymbols[i].Image.name;
              tempSymbolData.Payout = SlotController.Instance.AllSymbols[i].Payout;
              temp.Add(tempSymbolData);
          }
          AllSymbolsWrapper wrapper = new AllSymbolsWrapper();
          wrapper.Symbols = temp;
          Debug.Log(JsonUtility.ToJson(wrapper));*/




        /*
        int[][] paylines = DataManager.Instance.GetSlotPaylines();
        
        List<int[]> temp = new List<int[]>();
        for (int i = 0; i < paylines.GetLength(0); i++)
        {
            temp.Add(paylines[i]);
        }
        AllPaylinesWrapper wrapper = new AllPaylinesWrapper();
        wrapper.Paylines = new PaylinesWrapper[paylines.GetLength(0)] ;
        for (int i = 0; i < wrapper.Paylines.Length; i++)
        {
            PaylinesWrapper wrapper2 = new PaylinesWrapper();
            wrapper2.Payline = paylines[i];
            wrapper.Paylines[i] = wrapper2;
        }
        Debug.Log(JsonUtility.ToJson(wrapper));*/



    }
}
