using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class EventPopupBase : PopupBase
{
    protected CancellationTokenSource token;

    protected virtual void OnEnable()
    {
        transform.localScale = Vector3.one * data.CloseSize;
    }

    public override void OpenPopup()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        PopupOpenAsync(token.Token).Forget();
    }

    public override void ClosePopup()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        PopupClose(token.Token).Forget();
    }

    protected async UniTask PopupOpenAsync(CancellationToken ctk)
    {
        try
        {
            seq?.Kill();

            seq = DOTween.Sequence();
            await seq.Append(transform.DOScale(data.PopupSize, data.PopupDelay))
                .Append(transform.DOScale(data.OpenSize, data.PopupDelay))
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .ToUniTask(TweenCancelBehaviour.CompleteAndCancelAwait, ctk);
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    protected async UniTask PopupClose(CancellationToken ctk)
    {
        try
        {
            seq?.Kill();

            seq = DOTween.Sequence();
            await seq.Append(transform.DOScale(data.PopupSize, data.PopupDelay))
                .Append(transform.DOScale(data.CloseSize, data.PopupDelay))
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    GameManager.Instance.GameResume();
                    gameObject.SetActive(false);
                })
                .ToUniTask(TweenCancelBehaviour.CompleteAndCancelAwait, ctk);
        }
        catch (OperationCanceledException)
        {
            
        }
    }
}
