using UnityEngine;

public class Dim : UIBase
{
    public override UIName Name => UIName.Dim;

    private void Start()
    {
        if (UIManager.Instance == null)
            return;

        UIManager.Instance.DimRegister(this.gameObject);
        gameObject.SetActive(false);
    }
}
