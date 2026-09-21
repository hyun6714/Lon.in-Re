using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class AutoProduction : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private AutoProductionData data;

    [Header("n초당 생산량")]
    [SerializeField] private float moneyPerSec;

    public float MoneyPerSec => moneyPerSec;

    [Header("n초")]
    [SerializeField] private float autoSec;

    [Header("업그레이드")]
    [SerializeField] private AutoProductionUpgrade upgrade;

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
        if (data == null)
        {
            Utils.Log("자동 생산 데이터가 존재하지 않습니다.");
            return;
        }

        moneyPerSec = data.BaseMoneyPerSec;
        autoSec = data.BaseAutoSec;
    }

    private void SubscribeEvent()
    {

        if (upgrade != null)
        {
            upgrade.OnAutoUpgradeChanged += UpdateMoneyPerSec;
        }

        GameEventBridge.OnPausedChanged += PausedChanged;
        Utils.Log("AutoProduction 구독 완료");
    }

    private void UnSubscribeEvent()
    {

        if (upgrade != null)
        {
            upgrade.OnAutoUpgradeChanged -= UpdateMoneyPerSec;
        }

        GameEventBridge.OnPausedChanged -= PausedChanged;
    }

    // n초당 생산량 갱신 함수. 고용 인원 + 업그레이드 증가량
    private void UpdateMoneyPerSec()
    {
        Utils.Log("업데이트 시작");
        if (upgrade == null)
        {
            Utils.Log("자동 생산 업그레이드가 존재하지 않습니다.");
            return;
        }

        moneyPerSec = upgrade.TotalPerSec;
        Utils.Log(string.Format(data.MoneyPerSecText, moneyPerSec));

        UIManager.Instance.SetText(HUDTextType.CoinSec, 
            string.Format(data.MoneyPerSecText, moneyPerSec));
    }

    /// <summary>
    /// 자동 생산 비동기 함수
    /// </summary>
    /// <param name="token"> UniTask 토큰 </param>
    /// <returns></returns>
    private async UniTaskVoid AutoMoneyProduct(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(autoSec), cancellationToken: token);

                if (moneyPerSec != 0)
                {
                    GameEventBridge.CurrencyAdded(CurrencyType.Normal, (int)moneyPerSec);
                    
                    UIManager.Instance.SpawnFloatingText(transform.position, (int)moneyPerSec, true);
                }
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    // 정지 상태 확인용
    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
