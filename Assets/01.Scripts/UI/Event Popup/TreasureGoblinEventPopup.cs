using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class TreasureGoblinEventPopup : EventPopupBase
{
    public override UIName Name => UIName.Popup_Event_TreasureGoblin;

    [Header("고블린")]
    [SerializeField] private TreasureGoblin goblin;

    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI mainText;

    private TreasureGoblinEvent goblinEvent;

    private bool isFinished;

    private void Awake()
    {
        if (goblin != null)
        {
            goblin.Init(this);
        }
    }

    public override void OpenPopup()
    {
        base.OpenPopup();

        UpdateTime(token.Token).Forget();
    }

    protected override void OnEnable()
    {
        transform.localScale = Vector3.one * data.CloseSize;

        goblinEvent = null;

        Utils.Log("[Goblin Popup] OnEnable 시작");

        if (EventManager.instance == null)
        {
            SetText("EventManger NULL");
            return;
        }

        goblinEvent = EventManager.instance.GetActiveEvent(GameEventType.TreasureGoblin) as TreasureGoblinEvent;

        if (goblinEvent == null)
        {
            SetText("Goblin Event NULL");
            return;
        }

        Utils.Log("[Goblin Popup] Goblin Event 가져오기 성공");

        if (goblin != null)
        {
            goblin.gameObject.SetActive(true);
        }

        SetText(goblinEvent.StartText);
    }    

    private async UniTaskVoid UpdateTime(CancellationToken ctk)
    {
        try
        {
            await UniTask.WaitUntil(() => goblinEvent != null && goblinEvent.IsFinished, cancellationToken: ctk);

            Result(goblinEvent.IsSuccess);
        }
        catch (OperationCanceledException)
        {

        }
    }

    public void OnClickGoblin()
    {
        if (goblinEvent == null || goblinEvent.IsFinished)
            return;

        // 클릭할 때 마다 성공 판정 확인
        bool success = goblinEvent.HitGoblin();

        if (success)
        {
            SetText(goblinEvent.SuccessText);
        }
    }

    private void Result(bool result)
    {
        if (goblinEvent == null)
            return;

        if (result)
        {
            SetText(goblinEvent.SuccessText);
        }
        else
        {
            SetText(goblinEvent.FailText);
        }

        if (goblin != null)
        {
            goblin.gameObject.SetActive(false);
        }
    }
   

    private void SetText(string text)
    {
        if (mainText != null)
        {
            mainText.text = text;
        }        
    }
}
