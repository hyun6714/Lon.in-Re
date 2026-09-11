using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TapEffect : MonoBehaviour, IPoolable
{
    [Header("컴포넌트")]
    [SerializeField] private Image effectImage;

    [Header("이펙트 설정")]
    [SerializeField] private float duration = 0.2f;        // 퍼지는 시간
    [SerializeField] private Vector3 startScale = new Vector3(0.2f, 0.2f, 1f);
    [SerializeField] private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);

    private GameObject originPrefab;
    private Color originColor = Color.white;
    private Sequence effectSequence;

    private void Awake()
    {
        if (effectImage == null)
        {
            effectImage = GetComponent<Image>();
        }

        if (effectImage != null)
        {
            originColor = effectImage.color;
        }
    }

    public void SetOriginPrefab(GameObject prefab)
    {
        originPrefab = prefab;
    }

    public void PlayEffect()
    {
        effectSequence?.Kill();

        // 초기 상태 초기화
        transform.localScale = startScale;
        if (effectImage != null)
        {
            effectImage.color = originColor;
        }

        gameObject.SetActive(true);

        // 크기 확장 + 페이드아웃 트윈
        effectSequence = DOTween.Sequence();
        effectSequence.Append(transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad))
                      .Join(effectImage.DOFade(0f, duration).SetEase(Ease.OutQuad))
                      .SetUpdate(true)
                      .OnComplete(ReturnToPool);
    }

    public void ReturnToPool()
    {
        effectSequence?.Kill();

        if (ObjectPoolManager.instance != null && originPrefab != null)
        {
            ObjectPoolManager.instance.ReturnObject(originPrefab, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}