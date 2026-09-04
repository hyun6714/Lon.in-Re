using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StrategyTest : MonoBehaviour
{
    float timer;

    List<Season> seasons = new List<Season>()
    {
        Season.Spring,
        Season.Summer,
        Season.Fall,
        Season.Winter
    };
    CancellationTokenSource token;
    int seasonIndex = 0;
    Season currentSeason = Season.None;

    private void Start()
    {
        timer = 0f;
        Strategy(token.Token).Forget();
    }

    private void OnEnable()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        token?.Cancel();
        token?.Dispose();
        token = null;
    }

    private async UniTaskVoid Strategy(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                timer += Time.deltaTime;
                if (timer >= 5f)
                {
                    if (currentSeason != Season.None)
                    {
                        EventManager.instance.EndSeasonEvent();

                    }
                    
                    currentSeason = seasons[seasonIndex];

                    EventManager.instance.ChangeSeason(currentSeason);

                    EventManager.instance.StartSeasonEvent();

                    seasonIndex = (seasonIndex + 1) % seasons.Count;

                    timer -= 5f;
                }

                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, token);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
}
