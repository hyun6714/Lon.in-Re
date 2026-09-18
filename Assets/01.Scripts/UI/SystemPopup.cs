using UnityEngine;
using UnityEngine.UI;

public class SystemPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_System;

    [SerializeField] private Button backBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button quitBtn;

    private void Start()
    {
        backBtn.onClick.AddListener(ClosePopup);
        soundBtn.onClick.AddListener(() => UIManager.Instance.OpenPopup(UIName.Popup_Sound));
    }
}
