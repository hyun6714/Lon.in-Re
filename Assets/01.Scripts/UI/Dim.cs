using UnityEngine;

public class Dim : UIBase
{
    public override UIName Name => UIName.Dim;

    private void Awake()
    {
        UIManager.Instance.DimRegister(this.gameObject);
        gameObject.SetActive(false);
    }
}
