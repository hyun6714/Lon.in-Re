using System;
using System.Collections.Generic;
using UnityEngine;


public class CurrencyManager : MonoBehaviour
{

    public static CurrencyManager instance { get; private set; }

    private Dictionary<CurrencyType, long> currentCurrencies = new Dictionary<CurrencyType, long>();

    [SerializeField] private CurrencyDatabase currencyDatabase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeCurrencies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEventBridge.OnCurrencyAdded += AddCurrency;
        GameEventBridge.OnCurrencyUsed += UseCurrency;

        GameEventBridge.OnReincarnated += ResetCurrenciesExceptSpecial;
        GameEventBridge.OnCurrencyExchangeRequested += HandleExchange;
    }

    private void OnDisable()
    {
        GameEventBridge.OnCurrencyAdded -= AddCurrency;
        GameEventBridge.OnCurrencyUsed -= UseCurrency;

        GameEventBridge.OnReincarnated -= ResetCurrenciesExceptSpecial;
        GameEventBridge.OnCurrencyExchangeRequested -= HandleExchange;
    }

    private void InitializeCurrencies()
    {
        if (currencyDatabase == null)
        {
            Debug.Log("재화 데이터베이스가 없습니다");
            return;
        }

        foreach (CurrencyInfo info in currencyDatabase.currencies)
        {
            currentCurrencies[info.type] = info.initialAmount;
        }
    }


    //특정 재화 정보 가져오기
    public CurrencyInfo GetInfo(CurrencyType type)
    {
        return currencyDatabase.GetCurrencyInfo(type);
    }

    //현재 유저가 가진 재화 수량 반환
    public long GetAmount(CurrencyType type)
    {
        if (currentCurrencies.TryGetValue(type, out long amount))
        {
            return amount;
        }
        return 0;
    }

    //재화 획득
    public void AddCurrency(CurrencyType type, long amount)
    {
        if (amount <= 0)
            return;

        currentCurrencies[type] = GetAmount(type) + amount;

        GameEventBridge.CurrencyChanged(type, currentCurrencies[type]);
    }

    //재화 차감 
    public bool UseCurrency(CurrencyType type, long amount)
    {
        if (amount <= 0) return false;

        long current = GetAmount(type);
        if (current < amount)
        {
            Debug.Log($"{type} 재화가 부족합니다");
            return false;
        }

        currentCurrencies[type] = current - amount;

        GameEventBridge.CurrencyChanged(type, currentCurrencies[type]);

        return true;
    }

    // 재화 값 설정
    public void SetCurrency(CurrencyType type, long amount)
    {
        currentCurrencies[type] = amount;

        GameEventBridge.CurrencyChanged(type, currentCurrencies[type]);
    }

    //환생전용 특수 재화 제외 초기화 
    public void ResetCurrenciesExceptSpecial()
    {
        if (currencyDatabase == null) return;

        foreach (CurrencyInfo info in currencyDatabase.currencies)
        {
            if (info.type == CurrencyType.Special)
            {
                continue;
            }

            currentCurrencies[info.type] = info.initialAmount;

            GameEventBridge.CurrencyChanged(info.type, info.initialAmount);
        }

        Debug.Log("특수 재화를 제외한 모든 재화가 초기화되었습니다.");
    }

    private void HandleExchange(ExchangeDataSO exchangeData)
    {
        if (exchangeData == null) return;

       
        bool isSuccess = UseCurrency(exchangeData.SourceCurrencyType, exchangeData.CostAmount);

        if (isSuccess)
        {

            AddCurrency(exchangeData.TargetCurrencyType, exchangeData.RewardAmount);
            Debug.Log($"교환 성공: {exchangeData.SourceCurrencyType} -{exchangeData.CostAmount:N0} / {exchangeData.TargetCurrencyType} +{exchangeData.RewardAmount:N0}");
        }
        else
        {
            Debug.Log("재화가 부족합니다.");
            // TODO: 재화 부족 팝업 연출 추가
        }
    }
}
