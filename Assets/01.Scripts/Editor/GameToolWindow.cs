using UnityEditor;
using UnityEngine;

public class GameToolWindow : EditorWindow
{
    private int year = 1;
    private int month = 3;
    private int day = 1;
    private int hour = 7;

    [MenuItem("Tools/Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<GameToolWindow>("디버그 툴");
    }

    private void OnGUI()
    {
        GUILayout.Label("시간 제어");

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 중에만 동작", MessageType.Info);
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
    }
}
