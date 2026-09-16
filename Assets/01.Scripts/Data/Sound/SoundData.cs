using System;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    Tap,            // 탭
    ButtonClick,    // 기본 UI 버튼
    UpgradeSuccess, // 부품 강화 성공
    UpgradeFail,    // 부품 강화 실패
    LevelUp,        // 레벨업
    Hire,           // 직원 고용
    GameRelease,    // 게임 출시
    Reincarnation   // 환생 연출
}

public enum BGMType
{
    Main,           // 기본
    Spring,         // 봄
    Summer,         // 여름
    Fall,           // 가을
    Winter,         // 겨울
    Burning         // 버닝
}

[Serializable]
public struct SFXData
{
    public SFXType sfxType;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
}

[Serializable]
public struct BGMData
{
    public BGMType bgmType;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData")]
public class SoundData : ScriptableObject
{
    [Header("BGM 목록")]
    public List<BGMData> bgmList = new List<BGMData>();

    [Header("SFX 목록")]
    public List<SFXData> sfxList = new List<SFXData>();
}