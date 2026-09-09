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

    public bool IsActive => token != null;
    public GameDate EventEndDate => eventEndDate;
    public bool IsCool => isCool;

    public SummerEvent(SummerEventData data)
    {
        this.data = data;

        int day = data.SummerEventAddDay;
        eventEndDate = CalendarManager.instance.GetAfterDay(day);
    }

    public void StartEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();
        Utils.Log("여름 이벤트 시작");

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

    public void ReturnMultiplier()
    {
        coolAutoMultiplier = data.AutoMultiplier;
        coolClickMultiplier = data.ClickMultiplier;

        EventManager.instance.SummerMultiplier(coolAutoMultiplier);

        Utils.Log("생산 배수 초기화");
    }

    public void EndEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        ReturnMultiplier();
        
        Utils.Log("여름 이벤트 종료");
    }

    public void SaveEventData(EventSaveData eventSaveData)
    {
        eventSaveData.eventEndDate = eventEndDate;
        eventSaveData.isSummerCool = isCool;
    }

    public void LoadEvent(GameDate endDate, bool isSummerCool)
    {
        eventEndDate = endDate;
        isCool = isSummerCool;

        if (CalendarManager.instance.CurrentDate >= eventEndDate)
        {
            EndEvent();
            return;
        }

        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        GameDate currentDate = CalendarManager.instance.CurrentDate;
        GameDate targetDate = new GameDate()
        {
            year = currentDate.year,
            month = eventEndDate.month,
            day = eventEndDate.day,
            hour = CalendarManager.instance.AllEventStartHour
        };

        if (currentDate < targetDate)
        {
            Utils.Log("여름 이벤트 복원 완료(이벤트 시작 전)");
            return;
        }

        SetMultiplier();
        EventTimer(token.Token).Forget();
        Utils.Log("여름 이벤트 복원 완료");
    }
}
