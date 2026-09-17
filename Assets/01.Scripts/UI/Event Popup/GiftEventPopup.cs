using UnityEngine;
using UnityEngine.UI;

public class GiftEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_Event_Gift;

    [Header("¹öÆ°")]
    [SerializeField] private Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(OnClickGift);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(OnClickGift);
    }

    private void OnClickGift()
    {
        GiftEvent giftEvent = EventManager.instance.GetActiveEvent(GameEventType.Gift) as GiftEvent;

        if (giftEvent == null)
            return;

        giftEvent.ReceiveGift();        
    }
}
