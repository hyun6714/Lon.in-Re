using UnityEngine;
using UnityEngine.UI;

public class GameMakePopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_GameMake;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button developBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(ClosePopup);
        developBtn.onClick.AddListener(OnClickDevelopGame);
    }
    private void OnEnable()
    {
        GameDevManager.instance.ResetGameMakeUI();
    }
    private void OnClickDevelopGame()
    {
        GameDevResult result = GameDevManager.instance.OnClickDevelopGame();

        // 재화 부족 등으로 개발에 실패했으면 팝업 유지
        if (result == null)
            return;

        // 개발 성공했을 때만 팝업 닫기
        ClosePopup();
    }
}
