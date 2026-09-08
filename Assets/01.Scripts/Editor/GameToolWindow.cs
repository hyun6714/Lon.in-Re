using UnityEditor;
using UnityEngine;

public class GameToolWindow : EditorWindow
{
    private int year = 1;
    private int month = 3;
    private int day = 1;
    private int hour = 7;

    private int normalCurrency = 0;
    private int specialCurrency = 0;
    private int reputationCurrency = 0;

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

        if (GUILayout.Button("자동 생산, 이벤트, 캘린더 일시 정지"))
        {
            GameManager.Instance.GamePaused();
        }

        if (GUILayout.Button("자동 생산, 이벤트, 캘린더 재생"))
        {
            GameManager.Instance.GameResume();
        }

        EditorGUILayout.Space(10);

        GUILayout.Label("날짜 강제 변경");
        EditorGUILayout.HelpBox("일(Day)은 해당하는 월의 마지막 일수를 초과할 수 없습니다", MessageType.Warning);
        year = EditorGUILayout.IntField("연도(Year)", year);
        month = EditorGUILayout.IntSlider("월(Month)", month, 1, 12);
        day = EditorGUILayout.IntField("일(Day)", day);
        hour = EditorGUILayout.IntSlider("시(Hour)", hour, 0, 23);

        EditorGUILayout.Space(5);

        GUI.enabled = CalendarManager.instance != null;

        if (GUILayout.Button("날짜 적용"))
        {
            CalendarManager.instance.SetDateOnlyEditor(year, month, day, hour);
        }

        EditorGUILayout.Space(10);

        GUILayout.Label("재화 획득, 감소");
        EditorGUILayout.HelpBox("버튼을 누르면 작성한 수치만큼 재화를 획득하거나 잃습니다.", MessageType.Info);
        normalCurrency = EditorGUILayout.IntField("일반 재화", normalCurrency);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("일반 재화 획득"))
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Normal, normalCurrency);
            CurrencyManager.instance.CurrencyTestSet();
        }

        EditorGUILayout.Space(1);

        if(GUILayout.Button("일반 재화 감소"))
        {
            GameEventBridge.CurrencyUsed(CurrencyType.Normal, normalCurrency);
            CurrencyManager.instance.CurrencyTestSet();
        }

        EditorGUILayout.Space(7);

        specialCurrency = EditorGUILayout.IntField("특수 재화", specialCurrency);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("특수 재화 획득"))
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Special, specialCurrency);
            CurrencyManager.instance.CurrencyTestSet();
        }

        EditorGUILayout.Space(1);

        if (GUILayout.Button("특수 재화 감소"))
        {
            GameEventBridge.CurrencyUsed(CurrencyType.Special, specialCurrency);
            CurrencyManager.instance.CurrencyTestSet();
        }

        EditorGUILayout.Space(7);

        reputationCurrency = EditorGUILayout.IntField("명성", reputationCurrency);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("명성 획득"))
        {
            GameEventBridge.CurrencyAdded(CurrencyType.Reputation, reputationCurrency);
            CurrencyManager.instance.CurrencyTestSet();
        }

        EditorGUILayout.Space(1);

        if (GUILayout.Button("명성 감소"))
        {
            GameEventBridge.CurrencyUsed(CurrencyType.Reputation, reputationCurrency);
            CurrencyManager.instance.CurrencyTestSet();
        }

        EditorGUILayout.EndScrollView();
    }
}
