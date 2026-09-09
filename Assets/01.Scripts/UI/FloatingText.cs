using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float moveSpeed = 100f;    // 텍스트 떠오르는 속도
    [SerializeField] private float duration = 0.5f;     // 유지 시간
    [SerializeField] private float fadeDuration = 0.2f; // 페이드아웃 시간

    [SerializeField] private GameObject originPrefab;   // 풀링할 원본 프리팹

    private float timer;
    private Color originColor = Color.white;            // 원본 색상 캐싱
    private Vector3 originScale = Vector3.one;          // 원본 스케일 캐싱

    private void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (textMesh != null)
        {
            originColor = textMesh.color;
        }
        originScale = transform.localScale;
    }

    // 필요 시 외부에서 호출 가능
    public void SetOriginPrefab(GameObject prefab)
    {
        originPrefab = prefab;
    }

    public void Setup(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = originColor;   // 알파값 복구
        }

        // 숫자 등장 연출
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(originScale, 0.15f)
                 .SetEase(Ease.OutBack)
                 .SetLink(gameObject);

        timer = duration;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer <= fadeDuration && textMesh != null)
        {
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            Color c = originColor;
            c.a = alpha;
            textMesh.color = c;
        }

        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        transform.DOKill();
        transform.localScale = originScale;

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