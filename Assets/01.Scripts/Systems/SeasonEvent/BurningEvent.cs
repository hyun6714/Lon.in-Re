using System.Threading;
using UnityEngine;

public class BurningEvent : IEvent
{
    private CancellationTokenSource token;

    public void StartEvent()
    {
        Utils.Log("버닝 이벤트 실행 확인용");
    }

    public void EndEvent()
    {
        Utils.Log("버닝 이벤트 종료");
    }

    public void SaveEventData(EventSaveData data)
    {

    }
}
