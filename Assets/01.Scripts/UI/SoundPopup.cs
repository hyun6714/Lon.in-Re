using UnityEngine;
using UnityEngine.UI;

public class SoundPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Sound;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(ClosePopup);
    }
}
