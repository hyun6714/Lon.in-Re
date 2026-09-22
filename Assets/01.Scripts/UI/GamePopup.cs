using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Game;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button gameMakeBtn;

    [Header("게임 제작비")]
    [SerializeField] private TMP_Text gameMakeCostText;

    private void Start()
    {
        exitBtn.onClick.AddListener(ClosePopup);
        gameMakeBtn.onClick.AddListener(() => GameEventBridge.PopupOpened(UIName.Popup_GameMake));
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        UpdateGameMakeCostText();
    }

    private void UpdateGameMakeCostText()
    {
        if (gameMakeCostText == null || GameDevManager.instance == null)
        {
            return;
        }

        gameMakeCostText.text =
            CurrencyFormatter.Format(
                GameDevManager.instance.CurrentBaseDevelopmentCost
            );
    }
}
