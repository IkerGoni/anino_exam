using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PayoutCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private RectTransform _containerRectTransform;
    
    [SerializeField]
    private Button _closeButton;
    [SerializeField] private GameObject _payoutElement;

    [SerializeField]

    void Start()
    {
        _closeButton.onClick.AddListener(ClosePayoutCanvas);

    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    public void Setup(SymbolData[] allSymbols)
    {
        for (int i = 0; i < allSymbols.Length; i++)
        {
            Instantiate(_payoutElement, _containerRectTransform.transform).GetComponent<PayoutElement>().Setup(allSymbols[i]);    
        }
        
        _containerRectTransform.sizeDelta = new Vector2 ( _containerRectTransform.sizeDelta.x, _containerRectTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y * allSymbols.Length);
    }
    
    
    private void ClosePayoutCanvas()
    {
        AudioManager.Instance.PlayAudio(Audio.Click);
        this.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        });
    }
}
