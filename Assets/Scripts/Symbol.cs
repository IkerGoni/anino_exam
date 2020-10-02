using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{

    [SerializeField] private SymbolSO _symbolConfig;
    public SymbolSO ResultSymbol => _resultSymbol;
    [SerializeField] private SymbolSO _resultSymbol;
    [SerializeField] private int id;
    public bool HasDestination = false;
    public bool ResultInfoSet = false;
    public float destinationPos;
    
    [SerializeField] private SpriteRenderer _symbolImage;

    public void Setup(SymbolSO symbolConfig)
    {
        _symbolConfig = symbolConfig;
        _symbolImage.sprite = _symbolConfig.Image;
    }

    public void SetResultInfo(float posY, SymbolSO resultSymbol)
    {
        destinationPos = posY;
        HasDestination = true;
        _resultSymbol = resultSymbol; //delete
        Setup(resultSymbol);
        ResultInfoSet = true;
    }

    public void Reset()
    {
        HasDestination = false;
        ResultInfoSet = false; 
        _resultSymbol = null;
    }
}
