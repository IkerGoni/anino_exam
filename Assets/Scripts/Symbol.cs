using UnityEngine;

public class Symbol : MonoBehaviour
{

    [SerializeField] private SymbolData _symbolConfig;
    [SerializeField] private SymbolData _symbolData;
    public SymbolData ResultSymbol => _resultSymbol;
    [SerializeField] private SymbolData _resultSymbol;
    [SerializeField] private int id;
    public bool HasDestination = false;
    public bool ResultInfoSet = false;
    public float destinationPos;
    
    [SerializeField] private SpriteRenderer _symbolImage;

    public void Setup(SymbolData symbolData)
    {
        _symbolData = symbolData;
        _symbolImage.sprite = DataManager.Instance.NameToSpriteData[symbolData.Image] ;
    }
    
    public void SetResultInfo(float posY, SymbolData resultSymbol)
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
