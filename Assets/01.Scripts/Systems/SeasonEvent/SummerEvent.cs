using System;
using System.Threading;
using UnityEngine;

public class SummerEvent : IEvent
{
    private SummerEventData data;
    private bool isCool;
    private float coolAutoMultiplier;
    private float coolClickMultiplier;

    private CancellationTokenSource token;

    public SummerEvent(SummerEventData data)
    {
        this.data = data;
    }

    public void StartEvent()
    {
        token?.Cancel();
        token?.Dispose();
        Utils.Log("여름 이벤트 시작");
    }

    public void SetCool(bool value)
    {
        isCool = value;

        SetMultiplier();
    }

    public void SetMultiplier()
    {
        coolAutoMultiplier = isCool ? data.CoolAutoMultiplier : data.UnCoolAutoMultiplier;
        coolClickMultiplier = isCool ? data.CoolClickMultiplier : data.UnCoolClickMultiplier;

        string text = isCool ? data.CoolText : data.UnCoolText;
        Utils.Log(text);
        EventManager.instance.SummerMultiplier(coolAutoMultiplier);
    }



    public void EndEvent()
    {
        Utils.Log("여름 이벤트 종료");
    }
}
