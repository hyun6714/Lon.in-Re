using UnityEngine;

[CreateAssetMenu(fileName = "NewRankData", menuName = "Rank/RankData")]
public class RankData : ScriptableObject
{
    public RankManager.RankState rank;
    public string rankDisplayName; // 랭크이름
    public Sprite rankIcon; //icon

    [Header("스탯 정보")]
    public bool hasEmployees;
    public int maxEmployee;

    [Header("해당 등급 승급 조건")]
    public int reqGamesReleased; // 출시 횟수
    public int reqEmployeeCount; // 직원 인원수
    public int reqReputation; // 명성 수치 
    public long reqNormal; // 골드 수치 

}