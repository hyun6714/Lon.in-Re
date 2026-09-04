using UnityEngine;

// 확률 강화
// 세부 등급 랜덤 생성 
// 최종 등급 계산
// 게임 개발 

public class GameDevManager : MonoBehaviour
{
    public static GameDevManager instance;

    [Header("게임 개발 데이터")]
    [SerializeField] private GameDevData gameDevData;

    // 현재 확률 추가 횟수
    private int probabilityUpgradeCount = 0;

    private int nextGameId = 1;

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

    // 현재 A등급 확률 계산
    public float GetCurrentARate()
    {
        return gameDevData.GradeARate + (gameDevData.GradeAIncreaseRate * probabilityUpgradeCount);
    }

    // 현재 B등급 확률 계산
    public float GetCurrentBRate()
    {
        return gameDevData.GradeBRate + (gameDevData.GradeBIncreaseRate * probabilityUpgradeCount);
    }

    // 현재 C등급 확률 계산
    public float GetCurrentCRate()
    {
        return gameDevData.GradeCRate - (gameDevData.GradeCDecreaseRate * probabilityUpgradeCount);
    }

    // 추가 비용을 지불하고 개발 확률 증가
    public void UpgradeProbability()
    {
        // 최대 추가 횟수 확인
        if (probabilityUpgradeCount >= gameDevData.MaxProbabilityUpgradeCount)
        {
            Utils.Log("더 이상 확률을 올릴 수 없습니다.");
            return;
        }

        if (CurrencyManager.instance == null)
        {
            Utils.Log("CurrencyManager를 찾을 수 없습니다.");
            return;
        }

       // 추가 비용 차감
        bool success = CurrencyManager.instance.UseCurrency(CurrencyType.Normal, gameDevData.ProbabilityUpgradeCost);

        // 재화가 부족하면 취소
        if (!success)
        {
            return;
        }

        // 확률 증가 횟수 +1
        probabilityUpgradeCount++;

        Utils.Log($"확률 증가 완료 / A : {GetCurrentARate()}% / " +$"B : {GetCurrentBRate()}% / " +$"C : {GetCurrentCRate()}%");
    }

    // 확률에 따라 A, B,C 중 하나를 랜덤으로 결정
    public DevelopmentGrade GetRandomGrade()
    {
        float aRate = GetCurrentARate();
        float bRate = GetCurrentBRate();
        float cRate = GetCurrentCRate();


        // A + B + C 등급 확률의 총합 계산
        // 전체 합이 100%가 되도록 설정
        float totalRate = aRate + bRate + cRate;

        // 총 확률이 100%가 아니면 실행 안댐
        if (totalRate != 100f)
        {
            Utils.Log("확률 총합은 100%가 되어야 합니다");
            return DevelopmentGrade.C;
        }

        float randomValue = Random.Range(0f, 100f);

        // A 등급
        // 기본 확률 기준 : 0 이상 33 미만
        if (randomValue < aRate)
        {
            return DevelopmentGrade.A;
        }

        // B 등급
        // 기본 확률 기준 : 33 이상 66 미만
        if (randomValue < aRate + bRate)
        {
            return DevelopmentGrade.B;
        }

        // 나머지 C 등급 66이상 100이하
        return DevelopmentGrade.C;
    }

    // 게임 개발 결과 계산
    public GameDevResult GameResult()
    {
        GameDevResult result = new GameDevResult();

        // 개발된 게임 고유 ID 부여
        result.gameId = nextGameId;
        nextGameId++;

        // 각 항목별 등급 랜덤 결정
        result.funGrade = GetRandomGrade();
        result.graphicGrade = GetRandomGrade();
        result.optimizationGrade = GetRandomGrade();

        result.finalGrade = CalculateFinalGrade(result);

        return result;
    }

    // 재미, 그래픽, 최적화 점수를 합산하여 최종 등급 결정
    // 총점은 임시로 설정해둠
    public DevelopmentGrade CalculateFinalGrade(GameDevResult result)
    {
        int totalScore =(int)result.funGrade + (int)result.graphicGrade + (int)result.optimizationGrade;

        // 총점 7 ~ 9점 = A
        if (totalScore >= gameDevData.FinalAGradeScore)
        {
            return DevelopmentGrade.A;
        }

        // 총점 5 ~ 6점 = B
        if (totalScore >= gameDevData.FinalBGradeScore)
        {
            return DevelopmentGrade.B;
        }

        // 총점 3 ~ 4점 = C
        return DevelopmentGrade.C;
    }

    // 게임 개발
    public GameDevResult DevelopGame()
    {
        if (CurrencyManager.instance == null)
        {
            Utils.Log("CurrencyManager를 찾을 수 없습니다.");
            return null;
        }

        // 기본 개발 비용 차감
        bool success = CurrencyManager.instance.UseCurrency(CurrencyType.Normal, gameDevData.DevelopmentCost);

        // 개발 비용이 부족하면 취소
        if (!success)
        {
            Utils.Log("게임 개발 비용이 부족합니다.");
            return null;
        }

        // 게임 개발 결과 생성
        GameDevResult result = GameResult();
        GameManager.Instance.gameDevCount++;

        // 개발 완료 즉시 게임 출시
        GameReleaseManager.instance.ReleaseGame(result);

        // 이번 개발에 사용한 확률 강화 횟수 초기화
        probabilityUpgradeCount = 0;

        Utils.Log(
            $"게임 개발 완료 / ID : { result.gameId} / " +
            $"재미 : {result.funGrade} / " +
            $"그래픽 : {result.graphicGrade} / " +
            $"최적화 : {result.optimizationGrade} / " +
            $"최종 등급 : {result.finalGrade}"
        );

        return result;
    }

    // 버튼 테스트용
    public void TestDevelopGame()
    {
        DevelopGame();
    }
}

