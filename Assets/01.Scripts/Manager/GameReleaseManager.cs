using System.Collections.Generic;
using UnityEngine;
using System;

// 게임 출시
// 출시된 게임 관리

public class GameReleaseManager : MonoBehaviour
{
    public static GameReleaseManager instance;

    // 출시된 게임 목록
    public List<ReleasedGameSaveData> releasedGames = new List<ReleasedGameSaveData>();

    public event Action<ReleasedGameSaveData> OnGameReleased;

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

        // 출시 날짜
        releasedGame.releaseYear = currentDate.year;
        releasedGame.releaseMonth = currentDate.month;
        releasedGame.releaseDay = currentDate.day;

        // 출시 게임 목록에 추가
        releasedGames.Add(releasedGame);

        // 새로 출시된 게임 전달
        OnGameReleased?.Invoke(releasedGame);

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

        int activeGameCount = GetActiveGameCount(EventManager.instance.SettlementNum);

        GameEventBridge.GameDevSucceeded(activeGameCount,GameDevManager.instance.MaxActiveGameCount);

        Utils.Log(
            $"게임 정산 횟수 증가 / ID : {gameId} / " +
            $"현재 정산 횟수 : {releasedGame.settlementCount}"
        );
    }


    // 출시된 게임 버리기
    public bool RemoveGame(int gameId)
    {
        ReleasedGameSaveData releasedGame = GetReleasedGameData(gameId);

        if (releasedGame == null)
            return false;

        releasedGames.Remove(releasedGame);

        int activeGameCount = GetActiveGameCount(EventManager.instance.SettlementNum);

        GameEventBridge.GameDevSucceeded(activeGameCount,GameDevManager.instance.MaxActiveGameCount);

        Utils.Log($"게임 버리기 완료 / ID : {gameId}");

        return true;
    }

    // 현재 정산 진행 중인 게임 개수
    public int GetActiveGameCount(int settlementNum)
    {
        int activeGameCount = 0;

        foreach (ReleasedGameSaveData releasedGame in releasedGames)
        {
            // 모든 정산이 끝나지 않은 게임만 계산
            if (releasedGame.settlementCount < settlementNum)
            {
                activeGameCount++;
            }
        }

        return activeGameCount;
    }

    private void OnEnable()
    {
        GameEventBridge.OnReincarnated += ResetDataOnRebirth;
    }

    private void OnDisable()
    {
        GameEventBridge.OnReincarnated -= ResetDataOnRebirth;
    }

    //환생할 떄 게임 지우는 것
    private void ResetDataOnRebirth()
    {
        releasedGames.Clear();
    }
}