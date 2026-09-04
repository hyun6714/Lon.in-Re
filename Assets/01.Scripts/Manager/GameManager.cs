using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("총 환생/게임출시 횟수")]
    public int playerRebirthCount = 0;
    public int gameDevCount = 0;

    [Header("일시 정지")]
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GamePaused()
    {
        Time.timeScale = 0;
        IsPaused = true;
        GameEventBridge.PausedChanged(IsPaused);
    }

    public void GameResume()
    {
        Time.timeScale = 1;
        IsPaused = false;
        GameEventBridge.PausedChanged(IsPaused);
    }
}
