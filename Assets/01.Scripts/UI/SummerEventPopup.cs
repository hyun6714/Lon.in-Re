using UnityEngine;
using UnityEngine.UI;

public class SummerEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_Event_0715;

    [SerializeField] private Button button_1;
    [SerializeField] private Button button_2;

    protected override void OnEnable()
    {
        base.OnEnable();
        button_1.onClick.AddListener(() => EventManager.instance.OnClickSummerCool(false));
        button_2.onClick.AddListener(() => EventManager.instance.OnClickSummerCool(true));
    }

    private void OnDisable()
    {
        button_1.onClick.RemoveAllListeners();
        button_2.onClick.RemoveAllListeners();
    }    
}