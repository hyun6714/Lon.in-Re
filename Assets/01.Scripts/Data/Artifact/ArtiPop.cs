using UnityEngine;

public class ArtiPop : MonoBehaviour
{
    public GameObject ArtiPopup;

    public GameObject ArtiUnPop;

    public void OpenShopPop()
    {
        if(ArtiPopup == null) return;

        ArtiPopup.SetActive(true);
    }

    public void CloseShopPop()
    {
        if (ArtiPopup == null) return;

        ArtiPopup.SetActive(false);
    }

    public void OpenArtiUnPop()
    {
        if (ArtiUnPop == null) return;

        ArtiUnPop.SetActive(true);
    }

    public void CloseArtiUnPop()
    {
        if (ArtiUnPop == null) return;

        ArtiUnPop.SetActive(false);
    }
}
