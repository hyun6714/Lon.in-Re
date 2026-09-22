using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankUI : MonoBehaviour
{
    public static RankUI instance { get; private set; }

    public GameObject RankPopup;
    //public TextMeshProUGUI RankText;
    public TextMeshProUGUI RankUpText;

    [Header("승급 조건 텍스트 UI")]
    public TextMeshProUGUI CostText; //명성 요구량 텍스트
    public TextMeshProUGUI GameReqText; //출시 게임 수 요구량 텍스트
    public TextMeshProUGUI EmployeeReqText; // 직원 수 요구량 텍스트 
    public TextMeshProUGUI GoldReqText; // 일반 재화 요구량 텍스트 

    [Header("등급 아이콘 UI")]
    [SerializeField] private Image rankIconImage;

    [Header("승급 버튼")]
    [SerializeField] public Button rankUpButton;
    [SerializeField] private Button prevButton; // 좌측 화살표 버튼
    [SerializeField] private Button nextButton; // 우측 화살표 버튼

    private int viewingRankIndex = 0;
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
    private void OnEnable()
    {
        GameEventBridge.OnRankChanged += UpdateRankUI;
        GameEventBridge.OnReincarnated += UpdateRankUI;
        GameEventBridge.OnReincarnated += RankUpOnBtn;

        if (prevButton != null) prevButton.onClick.AddListener(OnClickPrevRank);
        if (nextButton != null) nextButton.onClick.AddListener(OnClickNextRank);

        if (RankManager.instance != null)
        {
            int currentRealIndex = (int)RankManager.instance.currentRank;
            int maxIndex = 4;

            viewingRankIndex = currentRealIndex + 1;
            if (viewingRankIndex > maxIndex) viewingRankIndex = maxIndex;

            UpdateRankUI();
        }
    }

    private void OnDisable()
    {
        GameEventBridge.OnRankChanged -= UpdateRankUI;
        GameEventBridge.OnReincarnated -= UpdateRankUI;
        GameEventBridge.OnReincarnated -= RankUpOnBtn;
    }

    public void OpenRankPop()
    {
        if (RankPopup == null) return;
        RankPopup.SetActive(true);

        if (RankManager.instance != null)
        {
            int currentRealIndex = (int)RankManager.instance.currentRank;
            int maxIndex = 4;

            if (currentRealIndex < maxIndex)
            {
                viewingRankIndex = currentRealIndex + 1; 
            }
            else
            {
                viewingRankIndex = currentRealIndex; 
            }
        }

        UpdateRankUI();
    }

    public void CloseRankPop()
    {
        if (RankPopup == null) return;

        RankPopup.SetActive(false);
    }

    public void OnClickPrevRank()
    {
        viewingRankIndex--;
        if (viewingRankIndex < 1)
        {
            viewingRankIndex = 1; // 인디부터
        }
        UpdateRankUI();
    }

    public void OnClickNextRank()
    {
        if (RankManager.instance == null) return;

        // RankManager에 등록된 데이터 개수 기준 최대 인덱스
        int maxIndex = 4;


        viewingRankIndex++;
        if (viewingRankIndex > maxIndex)
        {
            viewingRankIndex = maxIndex; // 최고 등급에서 멈춤
        }
        UpdateRankUI();
    }

    public void UpdateRankUI()
    {

        if (RankManager.instance == null) return;

        RankData currentData = RankManager.instance.CurrentRankData;
        if (currentData != null)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetText(HUDTextType.Level, currentData.rankDisplayName);
            }
        }

        int currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;
        RankData targetData = RankManager.instance.GetRankData((RankManager.RankState)viewingRankIndex);
        if (targetData == null) return;

        // 팝업 중앙에 표시할 등급 이름 (예: 인디 개발자, 중소기업 등)
        if (RankUpText != null)
        {
            RankUpText.text = targetData.rankDisplayName;
        }

        if (rankIconImage != null)
        {
            if (targetData.rankIcon != null)
            {
                rankIconImage.gameObject.SetActive(true);
                rankIconImage.sprite = targetData.rankIcon;
            }
            else
            {
                rankIconImage.gameObject.SetActive(false);
            }
        }

        // 요구 조건 표시 (첫번째 등급인 Solo는 보통 요구치가 없거나 0일 수 있음)
        if (viewingRankIndex == 0)
        {
            if (CostText != null) CostText.text = "-";
            if (GameReqText != null) GameReqText.text = "-";
            if (EmployeeReqText != null) EmployeeReqText.text = "-";
            if (GoldReqText != null) GoldReqText.text = "-";
        }
        else
        {
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
                EmployeeReqText.text = $"{GameManager.instance.currentEmployeeCount} / {targetData.reqEmployeeCount}";
            }
            if (GoldReqText != null)
            {
                int currentGold = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Normal) : 0;
                GoldReqText.text = $"{FormatCurrency(currentGold)} / {FormatCurrency(targetData.reqNormal)}";
            }
        }

        int currentRealIndex = (int)RankManager.instance.currentRank;

        if (rankUpButton != null)
        {
            if (viewingRankIndex == 0)
            {
                rankUpButton.gameObject.SetActive(false); // 솔로일 때는 승급 버튼 숨김
            }
            else if (viewingRankIndex == currentRealIndex + 1)
            {
                rankUpButton.gameObject.SetActive(true);
                rankUpButton.interactable = true;
            }
            else
            {
                rankUpButton.interactable = false;
                rankUpButton.gameObject.SetActive(false);
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

    public string FormatCurrency(long amount)
    {
        if (amount >= 100000000) // 1억 이상
        {
            return (amount / 100000000f).ToString("0.#") + "억";
        }
        else if (amount >= 10000) // 1만 이상
        {
            return (amount / 10000f).ToString("0.#") + "만";
        }
        else
        {
            return amount.ToString("N0"); // 1만 미만은 그냥 콤마(,) 찍어서 표시
        }
    }

}