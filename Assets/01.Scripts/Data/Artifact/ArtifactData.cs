using UnityEngine;

public enum ArtifactsType
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class ArtifactInfo
{
    public int artifactId; //고유 번호
    public ArtifactsType type;
    public string artiName;    // 아티팩트 이름
    public Sprite icon;  // UI에 띄울 아이콘 이미지

    [Header("증가 효과")]
    public float GainperClick; //클릭당 획득량
    public float PerSecond; //초당 획득량
    public float Probabilityincrease; //확률 증가 

    [Header("해금")]
    public int SpecialUnlockCost; //해금 비용
    public bool isUnlocked = false;

    [Header("조건별 필요 수치")]
    public int requiredRebirthCount; // 필요 환생 횟수 
    public int requiredReputation; // 필요 명성 수치
}
