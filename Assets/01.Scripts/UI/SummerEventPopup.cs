using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class SummerEventPopup : PopupBase
{
    public override UIName Name => UIName.Event_0715_Popup;

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

    public override void OpenPanel()
    {
        PopupOpen();
    }

    public override void ClosePanel()
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
