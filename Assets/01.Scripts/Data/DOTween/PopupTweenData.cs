using UnityEngine;

[CreateAssetMenu(fileName = "PopupTweenData", menuName = "DOTween/PopupTweenData")]
public class PopupTweenData : ScriptableObject
{
    [SerializeField] private float popupSize = 1.2f;
    [SerializeField] private float openSize = 1f;
    [SerializeField] private float closeSize = 0.1f;
    [SerializeField] private float popupDelay = 0.2f;

    public float PopupSize => popupSize;
    public float OpenSize => openSize;
    public float CloseSize => closeSize;
    public float PopupDelay => popupDelay;
}
