using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaylineCanvas : MonoBehaviour
{
    private const string CommaSpace = ", ";//prevent garbage unnecessary garbage creation

    
    [SerializeField]
    private Button _closeButton;
    
    [SerializeField]
    private Transform _paylinesContainer;
    
    private List<TMP_Text> paylinesInfoTexts = new List<TMP_Text>();

    
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        _closeButton.onClick.AddListener(ClosePaylineCanvas);
        
        AddTextReferences();
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Creates the references to the list //I KNOW THIS IS NOT GREAT
    /// Just avoiding setting on references on editor -> this is faster
    /// </summary>
    private void AddTextReferences()
    {
        foreach (Transform child in _paylinesContainer)
        {
            paylinesInfoTexts.Add(child.GetChild(0).GetComponent<TMP_Text>());
        }
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
                info = string.Concat(info,paylines[i][j],CommaSpace);
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
