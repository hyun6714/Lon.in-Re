using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("테스트용 이벤트 팝업")]
    [SerializeField] private List<EventPopupStorage> popupList;

    [Header("테스트용 캔버스")]
    [SerializeField] private Canvas uiCanvas;

    private Dictionary<Season, EventPopup> eventPopupDic = new Dictionary<Season, EventPopup>();
    private Dictionary<Season, EventPopup> runtimePopupDic = new Dictionary<Season, EventPopup>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        InitializeEventPopupDic();
    }

    private void InitializeEventPopupDic()
    {
        foreach (EventPopupStorage storage in popupList)
        {
            if (storage.popup == null)
            {
                Debug.LogWarning("EventPopup이 없습니다.");
                continue;
            }

            if (eventPopupDic.ContainsKey(storage.season))
            {
                Debug.LogWarning("이미 등록된 Season 입니다.");
                continue;
            }

            eventPopupDic.Add(storage.season, storage.popup);
        }
    }

    //public void OpenEventPopup(Season season)
    //{
    //    if (!runtimePopupDic.TryGetValue(season, out EventPopup popup))
    //    {
    //        if (!eventPopupDic.TryGetValue(season, out EventPopup targetPopup))
    //        {
    //            Debug.LogWarning("Season에 등록된 Popup이 없습니다.");
    //            return;
    //        }

    //        popup = Instantiate(targetPopup, uiCanvas.transform);

    //        runtimePopupDic.Add(season, popup);
    //    }

    //    popup.gameObject.SetActive(true);
    //    popup.OpenPanel();
    //}

    //public void CloseEventPopup(Season season)
    //{
    //    if (runtimePopupDic.TryGetValue(season, out EventPopup popup))
    //    {
    //        popup.ClosePanel();
    //    }
    //}
}
