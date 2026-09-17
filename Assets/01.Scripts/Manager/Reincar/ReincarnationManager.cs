using System;
using System.Collections.Generic;
using UnityEngine;

public class ReincarnationManager : MonoBehaviour
{
    public static event Action OnReincarnated;

    public GameObject ReincarnationPop;

    public Transform gameListContentParent;

    private void OnEnable()
    {
        OnReincarnated += ResetDataOnRebirth;
    }

    private void OnDisable()
    {
        OnReincarnated -= ResetDataOnRebirth;
    }

    public void BtnReincarnation()
    {
        if (!CanReincarnation())
        {
            return;
        }

        int currentReputation = GetCurrentReputation();

        if (CurrencyManager.instance != null)
        {
            //특수 재화 주는 식
            int excesReputation = currentReputation - 5000;
            if (excesReputation > 0 && CurrencyManager.instance != null)
            {
                GameEventBridge.CurrencyAdded(CurrencyType.Special, excesReputation);
                Debug.Log($"환생 완료 초과명성 {excesReputation}만큼 특수재화를 획득 ");
            }
        }

        OnReincarnated?.Invoke();
        GameManager.instance.UpdateRebirthUI();

        if (ReincarnationPop == null) return;
        ReincarnationPop.SetActive(false);

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

    //플레이어가 가지고 있는 명성 가져오는 함수 
    private int GetCurrentReputation()
    {
        if (CurrencyManager.instance != null)
        {
            return CurrencyManager.instance.GetAmount(CurrencyType.Reputation);
        }

        return 0;
    }

    public void ResetDataOnRebirth()
    {
        if (GameReleaseManager.instance != null)
        {
            GameReleaseManager.instance.releasedGames.Clear();
        }

        if (gameListContentParent != null)
        {
            GameCardUI[] spawnedCards = gameListContentParent.GetComponentsInChildren<GameCardUI>();

            foreach (Transform child in gameListContentParent)
            {
                GameCardUI cardUI = child.GetComponent<GameCardUI>();
                if (cardUI != null)
                {
                    Destroy(cardUI.gameObject);
                }
            }
        }
    }
}
