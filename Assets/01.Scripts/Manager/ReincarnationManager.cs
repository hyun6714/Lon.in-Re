using UnityEngine;

public class ReincarnationManager : MonoBehaviour
{
    public void BtnReincarnation()
    {
        if (!CanReincarnation())
        {
            return;
        }

        int currentReputation = GetCurrentReputation();

        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.ResetCurrenciesExceptSpecial();

            //특수 재화 주는 식
            int excesReputation = currentReputation - 5000;
            if (excesReputation > 0 && CurrencyManager.instance != null)
            {
                CurrencyManager.instance.AddCurrency(CurrencyType.Special, excesReputation);
                Debug.Log($"환생 완료 초과명성 {excesReputation}만큼 특수재화를 획득 ");
            }
        }

        GameManager.Instance.playerRebirthCount++;
        ResetRankDate();

        Debug.Log($"환생 완료");
    }

    //환생 조건 
    private bool CanReincarnation()
    {
        int currentReputation = GetCurrentReputation();

        if (currentReputation < 5000 )
        {
            Debug.Log($"환생 조건 미달");
            return false;
        }
        return true;
    }

    //등급 초기화
    private void ResetRankDate()
    {
        RankManager.instance.ResetRank();
    }


    //플레이어가 가지고 있는 명성 가져오는 함수 
    private int GetCurrentReputation()
    {
        if (CurrencyManager.instance != null)
        {
            return CurrencyManager.instance.GetAmount(CurrencyType.Reputation);
        }

        return 0;
    }
}
