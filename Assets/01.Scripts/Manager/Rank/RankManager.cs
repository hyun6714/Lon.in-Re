using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class RankManager : MonoBehaviour
{
    public enum RankState
    {
        Solo, //1인
        Indie, //인디
        Small, //중소
        Midsized, //중견
        MajorPublisher //대기업
    }

    public static event Action OnRankChanged;

    public static RankManager instance { get; private set; }

    [Header("등급 데이터 에셋 (순서대로 배치)")]
    [SerializeField] private List<RankData> rankDataList;

    public RankState currentRank = RankState.Solo;
    public int currentEmployeeCount = 0;

    // 현재 등급의 ScriptableObject 데이터 읽기
    public RankData CurrentRankData => GetRankData(currentRank);

    // 기존 변수 호환용 프로퍼티 (실시간 반영)
    public bool hasEmployees => CurrentRankData != null && CurrentRankData.hasEmployees;
    public int maxEmployee => CurrentRankData != null ? CurrentRankData.maxEmployee : 0;
    public int gamesReleased => GameManager.Instance != null ? GameManager.Instance.gameDevCount : 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => ReincarnationManager.OnReincarnated += ResetRank;
    private void OnDisable() => ReincarnationManager.OnReincarnated -= ResetRank;


    public RankData GetRankData(RankState state)
    {
        int index = (int)state;
        if (index >= 0 && index < rankDataList.Count)
        {
            return rankDataList[index];
        }
        return null;
    }

    public void CheckRankUp()
    {
        int currentIndex = (int)currentRank;
        int nextIndex = currentIndex + 1;

        // 최고 등급 도달 확인
        if (currentIndex >= rankDataList.Count - 1 || nextIndex >= rankDataList.Count)
        {
            Debug.Log("등급업을 더이상 못합니다");
            return;
        }

        RankData nextData = GetRankData((RankState)nextIndex);
        if (nextData == null) return;

        int currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;

        Debug.Log($"현재 명성: {currentReputation} / 요구 명성: {nextData.reqReputation}");

        // 승급 조건 검사 (다음 등급 에셋에 적힌 수치와 비교)
        bool isGameSatisfied = gamesReleased >= nextData.reqGamesReleased;
        bool isEmployeeSatisfied = currentEmployeeCount >= nextData.reqEmployeeCount;
        bool isReputationSatisfied = currentReputation >= nextData.reqReputation;

        if (isGameSatisfied && isEmployeeSatisfied && isReputationSatisfied)
        {
            currentRank = (RankState)nextIndex;
            Debug.Log($"승급 성공 현재 등급: {CurrentRankData.rankDisplayName}");

            OnRankChanged?.Invoke();
        }
        else
        {
            Debug.Log("조건이 부족합니다");
        }

    }

    //환생할 때 사용하는 데이터 초기화
    public void ResetRank()
    {
        currentRank = RankState.Solo;
        currentEmployeeCount = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameDevCount = 0;
        }
        Debug.Log("등급 초기화 완료");
    }
}
