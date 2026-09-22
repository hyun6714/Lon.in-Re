using UnityEngine;
using UnityEngine.UI;

public class ManualPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Tutorial;

    [SerializeField] private Button exitBtn;

    private void Start()
    {
        exitBtn.onClick.AddListener(ClosePopup);
    }
}
