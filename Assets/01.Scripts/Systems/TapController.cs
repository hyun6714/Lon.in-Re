using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapController : MonoBehaviour, IPointerDownHandler
{
    [Header("플레이어 업그레이드 연동")]
    [SerializeField] private PlayerTapUpgrade playerUpgrade;

    private void Awake()
    {
        // 컴포넌트 자동 연동
        if (playerUpgrade == null)
        {
            playerUpgrade = GetComponent<PlayerTapUpgrade>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 플레이어 강화 수치 가져오기
        int currentPower = playerUpgrade != null ? playerUpgrade.CurrentTapPower : 1;

        // 재화 지급
        if (CurrencyManager.instance != null)
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Normal, currentPower);
        }

        // 풀에서 텍스트 꺼내기
        UIManager.Instance.SpawnFloatingText(eventData.position, currentPower, false);
    }
}
