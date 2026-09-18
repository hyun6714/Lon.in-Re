using UnityEditor;
using UnityEngine;

public class GameToolWindow : EditorWindow
{
    private int setYear = 1;
    private int setMonth = 3;
    private int setDay = 1;
    private int setHour = 7;

    private int normalCurrency = 10000;
    private int specialCurrency = 10000;
    private int reputationCurrency = 10000;

    private Vector2 scrollPos;

    [MenuItem("Tools/Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<GameToolWindow>("디버그 툴");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("시간 제어");

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 중에만 동작", MessageType.Info);
            EditorGUILayout.EndScrollView();
            return;
        }

        EditorGUILayout.HelpBox("필요한 스크립트를 가진 오브젝트가 없으면 작동하지 않을 수 있습니다.", MessageType.Warning);

        EditorGUILayout.Space(10);

        EditorGUILayout.Space(10);

        #region 일시정지
        if (GUILayout.Button("일시정지"))
        {
            GameManager.instance.GamePaused();
        }

        if (GUILayout.Button("일시정지 해제"))
        {
            GameManager.instance.GameResume();
        }
        #endregion

        EditorGUILayout.Space(10);

        #region 날짜 변경
        GUILayout.Label("날짜 강제 변경");

        CalendarManager calendar = CalendarManager.instance;

        
        if (calendar != null)
        {
            GameDate currentDate = calendar.CurrentDate;

            int maxMonth = currentDate.maxMonthPerYear;
            int maxHour = currentDate.maxHourPerDay;

            setYear = EditorGUILayout.IntField("연도(Year)", setYear);
            setMonth = EditorGUILayout.IntSlider("월(Month)", setMonth, 1, maxMonth);
            
            int maxDay = calendar.GetLastDay(setMonth);
            
            setDay = EditorGUILayout.IntSlider("일(Day)", setDay, 1, maxDay);
            setHour = EditorGUILayout.IntSlider("시(Hour)", setHour, 0, maxHour);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("날짜 적용"))
            {
                GameDate date = new GameDate()
                {
                    year = setYear,
                    month = setMonth,
                    day = setDay,
                    hour = setHour
                };
                CalendarManager.instance.SetDateOnlyEditor(date);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("CalendarManager가 존재하지 않습니다.", MessageType.Warning);

        }        
        #endregion

        EditorGUILayout.Space(15);

        #region 재화 획득
        GUILayout.Label("재화 획득, 감소");
        EditorGUILayout.HelpBox("버튼을 누르면 작성한 수치만큼 재화를 획득하거나 잃습니다.", MessageType.Info);
       
        normalCurrency = EditorGUILayout.IntField("일반 재화", normalCurrency);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("일반 재화 획득"))
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Normal, normalCurrency);
        }

        EditorGUILayout.Space(1);

        if(GUILayout.Button("일반 재화 감소"))
        {
            GameEventBridge.CurrencyUsed(CurrencyType.Normal, normalCurrency);
        }

        EditorGUILayout.Space(10);

        specialCurrency = EditorGUILayout.IntField("특수 재화", specialCurrency);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("특수 재화 획득"))
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Special, specialCurrency);
        }

        EditorGUILayout.Space(1);

        if (GUILayout.Button("특수 재화 감소"))
        {
            GameEventBridge.CurrencyUsed(CurrencyType.Special, specialCurrency);
        }

        EditorGUILayout.Space(10);

        reputationCurrency = EditorGUILayout.IntField("명성", reputationCurrency);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("명성 획득"))
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Reputation, reputationCurrency);
        }

        EditorGUILayout.Space(1);

        if (GUILayout.Button("명성 감소"))
        {
            GameEventBridge.CurrencyUsed(CurrencyType.Reputation, reputationCurrency);
        }
        #endregion

        EditorGUILayout.Space(15);

        if (GUILayout.Button("진행도 초기화 및 종료"))
        {
            SaveManager.instance.DeleteSaveData();
        }

        EditorGUILayout.EndScrollView();
    }
}
