using System.Collections.Generic;
using UnityEngine;

public class GamePopupUI : MonoBehaviour
{
    [Header("게임 카드")]
    [SerializeField] private GameObject gameCardPrefab;
    [SerializeField] private Transform content;

    [Header("등급 이미지")]
    [SerializeField] private Sprite aGradeSprite;
    [SerializeField] private Sprite bGradeSprite;
    [SerializeField] private Sprite cGradeSprite;

    [Header("게임 출시 데이터")]
    [SerializeField] private GameReleaseData gameReleaseData;

    [Header("게임 정산 데이터")]
    [SerializeField] private EventManagerData eventManagerData;

    // 이미 화면에 표시된 게임 ID
    private HashSet<int> displayedGameIds = new HashSet<int>();

    // 게임 팝업이 열릴 때 게임 목록 갱신
    private void OnEnable()
    {
        GameReleaseManager.instance.OnGameReleased += AddGameCard;

        GameEventBridge.OnReincarnated += ResetDataOnRebirth;

        RefreshGameList();
    }
    private void OnDisable()
    {
        if (GameReleaseManager.instance != null)
        {
            GameReleaseManager.instance.OnGameReleased -= AddGameCard;
        }

        GameEventBridge.OnReincarnated -= ResetDataOnRebirth;
    }

    // 출시된 게임 목록을 UI에 표시
    public void RefreshGameList()
    {
        Utils.Log($"게임 목록 갱신 / 출시 게임 수 : {GameReleaseManager.instance.releasedGames.Count}");

        foreach (ReleasedGameSaveData releasedGame in GameReleaseManager.instance.releasedGames)
        {
            AddGameCard(releasedGame);
        }
    }

    // 새로 출시된 게임 카드 하나 추가
    private void AddGameCard(ReleasedGameSaveData releasedGame)
    {
        Utils.Log($"AddGameCard 호출됨 / ID : {releasedGame.gameResult.gameId}");
        int gameId = releasedGame.gameResult.gameId;

        // 이미 표시된 게임이면 생성하지 않음
        if (displayedGameIds.Contains(gameId))
        {
            return;
        }

        GameObject cardObject = Instantiate(gameCardPrefab, content);

        GameCardUI gameCardUI = cardObject.GetComponent<GameCardUI>();

        gameCardUI.SetData(releasedGame, gameReleaseData, eventManagerData, aGradeSprite, bGradeSprite, cGradeSprite);


        // 표시 완료된 게임 ID 저장
        displayedGameIds.Add(gameId);

        Utils.Log($"게임 카드 추가 / ID : {gameId}");
    }

    private void ResetDataOnRebirth()
    {
        displayedGameIds.Clear();
    }
}