using DG.Tweening;
using UnityEngine;

public class PopupBase : MonoBehaviour
{
    [Header("트윈용 팝업 데이터")]
    [SerializeField] protected PopupTweenData data;

    protected Sequence seq;

    protected void OnEnable()
    {
        transform.localScale = Vector3.one * data.CloseSize;
    }
}
