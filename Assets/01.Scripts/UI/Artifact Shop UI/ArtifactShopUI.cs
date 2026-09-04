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

    private List<ArtifactSlot> spawnedSlots = new List<ArtifactSlot>();

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

        // 데이터베이스의 아티팩트 목록을 기반으로 슬롯 생성
        foreach (var info in artifactDatabase.artifacts)
        {
            GameObject slotObj = Instantiate(slotPrefab, contentParent);
            ArtifactSlot slot = slotObj.GetComponent<ArtifactSlot>();

            if (slot != null)
            {
                slot.SetUp(info);
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
