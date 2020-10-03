using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReelController : MonoBehaviour
{
        
    public delegate void ReelStopped();
    public event ReelStopped OnReelStopped;

    [SerializeField] private SymbolData[] _reelSymbols;
    [SerializeField] public SymbolData[] ReelSymbols
    {
        get => _reelSymbols;
        set => _reelSymbols = value;
    }

    [SerializeField] private Symbol[] _displaySymbols;
    
    private float[] positions = new float[]{2f,0f,-2f,-4f,-6f}; //it is on reel, en case we have different size reels, for this example could be on SlotController but this adds more flexibility (plus its on reels obcjet domain)

    [SerializeField] private float _speed = 20f;


    private SymbolData[] currentResult;
    private int _resultToApplyIndex;
    private bool _settingResult;


    private int _resultSymbolsOnPos = 0;

  
    
    [SerializeField] private int _reelSize = 3;

    public int ReelSize
    {
        get { return _reelSize;}
        set { _reelSize = value; }
    }
    
    [SerializeField] private bool _isSpinning = false;


    public void Setup(int[] reelSymbols)
    {
        _reelSymbols = new SymbolData[reelSymbols.Length];
        for (int i = 0; i < _reelSymbols.Length; i++)
        {
            _reelSymbols[i] = DataManager.Instance.IdToSymbolData[reelSymbols[i]];
        }
        _reelSize = _displaySymbols.Length - 1;
        RandomizeStartingSymbols();
    }

    private void RandomizeStartingSymbols()
    {
        for (int i = 0; i < _displaySymbols.Length; i++)
        {
            _displaySymbols[i].Setup(_reelSymbols[Random.Range(0,_reelSymbols.Length)]);
        }
    }

    void Update()
    {
       if (_isSpinning)
        {
            float newY;
            for (int i = 0; i < _displaySymbols.Length; i++)
            {
                newY = _displaySymbols[i].transform.localPosition.y - _speed * Time.deltaTime;
                
                _displaySymbols[i].transform.localPosition = new Vector3(0, newY);

                if (_displaySymbols[i].transform.localPosition.y <= positions[positions.Length-1])
                {
                    MoveToFirstPos(_displaySymbols[i].transform);
                    _displaySymbols[i].Setup(_reelSymbols[Random.Range(0, _reelSymbols.Length)]);
                }
            }

            return;
        }

        if (_settingResult)
        {
            for (int i = 0; i < _displaySymbols.Length; i++)
            {
                if (_displaySymbols[i].ResultInfoSet &&
                    _displaySymbols[i].transform.position.y == _displaySymbols[i].destinationPos)
                {
                    continue; //Has result symbol, and on destination, continue
                }
                
                float newY;
                //Move down
                
                newY = _displaySymbols[i].transform.localPosition.y - _speed * Time.deltaTime;
                _displaySymbols[i].transform.localPosition = new Vector3(0,
                    newY);

                if (_displaySymbols[i].transform.localPosition.y <= positions[positions.Length - 1])
                {
                    MoveToFirstPos(_displaySymbols[i].transform);

                    if (!_displaySymbols[i].ResultInfoSet)
                    {
                        
                        if (_resultToApplyIndex <0)
                        {
                           continue;                    
                        }
                        
                        ApplyResultInfo(_displaySymbols[i]);
                        _resultToApplyIndex--; //decrease result index

                    }
                       
                }

                if (_displaySymbols[i].transform.localPosition.y <= _displaySymbols[i].destinationPos && _displaySymbols[i].HasDestination )
                {
                    _displaySymbols[i].transform.localPosition = new Vector2(0,_displaySymbols[i].destinationPos);
                    _resultSymbolsOnPos++;
                }
            }

        
        }

        if (_resultSymbolsOnPos == 3 && _settingResult)
        {
            //spin end
            _settingResult = false;

            for (int i = 0; i < _displaySymbols.Length; i++)
            {
                if (_displaySymbols[i].HasDestination == false)
                {
                    _displaySymbols[i].transform.localPosition = new Vector2(0,positions[_reelSize + 1]);
                }
            }

            if (OnReelStopped != null) OnReelStopped();
        }
    }

    private void ApplyResultInfo(Symbol symbol)
    {
        symbol.SetResultInfo(positions[_resultToApplyIndex+1],currentResult[_resultToApplyIndex]);
    }

    //  {{"A", "A", "A", "B", "C"}, {"B", "A", "C", "F", "G"}, {"A", "G", "B", "D", "E"}};

    public void SetResult(SymbolData[] result)
    {
        _resultToApplyIndex = _reelSize-1;
        currentResult = (SymbolData[])result.Clone();
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

        for (int i = 0; i < _displaySymbols.Length; i++)
        {
            _displaySymbols[i].Reset();
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
