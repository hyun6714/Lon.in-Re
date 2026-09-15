using UnityEngine;
using UnityEngine.UI;

public class GiftEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_Gift;

    [SerializeField] private Button btn;

    private GiftEvent giftEvent;

    private void OnEnable()
    {
        btn.onClick.AddListener(() => GameEventBridge.CurrencyAdded(CurrencyType.Normal, 10000));
    }

    private void OnClickGift()
    {

    }
}
