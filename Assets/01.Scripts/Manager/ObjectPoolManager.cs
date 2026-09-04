using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void ReturnToPool();
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    [Header("미리 풀링할 오브젝트")]
    [SerializeField] private List<GameObject> objList = new List<GameObject>();

    [Header("풀 사이즈")]
    [SerializeField] private int poolSize = 20;

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, Transform> parentPools = new Dictionary<GameObject, Transform>();

    // 오브젝트의 원본 프리팹 기억하기 위한 딕셔너리
    private Dictionary<GameObject, GameObject> objectKey = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        InitPool();
    }

    public void InitPool()
    {
        foreach (GameObject obj in objList)
        {
            if (obj == null)
                continue;

            CreatePool(obj, poolSize);
        }
    }

    public void CreatePool(GameObject obj, int size)
    {
        if (pools.ContainsKey(obj))
            return;

        pools[obj] = new Queue<GameObject>();

        GameObject parentPool = new GameObject($"{obj.name}_Pool");
        parentPool.transform.SetParent(this.transform);
        parentPools[obj] = parentPool.transform;

        for (int i = 0; i < size; i++)
        {
            GameObject go = Instantiate(obj);
            go.SetActive(false);

            go.transform.SetParent(parentPool.transform, false);

            pools[obj].Enqueue(go);

            // 새로 생성한 오브젝트의 원본을 기억해두기
            objectKey[go] = obj;
        }
    }

    public T GetObject<T>(GameObject key, Transform parent = null) where T : Component
    {
        if (!pools.ContainsKey(key))
            return null;

        GameObject go;
        if (pools[key].Count > 0)
        {
            go = pools[key].Dequeue();
        }
        else
        {
            go = Instantiate(key);
            objectKey[go] = key;
        }

        Transform targetTransform = parent != null ? parent : parentPools[key];
        go.transform.SetParent(targetTransform, false);

        go.SetActive(true);
        return go.GetComponent<T>();
    }

    public void ReturnObject(GameObject originPrefab, GameObject instanceObj)
    {
        // key값을 안넣거나 잘못 넣어도 딕셔너리 재 확인
        // 그래도 없으면 파괴
        if (originPrefab == null || !pools.ContainsKey(originPrefab))
        {
            if (objectKey.TryGetValue(instanceObj, out GameObject poolKey))
            {
                originPrefab = poolKey;
            }
            else
            {
                Destroy(instanceObj);
                return;
            }                
        }

        instanceObj.SetActive(false);
        instanceObj.transform.SetParent(parentPools[originPrefab], false);
        pools[originPrefab].Enqueue(instanceObj);        
    }
}
