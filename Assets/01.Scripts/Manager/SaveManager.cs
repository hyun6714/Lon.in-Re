using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string savePath;

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
        saveData.hasEmployees = RankManager.instance.hasEmployees;
        saveData.maxEmployee = RankManager.instance.maxEmployee;
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
        GameDate currentDate = CalendarManager.instance.CurrentDate;

        saveData.year = currentDate.year;
        saveData.month = currentDate.month;
        saveData.day = currentDate.day;
        saveData.hour = currentDate.hour;
        saveData.minute = currentDate.minutes;
    }

    // 출시된 게임 데이터 저장
    private void SaveReleasedGameData(SaveData saveData)
    {
        foreach (ReleasedGameSaveData releasedGame in GameReleaseManager.instance.releasedGames)
        {
            saveData.releasedGames.Add(releasedGame);
        }
    }
}
