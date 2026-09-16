using UnityEngine;
using UnityEngine.UI;

public class SystemPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_System;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(ClosePopup);
    }
}
