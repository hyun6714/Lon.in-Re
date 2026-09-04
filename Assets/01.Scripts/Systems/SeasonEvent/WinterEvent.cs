using UnityEngine;

public class WinterEvent : IEvent
{
    public void StartEvent()
    {
        Utils.Log("겨울 이벤트 시작");

    }

    public void EndEvent()
    {
        Utils.Log("겨울 이벤트 종료");

    }
}
