using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class UIButtonGroup : MonoBehaviour
{
    [SerializeField] private List<PopupButtonInfo> buttonList;

    private List<UnityAction> actions = new List<UnityAction>();

    private void OnEnable()
    {
        SubscribeButton();
    }

    private void OnDisable()
    {
        UnSubscribeButton();
    }

    private void SubscribeButton()
    {
        if (buttonList == null)
            return;

        actions.Clear();

        foreach (PopupButtonInfo info in buttonList)
        {
            if (info == null || info.button == null)
                continue;

            UIName popupName = info.popupName;
            ShopTab shopTab = info.shopTab;

            UnityAction action = () => UIManager.Instance.OpenPopup(popupName, shopTab);

            info.button.onClick.AddListener(action);
            actions.Add(action);
        }
    }

    private void UnSubscribeButton()
    {
        if (buttonList == null)
            return;

        int actionIndex = 0;

        foreach (PopupButtonInfo info in buttonList)
        {
            if (info == null || info.button == null)
                continue;

            if (actionIndex >= actions.Count)
                break;

            info.button.onClick.RemoveListener(actions[actionIndex]);
            actionIndex++;
        }

        actions.Clear();
    }
}
