using UnityEngine;
using UnityEngine.UI;

public class RebirthPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Rebirth;

    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(ClosePopup);
    }
}
