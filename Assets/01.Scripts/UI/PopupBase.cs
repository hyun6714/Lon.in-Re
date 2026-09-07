using DG.Tweening;
using UnityEngine;

public abstract class PopupBase : UIBase
{
    [Header("UI 이름")]
    public override UIName Name { get; }

    [Header("트윈용 팝업 데이터")]
    [SerializeField] protected PopupTweenData data;

    protected Sequence seq;

    public abstract void OpenPanel();
    public abstract void ClosePanel();

    protected virtual void OnEnable()
    {
        transform.localScale = Vector3.one * data.CloseSize;
    }
}
