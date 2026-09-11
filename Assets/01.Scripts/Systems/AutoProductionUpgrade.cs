using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class AutoProductionUpgrade : MonoBehaviour
{
    [Header("총 생산량 합산")]
    [SerializeField] private float totalPerSec = 0;

    private float autoMultiplier = 1f;
    public float AutoMultiplier => autoMultiplier;

    public event Action OnMultiplierUpgradeChanged;

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
    }

    private void UnSubscribeEvent()
    {
        GameEventBridge.OnAutoMultiplierChanged -= SetMultiplier;
    }

    public float TotalPerSecond()
    {
        if (ArtifactManager.instance != null)
        {
            totalPerSec = ArtifactManager.instance.GetTotalPerSecond();
        }

        return totalPerSec;
    }

    public void SetMultiplier(float multi)
    {
        autoMultiplier = multi;
        OnMultiplierUpgradeChanged?.Invoke();
    }
}
