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
        GameEventBridge.OnGameSettlementStarted += HandleGameSettlement;
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
        {
            GameEventBridge.OnGameSettlementStarted -= HandleGameSettlement;
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

        // 게임 정산 보상
        int currencyReward =
            gameReleaseData.GetCurrencyReward(
                gameResult.finalGrade,
                gameResult.baseDevelopmentCost
            );

        int reputationReward =
            gameReleaseData.GetReputationReward(
                gameResult.developmentRank,
                gameResult.finalGrade
            );

        GameEventBridge.CurrencyAdded(CurrencyType.Normal, currencyReward);
        GameEventBridge.CurrencyAdded(CurrencyType.Reputation, reputationReward);

        GameReleaseManager.instance.IncreaseSettlementCount(gameId);

        SoundManager.instance?.PlaySFX(SFXType.Settlement); // 게임 정산 효과음 재생

        Utils.Log(
            $"게임 정산 / ID : {gameId} / " +
            $"정산 회차 : {settlementCount} / " +
            $"제작 랭크 : {gameResult.developmentRank} / " +
            $"최종 등급 : {gameResult.finalGrade} / " +
            $"재화 보상 : {currencyReward} / " +
            $"명성 보상 : {reputationReward}"
        );
    }
}