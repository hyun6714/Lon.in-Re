using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 계절별 이벤트 시작, 종료를 정의하는 전략 인터페이스
/// </summary>
public interface IEvent
{
    void StartEvent();
    void EndEvent();
}

public enum GameEventType
{
    SpringEvent = 3001,
    SummerEvent,
    FallEvent,
    WinterEvent
}

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    [Header("데이터")]
    [SerializeField] private EventManagerData data;
    [SerializeField] private GameEventData eventData;

    [Header("계절 이벤트 데이터")]
    [SerializeField] private SummerEventData summerData;

    //private Dictionary<(int month, int day), GameEventInfo> eventDateDic = new Dictionary<(int month, int day), GameEventInfo>();
    private Dictionary<EventDate, GameEventInfo> eventDateDic = new Dictionary<EventDate, GameEventInfo>();

    private EventFactory eventFactory;

    private IEvent currentEvent;
    private GameEventType currentEventType;
    private GameEventInfo currentGameEventInfo;

    private CancellationTokenSource token;

    // 정산 이벤트
    public event Action<int, int> OnGameSettlement;

    [Header("일시 정지")]
    [SerializeField] private bool isPaused;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializeDic();

        eventFactory = new EventFactory(summerData);
    }

    private void InitializeDic()
    {
        if (eventData == null || eventData.EventList == null)
        {
            Debug.Log("이벤트 데이터가 존재하지 않습니다.");
            return;
        }

        foreach (GameEventInfo info in eventData.EventList)
        {
            EventDate dateKey = new EventDate(info.TargetMonth, info.TargetDay);

            if (eventDateDic.ContainsKey(dateKey))
            {
                Debug.Log($"같은 날짜에 이벤트가 이미 존재합니다. {info.TargetMonth}월 {info.TargetDay}일");
                continue;
            }

            eventDateDic.Add(dateKey, info);
        }
    }

    private void OnEnable()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        SubscribeEvent();
    }

    private void OnDisable()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        UnSubscribeEvent();
    }

    private void SubscribeEvent()
    {
        GameEventBridge.OnDayChanged += ChangeEvent;
        GameEventBridge.OnEventStarted += StartCurrentEvent;
        GameEventBridge.OnPausedChanged += PausedChanged;
        Utils.Log("EventManager 구독 완료");
    }

    private void UnSubscribeEvent()
    {
        GameEventBridge.OnDayChanged -= ChangeEvent;
        GameEventBridge.OnEventStarted -= StartCurrentEvent;
        GameEventBridge.OnPausedChanged -= PausedChanged;
    }

    /// <summary>
    /// 여름 이벤트 발동 시 배율 변경
    /// </summary>
    /// <param name="multi"> isCool에 따른 배율 변경값 </param>
    public void SummerMultiplier(float multi)
    {
        GameEventBridge.AutoMultiplierChanged(multi);
    }
    
    // UI 테스트용
    public void OnClickSummerCool(bool value)
    {
        if (currentEvent is SummerEvent strategy)
        {
            strategy.SetCool(value);

            UIManager.Instance.ClosePopup(currentGameEventInfo.PopupName);
        }
    }

    // 이벤트 교체
    private void ChangeEvent(EventDate dateKey)
    {
        if (!eventDateDic.TryGetValue(dateKey, out GameEventInfo info))
        {
            currentGameEventInfo = null;
            currentEvent = null;
            Utils.Log("존재하지 않는 날짜 키");
            return;
        }

        currentGameEventInfo = info;
        currentEventType = info.EventType;

        currentEvent = eventFactory.CreateEvent(currentEventType);

        Utils.Log($"이벤트 등록 성공 : {currentEventType}_{dateKey.month}월 {dateKey.day}일");
    }

    // 게임 출시 후 해당 게임의 정산 시작
    // 저장된 개발 이벤트 날짜와 gameId를 사용하여 게임별 정산
    public void StartGameSettlement(int gameId)
    {
        GameDate nowDate = CalendarManager.instance.CurrentDate;
        //StartGameDevAsync(date.year, date.month, date.day, gameId, CalendarManager.instance.Token).Forget();
        StartGameDevAsync(nowDate, gameId, token.Token).Forget();
    }

    // 개발 버튼 눌렀을 시 실행
    public void StartGameDevEvent()
    {
        // 게임 개발 이벤트 실행
        Utils.Log("게임 개발 이벤트 시작");
        
        // 일시 정지 후 UI 불러오기    
    }

    /// <summary>
    /// 게임 출시 후 일정 주기마다 판매금 지급.
    /// 한 달에 한 번 지급. 총 2번
    /// </summary>
    /// <param name="date"> 게임 출시 날짜를 담은 struct </param>
    /// <param name="gameId"> 출시 게임 고유 ID </param>
    /// <param name="token"> UniTask 토큰 </param>
    /// <returns></returns>
    public async UniTaskVoid StartGameDevAsync(GameDate date, int gameId, CancellationToken token)
    {
        try
        {
            for (int i = 1; i <= data.SettlementNum; i++)
            {
                int addDay = data.NextSettlements[i - 1];

                GameDate targetDate = date.GetAfterDay(addDay);

                await UniTask.WaitUntil(() =>
                !isPaused &&
                CalendarManager.instance.IsEventTime(targetDate.year, targetDate.month, targetDate.day),
                cancellationToken: token);

                OnGameSettlement?.Invoke(gameId, i);
                Utils.Log($"[{gameId}]ID 프로젝트의 [{i}]번째 정산 완료.");
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    public void StartCurrentEvent()
    {
        if (currentEvent == null)
        {
            Utils.Log("등록된 이벤트가 없습니다.");
            return;
        }

        currentEvent?.StartEvent();

        UIManager.Instance.OpenPopup(currentGameEventInfo.PopupName);
    }

    public void EndCurrentEvent()
    {
        currentEvent?.EndEvent();
    }    

    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
