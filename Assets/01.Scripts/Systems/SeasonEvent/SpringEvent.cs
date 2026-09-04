using UnityEngine;

public class SpringEvent : IEvent
{
    public void StartEvent()
    {
        // 이벤트 시작 로직
        Utils.Log("봄 이벤트 시작");
    }

    public void EndEvent()
    {
        // 이벤트 종료 로직
        Utils.Log("봄 이벤트 종료");
    }
}
