using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankUI : MonoBehaviour
{
    public GameObject RankPopup;
    public TextMeshProUGUI RankText;
    public TextMeshProUGUI RankUpText;

    [Header("승급 조건 텍스트 UI")]
    public TextMeshProUGUI CostText; //명성 요구량 텍스트
    public TextMeshProUGUI GameReqText; //출시 게임 수 요구량 텍스트
    public TextMeshProUGUI EmployeeReqText; // 직원 수 요구량 텍스트 

    [Header("승급 버튼")]
    public Button rankUpButton;

    private void OnEnable()
    {
        RankManager.OnRankChanged += UpdateRankUI;
        ReincarnationManager.OnReincarnated += UpdateRankUI;
        ReincarnationManager.OnReincarnated += RankUpOnBtn;
        UpdateRankUI();
    }

    private void OnDisable()
    {
        RankManager.OnRankChanged -= UpdateRankUI;
        ReincarnationManager.OnReincarnated -= UpdateRankUI;
        ReincarnationManager.OnReincarnated -= RankUpOnBtn;
    }

    public void OpenRankPop()
    {
        if (RankPopup == null) return;

        RankPopup.SetActive(true);
        UpdateRankUI();
    }

    public void CloseRankPop()
    {
        if (RankPopup == null) return;

        RankPopup.SetActive(false);
    }

    public void UpdateRankUI()
    {
        if (RankManager.instance == null) return;

        RankData currentData = RankManager.instance.CurrentRankData;
        if (currentData != null)
        {
            if (RankText != null)
            {
                RankText.text = currentData.rankDisplayName;
            }
        }

        int currentIndex = (int)RankManager.instance.currentRank;
        int nextIndex = currentIndex + 1;

        RankData targetData = RankManager.instance.GetRankData((RankManager.RankState)nextIndex);
        if (targetData == null)
        {
            targetData = currentData;

            if (RankUpText != null)
            {
                RankUpText.text = "최고 등급";
            }

            if (CostText != null)
            {
                CostText.text = "-";
            }

            if (rankUpButton != null)
            {
                rankUpButton.interactable = false;
            }

            return;
        }

        if (targetData != null)
        {
            if (RankUpText != null)
            {
                RankUpText.text = targetData.rankDisplayName;
            }

            if (CostText != null)
            {
                CostText.text = targetData.reqReputation.ToString("N0");
            }

            if (GameReqText != null)
            {
                GameReqText.text = $"{RankManager.instance.gamesReleased} / {targetData.reqGamesReleased}";
            }

            if (EmployeeReqText != null)
            {
                EmployeeReqText.text = $"{RankManager.instance.currentEmployeeCount} / {targetData.reqEmployeeCount}";
            }
        }

    }

    //환생 후 버튼 활성화 
    public void RankUpOnBtn()
    {
        if (rankUpButton != null)
        {
            rankUpButton.interactable = true;
        }
    }
}