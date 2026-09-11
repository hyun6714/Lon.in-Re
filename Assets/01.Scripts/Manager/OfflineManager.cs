using UnityEngine;
using System;

// 오프라인 보상 획득
public class OfflineManager : MonoBehaviour
{
    public static OfflineManager instance;

    // 마지막으로 게임을 종료한 실제 시간
    private DateTime lastQuitTime;

    // 게임 종료 당시 초당 생산량(합계)
    private int lastProductionPerSecond;

    // 오프라인 보상(재접속시 획득하는 보상)
    private int offlineReward;

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

    // 오프라인 보상 계산
    public int CalculateOfflineReward(DateTime quitTime, int productionPerSecond)
    {
        // 현재 시간과 마지막 종료 시간의 차이 계산
        TimeSpan offlineTime = DateTime.UtcNow - quitTime;

        // 오프라인 시간을 초 단위로 변환
        double offlineSeconds = offlineTime.TotalSeconds;

        // 초당 생산량 × 오프라인 시간
        int reward = (int)(productionPerSecond * offlineSeconds);

        return reward;
    }
}
