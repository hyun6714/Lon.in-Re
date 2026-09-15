using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class BurningEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_Event_Burning;

    [Header("이벤트 데이터")]
    [SerializeField] private BurningEventData burningData;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timerText;

    protected override void OnEnable()
    {
        transform.localScale = Vector3.one * data.CloseSize;
    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        SetTitleText(burningData.BurningStartText);
        TextTimer(token.Token).Forget();
    }

    private async UniTaskVoid TextTimer(CancellationToken ctk)
    {
        try
        {
            float timer = burningData.BurningTime;

            while (timer > 0)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    timer = 0f;
                }

                SetTimerText(timer);

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, ctk);
            }

            SetTitleText(burningData.BruningEndText);
        }
        catch (OperationCanceledException)
        {

        }
    }

    public void SetTimerText(float time)
    {
        timerText.text = $"{time:N2}";
    }

    private void SetTitleText(string text)
    {
        titleText.text = text;
    }
}
