using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventManagerDaeta", menuName = "Game/EventManagerData")]
public class EventManagerData : ScriptableObject
{
    [Header("n일 뒤 정산 리스트")]
    [SerializeField]
    private List<int> nextSettlements = new List<int>()
    {
        30,
        60
    };

    [SerializeField]
    private List<int> nextHighSettlements = new List<int>()
    {
        14,
        28
    };

    public List<int> NextSettlements => nextSettlements;
    public List<int> NextHighSettlements => nextHighSettlements;
    public int SettlementNum => nextSettlements.Count;
    public int HighSettlementNum => nextHighSettlements.Count;

    public List<int> GetCurrentSettlementList(RankManager.RankState rank)
    {
        if (rank >= RankManager.RankState.Small)
        {
            return nextHighSettlements; 
        }
        return nextSettlements;
    }
}
