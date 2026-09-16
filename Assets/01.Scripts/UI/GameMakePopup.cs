using UnityEngine;
using UnityEngine.UI;

public class GameMakePopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_GameMake;

    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(ClosePopup);
    }
}
