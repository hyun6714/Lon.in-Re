using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventManagerData", menuName = "Game/EventManagerData")]
public class EventManagerData : ScriptableObject
{
    [Header("n일 뒤 정산 리스트")]
    [SerializeField]
    private List<int> nextSettlements = new List<int>()
    {
        30,
        60
    };

    public List<int> NextSettlements => nextSettlements;
    public int SettlementNum => nextSettlements.Count;
}
