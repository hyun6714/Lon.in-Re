using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class BurningEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_Event_Burning;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timerText;

    protected override void OnEnable()
    {
        transform.localScale = Vector3.one * data.CloseSize;
    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        TextTimer(token.Token).Forget();
    }
    private async UniTaskVoid TextTimer(CancellationToken ctk)
    {
        try
        {
            float timer = 10f;

            while (timer <= 0)
            {
                timer -= Time.deltaTime;

                SetText(timer);

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, ctk);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    public void SetText(float time)
    {
        timerText.text = $"{time:N2}";
    }

}
