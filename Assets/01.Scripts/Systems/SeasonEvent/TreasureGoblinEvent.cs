using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TreasureGoblinEvent : IEvent
{
    private TreasureGoblinEventData data;

    private Dictionary<RankManager.RankState, int> rankMoney = new Dictionary<RankManager.RankState, int>();

    private int touchCount;
    private float timer;

    private bool isStarted;
    private bool isFinished;
    private bool isSuccess;

    public int TouchCount => touchCount;
    public int RequireTouchCount => data.RequireTouchCount;
    public float Timer => timer;

    public bool IsStarted => isStarted;
    public bool IsFinished => isFinished;
    public bool IsSuccess => isSuccess;

    public string StartText => data.EventStartText;
    public string SuccessText => data.EventSuccessText;
    public string FailText => data.EventFailText;

    private CancellationTokenSource token;

    public TreasureGoblinEvent(TreasureGoblinEventData data)
    {
        this.data = data;
    }

    public void StartEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        touchCount = 0;
        timer = data.EventTimer;

        isStarted = true;
        isFinished = false;
        isSuccess = false;

        InitDic();

        UpdateTimer(token.Token).Forget();
    }

    private void InitDic()
    {
        foreach (TreasureGoblinRewardInfo info in data.Reward)
        {
            if (rankMoney.ContainsKey(info.rank))
            {
                Utils.Log("이미 등록된 랭크 보상");
                continue;
            }

            rankMoney.Add(info.rank, info.rewardGold);
        }
    }

    private async UniTaskVoid UpdateTimer(CancellationToken ctk)
    {
        try
        {
            while (!isFinished && timer > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = 0f;
                    FailEvent();
                    break;
                }

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, ctk);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: ctk);
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            ClosePopup();
        }
    }

    // 성공 판정 판별
    public bool HitGoblin()
    {
        if (!isStarted || isFinished)
            return false;

        touchCount++;

        if (touchCount >= RequireTouchCount)
        {
            SuccessEvent();
            return true;
        }

        return false;
    }

    private void SuccessEvent()
    {
        if (isFinished)
            return;

        isFinished = true;
        isSuccess = true;

        Reward();
    }

    public void FailEvent()
    {
        if (isFinished)
            return;

        isFinished = true;
        isSuccess = false;
    }

    private void Reward()
    {
        if (RankManager.instance == null)
            return;

        RankManager.RankState rank = RankManager.instance.currentRank;

        if (!rankMoney.TryGetValue(rank, out int amount))
            return;

        GameEventBridge.CurrencyAdded(CurrencyType.Normal, amount);
    }

    private void ClosePopup()
    {
        UIManager.Instance.ClosePopup(UIName.Popup_Event_TreasureGoblin);
        EventManager.instance.EndCurrentEvent(GameEventType.TreasureGoblin);
    }

    public void EndEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        isStarted = false;

        rankMoney.Clear();
        rankMoney = null;
    }

    public void SaveEventData(EventSaveData eventSaveData)
    {

    }
}
