using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class RankUpPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_RankUp;

    [SerializeField] private Button exitBtn;
        
    private void Awake()
    {
        exitBtn.onClick.AddListener(ClosePopup);
    }    
}
