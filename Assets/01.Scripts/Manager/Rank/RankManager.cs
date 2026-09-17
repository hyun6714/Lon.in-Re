using System.Collections.Generic;
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

    public static RankManager instance { get; private set; }

    [Header("등급별 배경 오브젝트 (Solo, Indie, Small, Midsized, Major 순서로 씬 오브젝트 드래그)")]
    [SerializeField] private List<GameObject> backgroundObjectList;

    [Header("등급 데이터 에셋 (순서대로 배치)")]
    [SerializeField] private List<RankData> rankDataList;

    public RankState currentRank = RankState.Solo;

    // 현재 등급의 ScriptableObject 데이터 읽기
    public RankData CurrentRankData => GetRankData(currentRank);

    // 기존 변수 호환용 프로퍼티 (실시간 반영)
    public bool hasEmployees => CurrentRankData != null && CurrentRankData.hasEmployees;
    public int maxEmployee => CurrentRankData != null ? CurrentRankData.maxEmployee : 0;
    public int gamesReleased => GameManager.instance != null ? GameManager.instance.gameDevCount : 0;
    public int currenyEmployee => GameManager.instance != null ? GameManager.instance.currentEmployeeCount : 0;

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
    private void Start()
    {
        // 게임 시작 시 현재 랭크 배경 반영
        UpdateOfficeVisual();
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
        RankUI.instance.UpdateRankUI();
        int currentIndex = (int)currentRank;
        int nextIndex = currentIndex + 1;

        // 최고 등급 도달 확인
        if (currentIndex >= rankDataList.Count - 1 || nextIndex >= rankDataList.Count)
        {
            return;
        }

        RankData nextData = GetRankData((RankState)nextIndex);
        if (nextData == null) return;

        int currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;

        // 승급 조건 검사 (다음 등급 에셋에 적힌 수치와 비교)
        bool isGameSatisfied = gamesReleased >= nextData.reqGamesReleased;
        bool isEmployeeSatisfied = currenyEmployee >= nextData.reqEmployeeCount;
        bool isReputationSatisfied = currentReputation >= nextData.reqReputation;

        if (isGameSatisfied && isEmployeeSatisfied && isReputationSatisfied)
        {
            currentRank = (RankState)nextIndex;

            UpdateOfficeVisual();

            GameEventBridge.RankChanged();
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

        if (GameManager.instance != null)
        {
            GameManager.instance.gameDevCount = 0;
            GameManager.instance.currentEmployeeCount = 0;
        }

        UpdateOfficeVisual();
        Debug.Log("등급 초기화 완료");
    }

    private void UpdateOfficeVisual()
    {
        int currentIndex = (int)currentRank;

        for (int i = 0; i < backgroundObjectList.Count; i++)
        {
            if (backgroundObjectList[i] != null)
            {
                // 현재 등급 인덱스와 일치하는 배경만 켜고 나머지는 끕니다.
                bool isActive = (i == currentIndex);
                backgroundObjectList[i].SetActive(isActive);
            }
        }
    }

}
