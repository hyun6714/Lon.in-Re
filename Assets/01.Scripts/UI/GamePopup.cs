using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GamePopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Game;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button gameMakeBtn;

    private void Start()
    {
        exitBtn.onClick.AddListener(ClosePopup);
        gameMakeBtn.onClick.AddListener(() => GameEventBridge.PopupOpened(UIName.Popup_GameMake));
    }
}
