using DG.Tweening;
using UnityEngine;

public class SeasonEventPopup : PopupBase
{
    public void OpenPanel()
    {
        PopupOpen();
    }

    public void ClosePanel()
    {
        PopupClose();
    }

    private void PopupOpen()
    {
        seq?.Kill();

        seq = DOTween.Sequence();
        seq.Append(transform.DOScale(data.PopupSize, data.PopupDelay))
            .Append(transform.DOScale(data.OpenSize, data.PopupDelay))
            .SetLink(gameObject, LinkBehaviour.KillOnDisable)
            .SetUpdate(true);
    }

    private void PopupClose()
    {
        seq?.Kill();
        
        seq = DOTween.Sequence();
        seq.Append(transform.DOScale(data.PopupSize, data.PopupDelay))
            .Append(transform.DOScale(data.CloseSize, data.PopupDelay))
            .SetLink(gameObject, LinkBehaviour.KillOnDisable)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                Time.timeScale = 1f;
            });
    }
}
