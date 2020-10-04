using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    
    private int _chips = 500;
    public int Chips => _chips;

    public void AddChips(int chips)
    {
        _chips+=chips;
    }
    
    public void SubstractChips(int chips)
    {
        _chips-=chips;
    }
}
