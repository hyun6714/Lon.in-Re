using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GameStart(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void GameExit()
    {
        Application.Quit();
    }
}
