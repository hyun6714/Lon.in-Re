using System;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float moveSpeed = 100f;    // 텍스트 떠오르는 속도
    [SerializeField] private float duration = 0.5f;     // 유지 시간

    [SerializeField] private GameObject originPrefab;   // 풀링할 원본 프리팹

    private float timer;

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
        }

        timer = duration;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
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