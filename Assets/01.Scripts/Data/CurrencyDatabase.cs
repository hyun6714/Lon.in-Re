using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyDatabase", menuName = "Game/Currency Database")]
public class CurrencyDatabase : ScriptableObject
{
    [Header("재화 설정 리스트")]
    public List<CurrencyInfo> currencies = new List<CurrencyInfo>();

    // 특정 타입의 재화 정보를 쉽게 찾아오는 함수
    public CurrencyInfo GetCurrencyInfo(CurrencyType type)
    {
        return currencies.Find(c => c.type == type);
    }
}
