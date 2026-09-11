using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateHUD : UIBase
{    
    public override UIName Name => UIName.DateHUD;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI seasonText;
    [SerializeField] private TextMeshProUGUI timeText;

    //private Dictionary<Season, string> seasonDic = new Dictionary<Season, string>();

    private void Start()
    {
        UIManager.Instance.HUDRegister(this);
    }

    private void OnDestroy()
    {
        UIManager.Instance.HUDUnRegister(this);
    }

    //public void InitHUD(GameDate date)
    //{
    //    foreach (SeasonInfo info in date.seasonList)
    //    {
    //        seasonDic[info.season] = info.seasonName;
    //        Utils.Log("구독 성공");
    //    }

    //    Utils.Log("구독 완료");

    //    SetHUD(date);
    //}

    public void SetHUD(GameDate date)
    {
        dayText.text = $"{date.year}년 {date.month}월 {date.day}일";
        seasonText.text = $"{date.season}";
        timeText.text = $"{date.hour:D2} : {date.minutes:D2}";
    }
}
