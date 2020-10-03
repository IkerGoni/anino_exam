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
    [SerializeField]
    private TextMeshProUGUI _spiButtonText;

    private const string STOP = "STOP";
    private const string SPIN = "SPIN";

    [SerializeField] private Button _spintButton;
    [SerializeField] private Button _increaseBetButton;
    [SerializeField] private Button _decreaseBetButton;


    void Start()
    {
        AddListeners();
        _userTotalChipsText.text = DataManager.Instance.User.Chips.ToString();
        _betAmountText.text = SlotController.Instance.SelectedBet.ToString();
        _spiButtonText.text = SPIN;
    }

    private void OnDestroy()
    {
        _increaseBetButton.onClick.RemoveAllListeners();
        _decreaseBetButton.onClick.RemoveAllListeners();
        _spintButton.onClick.RemoveAllListeners();
        
    }

    private void AddListeners()
    {
        _increaseBetButton.onClick.AddListener(IncreaseBetStepClicked);
        _decreaseBetButton.onClick.AddListener(DecreaseBetStepClicked);
        _spintButton.onClick.AddListener(SpinButtonClicked);
    }

    public void UpdateUIPreSpin()
    {
       // _prizeText.text = 0.ToString();
    }
    
    public void UpdateUIStartpin()
    {
        _userTotalChipsText.text = DataManager.Instance.User.Chips.ToString();
        _spiButtonText.text = STOP;

    }
    public void UpdateUIPostSpin()
    {
        _userTotalChipsText.text = DataManager.Instance.User.Chips.ToString();
        _prizeText.text = (SlotController.Instance.CurrentPrize*SlotController.Instance.SelectedBet).ToString();
        _spiButtonText.text = SPIN;

    }

    private void IncreaseBetStepClicked()
    {
        SlotController.Instance.IncreaseBetStep();
        _betAmountText.text = SlotController.Instance.SelectedBet.ToString();

    }

    private void DecreaseBetStepClicked()
    {
        SlotController.Instance.DecreaseBetStep();
        _betAmountText.text = SlotController.Instance.SelectedBet.ToString();

    }
    
    
    private void SpinButtonClicked()
    {
        SlotController.Instance.SpinButtonClicked();
    }
}
