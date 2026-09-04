using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopTab
{
    Part,
    Employee,
    Artifact
}

public class NormalShop : MonoBehaviour
{
    [Header("상점 패널")]
    [SerializeField] private GameObject shopPanel;

    [Header("확인 팝업 연동")]
    [SerializeField] private ConfirmPopup confirmPopup;

    [Header("제어 버튼")]
    [SerializeField] private Button openShopBtn;   // 하단 메뉴의 [상점] 버튼
    [SerializeField] private Button exitBtn;       // 상점 창 내 [X] 닫기 버튼

    [Header("상단 탭 버튼 3종")]
    [SerializeField] private Button partTabBtn;
    [SerializeField] private Button employeeTabBtn;
    [SerializeField] private Button artifactTabBtn;

    [Header("공용 탭 스프라이트")]
    [SerializeField] private Sprite selectedTabSprite;    // 남색 탭 스프라이트
    [SerializeField] private Sprite unselectedTabSprite;  // 흰색 탭 스프라이트

    [Header("탭 텍스트 색상")]
    [SerializeField] private Color selectedTextColor = Color.white;                     // 흰색 글씨
    [SerializeField] private Color unselectedTextColor = new Color32(41, 53, 118, 255); // 남색 글씨

    [Header("스크롤 뷰 컨텐츠 부모")]
    [SerializeField] private Transform contentParent; // Scroll View > Viewport > Content

    [Header("슬롯 프리팹 3종")]
    [SerializeField] private GameObject partSlotPrefab;
    [SerializeField] private GameObject employeeHireSlotPrefab;
    [SerializeField] private GameObject artifactSlotPrefab;

    [Header("연동 매니저 및 데이터")]
    [SerializeField] private PlayerTapUpgrade playerUpgrade;
    [SerializeField] private EmployeeManager employeeManager;
    [SerializeField] private ArtifactDatabase artifactDatabase;

    private readonly List<PartUpgradeSlot> partSlots = new List<PartUpgradeSlot>();
    private readonly List<EmployeeHireSlot> employeeSlots = new List<EmployeeHireSlot>();
    private readonly List<ShopArtifactSlot> artifactSlots = new List<ShopArtifactSlot>();

    private ShopTab currentTab = ShopTab.Part;
    private bool isInitialized = false;

    private void Awake()
    {
        if (playerUpgrade == null) playerUpgrade = FindFirstObjectByType<PlayerTapUpgrade>();
        if (employeeManager == null) employeeManager = FindFirstObjectByType<EmployeeManager>();

        if (openShopBtn != null) openShopBtn.onClick.AddListener(OpenShop);
        if (exitBtn != null) exitBtn.onClick.AddListener(CloseShop);

        if (partTabBtn != null) partTabBtn.onClick.AddListener(() => SwitchTab(ShopTab.Part));
        if (employeeTabBtn != null) employeeTabBtn.onClick.AddListener(() => SwitchTab(ShopTab.Employee));
        if (artifactTabBtn != null) artifactTabBtn.onClick.AddListener(() => SwitchTab(ShopTab.Artifact));
    }

