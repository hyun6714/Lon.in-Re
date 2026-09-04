using UnityEngine;

public class GameSettlementManager : MonoBehaviour
{
    public static GameSettlementManager instance;

    [Header("게임 출시 보상 데이터")]
    [SerializeField] private GameReleaseData gameReleaseData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.instance.OnGameSettlement += HandleGameSettlement;
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.OnGameSettlement -= HandleGameSettlement;
        }
    }

    // 게임 정산 이벤트 발생 시 호출
    private void HandleGameSettlement(int gameId, int settlementCount)
    {
        GameDevResult gameResult = GameReleaseManager.instance.GetReleasedGame(gameId);

        if (gameResult == null)
        {
            Utils.Log($"정산할 게임을 찾을 수 없습니다. ID : {gameId}");
            return;
        }

        int currencyReward = 0;
        int reputationReward = 0;

        switch (gameResult.finalGrade)
        {
            case DevelopmentGrade.A:
                currencyReward = gameReleaseData.GradeACurrencyReward;
                reputationReward = gameReleaseData.GradeAReputationReward;
                break;

            case DevelopmentGrade.B:
                currencyReward = gameReleaseData.GradeBCurrencyReward;
                reputationReward = gameReleaseData.GradeBReputationReward;
                break;

            case DevelopmentGrade.C:
                currencyReward = gameReleaseData.GradeCCurrencyReward;
                reputationReward = gameReleaseData.GradeCReputationReward;
                break;
        }

        Utils.Log(
            $"게임 정산 / ID : {gameId} / " +
            $"정산 회차 : {settlementCount} / " +
            $"최종 등급 : {gameResult.finalGrade} / " +
            $"재화 보상 : {currencyReward} / " +
            $"명성 보상 : {reputationReward}"
        );
    }
}