using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using TMPro;

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
    [Header("이벤트 시작 날짜")]
    [SerializeField] private int seasonEventStartDay;
    [SerializeField] private int seasonEventMonth;
    [SerializeField] private int allEventStartHour;

    [Header("일시 정지")]
    [SerializeField] private bool isPaused = true;

    [Header("이벤트 플래그")]
    [SerializeField] private bool hasSeasonEvent;
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

        seasonEventStartDay = data.SeasonEventStartDay;

        seasonEventMonth = currentDate.month + 1;
        if (seasonEventMonth > 12)
        {
            seasonEventMonth = 1;
        }

        hasSeasonEvent = false;
        GameEventBridge.SeasonChanged(currentDate.season);
    }

    private async UniTaskVoid UpdateTimeTick(CancellationToken token)
    {
        try
        {
            //float elapsedTime = 0f;
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(realTimer), cancellationToken: token);

                //if (!isPaused)
                //{
                //    elapsedTime += Time.deltaTime;

                //    while (elapsedTime >= realTimer)
                //    {
                //        elapsedTime -= realTimer;
                //        AddTime(minutePerSec);
                //        TestTextShow();
                //    }
                //}

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

    private void EventDayCheck()
    {
        if (currentDate.month == seasonEventMonth && currentDate.day == seasonEventStartDay)
        {
            hasSeasonEvent = true;
        }
    }

    private void EventTrigger()
    {
        if (hasSeasonEvent)
        {
            hasSeasonEvent = false;
            GameEventBridge.SeasonEvent();
        }
    }

    private void NextDay()
    {
        Season season = currentDate.season;

        currentDate.NextDay();

        EventDayCheck();

        if (season != currentDate.season)
        {
            NextSeason();
        }
    }

    //private void NextMonth()
    //{
    //    currentDate.month++;

    //    if (currentDate.month > 12)
    //    {
    //        currentDate.month = 1;
    //        NextYear();
    //    }

    //    if (currentDate.month % 3 == 0)
    //    {
    //        NextSeason(currentDate.month);
    //    }
    //}

    //private void NextYear()
    //{
    //    currentDate.year++;
    //}

    private void NextSeason()
    {
        GameEventBridge.SeasonChanged(currentDate.season);

        seasonEventMonth = currentDate.month + 1;
        if (seasonEventMonth > 12)
        {
            seasonEventMonth = 1;
        }
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

    /// <summary>
    /// 지정한 날짜로부터 n일 후의 날짜를 계산해주는 함수
    /// </summary>
    /// <param name="year"> 계산 시작 연도 </param>
    /// <param name="month"> 계산 시작 달 </param>
    /// <param name="day"> 계산 시작 일 </param>
    /// <param name="addDay"> 추가 될 일수 </param>
    /// <returns></returns>
    //public (int year, int month, int day) GetNextDay(int year, int month, int day, int addDay)
    //{
    //    int nextYear = year;
    //    int nextMonth = month;
    //    int nextDay = day + addDay;

    //    while (nextDay > GetLastDay(nextYear, nextMonth))
    //    {
    //        nextDay -= GetLastDay(nextYear, nextMonth);
    //        nextMonth++;

    //        if (nextMonth > 12)
    //        {
    //            nextMonth = 1;
    //            nextYear++;
    //        }
    //    }

    //    return (nextYear, nextMonth, nextDay);
    ////}

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
        currentDate.year = year;
        currentDate.month = Mathf.Clamp(month, 1, 12);

        currentDate.lastDay = GetLastDay(currentDate.year, currentDate.month);
        currentDate.day = Mathf.Clamp(day, 1, currentDate.lastDay);

        currentDate.hour = Mathf.Clamp(hour, 0, 23);
        currentDate.minutes = 0;

        currentDate.season = currentDate.month switch
        {
            >= 3 and <= 5 => Season.Spring,
            >= 6 and <= 8 => Season.Summer,
            >= 9 and <= 11 => Season.Fall,
            _ => Season.Winter
        };

        seasonEventMonth = currentDate.season switch
        {
            Season.Spring => 4,
            Season.Summer => 7,
            Season.Fall => 10,
            Season.Winter => 1,
            _ => 4
        };

        EventDayCheck();

        if (currentDate.hour >= allEventStartHour)
        {
            EventTrigger();
        }

        TestTextShow();
        GameEventBridge.SeasonChanged(currentDate.season);
        Utils.Log("날짜 강제 변경 성공");
    }
#endif
}
