using UnityEngine;
using UnityEngine.UI;

public class TestTimeScale : MonoBehaviour
{
    [SerializeField] private Button scaleBtn;
    [SerializeField] private Button restoreBtn;

    private void Start()
    {
        scaleBtn.onClick.AddListener(TimeScaleUp);
        restoreBtn.onClick.AddListener(TimeScaleRestore);
    }

    private void TimeScaleUp()
    {
        Time.timeScale = 30;
    }

    private void TimeScaleRestore()
    {
        Time.timeScale = 1;
    }
}
