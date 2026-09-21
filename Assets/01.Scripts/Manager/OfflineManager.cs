using UnityEngine;
using System;

// 오프라인 보상 획득
public class OfflineManager : MonoBehaviour
{
    public static OfflineManager instance;

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
    public int CalculateOfflineReward(long quitTime, float productionPerSecond)
    {
        // 저장된 종료 시간이 없으면 오프라인 보상 없음
        if (quitTime <= 0)
        {
            return 0;
        }

        // 저장된 시간을 DateTime으로 변환
        DateTime lastSaveTime = new DateTime(quitTime, DateTimeKind.Utc);

        // 현재 시간과 마지막 저장 시간의 차이 계산
        TimeSpan offlineTime = DateTime.UtcNow - lastSaveTime;

        // 오프라인 시간을 초 단위로 변환
        double offlineSeconds = offlineTime.TotalSeconds;

        // 초당 생산량 × 오프라인 시간
        int reward = (int)(productionPerSecond * offlineSeconds);

        return reward;
    }
}
