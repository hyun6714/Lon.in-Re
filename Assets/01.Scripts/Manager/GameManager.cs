using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public TextMeshProUGUI rebirthText;

    [Header("총 환생/게임출시 횟수/현재 직원수")]
    public int playerRebirthCount = 0;
    public int gameDevCount = 0;
    public int currentEmployeeCount;

    [Header("일시 정지")]
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateRebirthUI()
    {
        playerRebirthCount++;

        if (rebirthText != null)
        {
            rebirthText.text = playerRebirthCount.ToString();
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
