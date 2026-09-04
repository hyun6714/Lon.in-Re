using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapController : MonoBehaviour, IPointerDownHandler
{
    [Header("플레이어 업그레이드 연동")]
    [SerializeField] private PlayerTapUpgrade playerUpgrade;

    [Header("풀링 연동 설정")]
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private Transform effectCanvasTransform; // 이펙트가 스폰될 Canvas (부모)
    [SerializeField] private int initialPoolSize = 20;

    private void Awake()
    {
        // 컴포넌트 자동 연동
        if (playerUpgrade == null)
        {
            playerUpgrade = GetComponent<PlayerTapUpgrade>();
        }

        if(effectCanvasTransform == null)
        {
            Canvas ParentCanvas = GetComponentInParent<Canvas>();
            if(ParentCanvas != null)
            {
                effectCanvasTransform = ParentCanvas.transform;
            }
        }
    }

    private void Start()
    {
        // 예외처리
        if (ObjectPoolManager.instance != null && floatingTextPrefab != null)
        {
            ObjectPoolManager.instance.CreatePool(floatingTextPrefab.gameObject, initialPoolSize);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 플레이어 강화 수치 가져오기
        int currentPower = playerUpgrade != null ? playerUpgrade.CurrentTapPower : 1;

        // 재화 지급
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddCurrency(CurrencyType.Normal, currentPower);
        }

        // 풀에서 텍스트 꺼내기
        SpawnFloatingText(eventData.position, currentPower);
    }

    private void SpawnFloatingText(Vector2 screenPosition, int power)
    {
        if (floatingTextPrefab == null || ObjectPoolManager.instance == null)
        {
            return;
        }

        // 매니저에서 텍스트 오브젝트 꺼내오기 (부모는 Canvas로 지정)
        FloatingText textObj = ObjectPoolManager.instance.GetObject<FloatingText>(
            floatingTextPrefab.gameObject,
            effectCanvasTransform
        );

        if (textObj != null)
        {

            textObj.transform.position = screenPosition;
            textObj.SetOriginPrefab(floatingTextPrefab.gameObject);
            // CurrencyFormatter 적용
            textObj.Setup($"+{CurrencyFormatter.Format(power)}");
        }
    }
}
