using System.Collections;
using AninoExam;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : Singleton<UI_Controller>
{
    [SerializeField] private PaylineCanvas _paylineCanvas;
    public PaylineCanvas PaylineCanvas => _paylineCanvas;
    
    [SerializeField] private PayoutCanvas _payoutCanvas;
    public PayoutCanvas PayoutCanvas => _payoutCanvas;
    
    [SerializeField]
    private Image _blackPanel;
    
    [SerializeField]
    private TextMeshProUGUI _userTotalChipsText;
    [SerializeField]
    private TextMeshProUGUI _prizeText;
    [SerializeField]
    private TextMeshProUGUI _betAmountText;
    [SerializeField]
    private TextMeshProUGUI _spinButtonText;

    private const string STOP = "STOP";
    private const string SPIN = "SPIN";

    [SerializeField] private Button _spintButton;
    [SerializeField] private Button _increaseBetButton;
    [SerializeField] private Button _decreaseBetButton;
    [SerializeField] private Button _infoPaylinesButton;
    [SerializeField] private Button _infoPayoutButton;

    WaitForSeconds stepWaitSeconds = new WaitForSeconds(0.4f);

    void Start()
    {
        _paylineCanvas.gameObject.SetActive(true);
        _blackPanel.enabled = true;
        AddListeners();
        _userTotalChipsText.text = DataManager.Instance.User.Chips.ToString();
        _betAmountText.text = SlotController.Instance.SelectedBet.ToString();
        _spinButtonText.text = SPIN;
    }

    private void OnDestroy()
    {
        _increaseBetButton.onClick.RemoveAllListeners();
        _decreaseBetButton.onClick.RemoveAllListeners();
        _spintButton.onClick.RemoveAllListeners();
        _infoPaylinesButton.onClick.RemoveAllListeners();
        _infoPayoutButton.onClick.RemoveAllListeners();
        
    }

    private void AddListeners()
    {
        _increaseBetButton.onClick.AddListener(IncreaseBetStepClicked);
        _decreaseBetButton.onClick.AddListener(DecreaseBetStepClicked);
        _spintButton.onClick.AddListener(SpinButtonClicked);
        _infoPaylinesButton.onClick.AddListener(PaylineInfoButtonClicked);
        _infoPayoutButton.onClick.AddListener(PayoutInfoButtonClicked);
    }

    public void HideBlackPanel()
    {
        _blackPanel.DOFade(0, 0.5f).OnComplete(() => { _blackPanel.gameObject.SetActive(false); });
    }
    
    public void UpdateUIStartpin()
    {
        _userTotalChipsText.text = DataManager.Instance.User.Chips.ToString();
        _spinButtonText.text = STOP;

    }
    public void UpdateUIPostSpin()
    {
        SlotController.Instance.ChangeGameState(GameState.SpinFeedback);

        StartCoroutine(PostSpinSteps());
    }

    IEnumerator PostSpinSteps()
    {
        if (SlotController.Instance.CurrentPrize > 0)
        {
            _prizeText.text = (SlotController.Instance.CurrentPrize*SlotController.Instance.SelectedBet).ToString();
            AudioManager.Instance.PlayAudio(Audio.WinResult);
            yield return stepWaitSeconds;
            _userTotalChipsText.text = DataManager.Instance.User.Chips.ToString();
            AudioManager.Instance.PlayAudio(Audio.Chips);
            yield return stepWaitSeconds;     
        }

        _spinButtonText.text = SPIN;
        SlotController.Instance.ChangeGameState(GameState.GettingResult);


    }

    private void IncreaseBetStepClicked()
    {
        SlotController.Instance.IncreaseBetStep();
        _betAmountText.text = SlotController.Instance.SelectedBet.ToString();
        AudioManager.Instance.PlayAudio(Audio.IncreaseBet);

    }

    private void DecreaseBetStepClicked()
    {
        SlotController.Instance.DecreaseBetStep();
        _betAmountText.text = SlotController.Instance.SelectedBet.ToString();
        AudioManager.Instance.PlayAudio(Audio.DecreaseBet);

    }
    
    
    private void SpinButtonClicked()
    {
        SlotController.Instance.SpinButtonClicked();
        AudioManager.Instance.PlayAudio(Audio.Click);
    }    
    
    private void PaylineInfoButtonClicked()
    {
        _paylineCanvas.gameObject.SetActive(true);
        AudioManager.Instance.PlayAudio(Audio.Click);
    }    
    
    private void PayoutInfoButtonClicked()
    {
        _payoutCanvas.gameObject.SetActive(true);
        AudioManager.Instance.PlayAudio(Audio.Click);
    }
    
    
}
