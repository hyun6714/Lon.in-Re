using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartSaveData
{
    public string partId;
    public int level;
}

public class PlayerTapUpgrade : MonoBehaviour
{
    // 탭 파워 변경될때 수치 전달 이벤트
    public event Action<int> OnTapPowerChanged;

    [Header("기본 탭 파워")]
    [SerializeField] private int defaultBasePower = 1;

    [Header("부품 상태 목록 (3종)")]
    [SerializeField] private List<PartState> partStates = new List<PartState>();

    public List<PartState> PartStates => partStates;

    private int cachedTapPower = 1;     // 캐싱 변수

    public int CurrentTapPower => cachedTapPower;

    // 환생 이벤트 구독 / 해제
    private void OnEnable()
    {
        ReincarnationManager.OnReincarnated += ResetUpgrade;
    }

    private void OnDisable()
    {
        ReincarnationManager.OnReincarnated -= ResetUpgrade;
    }

    private void Start()
    {
        // 게임 시작 시 초기 탭 파워 1회 계산
        RecalculateTapPower();
    }

    // 최종 탭 파워 = (기본 1 + 모든 부품 파워 합) * 아티팩트 배율
    public void RecalculateTapPower()
    {
        float totalPower = defaultBasePower;

        // 1. 부품 파워 합산
        for (int i = 0; i < partStates.Count; i++)
        {
            var state = partStates[i];
            if (state != null && state.partData != null)
            {
                totalPower += state.GetTotalPower();
            }
        }

        // 2. 아티팩트 배율 반영
        if (ArtifactManager.instance != null)
        {
            float totalPercent = ArtifactManager.instance.GetTotalGainPerClick();
            totalPower *= (1f + totalPercent);
        }

        cachedTapPower = Mathf.Max(1, Mathf.RoundToInt(totalPower));

        // 최종 탭 파워 브로드캐스팅
        OnTapPowerChanged?.Invoke(cachedTapPower);
    }

    // 업그레이드 가능 여부 판별
    public bool CanUpgrade(PartState state)
    {
        if (state == null || state.partData == null || CurrencyManager.instance == null || RankManager.instance == null)
        {
            return false;
        }

        bool isUnlocked = RankManager.instance.currentRank >= state.partData.UnlockGrade;
        bool canAfford = CurrencyManager.instance.GetAmount(CurrencyType.Normal) >= state.GetNextCost();

        return isUnlocked && canAfford;
    }

    // 업그레이드 함수
    public bool TryUpgrade(PartState state)
    {
        if (!CanUpgrade(state))
        {
            return false;
        }

        int cost = state.GetNextCost();
        if (CurrencyManager.instance.UseCurrency(CurrencyType.Normal, cost))
        {
            state.LevelUp();
            RecalculateTapPower();
            return true;
        }

        return false;
    }

    // 부품 ID로 부품 상태 찾기
    public PartState GetPartState(string partId)
    {
        for (int i = 0; i < partStates.Count; i++)
        {
            PartState state = partStates[i];
            if (state != null && state.partData != null)
            {
                if (state.partData.PartId == partId)
                {
                    return state;
                }
            }
        }
        return null;
    }

    // 세이브
    public List<PartSaveData> GetSaveData()
    {
        List<PartSaveData> list = new List<PartSaveData>();

        for (int i = 0; i < partStates.Count; i++)
        {
            PartState state = partStates[i];
            if (state != null && state.partData != null)
            {
                PartSaveData data = new PartSaveData();
                data.partId = state.partData.PartId;
                data.level = state.Level;

                list.Add(data);
            }
        }

        return list;
    }

    // 로드
    public void LoadSaveData(List<PartSaveData> savedList)
    {
        if (savedList == null)
        {
            return;
        }

        for (int i = 0; i < savedList.Count; i++)
        {
            PartSaveData saved = savedList[i];
            PartState state = GetPartState(saved.partId);

            if (state != null)
            {
                state.SetLevel(saved.level);
            }
        }

        RecalculateTapPower();
    }


    // 환생 시 레벨 리셋 함수
    public void ResetUpgrade()
    {
        foreach (var state in partStates)
        {
            state.ResetLevel();
        }
        RecalculateTapPower();
    }
}
