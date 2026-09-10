using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SceneCurrencyHUD : MonoBehaviour
{
    [SerializeField] private CurrencyDatabase data;

    [SerializeField] private List<TextMeshProUGUI> currencyTextList;
    [SerializeField] private List<Image> images;


    private void OnEnable()
    {
        HUDInit();
    }

    private void HUDInit()
    {
        if (data == null || data.currencies == null)
            return;

        for (int i = 0; i < data.currencies.Count; i++)
        {
            if (i >= images.Count || i >= currencyTextList.Count)
                break;

            CurrencyInfo info = data.currencies[i];

            if (data.currencies[i] != null)
            {
                images[i].sprite = info.icon;
                currencyTextList[i].text = $"{info.initialAmount:N0}";
            }
        }
    }

    public void SetHUD(CurrencyType type, int amount)
    {
        int index = (int)type;

        if (index >= 0 && index < currencyTextList.Count)
        {
            currencyTextList[index].text = $"{amount:N0}";
        }
    }
}
