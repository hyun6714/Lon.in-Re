using UnityEngine;

[CreateAssetMenu(fileName = "TreasureGoblinEventData", menuName = "Event/TreasureGoblinEventData")]
public class TreasureGoblinEventData : ScriptableObject
{
    [Header("이벤트 텍스트")]
    [SerializeField] private string eventStartText = "보물 고블린 등장!";
    [SerializeField] private string eventSuccessText = "보물 고블린을 잡았다!";
    [SerializeField] private string eventFailText = "보물 고블린이 도망갔다...";

    [Header("이벤트")]
    [SerializeField] private float eventTimer = 5f;
    [SerializeField] private int requireTouchCount = 15;

    [Header("이벤트 보상")]
    [SerializeField] private int solo = 1000;
    [SerializeField] private int indie = 5000;
    [SerializeField] private int small = 30000;
    [SerializeField] private int midsized = 200000;
    [SerializeField] private int major = 800000;

    public string EventStartText => eventStartText;
    public string EventSuccessText => eventSuccessText;
    public string EventFailText => eventFailText;

    public float EventTimer => eventTimer;
    public int RequireTouchCount => requireTouchCount;

    public int Solo => solo;
    public int Indie => indie;
    public int Small => small;
    public int Midsized => midsized;
    public int Major => major;
}
