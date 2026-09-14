using UnityEngine;

[CreateAssetMenu(fileName = "AutoProductionUpgradeData", menuName = "Game/AutoProductuonUpgrade")]
public class AutoProductionUpgradeData : ScriptableObject
{
    [SerializeField] private float defaultAutoUpgradeTotalPerSec = 0f;
    [SerializeField] private float baseAutoMultiplier = 1f;

    public float DefaultAutoUpgradeTotalPerSec => defaultAutoUpgradeTotalPerSec;
    public float BaseAutoMultiplier => baseAutoMultiplier;
}
