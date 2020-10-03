using System.Collections;
using System.Collections.Generic;
using AninoExam;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : Singleton<UI_Controller>
{
    [SerializeField]
    private TextMeshProUGUI _userTotalChipsText;
    [SerializeField]
    private TextMeshProUGUI _prizeText;
    [SerializeField]
    private TextMeshProUGUI _betAmountText;

    [SerializeField] private Button _spintButton;
    [SerializeField] private Button _increaseBetButton;
    [SerializeField] private Button _decreaseBetButton;


    void Start()
    {
        AddListeners();
    }

    private void AddListeners()
    {
        _increaseBetButton.onClick.AddListener(SlotController.Instance.IncreaseBetStep);
        _decreaseBetButton.onClick.AddListener(SlotController.Instance.DecreaseBetStep);
        _spintButton.onClick.AddListener(SlotController.Instance.SpinButtonClicked);
    }
}
