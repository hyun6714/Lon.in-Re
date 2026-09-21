using UnityEngine;

public interface IGameDataGet<T>
{
    T GetData();
}

public static class GameDataGetter<T>
{
    private static IGameDataGet<T> getData;

    public static void Register(IGameDataGet<T> data)
    {
        if (data == null)
        {
            Utils.Log($"{typeof(T).Name} 데이터가 null 입니다.");
            return;
        }

        getData = data;
    }

    public static void UnRegister(IGameDataGet<T> data)
    {
        if (getData == data)
        {
            getData = null;
        }
    }

    public static T GetData()
    {
        if (getData == null)
        {
            Utils.Log($"{typeof(T).Name} 데이터가 등록되지 않았습니다.");
            return default;
        }

        return getData.GetData();
    }
}