    private void Start()
    {
        // 최초 1회만 모든 슬롯 생성
        InitSlots();

        // 부품 탭 비주얼 활성화
        SwitchTab(ShopTab.Part);

        //재화 변동 이벤트 구독
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.OnCurrencyChanged += OnCurrencyChanged;
        }
    }

    private void OnDestroy()
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.OnCurrencyChanged -= OnCurrencyChanged;
        }
    }

    // 슬롯 최초 1회 생성 및 초기화
    private void InitSlots()
    {
        if (isInitialized || contentParent == null) return;

        // 부품 슬롯 생성
        if (playerUpgrade != null && partSlotPrefab != null)
        {
            foreach (var state in playerUpgrade.PartStates)
            {
                GameObject obj = Instantiate(partSlotPrefab, contentParent);
                var slot = obj.GetComponent<PartUpgradeSlot>();
                if (slot != null)
                {
                    // RefreshPartSlots 콜백 전달
                    slot.SetUp(state, playerUpgrade, RefreshPartSlots);
                    partSlots.Add(slot);
                }
            }
        }

        // 직원 슬롯 생성
        if (employeeManager != null && employeeHireSlotPrefab != null)
        {
            foreach (var state in employeeManager.EmployeeStates)
            {
                GameObject obj = Instantiate(employeeHireSlotPrefab, contentParent);
                var slot = obj.GetComponent<EmployeeHireSlot>();
                if (slot != null)
                {
                    slot.SetUp(state, employeeManager);
                    employeeSlots.Add(slot);
                }
            }
        }

        // 아티팩트 슬롯 생성
        if (artifactDatabase != null && artifactSlotPrefab != null)
        {
            foreach (var info in artifactDatabase.artifacts)
            {
                GameObject obj = Instantiate(artifactSlotPrefab, contentParent);
                var slot = obj.GetComponent<ShopArtifactSlot>();
                if (slot != null)
                {
                    slot.SetUp(info, RefreshArtifactSlots, ShowConfirmPopup);
                    artifactSlots.Add(slot);
                }
            }
        }

        isInitialized = true;
    }

    public void OpenShop()
    {
        if (!isInitialized)
        {
            InitSlots();
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
        SwitchTab(currentTab);
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void SwitchTab(ShopTab tab)
    {
        currentTab = tab;

        // 탭에 따라 슬롯 Y높이 동적 조절
        GridLayoutGroup grid = contentParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            if (tab == ShopTab.Artifact)
            {
                // 아티팩트 슬롯 높이
                grid.cellSize = new Vector2(grid.cellSize.x, 256f);
            }
            else
            {
                // 부품 / 직원 슬롯 높이
                grid.cellSize = new Vector2(grid.cellSize.x, 128f);
            }
        }

        // 슬롯 목록 활성/비활성
        SetListActive(partSlots, tab == ShopTab.Part);
        SetListActive(employeeSlots, tab == ShopTab.Employee);
        SetListActive(artifactSlots, tab == ShopTab.Artifact);

        // 데이터 갱신
        switch (tab)
        {
            case ShopTab.Part:
                for (int i = 0; i < partSlots.Count; i++) partSlots[i].Refresh();
                break;
            case ShopTab.Employee:
                for (int i = 0; i < employeeSlots.Count; i++) employeeSlots[i].Refresh();
                break;
            case ShopTab.Artifact:
                for (int i = 0; i < artifactSlots.Count; i++) artifactSlots[i].Refresh();
                break;
        }

        // 탭 버튼 비주얼(스프라이트/텍스트) 교체
        UpdateTabVisuals();
    }

    // 재화 변동 시 상점이 켜져 있을 때만 갱신
    private void OnCurrencyChanged(CurrencyType type, int amount)
    {
        // 닫혀 있으면 갱신 X
        if (shopPanel == null || !shopPanel.activeSelf)
        {
            return;
        }

        RefreshCurrentTab();
    }

    // 현재 활성화된 탭의 슬롯들만 갱신
    public void RefreshCurrentTab()
    {
        switch (currentTab)
        {
            case ShopTab.Part:
                RefreshPartSlots();
                break;
            case ShopTab.Employee:
                for (int i = 0; i < employeeSlots.Count; i++)
                {
                    if (employeeSlots[i] != null) employeeSlots[i].Refresh();
                }
                break;
            case ShopTab.Artifact:
                RefreshArtifactSlots();
                break;
        }
    }

    public void RefreshPartSlots()
    {
        for (int i = 0; i < partSlots.Count; i++)
        {
            if (partSlots[i] != null)
            {
                partSlots[i].Refresh();
            }
        }
    }

    public void RefreshArtifactSlots()
    {
        for (int i = 0; i < artifactSlots.Count; i++)
        {
            if (artifactSlots[i] != null)
            {
                artifactSlots[i].Refresh();
            }
        }
    }

    private void ShowConfirmPopup(string title, string message, Action onConfirm)
    {
        if (confirmPopup != null)
        {
            confirmPopup.Show(title, message, onConfirm);
        }
        else
        {
            // 팝업 미연결 시 처리
            onConfirm?.Invoke();
        }
    }

    private void UpdateTabVisuals()
    {
        SetTabVisual(partTabBtn, currentTab == ShopTab.Part);
        SetTabVisual(employeeTabBtn, currentTab == ShopTab.Employee);
        SetTabVisual(artifactTabBtn, currentTab == ShopTab.Artifact);
    }

    private void SetTabVisual(Button btn, bool isSelected)
    {
        if (btn == null) return;

        // 스프라이트 교체
        var img = btn.GetComponent<Image>();
        if (img != null)
        {
            Sprite targetSprite = isSelected ? selectedTabSprite : unselectedTabSprite;
            if (targetSprite != null)
            {
                img.sprite = targetSprite;
            }
            img.color = Color.white;
        }

        // 글자 색상 교체
        var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.color = isSelected ? selectedTextColor : unselectedTextColor;
        }
    }

    private void SetListActive<T>(List<T> list, bool isActive) where T : Component
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                list[i].gameObject.SetActive(isActive);
            }
        }
    }
}