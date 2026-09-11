using System.Collections.Generic;
using UnityEngine;

// 게임 출시
// 출시된 게임 관리

public class GameReleaseManager : MonoBehaviour
{
    public static GameReleaseManager instance;

    // 출시된 게임 목록
    public List<ReleasedGameSaveData> releasedGames = new List<ReleasedGameSaveData>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 개발 완료된 게임을 출시 목록에 등록
    public void ReleaseGame(GameDevResult gameResult)
    {
        GameDate currentDate = CalendarManager.instance.CurrentDate;
        ReleasedGameSaveData releasedGame = new ReleasedGameSaveData();

        // 게임 개발 결과
        releasedGame.gameResult = gameResult;

        // 출시 직후에는 정산 횟수 0
        releasedGame.settlementCount = 0;

        // 출시 게임 목록에 추가
        releasedGames.Add(releasedGame);

        // 출시 날짜
        releasedGame.releaseYear = currentDate.year;
        releasedGame.releaseMonth = currentDate.month;
        releasedGame.releaseDay = currentDate.day;

        // 출시 날짜를 기준으로 정산 시작
        EventManager.instance.StartGameSettlement(gameResult.gameId);

        Utils.Log(
            $"게임 출시 완료  / ID : { gameResult.gameId} / " +
            $"최종 등급 : {gameResult.finalGrade} / " +
            $"현재 출시 게임 수 : {releasedGames.Count}"
        );
    }

    // 출시된 게임, ID 로 찾기
    public ReleasedGameSaveData GetReleasedGameData(int gameId)
    {
        foreach (ReleasedGameSaveData releasedGame in releasedGames)
        {
            if (releasedGame.gameResult.gameId == gameId)
            {
                return releasedGame;
            }
        }

        Utils.Log($"출시된 게임을 찾을 수 없습니다. ID : {gameId}");
        return null;
    }

    // 기존 GameSettlementManager에서 사용하기 위한 함수
    public GameDevResult GetReleasedGame(int gameId)
    {
        ReleasedGameSaveData releasedGame = GetReleasedGameData(gameId);

        if (releasedGame == null)
        {
            return null;
        }

        return releasedGame.gameResult;
    }


    // 정산 완료 횟수 증가
    public void IncreaseSettlementCount(int gameId)
    {
        ReleasedGameSaveData releasedGame = GetReleasedGameData(gameId);

        if (releasedGame == null)
        {
            return;
        }

        releasedGame.settlementCount++;

        Utils.Log(
            $"게임 정산 횟수 증가 / ID : {gameId} / " +
            $"현재 정산 횟수 : {releasedGame.settlementCount}"
        );
    }
}