using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class SummerEvent : IEvent
{
    private SummerEventData data;
    private bool isCool;

    private GameDate eventEndDate;

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
        token = new CancellationTokenSource();
        Utils.Log("여름 이벤트 시작");

        int day = data.SummerEventAddDay;
        eventEndDate = CalendarManager.instance.GetAfterDay(day);
        Utils.Log($"이벤트 종료 날짜 : {eventEndDate.year}년 {eventEndDate.month}월 {eventEndDate.day}일 {eventEndDate.hour}시 {eventEndDate.minutes}분");

        EventTimer(token.Token).Forget();
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

    private async UniTask EventTimer(CancellationToken token)
    {
        try
        {
            await UniTask.WaitUntil(() => CalendarManager.instance.CurrentDate >= eventEndDate, cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            Utils.LogWarning("이벤트가 취소되었습니다.");
        }
        finally
        {
            EndEvent();
        }
    }

    public void EndEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        coolAutoMultiplier = data.AutoMultiplier;
        coolClickMultiplier = data.ClickMultiplier;

        EventManager.instance.SummerMultiplier(coolAutoMultiplier);
        Utils.Log("여름 이벤트 종료");
        Utils.Log("생산 배수 초기화");
    }
}
