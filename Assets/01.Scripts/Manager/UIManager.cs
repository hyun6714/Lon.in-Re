using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public enum UIName
{
    None,
    Event_0715_Popup,
    HUDTextGroup
}

public enum HUDTextType
{
    Level,
    Rebirth,

    Coin,
    SpecialCoin,
    Fame,
    CoinSec,

    Date,

    Employee,
    Game
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("테스트용(삭제 예정)")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI frameText;
    [SerializeField] TextMeshProUGUI rebirthText;
    [SerializeField] TextMeshProUGUI eventTitleText;
    [SerializeField] TextMeshProUGUI eventCostText;
    [SerializeField] TextMeshProUGUI specialCoinText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI goldSecText;
    [SerializeField] TextMeshProUGUI gameNumText;
    [SerializeField] TextMeshProUGUI peopleNumText;
    
    [SerializeField] Button levelUpBtn;
    [SerializeField] Button marketBtn;
    [SerializeField] Button gameBtn;
    [SerializeField] Button artifactBtn;
    [SerializeField] Button peopleBtn;
    [SerializeField] Button rebirthBtn;

    [Header("HUD")]
    [SerializeField] private HUDTextGroup textGroup;

    [Header("캔버스")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Canvas effectCanvas;

    [Header("플로팅 텍스트")]
    [SerializeField] private FloatingText floatingTextPrefab;

    [Header("팝업 프리팹")]
    [SerializeField] private List<PopupBase> popupList;

    private Dictionary<UIName, PopupBase> popupDic = new Dictionary<UIName, PopupBase>();
    private Dictionary<UIName, PopupBase> runtimePopupDic = new Dictionary<UIName, PopupBase>();

    //private Dictionary<UIName, UIBase> hudDic = new Dictionary<UIName, UIBase>();

    // CurrencyType - HUDTextType 매핑
    private Dictionary<CurrencyType, HUDTextType> currencyTextDic = new Dictionary<CurrencyType, HUDTextType>()
    {
        { CurrencyType.Normal, HUDTextType.Coin },
        { CurrencyType.Special, HUDTextType.SpecialCoin },
        { CurrencyType.Reputation, HUDTextType.Fame }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        InitializeDictionary();
    }

    private void OnEnable()
    {
        SubscribeEvent();
    }

    private void OnDisable()
    {
        UnSubscribeEvent();
    }

    private void SubscribeEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventBridge.OnCurrencyChanged += SetText;
        GameEventBridge.OnTimeChanged += SetText;
    }

    private void UnSubscribeEvent()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEventBridge.OnCurrencyChanged -= SetText;
        GameEventBridge.OnTimeChanged -= SetText;
    }

    // 런타임 딕셔너리에 저장된 팝업 비우기
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        runtimePopupDic.Clear();
    }

    private void InitializeDictionary()
    {
        if (popupList == null)
            return;

        foreach (PopupBase popup in popupList)
        {
            if (popup == null)
                continue;

            if (popupDic.ContainsKey(popup.Name))
            {
                Utils.Log($"중복 UI : {popup.Name}");
                continue;
            }

            popupDic.Add(popup.Name, popup);
        }
    }

    public void TextRegister(HUDTextGroup group)
    {
        textGroup = group;
    }

    public void TextUnRegister(HUDTextGroup group)
    {
        if (textGroup == group)
        {
            textGroup = null;
        }
    }

    /// <summary>
    /// 재화 획득 텍스트를 화면에 띄우는 함수
    /// </summary>
    /// <param name="position"> 텍스트가 나올 위치 </param>
    /// <param name="power"> 획득한 재화량 </param>
    /// <param name="isWorldPos"> 월드 캔버스에 표시할지 결정 </param>
    public void SpawnFloatingText(Vector3 position, int power, bool isWorldPos)
    {
        if (floatingTextPrefab == null || ObjectPoolManager.instance == null)
        {
            Utils.Log("프리팹 또는 매니저를 찾지 못함");
            return;
        }

        FloatingText textObj = ObjectPoolManager.instance.GetObject<FloatingText>(
            floatingTextPrefab.gameObject,
            effectCanvas.transform
        );

        if (textObj != null)
        {
            RectTransform rect = textObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (isWorldPos)
                {
                    Vector3 worldPos = position + new Vector3(0f, 1f, 0f);
                    rect.position = Camera.main.WorldToScreenPoint(worldPos);
                }
                else
                {
                    rect.position = position;
                }
            }

            textObj.SetOriginPrefab(floatingTextPrefab.gameObject);
            textObj.Setup($"+{CurrencyFormatter.Format(power)}");
        }
    }

    public void OpenPopup(UIName name)
    {
        if (runtimePopupDic.TryGetValue(name, out PopupBase popup))
        {
            if (popup != null && popup.gameObject.activeSelf)
            {
                Utils.Log($"이미 열린 UI : {name}");
                return;
            }
        }
        else
        {
            if (!popupDic.TryGetValue(name, out PopupBase popupPrefab))
            {
                Utils.Log($"등록되지 않은 UI : {name}");
                return;
            }

            popup = Instantiate(popupPrefab, uiCanvas.transform);
            runtimePopupDic.Add(name, popup);
        }

        GameManager.Instance.GamePaused();
        popup.gameObject.SetActive(true);
        popup.transform.SetAsLastSibling();
        popup.OpenPanel();
    }

    public void ClosePopup(UIName name)
    {
        if (!runtimePopupDic.TryGetValue(name, out PopupBase popup))
        {
            Utils.Log($"열리지 않은 UI : {name}");
            return;
        }

        popup.ClosePanel();
    }

    #region SetText Method
    public void SetText(HUDTextType type, string value)
    {
        textGroup.SetText(type, value);
    }

    public void SetText(HUDTextType type, int amount)
    {
        textGroup.SetText(type, amount);
    }

    public void SetText(CurrencyType type, int amount)
    {
        if (!currencyTextDic.TryGetValue(type, out HUDTextType textType))
        {
            Utils.Log($"등록되지 않는 재화입니다 : {type}");
            return;
        }

        textGroup.SetText(textType, amount);
    }

    public void SetText(GameDate date)
    {
        textGroup.SetText(HUDTextType.Date, date);
    }
    #endregion
}
