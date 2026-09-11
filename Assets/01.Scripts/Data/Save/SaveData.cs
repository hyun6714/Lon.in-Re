using UnityEngine;
using System;
using System.Collections.Generic;

// 게임 전체 저장 데이터
[Serializable]
public class SaveData
{
    #region 재화

    // 일반 재화
    public int normalCurrency;

    // 특수 재화
    public int specialCurrency;

    // 명성
    public int reputation;

    #endregion


    #region 게임 진행

    // 총 환생 횟수
    public int playerRebirthCount;

    // 게임 개발 / 출시 횟수
    public int gameDevCount;

    #endregion


    #region 회사 등급

    // 현재 회사 등급
    public RankManager.RankState currentRank;

    // 현재 총 직원 수
    public int currentEmployeeCount;

    #endregion


    #region 직원

    // 직원별 보유 수
    public List<EmployeeSaveData> employees = new List<EmployeeSaveData>();

    #endregion


    #region 게임 개발 / 출시

    // 다음 게임에 부여할 ID
    public int nextGameId;

    // 출시된 게임 정보
    public List<ReleasedGameSaveData> releasedGames = new List<ReleasedGameSaveData>();

    #endregion


    #region 게임 내 날짜

    public GameDateSaveData gameDate;

    #endregion


    #region 오프라인 보상

    // 마지막 게임 종료 실제 시간
    public long lastQuitTime;

    // 게임 종료 당시 초당 생산량
    public int lastProductionPerSecond;

    #endregion
}


// 직원 한 종류의 저장 데이터
[Serializable]
public class EmployeeSaveData
{
    public string employeeId;
    public int count;
}