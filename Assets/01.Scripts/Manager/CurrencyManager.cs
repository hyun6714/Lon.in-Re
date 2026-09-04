using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance { get; private set; }

    private Dictionary<CurrencyType, int> currentCurrencies = new Dictionary<CurrencyType, int>();

    public event Action<CurrencyType, int> OnCurrencyChanged;

    [SerializeField] private CurrencyDatabase currencyDatabase;

    // 임시 연동
    [SerializeField] private AutoProduction auto;

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
        if (auto != null)
        {
            auto.OnNormalCurrencyChanged += AddCurrency;
        }
    }

    private void OnDisable()
    {
        if (auto != null)
        {
            auto.OnNormalCurrencyChanged -= AddCurrency;
        }
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
            string saveKey = $"Currency_{info.type}";

            int amount = PlayerPrefs.GetInt(saveKey, info.initialAmount);

            currentCurrencies[info.type] = amount;
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

        OnCurrencyChanged?.Invoke(type, currentCurrencies[type]);
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

        OnCurrencyChanged?.Invoke(type, currentCurrencies[type]);

        return true;
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
            string saveKey = $"Currency_{info.type}";
            PlayerPrefs.SetInt(saveKey, info.initialAmount);

            OnCurrencyChanged?.Invoke(info.type, info.initialAmount);
        }
        PlayerPrefs.Save();
        Debug.Log("특수 재화를 제외한 모든 재화가 초기화되었습니다.");
    }
}
