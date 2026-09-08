using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class SummerEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Event_0715_Popup;

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