using UnityEngine;

public enum CurrencyType
{
    Normal, //일반
    Special,//특수
    Reputation // 명성 
}

[System.Serializable]
public class CurrencyInfo
{
    public CurrencyType type;
    public string currencyName;    // 재화 이름
    public Sprite icon;            // UI에 띄울 아이콘 이미지
    public int initialAmount = 0;  //시작 시 기본 지급 재화량
}

