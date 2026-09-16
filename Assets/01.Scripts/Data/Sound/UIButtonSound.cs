using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SFXType soundType = SFXType.ButtonClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        var selectable = GetComponent<Selectable>();
        if (selectable != null && !selectable.interactable)
        {
            return;
        }

        SoundManager.instance?.PlaySFX(soundType);
    }
}