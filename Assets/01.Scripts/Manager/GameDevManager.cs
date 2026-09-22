using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 확률 강화
// 세부 등급 랜덤 생성 
// 최종 등급 계산
// 게임 개발 

public class GameDevManager : MonoBehaviour
{
    public static GameDevManager instance;

    [Header("게임 개발 데이터")]
    [SerializeField] private GameDevData gameDevData;

    [Header("게임 개발 UI")]
    [SerializeField] private TMP_Text probabilityCountText;
    [SerializeField] private Image funIcon;
    [SerializeField] private Image graphicIcon;
    [SerializeField] private Image optimizationIcon;
    [SerializeField] private TMP_Text developmentCostText;

    [Header("등급 이미지")]
    [SerializeField] private Sprite defaultGradeSprite;
    [SerializeField] private Sprite aGradeSprite;
    [SerializeField] private Sprite bGradeSprite;
    [SerializeField] private Sprite cGradeSprite;

    // 현재 확률 추가 횟수
    private int probabilityUpgradeCount = 0;

    private int nextGameId = 1;

    public int MaxActiveGameCount => gameDevData.MaxActiveGameCount;

    // 저장된 게임 ID 불러오기
    public void LoadNextGameId(int value)
    {
        nextGameId = value;
    }

    public int NextGameId => nextGameId;
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

    // 현재 랭크의 기본 게임 제작비
    private int GetBaseDevelopmentCost()
    {
        return gameDevData.GetDevelopmentCost(
            RankManager.instance.currentRank
        );
    }
    public int CurrentBaseDevelopmentCost => GetBaseDevelopmentCost();

    // 확률 강화 1회 비용
    private int GetProbabilityUpgradeCost()
    {
        return Mathf.RoundToInt(
            GetBaseDevelopmentCost() *
            gameDevData.ProbabilityUpgradeCostRate
        );
    }

    // 확률 강화 비용까지 포함한 최종 제작비
    private int GetTotalDevelopmentCost()
    {
        return GetBaseDevelopmentCost() +
            (GetProbabilityUpgradeCost() * probabilityUpgradeCount);
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

        probabilityUpgradeCount++;

        UpdateProbabilityCountText();
        UpdateDevelopmentCostText();

        Utils.Log(
            $"확률 증가 완료 / A : {GetCurrentARate()}% / " +
            $"B : {GetCurrentBRate()}% / " +
            $"C : {GetCurrentCRate()}%"
        );
    }

    // 확률 강화 횟수 감소
    public void DecreaseProbability()
    {
        if (probabilityUpgradeCount <= 0)
        {
            return;
        }

        probabilityUpgradeCount--;

        UpdateProbabilityCountText();
        UpdateDevelopmentCostText();

        Utils.Log(
            $"확률 강화 취소 / 현재 강화 횟수 : {probabilityUpgradeCount}"
        );
    }


    // 확률 강화 횟수 UI 갱신
    private void UpdateProbabilityCountText()
    {
        probabilityCountText.text = probabilityUpgradeCount.ToString();
    }

    // 게임 만들기 버튼 비용 UI 갱신
    private void UpdateDevelopmentCostText()
    {
        developmentCostText.text =
            CurrencyFormatter.Format(GetTotalDevelopmentCost());
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

        // 현재 정산 진행 중인 게임 개수
          int activeGameCount = GameReleaseManager.instance.GetActiveGameCount(EventManager.instance.SettlementNum);

        // 최대 동시 개발 게임 수 초과 시 개발 불가
        if (activeGameCount >= gameDevData.MaxActiveGameCount)
        {
            Utils.Log(
                $"동시 개발 가능한 게임 수를 초과했습니다. " +
                $"현재 : {activeGameCount} / 최대 : {gameDevData.MaxActiveGameCount}"
            );

            return null;
        }

        int totalDevelopmentCost = GetTotalDevelopmentCost();

        //기본 개발 비용 차감
        bool success = CurrencyManager.instance.UseCurrency(CurrencyType.Normal, totalDevelopmentCost);

        // 개발 비용이 부족하면 취소
        if (!success)
        {
            Utils.Log("게임 개발 비용이 부족합니다.");
            return null;
        }

        // 게임 개발 결과 생성
        GameDevResult result = GameResult();
        result.developmentRank = RankManager.instance.currentRank;
        result.baseDevelopmentCost = GetBaseDevelopmentCost();
        GameManager.instance.gameDevCount++;

        // 개발 완료 즉시 게임 출시
        GameReleaseManager.instance.ReleaseGame(result);

        // 이번 개발에 사용한 확률 강화 횟수 초기화
        probabilityUpgradeCount = 0;
        UpdateProbabilityCountText();
        UpdateDevelopmentCostText();

        Utils.Log(
            $"게임 개발 완료 / ID : { result.gameId} / " +
            $"재미 : {result.funGrade} / " +
            $"그래픽 : {result.graphicGrade} / " +
            $"최적화 : {result.optimizationGrade} / " +
            $"최종 등급 : {result.finalGrade}"
        );

        if (RankUI.instance != null)
        {
            RankUI.instance.UpdateRankUI();
        }

        activeGameCount = GameReleaseManager.instance.GetActiveGameCount(EventManager.instance.SettlementNum);

        GameEventBridge.GameDevSucceeded(activeGameCount, gameDevData.MaxActiveGameCount);
        return result;
    }

    private void SetGradeIcon(Image icon, DevelopmentGrade grade)
    {
        switch (grade)
        {
            case DevelopmentGrade.A:
                icon.sprite = aGradeSprite;
                break;

            case DevelopmentGrade.B:
                icon.sprite = bGradeSprite;
                break;

            case DevelopmentGrade.C:
                icon.sprite = cGradeSprite;
                break;
        }
    }

    // 게임메이크 UI 초기화
    public void ResetGameMakeUI()
    {
        probabilityUpgradeCount = 0;

        UpdateProbabilityCountText();
        UpdateDevelopmentCostText();

        funIcon.sprite = defaultGradeSprite;
        graphicIcon.sprite = defaultGradeSprite;
        optimizationIcon.sprite = defaultGradeSprite;
    }

    // 게임 만들기 버튼
    public GameDevResult OnClickDevelopGame()
    {
        GameDevResult result = DevelopGame();

        if (result == null)
            return null;

        SetGradeIcon(funIcon, result.funGrade);
        SetGradeIcon(graphicIcon, result.graphicGrade);
        SetGradeIcon(optimizationIcon, result.optimizationGrade);

        return result;
    }
}

