using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class HUDTextGroup : UIBase
{
    public override UIName Name => UIName.HUDTextGroup;

    [SerializeField] private List<HUDTextInfo> textList;

    private Dictionary<HUDTextType, TextMeshProUGUI> textDic = new Dictionary<HUDTextType, TextMeshProUGUI>();

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        textDic.Clear();

        foreach (HUDTextInfo info in textList)
        {
            if (info.text = null)
                continue;

            if (textDic.ContainsKey(info.type))
            {
                Utils.Log($"이미 등록된 Text UI : {info.type}");
                continue;
            }

            textDic.Add(info.type, info.text);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.TextRegister(this);
        }
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.TextUnRegister(this);
        }
    }

    public void SetText(HUDTextType type, string value)
    {
        if (!textDic.TryGetValue(type, out TextMeshProUGUI text))
        {
            Utils.Log($"등록되지 않은 텍스트 타입 : {type}");
            return;
        }

        text.text = value;
    }

    public void SetText(HUDTextType type, int value)
    {
        SetText(type, $"{value:N0}");
    }

    public void SetText(HUDTextType type, GameDate date)
    {
        SetText(type, $"{date.year}년\n{date.month}월 {date.day}일\n{date.hour} : {date.minutes}");
    }
}
