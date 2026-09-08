using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIName
{
    None,
    Event_0715_Popup
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

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

    [Header("테스트용 캔버스")]
    [SerializeField] private Canvas uiCanvas;

    [Header("팝업 프리팹")]
    [SerializeField] private List<PopupBase> popupList;

    private Dictionary<UIName, PopupBase> popupDic = new Dictionary<UIName, PopupBase>();
    private Dictionary<UIName, PopupBase> runtimePopupDic = new Dictionary<UIName, PopupBase>();

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
}
