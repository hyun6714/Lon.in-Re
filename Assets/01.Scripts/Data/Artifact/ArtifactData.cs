using System.Collections.Generic;
using UnityEngine;

public enum ArtifactsType
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum EffectType
{
    GainPerClick,
    PerSecond,
    ProbabilityIncrease,
}

[System.Serializable]
public class ArtifactEffect
{
    public EffectType effectType;
    public float effectValue;    // 증가 수치
}

[System.Serializable]
public class ArtifactInfo
{
    public int artifactId; //고유 번호
    public ArtifactsType type;
    public string artiName;    // 아티팩트 이름
    public Sprite icon;  // UI에 띄울 아이콘 이미지

    [Header("증가 효과 리스트")]
    public List<ArtifactEffect> effects = new List<ArtifactEffect>();

    [Header("해금")]
    public int SpecialUnlockCost; //해금 비용

    [Header("조건별 필요 수치")]
    public int requiredRebirthCount; // 필요 환생 횟수 
    public int requiredReputation; // 필요 명성 수치
}
