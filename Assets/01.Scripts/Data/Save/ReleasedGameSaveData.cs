using UnityEngine;
using System;

// 출시된 게임의 저장 데이터
[Serializable]
public class ReleasedGameSaveData
{
    // 게임 개발 결과
    public GameDevResult gameResult;

    // 게임 출시 날짜
    public int releaseYear;
    public int releaseMonth;
    public int releaseDay;

    // 현재까지 완료된 정산 횟수
    public int settlementCount;
}
