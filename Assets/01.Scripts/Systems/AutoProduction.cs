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

    [Header("직원 정보 가져오기")]
    [SerializeField] private EmployeeManager employee;

    [Header("일시 정지")]
    [SerializeField] private bool isPaused = false;

    private CancellationTokenSource token;

    private void Awake()
    {
        AutoProductionInit();
    }

    private void OnEnable() 
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        SubscribeEvent();

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
        if (employee != null)
        {
            employee.OnEmployeeChanged += UpdateMoneyPerSec;
        }

        if (upgrade != null)
        {
            upgrade.OnMultiplierUpgradeChanged += UpdateMoneyPerSec;
        }

        GameEventBridge.OnPausedChanged += PausedChanged;
        Utils.Log("AutoProduction 구독 완료");
    }

    private void UnSubscribeEvent()
    {
        if (employee != null)
        {
            employee.OnEmployeeChanged -= UpdateMoneyPerSec;
        }

        if (upgrade != null)
        {
            upgrade.OnMultiplierUpgradeChanged -= UpdateMoneyPerSec;
        }

        GameEventBridge.OnPausedChanged -= PausedChanged;
    }

    // n초당 생산량 갱신 함수. 고용 인원 + 업그레이드 증가량(임시 계산)
    private void UpdateMoneyPerSec()
    {
        int employeeProduction = employee != null ? employee.GetTotalProductionPerSecond() : 0;
        float upgradeProduction = upgrade != null ? upgrade.TotalPerSecond() : 0;
        float multiplier = upgrade != null ? upgrade.AutoMultiplier : 1f;

        moneyPerSec = Mathf.RoundToInt((employeeProduction + upgradeProduction) * multiplier);

        UIManager.Instance.SetText(HUDTextType.CoinSec, $"{moneyPerSec}G/초");
    }

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
                    GameEventBridge.CurrencyAdded(CurrencyType.Normal, (int)moneyPerSec);
                    
                    UIManager.Instance.SpawnFloatingText(transform.position, (int)moneyPerSec, true);
                }

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, token);
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
