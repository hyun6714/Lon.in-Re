using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public RankState currentRank = RankState.Solo;

    public int gamesReleased
    { 
        get
        {
            if (GameManager.Instance != null)
            {
                return GameManager.Instance.gameDevCount;
            }

            return 0;
        }
    }

    public bool hasEmployees = false; // 직원 고용 가능/불가능 
    public int maxEmployee = 0;
    public int currentEmployeeCount = 0;

    public static RankManager instance { get; private set; }

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

    //이것도 지워야함 테스트용
    private void Start()
    {
        Debug.Log($"현재 랭크:{currentRank}");
    }

    public void CheckRankUp()
    {
        int currentReputation = 0;
        if (CurrencyManager.instance != null)
        {
            currentReputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation);
            Debug.Log($"현재 명성{currentReputation}");
        }

        switch (currentRank)
        {
            case RankState.Solo:
                if (gamesReleased >= 1)
                {
                    currentRank = RankState.Indie;
                    hasEmployees = true;
                    maxEmployee = 5;
                    Debug.Log("인디");
                }
                else
                {
                    Debug.Log("조건 미달: 게임 출시 1회 이상");
                }
                break;

            case RankState.Indie:
                if (currentEmployeeCount >= 1 && currentReputation >= 500)
                {
                    currentRank = RankState.Small;
                    maxEmployee = 10;
                    Debug.Log("중소기업");
                }
                else
                {
                    Debug.Log("조건이 부족합니다");
                    return;
                }
                break;

            case RankState.Small:
                if(currentReputation >= 1000 && gamesReleased >=5)
                {
                    currentRank = RankState.Midsized;
                    maxEmployee = 20;
                    Debug.Log("중견기업");
                }
                else
                {
                    Debug.Log("조건이 부족합니다");
                }
                break;

            case RankState.Midsized:
                if (currentReputation >= 25000)
                {
                    currentRank = RankState.MajorPublisher;
                    maxEmployee = 50;
                    Debug.Log("대기업");
                }
                else
                {
                    Debug.Log("명성이 부족합니다");
                }
                break;

            case RankState.MajorPublisher:
                if(currentRank == RankState.MajorPublisher)
                {
                    Debug.Log("등급업을 더이상 못합니다");
                }
                break;
        }
        
    }

    //환생할 때 사용하는 데이터 초기화
    public void ResetRank()
    {
        currentRank = RankState.Solo;
        currentEmployeeCount = 0;
        maxEmployee = 0;
        hasEmployees = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameDevCount = 0;
        }

        PlayerPrefs.SetInt("PlayerRank", (int)currentRank);
        PlayerPrefs.Save();
        Debug.Log($"등급 초기화 완료");
    }
}
