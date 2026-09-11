using UnityEngine;

public class ReincarnationUI : MonoBehaviour
{
    public GameObject ReincarnationBtn;
    public GameObject ReincarnationPop;

    public void OpenReincarnatioBtn()
    {
        if (ReincarnationBtn == null) return;
        ReincarnationBtn.SetActive(true);
    }

    public void CloseReincarnatioBtn()
    {
        if (ReincarnationBtn == null) return;
        ReincarnationBtn.SetActive(false);
    }

    public void OpenReincarnationPop()
    {
        if (ReincarnationPop == null) return;
        ReincarnationPop.SetActive(true);
    }

    public void CloseReincarnationPop()
    {
        if (ReincarnationPop == null) return;
        ReincarnationPop.SetActive(false);
    }
}
