using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class AutoProduction : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private AutoProductionData data;

    [Header("테스트 확인용")]
    [SerializeField] private float nowMoney;

    [Header("n초당 생산량")]
    [SerializeField] private float moneyPerSec;

    [Header("n초")]
    [SerializeField] private float autoSec;

    [Header("업그레이드")]
    [SerializeField] private AutoProductionUpgrade upgrade;

    [Header("풀링 연동")]
    [SerializeField] private FloatingText textPrefab;
    [SerializeField] private Transform effectCanvasTransform;

    [Header("직원 정보 가져오기")]
    [SerializeField] private EmployeeManager employee;

    [Header("일시 정지")]
    [SerializeField] private bool isPaused = false;

    private CancellationTokenSource token;

    // 돈 획득 시 발생할 이벤트
    public event Action<CurrencyType, int> OnNormalCurrencyChanged;

    private void Awake()
    {
        AutoProductionInit();
    }

    private void OnEnable() 
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        if (employee != null)
        {
            SubscribeEvent();
        }

        AutoMoneyProduct(token.Token).Forget();
    }

    private void OnDisable()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        UnSubscribeEvent();
    }

    private void AutoProductionInit()
    {
        moneyPerSec = data.BaseMoneyPerSec;
        autoSec = data.BaseAutoSec;
    }

    private void SubscribeEvent()
    {
        employee.OnEmployeeChanged += UpdateMoneyPerSec;
        upgrade.OnMultiplierUpgradeChanged += UpdateMoneyPerSec;
        GameEventBridge.OnPausedChanged += PausedChanged;
        Utils.Log("구독 완료");
    }

    private void UnSubscribeEvent()
    {
        employee.OnEmployeeChanged -= UpdateMoneyPerSec;
        upgrade.OnMultiplierUpgradeChanged -= UpdateMoneyPerSec;
        GameEventBridge.OnPausedChanged -= PausedChanged;
    }

    // n초당 생산량 갱신 함수. 고용 인원 + 업그레이드 증가량(임시 계산)
    private void UpdateMoneyPerSec()
    {
        int employeeProduction = employee != null ? employee.GetTotalProductionPerSecond() : 0;
        float upgradeProduction = upgrade != null ? upgrade.TotalPerSecond() : 0;
        float multiplier = upgrade != null ? upgrade.AutoMultiplier : 1f;

        moneyPerSec = Mathf.RoundToInt((employeeProduction + upgradeProduction) * multiplier);
        //moneyPerSec *= multiplier;
    }

    // n초당 생산량 갱신 테스트 용
    //private async UniTaskVoid UpdateMoneyPerSecCheck(CancellationToken token)
    //{
    //    while (!token.IsCancellationRequested)
    //    {
    //        UpdateMoneyPerSec();

    //        await UniTask.Delay(1000, cancellationToken: token);
    //    }
    //}

    // 자동 생산
    private async UniTaskVoid AutoMoneyProduct(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(autoSec), cancellationToken: token);

                // n초당 생산량이 0일 때 연산X
                if (moneyPerSec != 0)
                {
                    nowMoney += moneyPerSec;
                    OnNormalCurrencyChanged?.Invoke(CurrencyType.Normal, (int)moneyPerSec);
                    SpawnFloatingText(transform.position);
                }

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, token);
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    // 텍스트 특정 위치에 출력. 현재는 해당 오브젝트 상단에 출력
    public void SpawnFloatingText(Vector2 pos)
    {
        if (textPrefab == null || ObjectPoolManager.instance == null)
            return;
        GameObject text = textPrefab.gameObject;
        
        FloatingText textObj = ObjectPoolManager.instance.GetObject<FloatingText>(
            textPrefab.gameObject,
            effectCanvasTransform
            );

        if (textObj != null)
        {
            RectTransform rect = textObj.GetComponent<RectTransform>();

            
            if (rect != null && effectCanvasTransform is RectTransform canvasRect)
            {
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPos,
                    canvasRect.GetComponent<Canvas>().worldCamera,
                    out Vector2 localPoint
                    );

                rect.anchoredPosition = localPoint + new Vector2(0f, 100f);
            }
            textObj.SetOriginPrefab(textPrefab.gameObject);
            textObj.Setup($"+{moneyPerSec}");
        }
    }

    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
