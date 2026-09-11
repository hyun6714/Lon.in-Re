using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public TextMeshProUGUI currencyText;

    public void CurrencyTestSet()
    {
        if (currencyText != null && CurrencyManager.instance != null)
        {
            int normal = CurrencyManager.instance.GetAmount(CurrencyType.Normal);
            int special = CurrencyManager.instance.GetAmount(CurrencyType.Special);
            int reputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation);

            currencyText.text = $"일반: {normal:N0}\n특수: {special:N0}\n명성: {reputation:N0}";
        }
    }

    public static CurrencyManager instance { get; private set; }

    private Dictionary<CurrencyType, int> currentCurrencies = new Dictionary<CurrencyType, int>();

    public event Action<CurrencyType, int> OnCurrencyChanged;

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

        CurrencyTestSet();
    }

    private void OnEnable()
    {
        GameEventBridge.OnCurrencyAdded += AddCurrency;
        GameEventBridge.OnCurrencyUsed += UseCurrency;

        ReincarnationManager.OnReincarnated += ResetCurrenciesExceptSpecial;
    }

    private void OnDisable()
    {
        GameEventBridge.OnCurrencyAdded -= AddCurrency;
        GameEventBridge.OnCurrencyUsed -= UseCurrency;

        ReincarnationManager.OnReincarnated -= ResetCurrenciesExceptSpecial;
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
    public int GetAmount(CurrencyType type)
    {
        if (currentCurrencies.TryGetValue(type, out int amount))
        {
            return amount;
        }
        return 0;
    }

    //재화 획득
    public void AddCurrency(CurrencyType type, int amount)
    {
        if (amount <= 0)
            return;

        currentCurrencies[type] = GetAmount(type) + amount;

        GameEventBridge.CurrencyChanged(type, currentCurrencies[type]);
    }

    //재화 차감 
    public bool UseCurrency(CurrencyType type, int amount)
    {
        if (amount <= 0) return false;

        int current = GetAmount(type);
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
    public void SetCurrency(CurrencyType type, int amount)
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
}
