using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 계절별 이벤트 시작, 종료를 정의하는 전략 인터페이스
/// </summary>
public interface IEvent
{
    void StartEvent();
    void EndEvent();
}

public enum Season
{
    None = 0,
    Spring = 3,
    Summer = 6,
    Fall = 9,
    Winter = 12
}

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    [Header("데이터")]
    [SerializeField] private EventManagerData data;

    [Header("계절 이벤트 데이터")]
    [SerializeField] private SummerEventData summerData;

    private EventFactory eventFactory;

    private IEvent currentEvent;

    private CancellationTokenSource token;

    // 정산 이벤트
    public event Action<int, int> OnGameSettlement;
    public event Action<float> OnMultiplierChanged;

    [Header("일시 정지")]
    [SerializeField] private bool isPaused;

    [Header("테스트용 UI")]
    [SerializeField] private GameObject testPanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        eventFactory = new EventFactory(summerData);
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
        GameEventBridge.OnSeasonChanged += ChangeSeason;
        GameEventBridge.OnSeasonEvent += StartSeasonEvent;
        GameEventBridge.OnPausedChanged += PausedChanged;
        Utils.Log("EventManager 구독 완료");
    }

    private void UnSubscribeEvent()
    {
        GameEventBridge.OnSeasonChanged -= ChangeSeason;
        GameEventBridge.OnSeasonEvent -= StartSeasonEvent;
        GameEventBridge.OnPausedChanged -= PausedChanged;
    }

    /// <summary>
    /// 여름 이벤트 발동 시 배율 변경
    /// </summary>
    /// <param name="multi"> isCool에 따른 배율 변경값 </param>
    public void SummerMultiplier(float multi)
    {
        OnMultiplierChanged?.Invoke(multi);
    }
    
    // UI 테스트용
    public void OnClickSummerCool(bool value)
    {
        if (currentEvent is SummerEvent strategy)
        {
            strategy.SetCool(value);
            Time.timeScale = 1;

            testPanel.SetActive(false);
        }
    }

    // 계절 변경 시 전략 교체
    public void ChangeSeason(Season season)
    {
        currentEvent = eventFactory.CreateEvent(season);
        Utils.Log($"현재 전략{currentEvent}");
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

    //public void StartGameDevTest(GameDate date, int gameId)
    //{
    //    targetDate = CalendarManager.instance.CurrentDate;
    //    this.gameId = gameId;

    //    CalendarManager.instance.OnDateChanged += SubscribeDevEventTest;
    //}

    //GameDate targetDate;
    //int gameId;

    //public void SubscribeDevEventTest(GameDate date)
    //{
    //    if (targetDate == date)
    //    {
    //        // 해당 날짜에 실행
    //        OnGameSettlement?.Invoke(gameId, i);
    //        Utils.Log($"[{gameId}]ID 프로젝트의 [{i}]번째 정산 완료.");

    //        CalendarManager.instance.OnDateChanged -= SubscribeDevEventTest;
    //    }
    //}

    public void StartSeasonEvent()
    {
        currentEvent?.StartEvent();

        // 테스트 용 코드. 나중에 삭제
        if (currentEvent is SummerEvent)
        {
            testPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void EndSeasonEvent()
    {
        currentEvent?.EndEvent();
    }    

    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
