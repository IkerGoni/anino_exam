using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaylineCanvas : MonoBehaviour
{
    [SerializeField]
    private Button _closeButton;
    
    [SerializeField]
    private Transform _paylinesContainer;
    
    private List<TMP_Text> paylinesInfoTexts = new List<TMP_Text>();

    private const string commaSpace = ", ";

    private Vector3 _canvasSize;
    
    // Start is called before the first frame update
    void Start()
    {
        _closeButton.onClick.AddListener(ClosePaylineCanvas);
        
        foreach (Transform child in _paylinesContainer)
        {
            paylinesInfoTexts.Add(child.GetChild(0).GetComponent<TMP_Text>());
        }
        
        gameObject.SetActive(false);
    }    
    
    private void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Fast implementation of all regarding to this pannel. Not performance, lot of garbage called on creating this texts/
    /// </summary>
    /// <param name="paylines"></param>
    public void SetupPaylineCanvas(int[][] paylines)
    {
        for (int i = 0; i < DataManager.Instance.MaxPaylines; i++)
        {
            string info = $"Payline {i + 1}: (";
            for (int j = 0; j < paylines[i].Length; j++)
            {
                info = string.Concat(info,paylines[i][j],commaSpace);
            }
            if(paylines[i].Length-1>0)
                info = string.Concat(info,paylines[i][ paylines[i].Length-1],")");

            paylinesInfoTexts[i].text = info;

        }
        
        //Disable not used ones
        for (int i = DataManager.Instance.MaxPaylines; i < paylinesInfoTexts.Count; i++)
        {
            paylinesInfoTexts[i].transform.parent.gameObject.SetActive(false);
        }
        
    }

    public void ClosePaylineCanvas()
    {
        AudioManager.Instance.PlayAudio(Audio.Click);
        this.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        });
    }
    
}
