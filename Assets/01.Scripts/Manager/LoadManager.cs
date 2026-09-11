using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;

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

    // 게임 불러오기
    public void LoadGame()
    {
        // 저장 파일이 없으면 불러오지 않음
        if (!File.Exists(savePath))
        {
            Utils.Log("저장된 게임 데이터가 없습니다.");
            return;
        }

        // JSON 파일 읽기
        string json = File.ReadAllText(savePath);

        // JSON -> SaveData 변환
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        // 각 시스템 데이터 불러오기
        LoadCurrencyData(saveData);
        LoadGameData(saveData);
        LoadRankData(saveData);
        LoadEmployeeData(saveData);
        LoadGameDevData(saveData);
        LoadCalendarData(saveData);
        LoadReleasedGameData(saveData);

        Utils.Log("게임 불러오기 완료");
    }

    // 재화 데이터 불러오기
    private void LoadCurrencyData(SaveData saveData)
    {
        CurrencyManager.instance.SetCurrency(CurrencyType.Normal, saveData.normalCurrency);

        CurrencyManager.instance.SetCurrency(CurrencyType.Special, saveData.specialCurrency);

        CurrencyManager.instance.SetCurrency(CurrencyType.Reputation, saveData.reputation);
    }

    // 게임 진행 데이터 불러오기
    private void LoadGameData(SaveData saveData)
    {
        GameManager.Instance.playerRebirthCount = saveData.playerRebirthCount;

        GameManager.Instance.gameDevCount = saveData.gameDevCount;
    }

    // 회사 등급 데이터 불러오기
    private void LoadRankData(SaveData saveData)
    {
        RankManager.instance.currentRank = saveData.currentRank;

        RankManager.instance.currentEmployeeCount = saveData.currentEmployeeCount;
    }
 
    // 직원 데이터 불러오기
    private void LoadEmployeeData(SaveData saveData)
    {
        foreach (EmployeeSaveData employeeSaveData in saveData.employees)
        {
            EmployeeState state = EmployeeManager.instance.GetEmployeeState(employeeSaveData.employeeId);

            state.SetCount(employeeSaveData.count);
        }
    }

    // 게임 개발 데이터 불러오기
    private void LoadGameDevData(SaveData saveData)
    {
        GameDevManager.instance.LoadNextGameId(saveData.nextGameId);
    }

    // 게임 내 날짜 불러오기
    private void LoadCalendarData(SaveData saveData)
    {
        CalendarManager.instance.LoadDate(saveData.gameDate);
    }

    // 출시된 게임 데이터 불러오기
    private void LoadReleasedGameData(SaveData saveData)
    {
        GameReleaseManager.instance.releasedGames.Clear();

        foreach (ReleasedGameSaveData releasedGame in saveData.releasedGames)
        {
            GameReleaseManager.instance.releasedGames.Add(releasedGame);

            EventManager.instance.LoadGameSettlement(releasedGame);
        }
    }
}