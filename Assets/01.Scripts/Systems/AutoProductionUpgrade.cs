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

    private CancellationTokenSource token;

    public event Action OnMultiplierUpgradeChanged;

    private void OnEnable()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();

        SubscribeEvent(token.Token).Forget();
    }

    private void OnDisable()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;

        UnSubscribeEvent();
    }

    private async UniTaskVoid SubscribeEvent(CancellationToken token)
    {
        try
        {
            await UniTask.WaitUntil(() => EventManager.instance != null, cancellationToken: token);

            SubscribeEvent();
        }
        catch (OperationCanceledException)
        {

        }
    }

    private void SubscribeEvent()
    {
        EventManager.instance.OnMultiplierChanged += SetMultiplier;
    }

    private void UnSubscribeEvent()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.OnMultiplierChanged -= SetMultiplier;
        }
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
