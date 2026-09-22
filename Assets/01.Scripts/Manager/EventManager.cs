using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 이벤트 시작, 종료를 정의하는 인터페이스
/// </summary>
public interface IEvent
{
    void StartEvent();
    void EndEvent();

    void SaveEventData(EventSaveData eventSaveData);
}

public enum GameEventType
{
    None,
    Burning = 3001,
    AirConditional,
    Gift,
    TreasureGoblin
}

public class EventManager : MonoBehaviour
{
    // 활성화된 이벤트 저장용 중첩 클래스
    private class ActiveEvent
    {
        public GameEventType Type;
        public GameEventInfo Info;
        public GameDate EventDate;
        public IEvent Event;
        public bool IsStarted;
    }

    public static EventManager instance;

    [Header("데이터")]
    [SerializeField] private EventManagerData data;
    [SerializeField] private GameEventData eventData;

    public int SettlementNum => data.SettlementNum;

    [Header("이벤트 데이터")]
    [SerializeField] private EventDataContainer dataContainer;

    // 날짜별 이벤트 저장용 딕셔너리
    private Dictionary<GameDate, GameEventInfo> eventDateDic = new Dictionary<GameDate, GameEventInfo>();

    // 발생할 이벤트 저장용 딕셔너리
    private Dictionary<GameEventType, ActiveEvent> activeEventDic = new Dictionary<GameEventType, ActiveEvent>();

    private EventFactory eventFactory;

    //private IEvent currentEvent;
    //private GameEventType currentEventType;
    //private GameEventInfo currentGameEventInfo;

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

        eventFactory = new EventFactory(dataContainer);

