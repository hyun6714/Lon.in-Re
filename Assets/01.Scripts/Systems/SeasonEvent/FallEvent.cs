using UnityEngine;

public class FallEvent : IEvent
{
    public void StartEvent()
    {
        Utils.Log("가을 이벤트 시작");

    }

    public void EndEvent()
    {
        Utils.Log("가을 이벤트 종료");

    }
}
