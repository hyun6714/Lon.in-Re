using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCardUI : MonoBehaviour
{
    [Header("게임 기본 정보")]
    [SerializeField] private TMP_Text gameIdText;

    [Header("세부 등급")]
    [SerializeField] private Image funGradeIcon;
    [SerializeField] private Image graphicGradeIcon;
    [SerializeField] private Image optimizationGradeIcon;

    [Header("출시 정보")]
    [SerializeField] private TMP_Text releaseDateText;
    [SerializeField] private TMP_Text settlementCountText;

    [Header("등급 이미지")]
    [SerializeField] private Sprite aGradeSprite;
    [SerializeField] private Sprite bGradeSprite;
    [SerializeField] private Sprite cGradeSprite;

    public void SetData(ReleasedGameSaveData releasedGame)
    {
        if (releasedGame == null || releasedGame.gameResult == null)
            return;

        GameDevResult result = releasedGame.gameResult;

        gameIdText.text = $"{result.gameId}번째 게임";

        SetGradeIcon(funGradeIcon, result.funGrade);
        SetGradeIcon(graphicGradeIcon, result.graphicGrade);
        SetGradeIcon(optimizationGradeIcon, result.optimizationGrade);

        releaseDateText.text = $"{releasedGame.releaseYear}년 {releasedGame.releaseMonth}월 {releasedGame.releaseDay}일";

        settlementCountText.text = $"정산 {releasedGame.settlementCount}회";
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
}