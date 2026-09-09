using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager instance { get; private set; }

    public ArtifactDatabase artifactDatabase;

    private Dictionary<int, bool> unlockedStates = new Dictionary<int, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitUnlockedStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitUnlockedStates()
    {
        if (artifactDatabase == null) return;

        foreach (var info in artifactDatabase.artifacts)
        {
            if (!unlockedStates.ContainsKey(info.artifactId))
            {
                unlockedStates.Add(info.artifactId, false);
            }
        }
    }

    public bool IsUnlocked(int artifactID)
    {
        return unlockedStates.TryGetValue(artifactID, out bool isUnlocked) && isUnlocked;
    }

    public bool TryUnlockArtifact(int artifactID, int playerRebirth, int playerReputation, int playerSpecialCurrency)
    {
        ArtifactInfo info = artifactDatabase.GetArtifactsInfo(artifactID);
        if(info==null || IsUnlocked(artifactID))
        {
            return false;
        }

        //환생 횟수조건 검사 
        if(info.requiredRebirthCount > 0 && playerRebirth< info.requiredRebirthCount)
        {
            Debug.Log($"환생 횟수가 부족합니다 필요한 횟수 :{info.requiredRebirthCount}");
            return false;
        }

        //명성 수치 검사
        if (info.requiredReputation > 0 && playerReputation < info.requiredReputation)
        {
            Debug.Log($"명성 부족 필요한 명성:{info.requiredReputation}");
            return false;
        }

        if(info.SpecialUnlockCost > 0 && playerSpecialCurrency < info.SpecialUnlockCost)
        {
            Debug.Log($"특수 재화 부족 필요한 특수 재화 : {info.SpecialUnlockCost}");
            return false;
        }

        unlockedStates[artifactID] = true;

        GameEventBridge.CurrencyUsed(CurrencyType.Special, info.SpecialUnlockCost);
        Debug.Log($"{info.artiName} 아티팩트 해금");
        return true;
    }


    //클릭당 효과 증가
    public float GetTotalGainPerClick()
    {
        float total = 0f;

        foreach (var info in artifactDatabase.artifacts)
        {
            if (IsUnlocked(info.artifactId))
            {
                total += info.GainperClick; 
            }
        }
        return total;
    }

    //초당 획득량 증가
    public float GetTotalPerSecond()
    {
        float total = 0f;
        foreach (var info in artifactDatabase.artifacts)
        {
            if (IsUnlocked(info.artifactId))
            {
                total += info.PerSecond;
            }
        }
        return total;
    }

    // 확률 증가
    public float GetTotalProbabilityIncrease()
    {
        float total = 0f;
        foreach (var info in artifactDatabase.artifacts)
        {
            if (IsUnlocked(info.artifactId))
            {
                total += info.Probabilityincrease;
            }
        }
        return total;
    }
}
