using UnityEngine;
using UnityEngine.UI;

public class ChagePopup : NormalPopupBase
{
    [Header("UI References")]
    [SerializeField] private Button closeButton;

    public override UIName Name => UIName.Popup_GoldChage;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopup);
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePopup);
        }
    }
}