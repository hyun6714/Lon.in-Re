using UnityEngine;

[CreateAssetMenu(fileName = "GiftEventData", menuName = "Event/GiftEventData")]
public class GiftEventData : ScriptableObject
{
    [Header("등급별 수령 금액")]
    [SerializeField] private int solo = 500;
    [SerializeField] private int indie = 2000;
    [SerializeField] private int small = 10000;
    [SerializeField] private int midsized = 50000;
    [SerializeField] private int major = 200000;
    
    public int Solo => solo;
    public int Indie => indie;
    public int Small => small;
    public int Midsized => midsized;
    public int Major => major;
}
