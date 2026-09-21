using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AutoProductionUpgrade : MonoBehaviour
{
    [Header("업그레이드 데이터")]
    [SerializeField] private AutoProductionUpgradeData data;

    [Header("총 생산량 합산")]
    [SerializeField] private float totalPerSec;
    public float TotalPerSec => totalPerSec;

    private float defaultAutoUpgradeTotalPerSec;
    private float defaultAutoMultiplier;

    public float DefaultMultiplier => defaultAutoMultiplier;

    public event Action OnAutoUpgradeChanged;

    private Dictionary<GameEventType, float> eventMulti = new Dictionary<GameEventType, float>();

    private void Awake()
    {
        defaultAutoUpgradeTotalPerSec = data.DefaultAutoUpgradeTotalPerSec;
        defaultAutoMultiplier = data.BaseAutoMultiplier;
    }

    private void OnEnable()
    {
        SubscribeEvent();
    }

    private void OnDisable()
    {
        UnSubscribeEvent();
    }

    private void SubscribeEvent()
    {
        GameEventBridge.OnAutoMultiplierChanged += SetMultiplier;
        GameEventBridge.OnEmployeeChaned += TotalPerSecond;
        GameEventBridge.OnArtifactUnlocked += TotalPerSecond;
    }

    private void UnSubscribeEvent()
    {
        GameEventBridge.OnAutoMultiplierChanged -= SetMultiplier;
        GameEventBridge.OnEmployeeChaned -= TotalPerSecond;
        GameEventBridge.OnArtifactUnlocked -= TotalPerSecond;
    }

    public void TotalPerSecond()
    {
        float total = defaultAutoUpgradeTotalPerSec;

        // 직원 고용 계산
        if (EmployeeManager.instance != null)
        {
            total += EmployeeManager.instance.GetTotalProductionPerSecond();
        }

        Utils.Log($"초당 생산랑 업그레이드 1 : {total}");

        float totalMulti = defaultAutoMultiplier;

        // 아티펙트 계산(임시)
        if (ArtifactManager.instance != null)
        {
            totalMulti += ArtifactManager.instance.GetTotalPerSecond() -1f ; 
        }

        Utils.Log($"초당 생산랑 업그레이드 2 : {total}");

        // 변경된 배율 계산
        foreach (var value in eventMulti)
        {
            totalMulti += (value.Value - 1f);
        }

        Utils.Log($"초당 생산량 배율 업그레이드 : {totalMulti}");

        totalMulti = Mathf.Max(0.1f, totalMulti);
        total *= totalMulti;

        totalPerSec = Mathf.RoundToInt(total);

        Utils.Log($"초당 생산량 합산 완료 : {totalPerSec}G/초");

        OnAutoUpgradeChanged?.Invoke();
    }

    public void SetMultiplier(GameEventType type, float multi)
    {
        if (type == GameEventType.None)
            return;

        if (multi == data.BaseAutoMultiplier)
        {
            eventMulti.Remove(type);
            Utils.Log("자동 생산 배율 딕셔너리 제거 완료");
        }
        else
        {
            eventMulti[type] = multi;
            Utils.Log("자동 생산 배율 딕셔너리 추가 완료");
        }

        TotalPerSecond();
    }
}
