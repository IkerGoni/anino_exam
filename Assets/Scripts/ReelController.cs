using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReelController : MonoBehaviour
{


    [SerializeField] private SymbolSO[] _reelSymbols;
    [SerializeField] public SymbolSO[] ReelSymbols=>_reelSymbols;
    [SerializeField] private Symbol[] displaySymbols;
    
    private float[] positions = new float[]{2f,0f,-2f,-4f,-6f}; //it is on reel, en case we have different size reels, for this example could be on SlotController but this adds more flexibility (plus its on reels obcjet domain)

    [SerializeField] private float _speed = 5f;


    private SymbolSO[] currentResult;
    private int _resultToApplyIndex;
    private bool _settingResult;


    private int _resultSymbolsOnPos = 0;

    public float Speed
    {
        get { return _speed;}
        set { _speed = value; }
    }
    
    [SerializeField] private int _reelSize = 3;

    public int ReelSize
    {
        get { return _reelSize;}
        set { _reelSize = value; }
    }
    
    [SerializeField] private bool _isSpinning = false;


    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        _reelSize = displaySymbols.Length - 1;
    }


    
    /*ser results array
    
    if displaying
    
    symbol gets to bottom - > set result symbol, decrese result symbol index, mark as result set move up and wait in pos
    
    
    */
    void Update()
    {
       if (_isSpinning)
        {
            float newY;
            for (int i = 0; i < displaySymbols.Length; i++)
            {
                newY = displaySymbols[i].transform.localPosition.y - _speed * Time.deltaTime;
                
                displaySymbols[i].transform.localPosition = new Vector3(0, newY);

                if (displaySymbols[i].transform.localPosition.y <= positions[positions.Length-1])
                {
                    MoveToFirstPos(displaySymbols[i].transform);
                    displaySymbols[i].Setup(_reelSymbols[Random.Range(0, _reelSymbols.Length)]);
                }
            }

            return;
        }

        if (_settingResult)
        {
            for (int i = 0; i < displaySymbols.Length; i++)
            {
                if (displaySymbols[i].ResultInfoSet &&
                    displaySymbols[i].transform.position.y == displaySymbols[i].destinationPos)
                {
                    continue; //Has result symbol, and on destination, continue
                }
                
                float newY;
                //Move down
                
                newY = displaySymbols[i].transform.localPosition.y - _speed * Time.deltaTime;
                displaySymbols[i].transform.localPosition = new Vector3(0,
                    newY);

                if (displaySymbols[i].transform.localPosition.y <= positions[positions.Length - 1])
                {
                    MoveToFirstPos(displaySymbols[i].transform);

                    if (!displaySymbols[i].ResultInfoSet)
                    {
                        
                        if (_resultToApplyIndex <0)
                        {
                           continue;                    
                        }
                        
                        ApplyResultInfo(displaySymbols[i]);
                        _resultToApplyIndex--; //decrease result index

                    }
                       
                }

                if (displaySymbols[i].transform.localPosition.y <= displaySymbols[i].destinationPos && displaySymbols[i].HasDestination )
                {
                    displaySymbols[i].transform.localPosition = new Vector2(0,displaySymbols[i].destinationPos);
                    _resultSymbolsOnPos++;
                }
            }

        
        }

        if (_resultSymbolsOnPos == 3)
        {
            //spin end
            _settingResult = false;

            for (int i = 0; i < displaySymbols.Length; i++)
            {
                if (displaySymbols[i].HasDestination == false)
                {
                    displaySymbols[i].transform.localPosition = new Vector2(0,positions[_reelSize + 1]);
                }
            }
        }
    }

    private void ApplyResultInfo(Symbol symbol)
    {
        symbol.SetResultInfo(positions[_resultToApplyIndex+1],currentResult[_resultToApplyIndex]);
    }

    //  {{"A", "A", "A", "B", "C"}, {"B", "A", "C", "F", "G"}, {"A", "G", "B", "D", "E"}};

    public void SetResult(SymbolSO[] result)
    {
        _resultToApplyIndex = _reelSize-1;
        currentResult = (SymbolSO[])result.Clone();
        //Debug.Log("result applied: " + result[0].Name + " "+ result[1].Name + " "+ result[2].Name + " ");
    }

 
    void MoveToFirstPos(Transform symbol)
    {
        symbol.localPosition = new Vector2(0, positions[0] - (positions[positions.Length-1] - symbol.localPosition.y));
    }


    public void StartSpinning()
    {
        _settingResult = false;
        _isSpinning = true;

        _resultSymbolsOnPos = 0;

        for (int i = 0; i < displaySymbols.Length; i++)
        {
            displaySymbols[i].Reset();
        }
    }

  /*  public void StopSpinning()
    {
        _isSpinning = false;
        _settingResult = true;
    }
*/
    public void DisplayResults()
    {
        _isSpinning = false;
        _settingResult = true;
    }
}
