using System;
using System.Collections.Generic;
using UnityEngine;

public class ReincarnationManager : MonoBehaviour
{
    public static ReincarnationManager instance;

    public GameObject ReincarnationPop;

    public Transform gameListContentParent;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameEventBridge.OnReincarnated += ResetDataOnRebirth;
    }

    private void OnDisable()
    {
        GameEventBridge.OnReincarnated -= ResetDataOnRebirth;
    }

    public void BtnReincarnation()
    {
        if (!CanReincarnation())
        {
            return;
        }

        int currentReputation = GetCurrentReputation();

        int requirement = DataManager.instance.reincarnationData.reincarnationRequirement;

        if (CurrencyManager.instance != null)
        {
            //특수 재화 주는 식
            int excesReputation = currentReputation - requirement;
            if (excesReputation > 0 && CurrencyManager.instance != null)
            {
                GameEventBridge.CurrencyAdded(CurrencyType.Special, excesReputation);
                Debug.Log($"환생 완료 초과명성 {excesReputation}만큼 특수재화를 획득 ");
            }
        }

        GameEventBridge.Reincarnated();

        Debug.Log($"환생 완료");
    }

    //환생 조건 
    public bool CanReincarnation()
    {
        int currentReputation = GetCurrentReputation();

        if (DataManager.instance == null || DataManager.instance.reincarnationData == null)
        {
            return false; 
        }

        int requirement = DataManager.instance.reincarnationData.reincarnationRequirement;

        if (currentReputation < requirement)
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
