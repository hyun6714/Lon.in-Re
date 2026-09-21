using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

public class AirConditionalEvent : IEvent
{
    private AirConditionalEventData data;
    private bool isCool;

    private GameDate eventEndDate;

    private float coolAutoMultiplier;
    private float coolClickMultiplier;

    private CancellationTokenSource token;

    private Dictionary<RankManager.RankState, int> rankFame = new Dictionary<RankManager.RankState, int>();

    public bool IsActive => token != null;
    public GameDate EventEndDate => eventEndDate;
    public bool IsCool => isCool;

    public AirConditionalEvent(AirConditionalEventData data)
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
        
        Utils.Log($"이벤트 종료 날짜 : {eventEndDate.year}년 {eventEndDate.month}월 {eventEndDate.day}일 {eventEndDate.hour}시 {eventEndDate.minutes}분");

        InitDic();

        EventTimer(token.Token).Forget();
    }

    private void InitDic()
    {
        foreach (AirConditionalRewardInfo info in data.Reward)
        {
            if (rankFame.ContainsKey(info.rank))
            {
                Utils.Log($"이미 등록된 랭크 보상 : {info.rewardFame}");
                continue;
            }

            rankFame.Add(info.rank, info.rewardFame);
        }
    }

    public void SetCool(bool value)
    {
        isCool = value;

        SetMultiplier();
        Reward();
    }

    public void Reward()
    {
        if (!isCool)
            return;

        RankManager.RankState rank = RankManager.instance.currentRank;

        if (!rankFame.TryGetValue(rank, out int amount))
            return;

        GameEventBridge.CurrencyAdded(CurrencyType.Reputation, amount);
    }

    public void SetMultiplier()
    {
        coolClickMultiplier = isCool ? data.CoolClickMultiplier : data.UnCoolClickMultiplier;
        coolAutoMultiplier = isCool ? data.CoolAutoMultiplier : data.UnCoolAutoMultiplier;

        string text = isCool ? data.CoolText : data.UnCoolText;
        Utils.Log(text);
        GameEventBridge.TapMultiplierChanged(GameEventType.AirConditional, coolClickMultiplier);
        GameEventBridge.AutoMultiplierChanged(GameEventType.AirConditional, coolAutoMultiplier);
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
            EventManager.instance.EndCurrentEvent(GameEventType.AirConditional);
        }
    }

    public void ReturnMultiplier()
    {
        coolAutoMultiplier = data.AutoMultiplier;
        coolClickMultiplier = data.ClickMultiplier;

        GameEventBridge.TapMultiplierChanged(GameEventType.AirConditional, coolClickMultiplier);
        GameEventBridge.AutoMultiplierChanged(GameEventType.AirConditional, coolAutoMultiplier);

        Utils.Log("생산 배수 초기화");
    }

    public void EndEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        ReturnMultiplier();
    }

    public void SaveEventData(EventSaveData eventSaveData)
    {
        eventSaveData.eventEndDate = new GameDateSaveData(eventEndDate);
        eventSaveData.isSummerCool = isCool;
    }

    public void LoadEvent(GameDateSaveData endDate, bool isSummerCool)
    {
        GameDate loadEndDate = CalendarManager.instance.CurrentDate;

        loadEndDate.LoadDate(endDate);

        eventEndDate = loadEndDate;
        isCool = isSummerCool;

        GameDate currentDate = CalendarManager.instance.CurrentDate;

        if (currentDate >= eventEndDate)
        {
            Utils.Log("이벤트 종료 시간이 지나서 로드하지 않음");
            EndEvent();
            return;
        }

        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        SetMultiplier();
        EventTimer(token.Token).Forget();
        Utils.Log("여름 이벤트 복원 완료");
    }
}
