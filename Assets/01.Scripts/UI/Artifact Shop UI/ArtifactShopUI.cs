using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactShopUI : MonoBehaviour
{
    [Header("Reference")]
    public ArtifactDatabase artifactDatabase;
    public Transform contentParent; //스크롤 뷰 Content
    public GameObject slotPrefab; //Slot스크립트가 붙어있는 프리팹

    [Header("상점 패널")]
    public GameObject shopPanel;//상점 전체 패널 오브젝트 

    [Header("상점 버튼")]
    public Button openShopBtn;
    public Button closeShopBtn;

    private List<ShopArtifactSlot> spawnedSlots = new List<ShopArtifactSlot>();

    private void Start()
    {
        RefreshShop();
        shopPanel.SetActive(false);
    }

    public void RefreshShop()
    {
        // 기존에 있다면 삭제 및 리스트 정리 
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        spawnedSlots.Clear();

        int currentSpecial = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Special) : 0;
        int currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;
        int currentRebirth = GameManager.instance != null ? GameManager.instance.playerRebirthCount : 0;

        // 데이터베이스의 아티팩트 목록을 기반으로 슬롯 생성
        foreach (var info in artifactDatabase.artifacts)
        {
            GameObject slotObj = Instantiate(slotPrefab, contentParent);
            ShopArtifactSlot slot = slotObj.GetComponent<ShopArtifactSlot>();

            if (slot != null)
            {
                slot.SetUp(info, currentSpecial, currentReputation, currentRebirth, RefreshShop, null);
                spawnedSlots.Add(slot);
            }
        }
    }

    public void OpenShop()
    {
        if (shopPanel.activeSelf)
        {
            Debug.Log("없음");
            return;
        }

        if (openShopBtn != null)
        {
            openShopBtn.interactable = false;
        }

        shopPanel.SetActive(true);
        RefreshShop();
    }

    public void CloseShop()
    {
        if (closeShopBtn != null)
        {
            closeShopBtn.interactable = false;
        }

        shopPanel.SetActive(false);

        if (closeShopBtn != null)
        {
            closeShopBtn.interactable = true;
        }

        if (openShopBtn != null)
        {
            openShopBtn.interactable = true; 
        }
    }
}
