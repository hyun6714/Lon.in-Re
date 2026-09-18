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

    //해금 초기화 
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

    public bool TryUnlockArtifact(int artifactID)
    {
        ArtifactInfo info = artifactDatabase.GetArtifactsInfo(artifactID);
        if(info==null || IsUnlocked(artifactID))
        {
            return false;
        }

        int playerRebirth = GameManager.instance != null ? GameManager.instance.playerRebirthCount : 0;
        int playerReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;
        int playerSpecial = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Special) : 0;

        if (playerRebirth < info.requiredRebirthCount ||
            playerReputation < info.requiredReputation ||
            playerSpecial < info.SpecialUnlockCost)
        {
            Debug.Log("조건 부족으로 해금 실패");
            return false;
        }

        unlockedStates[artifactID] = true;

        GameEventBridge.CurrencyUsed(CurrencyType.Special, info.SpecialUnlockCost);

        GameEventBridge.ArtifactUnlocked();
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
                foreach (var effect in info.effects)
                {
                    if (effect.effectType == EffectType.GainPerClick)
                    {
                        total += effect.effectValue;
                    }
                }
            }
        }
        return total;
    }

    //초당 획득량 증가
    public float GetTotalPerSecond()
    {
        float total = 1f;
        foreach (var info in artifactDatabase.artifacts)
        {
            if (IsUnlocked(info.artifactId))
            {
                // 리스트 안의 효과들을 순회
                foreach (var effect in info.effects)
                {
                    if (effect.effectType == EffectType.PerSecond)
                    {
                        total *= effect.effectValue;
                    }
                }
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
                // 리스트 안의 효과들을 순회
                foreach (var effect in info.effects)
                {
                    if (effect.effectType == EffectType.ProbabilityIncrease)
                    {
                        total += effect.effectValue;
                    }
                }
            }
        }
        return total;
    }

    // 아티팩트 저장 데이터 반환
    public ArtifactSaveData GetSaveData()
    {
        ArtifactSaveData saveData = new ArtifactSaveData();

        foreach (var state in unlockedStates)
        {
            if (state.Value)
            {
                saveData.unlockedArtifactIDs.Add(state.Key);
            }
        }

        return saveData;
    }

    // 아티팩트 저장 데이터 불러오기
    public void LoadSaveData(ArtifactSaveData saveData)
    {
        unlockedStates.Clear();
        InitUnlockedStates();

        foreach (int artifactId in saveData.unlockedArtifactIDs)
        {
            if (unlockedStates.ContainsKey(artifactId))
            {
                unlockedStates[artifactId] = true;
            }
        }
    }
}
