using UnityEngine;


[CreateAssetMenu(fileName = "SymbolData", menuName = "ScriptableObjects/SymbolData", order = 1)]

public class SymbolSO : ScriptableObject
{
    public int Id;
    public string Name;
    public Sprite Image;
    public int[] Payout;
}