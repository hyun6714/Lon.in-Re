using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("사운드 옵션")]
    [SerializeField] private SoundPopup soundPopup;
    [SerializeField] private GameObject soundDim;

    public void GameStart(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSoundOption()
    {
        if (soundPopup == null)
        {
            return;
        }

        if (soundDim != null)
        {
            soundDim.SetActive(true);
        }

        soundPopup.gameObject.SetActive(true);
        soundPopup.OpenPopup();
    }

    public void GameExit()
    {
        Application.Quit();
    }
}