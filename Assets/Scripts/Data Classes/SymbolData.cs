using System;
using UnityEngine;


[Serializable]
public class SymbolData
{
   public int Id;
   public string Name;
   public string Image;
   public int[] Payout;

   public string ToJSON()
   {
      return JsonUtility.ToJson(this);
   }

   public SymbolData FromJSON(string data)
   {
      return JsonUtility.FromJson<SymbolData>(data);
   }
}
