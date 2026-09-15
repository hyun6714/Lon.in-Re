using TMPro;
using UnityEngine;
using UnityEngine.UI;

// GamePopupUI에서 받은 게임 정보를 카드에 표시
public class GameCardUI : MonoBehaviour
{
    [Header("게임 등급")]
    [SerializeField] private Image finalGradeIcon;

    [SerializeField] private TMP_Text funGradeText;
    [SerializeField] private TMP_Text graphicGradeText;
    [SerializeField] private TMP_Text optimizationGradeText;

    [Header("게임 보상")]
    [SerializeField] private TMP_Text fameRewardText;
    [SerializeField] private TMP_Text goldRewardText;

    [Header("정산 시간")]
    [SerializeField] private TMP_Text remainingTimeText;

    private ReleasedGameSaveData releasedGameData;
    private EventManagerData settlementData;

    private void OnEnable()
    {
        GameEventBridge.OnDayChanged += HandleDayChanged;
    }

    private void OnDisable()
    {
        GameEventBridge.OnDayChanged -= HandleDayChanged;
    }

    private void HandleDayChanged(GameDate date)
    {
        if (releasedGameData == null || settlementData == null)
            return;

        UpdateRemainingTime(releasedGameData, settlementData);
    }

    // 게임 하나의 데이터를 카드에 표시
    public void SetData(ReleasedGameSaveData releasedGame, GameReleaseData gameReleaseData, EventManagerData eventManagerData,
        Sprite aGradeSprite, Sprite bGradeSprite, Sprite cGradeSprite)
    {
        releasedGameData = releasedGame;
        settlementData = eventManagerData;

        GameDevResult result = releasedGame.gameResult;

        // 최종 등급 이미지
        SetGradeIcon(finalGradeIcon, result.finalGrade, aGradeSprite, bGradeSprite, cGradeSprite);

        // 세부 등급
        funGradeText.text = result.funGrade.ToString();
        graphicGradeText.text = result.graphicGrade.ToString();
        optimizationGradeText.text = result.optimizationGrade.ToString();

        // 최종 등급에 따른 보상
        int currencyReward = gameReleaseData.GetCurrencyReward(result.finalGrade);
        int reputationReward = gameReleaseData.GetReputationReward(result.finalGrade);

        // 보상 UI 표시
        goldRewardText.text = currencyReward.ToString("N0");
        fameRewardText.text = reputationReward.ToString("N0");

        UpdateRemainingTime(releasedGame, eventManagerData);
    }

    private void UpdateRemainingTime(ReleasedGameSaveData releasedGame, EventManagerData eventManagerData)
    {
        // 모든 정산이 끝난 경우
        if (releasedGame.settlementCount >= eventManagerData.SettlementNum)
        {
            remainingTimeText.text = "정산 완료";
            return;
        }

        // 다음 정산까지 필요한 일수
        int settlementDay = eventManagerData.NextSettlements[releasedGame.settlementCount];

        // 출시일부터 현재까지 며칠이 지났는지 계산
        int passedDays = GetPassedDays(releasedGame);

        // 다음 정산까지 남은 일수
        int remainingDays = settlementDay - passedDays;

        if (remainingDays < 0)
            remainingDays = 0;

        remainingTimeText.text = $"{remainingDays}일";
    }

    private int GetPassedDays(ReleasedGameSaveData releasedGame)
    {
        GameDate currentDate = CalendarManager.instance.CurrentDate;

        int year = releasedGame.releaseYear;
        int month = releasedGame.releaseMonth;
        int day = releasedGame.releaseDay;

        int passedDays = 0;

        while (year != currentDate.year || month != currentDate.month || day != currentDate.day)
        {
            day++;
            passedDays++;

            int lastDay = CalendarManager.instance.GetLastDay(month);

            if (day > lastDay)
            {
                day = 1;
                month++;

                if (month > currentDate.maxMonthPerYear)
                {
                    month = 1;
                    year++;
                }
            }
        }

        return passedDays;
    }

    private void SetGradeIcon(Image icon, DevelopmentGrade grade, Sprite aGradeSprite, Sprite bGradeSprite, Sprite cGradeSprite)
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
}