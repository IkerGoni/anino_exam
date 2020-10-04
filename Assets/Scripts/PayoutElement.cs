using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PayoutElement : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _payout1;
    [SerializeField] private TMP_Text _payout2;
    [SerializeField] private TMP_Text _payout3;
    [SerializeField] private TMP_Text _payout4;
    [SerializeField] private TMP_Text _payout5;

    public void Setup(SymbolData symbolData)
    {
        _image.sprite = DataManager.Instance.NameToSpriteData[symbolData.Image];
        _name.text = symbolData.Name;
        _payout1.text = symbolData.Payout[0].ToString();
        _payout2.text = symbolData.Payout[1].ToString();
        _payout3.text = symbolData.Payout[2].ToString();
        _payout4.text = symbolData.Payout[3].ToString();
        _payout5.text = symbolData.Payout[4].ToString();
    }
}
