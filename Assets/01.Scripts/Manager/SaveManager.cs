using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string savePath;

    [SerializeField] private float autoSaveInterval = 10f;

    private bool isSaveDeleted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    private void Start()
    {
        InvokeRepeating(nameof(SaveGame), autoSaveInterval, autoSaveInterval);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null &&
            Keyboard.current.f1Key.wasPressedThisFrame)
        {
            DeleteSaveData();
        }
#endif
    }


    private void OnApplicationQuit()
    {
        if (isSaveDeleted)
            return;

        SaveGame();
    }

    // 게임 저장
    public void SaveGame()
    {
        SaveData saveData = new SaveData();

        SaveCurrencyData(saveData);
        SaveGameData(saveData);
        SaveRankData(saveData);
        SaveEmployeeData(saveData);
        SaveGameDevData(saveData);
        SaveCalendarData(saveData);
        SaveReleasedGameData(saveData);
        SaveEventData(saveData);
        SaveArtifactData(saveData);

        // Json으로 변환
        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(savePath, json);

        Utils.Log($"게임 저장 완료 : {savePath}");
    }

    // 재화 저장
    private void SaveCurrencyData(SaveData saveData)
    {
        saveData.normalCurrency = CurrencyManager.instance.GetAmount(CurrencyType.Normal);
        saveData.specialCurrency = CurrencyManager.instance.GetAmount(CurrencyType.Special);
        saveData.reputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation);
    }


    // 게임 진행 데이터 저장
    private void SaveGameData(SaveData saveData)
    {
        saveData.playerRebirthCount = GameManager.Instance.playerRebirthCount;
        saveData.gameDevCount = GameManager.Instance.gameDevCount;
    }


    // 회사 등급 데이터 저장
    private void SaveRankData(SaveData saveData)
    {
        saveData.currentRank = RankManager.instance.currentRank;
        saveData.currentEmployeeCount = RankManager.instance.currentEmployeeCount;

    }

    // 직원 데이터 저장
    private void SaveEmployeeData(SaveData saveData)
    {
        foreach (EmployeeState state in EmployeeManager.instance.EmployeeStates)
        {
            EmployeeSaveData employeeSaveData = new EmployeeSaveData();

            employeeSaveData.employeeId = state.employeeData.EmployeeId;

            employeeSaveData.count = state.Count;

            saveData.employees.Add(employeeSaveData);
        }
    }

    // 게임 개발 데이터 저장
    private void SaveGameDevData(SaveData saveData)
    {
        saveData.nextGameId = GameDevManager.instance.NextGameId;
    }

    // 게임 내 날짜 저장
    private void SaveCalendarData(SaveData saveData)
    {
        saveData.gameDate = CalendarManager.instance.SaveDate();
    }

    // 출시된 게임 데이터 저장
    private void SaveReleasedGameData(SaveData saveData)
    {
        foreach (ReleasedGameSaveData releasedGame in GameReleaseManager.instance.releasedGames)
        {
            saveData.releasedGames.Add(releasedGame);
        }
    }

    // 이벤트 저장
    private void SaveEventData(SaveData saveData)
    {
        saveData.eventSaveDatas = EventManager.instance.GetEventSaveData();
    }

    // 아티펙트 저장
    private void SaveArtifactData(SaveData saveData)
    {
        saveData.artifactSaveData = ArtifactManager.instance.GetSaveData();
    }

    // 저장 데이터 삭제
    public void DeleteSaveData()
    {
        isSaveDeleted = true;

        CancelInvoke(nameof(SaveGame));

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Utils.Log("저장 데이터 삭제 완료");
        }
        else
        {
            Utils.Log("삭제할 데이터가 없습니다.");
        }
    }
}
