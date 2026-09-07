using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EventPopup : PopupBase
{
    [SerializeField] private Button button_1;
    [SerializeField] private Button button_2;

    protected override void OnEnable()
    {
        base.OnEnable();
        button_1.onClick.AddListener(() => EventManager.instance.OnClickSummerCool(false));
        button_2.onClick.AddListener(() => EventManager.instance.OnClickSummerCool(true));
    }

    private void OnDisable()
    {
        button_1.onClick.RemoveAllListeners();
        button_2.onClick.RemoveAllListeners();
    }

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
                GameManager.Instance.GameResume();
                gameObject.SetActive(false);
            });
    }
}
