using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapController : MonoBehaviour, IPointerDownHandler
{
    [Header("플레이어 업그레이드 연동")]
    [SerializeField] private PlayerTapUpgrade playerUpgrade;

    [Header("터치 이펙트 연동")]
    [SerializeField] private TapEffect tapEffectPrefab;  // 이펙트 프리팹
    [SerializeField] private Transform effectParent;     // 띄울 캔버스

    private Camera camera;

    private void Awake()
    {
        // 다중 터치 활성화
        Input.multiTouchEnabled = true;

        // 컴포넌트 자동 연동
        if (playerUpgrade == null)
        {
            playerUpgrade = GetComponent<PlayerTapUpgrade>();
        }

        camera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TryClickTreasureGoblin(eventData.position))
            return;

        SoundManager.instance?.PlaySFX(SFXType.Tap, true);  // 탭 효과음 재생

        // 플레이어 강화 수치 가져오기
        int currentPower = playerUpgrade != null ? playerUpgrade.CurrentTapPower : 1;

        // 재화 지급
        if (CurrencyManager.instance != null)
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Normal, currentPower);
        }

        // 풀에서 텍스트 꺼내기
        UIManager.Instance.SpawnFloatingText(eventData.position, currentPower, false);

        // 터치 좌표에 이펙트 출력
        SpawnTapEffect(eventData.position);
    }

    private bool TryClickTreasureGoblin(Vector2 screenPos)
    {
        if (EventManager.instance == null)
        {
            return false;
        }

        TreasureGoblinEvent goblinEvent = EventManager.instance.GetActiveEvent(
            GameEventType.TreasureGoblin) as TreasureGoblinEvent;

        if (goblinEvent == null || goblinEvent.IsFinished)
            return false;

        Vector2 worldPos = camera.ScreenToWorldPoint(screenPos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit == null)
        {
            return false;
        }

        TreasureGoblin goblin = hit.GetComponent<TreasureGoblin>();

        if (goblin == null)
            return false;

        goblin.OnClick();

        return true;
    }

    private void SpawnTapEffect(Vector2 position)
    {
        if (tapEffectPrefab == null || ObjectPoolManager.instance == null)
        {
            return;
        }

        Transform parent = effectParent != null ? effectParent : transform;
        TapEffect effect = ObjectPoolManager.instance.GetObject<TapEffect>(tapEffectPrefab.gameObject, parent);

        if (effect != null)
        {
            effect.transform.position = position;
            effect.SetOriginPrefab(tapEffectPrefab.gameObject);
            effect.PlayEffect();
        }
    }
}
