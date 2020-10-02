using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : Singleton<User>
{
    
    private int _chips;

    public void AddChips(int chips)
    {
        _chips+=chips;
    }
    
    public void SubstractChips(int chips)
    {
        _chips-=chips;
    }
}
