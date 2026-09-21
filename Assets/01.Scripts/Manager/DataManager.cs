using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public ReincarnationData reincarnationData;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        LoadReincarnationData();
    }

    private void LoadReincarnationData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ReincarnationData");
        if (jsonFile != null)
        {
            reincarnationData = JsonUtility.FromJson<ReincarnationData>(jsonFile.text);
        }
        else
        {
            Debug.LogWarning("환생 데이터 JSON 파일을 찾을 수 없습니다. 기본값을 사용합니다.");
            reincarnationData = new ReincarnationData { reincarnationRequirement = 5000 };
        }
    }
}
