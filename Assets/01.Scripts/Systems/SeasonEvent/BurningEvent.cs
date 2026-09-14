using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class BurningEvent : IEvent
{
    private BurningEventData data;

    private CancellationTokenSource token;

    public BurningEvent(BurningEventData data)
    {
        this.data = data;
    }

    public void StartEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        BurningTime(token.Token).Forget();

        Utils.Log("버닝 이벤트 실행 확인용");
    }

    private async UniTaskVoid BurningTime(CancellationToken ctk)
    {
        try
        {
            SetMultiplier();

            await UniTask.Delay(TimeSpan.FromSeconds(data.BurningTime), cancellationToken: ctk);

            ReturnMultiplier();

            await UniTask.Delay(TimeSpan.FromSeconds(data.EndTime), cancellationToken: ctk);

            UIManager.Instance.ClosePopup(UIName.Popup_Event_Burning);
            EventManager.instance.EndCurrentEvent(GameEventType.Burning);

        }
        catch (OperationCanceledException)
        {

        }
    }

    private void SetMultiplier()
    {
        GameEventBridge.TapMultiplierChanged(GameEventType.Burning, data.ClickMultiplier);
        GameEventBridge.AutoMultiplierChanged(GameEventType.Burning, data.AutoMultiplier);
    }

    private void ReturnMultiplier()
    {
        GameEventBridge.TapMultiplierChanged(GameEventType.Burning, data.BaseClickMultiplier);
        GameEventBridge.AutoMultiplierChanged(GameEventType.Burning, data.BaseAutoMultiplier);
    }

    public void EndEvent()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        Utils.Log("버닝 이벤트 종료");
    }

    public void SaveEventData(EventSaveData data)
    {

    }
}