        UpdateNextEvent();
    }

    private void InitializeDic()
    {
        if (eventData == null || eventData.EventList == null)
        {
            Debug.Log("이벤트 데이터가 존재하지 않습니다.");
            return;
        }

        eventDateDic.Clear();

        // 날짜만 다른 같은 이벤트를 날짜를 키값으로 딕셔너리에 추가
        foreach (GameEventInfo info in eventData.EventList)
        {
            // 이벤트에 저장된 발생 날짜를 확인해서 딕셔너리에 키값에 추가
            foreach (GameEventDate eventDate in info.GameEventDateList)
            {
                GameDate dateKey = new GameDate
                {
                    month = eventDate.Month,
                    day = eventDate.Day,
                    hour = eventDate.Hour
                };

                if (eventDateDic.ContainsKey(dateKey))
                {
                    Debug.Log($"같은 날짜에 이벤트가 이미 존재합니다 : {eventDate.Month}월 {eventDate.Day}일 {eventDate.Hour}시");
                    continue;
                }

                eventDateDic.Add(dateKey, info);
            }
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
        GameEventBridge.OnTimeChanged += RefreshNextEvent;
        Utils.Log("EventManager 구독 완료");
    }

    private void UnSubscribeEvent()
    {
        GameEventBridge.OnDayChanged -= ChangeEvent;
        GameEventBridge.OnEventStarted -= StartCurrentEvent;
        GameEventBridge.OnPausedChanged -= PausedChanged;
        GameEventBridge.OnTimeChanged -= RefreshNextEvent;
    }

    /// <summary>
    /// 게임 로드 시 이벤트 불러오는 함수
    /// </summary>
    /// <param name="eventSaveDatas"> 저장된 이벤트 리스트 </param>
    public void CheckEventSave(List<EventSaveData> eventSaveDatas)
    {
        if (eventSaveDatas == null || eventSaveDatas.Count == 0)
        {
            Utils.Log("저장된 이벤트가 없습니다.");
            return;
        }

        Utils.Log($"저장된 이벤트 수 : {eventSaveDatas.Count}");
        
        foreach (EventSaveData saveData in eventSaveDatas)
        {
            if (!saveData.isEventActive)
            {
                Utils.Log($"활성화 이벤트가 아님 : {saveData.eventType}");
                continue;
            }

            GameEventInfo targetInfo = null;

            foreach (var events in eventDateDic)
            {
                if (events.Value.EventType == saveData.eventType)
                {
                    targetInfo = events.Value;
                    break;
                }
            }

            if (targetInfo == null)
            {
                Utils.Log($"이벤트 타입 [{saveData.eventType}]을 찾지 못했습니다.");
                continue;
            }

            GameDate loadEndDate = GameDataGetter<GameDate>.GetData(); ;

            loadEndDate.LoadDate(saveData.eventEndDate);

            GameDate currentDate = GameDataGetter<GameDate>.GetData();

            Utils.Log($"현재 : {currentDate.year}/{currentDate.month}/{currentDate.day} {currentDate.hour}:{currentDate.minutes}");
            Utils.Log($"종료 : {loadEndDate.year}/{loadEndDate.month}/{loadEndDate.day} {loadEndDate.hour}:{loadEndDate.minutes}");

            if (currentDate >= loadEndDate)
            {
                Utils.Log($"이벤트 시간이 지남 : {saveData.eventType}");
                continue;
            }

            IEvent newEvent = eventFactory.CreateEvent(saveData.eventType);

            if (newEvent == null)
            {
                Utils.Log($"이벤트 생성 실패 : {saveData.eventType}");
                continue;
            }

            activeEventDic[saveData.eventType] = new ActiveEvent
            {
                Type = saveData.eventType,
                Info = targetInfo,
                EventDate = default,
                Event = newEvent,
                IsStarted = true
            };

            if (newEvent is AirConditionalEvent airconEvent)
            {
                airconEvent.LoadEvent(saveData.eventEndDate, saveData.isSummerCool);
            }

            Utils.Log($"이벤트 복구 완료 : {saveData.eventType}");
        }

        UpdateNextEvent();
    }

    /// <summary>
    /// 게임 세이브 시 불러오는 함수
    /// </summary>
    /// <returns> 현재 진행중인 이벤트 리스트를 반환  </returns>
    public List<EventSaveData> GetEventSaveData()
    {
        List<EventSaveData> saveDatas = new List<EventSaveData>();

        foreach (var value in activeEventDic)
        {
            ActiveEvent activeEvent = value.Value;

            EventSaveData saveData = new EventSaveData()
            {
                eventType = activeEvent.Type,
                isEventActive = true
            };
            activeEvent.Event.SaveEventData(saveData);

            saveDatas.Add(saveData);
        }

        return saveDatas;
    }
    
    public void OnClickSummerCool(bool value)
    {
        if (!activeEventDic.TryGetValue(GameEventType.AirConditional, out ActiveEvent activeEvent))
            return;

        if (activeEvent.Event is AirConditionalEvent airconEvent)
        {
            airconEvent.SetCool(value);
            GameEventBridge.PopupClosed(activeEvent.Info.PopupName);
        }
    }

    // 이벤트 교체
    private void ChangeEvent(GameDate date)
    {
        foreach (var kv in eventDateDic)
        {
            // 딕셔너리에 저장된 카값이 넘어온 날짜(월/일)과 동일한지 확인
            if (!kv.Key.EqualMonthDay(date))
                continue;

            GameEventInfo eventInfo = kv.Value;
            GameEventType eventType = kv.Value.EventType;

            if (activeEventDic.ContainsKey(eventType))
            {
                Utils.Log($"이미 진행 중인 이벤트 입니다 : {eventType}");
                continue;
            }

            IEvent newEvent = eventFactory.CreateEvent(eventType);

            if (newEvent == null)
                continue;

            // 당일에 실행될 이벤트 저장(월/일/시)
            activeEventDic[eventType] = new ActiveEvent
            {
                Type = eventType,
                Info = eventInfo,
                EventDate = kv.Key,
                Event = newEvent,
                IsStarted = false
            };

            Utils.Log($"[EventManager] 이벤트 등록 : {eventType}");
            Utils.Log($"이벤트 등록 성공 : {eventType}_{kv.Key.month}월 {kv.Key.day}일 {kv.Key.hour}시");
        }

        UpdateNextEvent();
    }

    // 게임 출시 후 해당 게임의 정산 시작
    // 저장된 개발 이벤트 날짜와 gameId를 사용하여 게임별 정산
    public void StartGameSettlement(int gameId)
    {
        GameDate nowDate = GameDataGetter<GameDate>.GetData();
        StartGameSettlementAsync(nowDate, gameId, 0, token.Token).Forget();
    }

    /// <summary>
    /// 게임 출시 후 일정 주기마다 판매금 지급.
    /// 한 달에 한 번 지급. 총 2번
    /// </summary>
    /// <param name="date"> 게임 출시 날짜를 담은 struct </param>
    /// <param name="gameId"> 출시 게임 고유 ID </param>
    /// <param name="token"> UniTask 토큰 </param>
    /// <returns></returns>
    public async UniTaskVoid StartGameSettlementAsync(GameDate date, int gameId, int settlementCount, CancellationToken token)
    {
        try
        {
            for (int i = settlementCount + 1; i <= data.SettlementNum; i++)
            {
                int addDay = data.NextSettlements[i - 1];

                GameDate targetDate = date.GetAfterDay(addDay);
                Utils.Log($"종료 날짜 : {targetDate.year}년 {targetDate.month}월 {targetDate.day}일 {targetDate.hour}시");

                await UniTask.WaitUntil(() =>
                GameDataGetter<GameDate>.GetData() >= targetDate,
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
        GameDate currentDate = GameDataGetter<GameDate>.GetData();

        // 활성화된 이벤트 딕셔너리의 Key, Value값
        foreach (var value in activeEventDic)
        {
            // 딕셔너리에 저장된 이벤트 꺼내오기
            ActiveEvent activeEvent = value.Value;

            if (!currentDate.EqualMonthDayHour(activeEvent.EventDate))
                continue;

            if (activeEvent.IsStarted)
            {
                Utils.Log($"이미 시작된 이벤트 입니다 : {activeEvent.Type}");
                continue;
            }

            activeEvent.Event.StartEvent();

            activeEvent.IsStarted = true;

            Utils.Log($"[EventManager] 이벤트 시작 완료 : {activeEvent.Info.PopupName}");

            GameEventBridge.PopupOpened(activeEvent.Info.PopupName);

            Utils.Log($"[EventManager] 팝업 열기 완료 : {activeEvent.Info.PopupName}");
            Utils.Log($"이벤트 시작 : {activeEvent.Type}");
        }        
    }

    public void EndCurrentEvent(GameEventType type)
    {
        if (!activeEventDic.TryGetValue(type, out ActiveEvent activeEvent))
            return;

        activeEvent.Event.EndEvent();
        activeEventDic.Remove(type);

        Utils.Log($"이벤트 종료 : {type}");
    }    

    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    // 저장된 게임 정산 이어서 시작
    public void LoadGameSettlement(ReleasedGameSaveData releasedGame)
    {
        GameDate releaseDate = GameDataGetter<GameDate>.GetData();

        releaseDate.year = releasedGame.releaseYear;
        releaseDate.month = releasedGame.releaseMonth;
        releaseDate.day = releasedGame.releaseDay;

        StartGameSettlementAsync(releaseDate, releasedGame.gameResult.gameId, releasedGame.settlementCount, token.Token ).Forget();
    }

    // 금일 실행 중(또는 예정된) 이벤트 가져오기
    public IEvent GetActiveEvent(GameEventType type)
    {
        if (activeEventDic.TryGetValue(type, out ActiveEvent activeEvent))
            return activeEvent.Event;

        return null;
    }

    public bool TryGetNextEventDate(out GameDate nextEventDate)
    {
        GameDate currentDate = GameDataGetter<GameDate>.GetData();

        bool isFind = false;
        nextEventDate = default;

        foreach (var value in eventDateDic)
        {
            GameDate eventDate = value.Key;

            GameDate candidate = currentDate;

            candidate.month = eventDate.month;
            candidate.day = eventDate.day;
            candidate.hour = eventDate.hour;
            candidate.minutes = 0;

            // 이미 지난 이벤트라면 다음 해 이벤트로 넘김
            if (candidate <= currentDate)
            {
                candidate.year++;
            }

            // 가장 가까운 이벤트를 다음 날짜 이벤트로 지정
            if (!isFind || candidate < nextEventDate)
            {
                nextEventDate = candidate;
                isFind = true;
            }
        }

        return isFind;
    }

    private void UpdateNextEvent()
    {
        if (!TryGetNextEventDate(out GameDate nextEventDate))
            return;

        GameEventBridge.NextEventChanged(nextEventDate);
    }

    public void RefreshNextEvent(GameDate _)
    {
        UpdateNextEvent();
    }
}
