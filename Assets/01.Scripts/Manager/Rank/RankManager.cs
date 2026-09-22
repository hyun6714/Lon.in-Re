using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;


public class RankManager : MonoBehaviour , IGameDataGet<RankManager.RankState>
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

        UpdateOfficeVisual();
    }

    private void OnEnable()
    {
        GameDataGetter<RankState>.Register(this);

        GameEventBridge.OnReincarnated += ResetRank;
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void OnDisable()
    {
        GameDataGetter<RankState>.UnRegister(this);

        GameEventBridge.OnReincarnated -= ResetRank;
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

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

        long currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;
        long currentGold = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Normal) : 0;

        // 승급 조건 검사 (다음 등급 에셋에 적힌 수치와 비교)
        bool isGameSatisfied = gamesReleased >= nextData.reqGamesReleased;
        bool isEmployeeSatisfied = currenyEmployee >= nextData.reqEmployeeCount;
        bool isReputationSatisfied = currentReputation >= nextData.reqReputation;

        bool isGoldSatisfied = currentGold >= nextData.reqNormal;

        if (isGameSatisfied && isEmployeeSatisfied && isReputationSatisfied && isGoldSatisfied)
        {
            if (CurrencyManager.instance != null)
            {
                CurrencyManager.instance.UseCurrency(CurrencyType.Normal, nextData.reqNormal);
            }
            currentRank = (RankState)nextIndex;

            UpdateOfficeVisual();

            GameEventBridge.RankChanged();
            GameEventBridge.EmployeeCountChanged(currenyEmployee, maxEmployee);
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedInitializeRank());
    }

    private IEnumerator DelayedInitializeRank()
    {
        yield return null; 

        UpdateOfficeVisual();

        if (RankUI.instance != null)
        {
            RankUI.instance.UpdateRankUI();
        }
    }

    public void UpdateOfficeVisual()
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

    public RankState GetData()
    {
        return currentRank;
    }

}
