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

    [Header("일시 정지")]
    [SerializeField] private bool isPaused;
    #endregion

    [Header("테스트 전용")]
    [SerializeField] private TextMeshProUGUI text;


    private Dictionary<Season, string> seasonString = new Dictionary<Season, string>()
    {
        { Season.Spring, "봄" },
        { Season.Summer, "여름" },
        { Season.Fall, "가을" },
        { Season.Winter, "겨울" }
    };

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
        
        allEventStartHour = data.StartHour;
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

                TestTextShow();

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

        while (currentDate.minutes >= 60)
        {
            currentDate.minutes -= 60;
            currentDate.hour++;

            if (currentDate.hour >= 24)
            {
                currentDate.hour -= 24;
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
        EventDate date = new EventDate(currentDate.month, currentDate.day);

        GameEventBridge.DayChanged(date);
    }

    public string MinuteText()
    {
        return $"{(int)currentDate.minutes:D2}";
    }

    public string HourText()
    {
        return $"{(int)currentDate.hour:D2}";
    }

    public int GetLastDay(int year, int month)
    {
        if (month > 12 || month < 1)
        {
            Utils.Log("달이 12를 초과했거나 1 미만입니다.");
            return 0;
        }

        return DateTime.DaysInMonth(year, month);
    }

    public void TestTextShow()
    {
        text.text = $"{currentDate.year}년 {currentDate.month}월 {currentDate.day}일\n{seasonString[currentDate.season]}\n{HourText()} : {MinuteText()}";
    }

    /// <summary>
    /// 현재 날짜와 맞는지, 이벤트 발생 시간인지 비교
    /// </summary>
    /// <param name="year"> 목표 연도 </param>
    /// <param name="month"> 목표 월 </param>
    /// <param name="day"> 목표 일 </param>
    /// <returns></returns>
    public bool IsEventTime(int year, int month, int day)
    {
        return currentDate.year == year && currentDate.month == month && currentDate.day == day && currentDate.hour == allEventStartHour;
    }        

    // addDay 만큼의 일 수가 지난 후 날짜

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

        EventDate date = new EventDate(month, day);

        GameEventBridge.DayChanged(date);

        if (currentDate.hour >= allEventStartHour)
        {
            EventTrigger();
        }

        TestTextShow();
        Utils.Log("날짜 강제 변경 성공");
    }
#endif
}
