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
    private float timer;
    private bool isPlaying;

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
        timer = 0f;
        isPlaying = true;

        transform.localScale = startScale;
        if (effectImage != null)
        {
            effectImage.color = originColor;
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        // 타이머 누적
        timer += Time.unscaledDeltaTime;

        float t = timer / duration;

        transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        if (effectImage != null)
        {
            Color c = originColor;
            c.a = Mathf.Lerp(originColor.a, 0f, t); // 투명화
            effectImage.color = c;
        }

        if (timer >= duration)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        isPlaying = false;

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