using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager instance { get; private set; }

    public ArtifactDatabase artifactDatabase;

    private void Awake()
    {
        ResetArtifactsForEditor();

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


    //playerRebirth 플레이어 환생횟수 변수 변경할 필요 있음 
    public bool TryUnlockArtifact(int artifactID, int playerRebirth, int playerReputation, int playerSpecialCurrency)
    {
        ArtifactInfo info = artifactDatabase.GetArtifactsInfo(artifactID);
        if(info==null || info.isUnlocked)
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

        info.isUnlocked = true;
        CurrencyManager.instance.UseCurrency(CurrencyType.Special, info.SpecialUnlockCost);
        Debug.Log($"{info.artiName} 아티팩트 해금");
        return true;
    }


    //클릭당 효과 증가
    public float GetTotalGainPerClick()
    {
        float total = 0f;

        foreach (var info in artifactDatabase.artifacts)
        {
            if (info.isUnlocked)
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
            if (info.isUnlocked)
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
            if (info.isUnlocked)
            {
                total += info.Probabilityincrease;
            }
        }
        return total;
    }


    //확인 할려고 만든 아티팩트 초기화 함수 나중에 삭제해야함
    public void ResetArtifactsForEditor()
    {
        //삭제해야함 
        PlayerPrefs.DeleteAll(); // 모든 저장 데이터 초기화 (테스트용)
        Debug.Log("PlayerPrefs 초기화 완료");

        if (artifactDatabase == null || artifactDatabase.artifacts == null)
        {
            return;
        }

        foreach (var info in artifactDatabase.artifacts)
        {
            if (info != null)
            {
                info.isUnlocked = false;
            }
        }
        Debug.Log("아티팩트 초기화 완료");
    }
}
