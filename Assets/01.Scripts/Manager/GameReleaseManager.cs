using System.Collections.Generic;
using UnityEngine;

// 게임 출시
// 출시된 게임 관리

public class GameReleaseManager : MonoBehaviour
{
    public static GameReleaseManager instance;

    public List<GameDevResult> releasedGames = new List<GameDevResult>();

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
        // 출시 게임 목록에 등록
        releasedGames.Add(gameResult);

        // 출시 날짜를 기준으로 정산 시작
        EventManager.instance.StartGameSettlement(gameResult.gameId);

        Utils.Log(
            $"게임 출시 완료  / ID : { gameResult.gameId} / " +
            $"최종 등급 : {gameResult.finalGrade} / " +
            $"현재 출시 게임 수 : {releasedGames.Count}"
        );
    }

    // 출시된 게임, ID 로 찾기
    public GameDevResult GetReleasedGame(int gameId)
    {
        foreach (GameDevResult game in releasedGames)
        {
            if (game.gameId == gameId)
            {
                return game;
            }
        }

        Utils.Log($"출시된 게임을 찾을 수 없습니다. ID : {gameId}");
        return null;
    }
}