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

        // 게임 전체 정산 보상
        int totalCurrencyReward =
            gameReleaseData.GetCurrencyReward(
                gameResult.finalGrade,
                gameResult.baseDevelopmentCost
            );

        int totalReputationReward =
            gameReleaseData.GetReputationReward(
                gameResult.finalGrade
            );

        int settlementNum = EventManager.instance.SettlementNum;

        if (settlementNum <= 0)
        {
            Utils.Log("게임 정산 횟수가 설정되어 있지 않습니다.");
            return;
        }

        // 기본 분할 보상
        int currencyReward = totalCurrencyReward / settlementNum;
        int reputationReward = totalReputationReward / settlementNum;

        // 나누고 남은 값은 마지막 정산 때 지급
        if (settlementCount == settlementNum)
        {
            currencyReward += totalCurrencyReward % settlementNum;
            reputationReward += totalReputationReward % settlementNum;
        }

        GameEventBridge.CurrencyAdded(CurrencyType.Normal, currencyReward);
        GameEventBridge.CurrencyAdded(CurrencyType.Reputation, reputationReward);

        GameReleaseManager.instance.IncreaseSettlementCount(gameId);

        Utils.Log(
            $"게임 정산 / ID : {gameId} / " +
            $"정산 회차 : {settlementCount} / " +
            $"최종 등급 : {gameResult.finalGrade} / " +
            $"재화 보상 : {currencyReward} / " +
            $"명성 보상 : {reputationReward}"
        );
    }
}