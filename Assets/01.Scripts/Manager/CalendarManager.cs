using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using TMPro;

public enum Season
{
    None = 0,
    Spring = 3,
    Summer = 6,
    Fall = 9,
    Winter = 12
}

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager instance;

    [Header("초기값 데이터")]
    [SerializeField] private CalendarData data;

    [Header("날짜 저장용 구조체")]
    [SerializeField] private GameDate currentDate;
    public GameDate CurrentDate => currentDate;

    #region Time & Date Settings
    // 현실 1초당 인게임 15분
    [Header("시간 설정")]
    [SerializeField] private int minutePerSec;
    [SerializeField] private float realTimer;
    [SerializeField] private float totalTime = 0f;
    #endregion

    #region Event Settings
    [Header("이벤트 시작 시간")]
    [SerializeField] private int allEventStartHour;
    public int AllEventStartHour => allEventStartHour;

    [Header("일시 정지")]
    [SerializeField] private bool isPaused;
    #endregion

    [Header("테스트 전용")]
    [SerializeField] private TextMeshProUGUI text;


    private Dictionary<Season, string> seasonString = new Dictionary<Season, string>();

    private CancellationTokenSource token;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        StartDayInit();
    }

    private void OnEnable()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        SubscribeEvent();
        UpdateTimeTick(token.Token).Forget();
    }

    private void OnDisable()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        UnSubscribeEvent();
    }

    public void SubscribeEvent()
    {
        GameEventBridge.OnPausedChanged += PausedChanged;
        Utils.Log("CalendarManager 구독 완료");
    }

    public void UnSubscribeEvent()
    {
        GameEventBridge.OnPausedChanged -= PausedChanged;
    }
    
    // 시작 날짜, 이벤트 날짜 초기화
    public void StartDayInit()
    {
        if (data == null)
        {
            Utils.Log("날짜 데이터가 존재하지 않습니다.");
            return;
        }

        currentDate = new GameDate(data);        
        
        allEventStartHour = data.BaseEventHour;
        realTimer = data.RealTime;
        minutePerSec = data.MinutePerSec;

        seasonString.Clear();
        foreach (SeasonInfo info in data.SeasonList)
        {
            seasonString[info.season] = info.seasonName;
        }

    }

    public GameDateSaveData SaveDate()
    {
        GameDateSaveData saveData = new GameDateSaveData(currentDate);
        return saveData;
    }

    public void LoadDate(GameDateSaveData saveData)
    {
        currentDate.LoadDate(saveData);
    }

    private async UniTaskVoid UpdateTimeTick(CancellationToken token)
    {
        try
        {
            //float elapsedTime = 0f;
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(realTimer), cancellationToken: token);

                AddTime(minutePerSec);

                GameEventBridge.TimeChanged(UIName.DateHUD, currentDate);

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, token);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    private void AddTime(int minutes)
    {
        currentDate.minutes += minutes;
        totalTime += minutes;

        while (currentDate.minutes >= data.MaxMinutePerHour)
        {
            currentDate.minutes -= data.MaxMinutePerHour;
            currentDate.hour++;

            if (currentDate.hour >= data.MaxHourPerDay)
            {
                currentDate.hour -= data.MaxHourPerDay;
                NextDay();
            }

            if (currentDate.hour == allEventStartHour)
            {
                EventTrigger();
            }
        }    
    }

    private void EventTrigger()
    {
        GameEventBridge.EventStarted();
    }

    private void NextDay()
    {
        currentDate.NextDay();

        GameEventBridge.DayChanged(currentDate);
    }

    /// <summary>
    /// 해당 달의 마지막날 계산
    /// </summary>
    /// <param name="month"> 목표 달 </param>
    /// <returns> 달의 마지막 날 </returns>
    public int GetLastDay(int month)
    {
        if (month > data.MaxMonthPerYear || month < 1)
        {
            Utils.Log($"달이 범위를 초과했습니다. {month}");
            return 0;
        }

        if (month <= data.DaysInMonthList.Count)
        {
            return data.DaysInMonthList[month - 1];
        }

        return data.DefaultDaysInMonth;
    }

    /// <summary>
    /// 현재 날짜와 맞는지, 이벤트 발생 시간인지 비교하는 함수
    /// </summary>
    /// <param name="year"> 목표 연도 </param>
    /// <param name="month"> 목표 월 </param>
    /// <param name="day"> 목표 일 </param>
    /// <returns></returns>
    public bool IsEventTime(GameDate date)
    {
        return currentDate.year == date.year && currentDate.month == date.month && currentDate.day == date.day && currentDate.hour == allEventStartHour;
    }

    /// <summary>
    /// addDay 만큼의 일 수가 지난 후 날짜
    /// </summary>
    /// <param name="addDay"> 추가할 일 수 </param>
    /// <returns></returns>
    public GameDate GetAfterDay(int addDay)
    {
        return currentDate.GetAfterDay(addDay);
    }

    public void PausedChanged(bool isPaused)
    {
        this.isPaused = isPaused;
    }

#if UNITY_EDITOR
    public void SetDateOnlyEditor(int year, int month, int day, int hour)
    {
        currentDate.SetDate(year, month, day, hour);

        GameEventBridge.DayChanged(currentDate);

        if (currentDate.hour >= allEventStartHour)
        {
            EventTrigger();
        }

        GameEventBridge.TimeChanged(UIName.DateHUD, currentDate);
        Utils.Log("날짜 강제 변경 성공");
    }
#endif
}
