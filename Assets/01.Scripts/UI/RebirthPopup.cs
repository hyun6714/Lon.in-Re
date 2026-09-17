using UnityEngine;
using UnityEngine.UI;

public class RebirthPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Rebirth;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button rebirthBtn;

    private void Start()
    {
        exitBtn.onClick.AddListener(ClosePopup);
        rebirthBtn.onClick.AddListener(OnClickRebirth);
    }

    private void OnClickRebirth()
    {
        if (ReincarnationManager.instance.CanReincarnation())
        {
            ReincarnationManager.instance.BtnReincarnation();

            ClosePopup();
        }
    }
}
