using UnityEngine;
using UnityEngine.UI;

public class FixedScrollbar : MonoBehaviour
{
    [SerializeField] private float fixedHandleSize = 0.05f;
    private Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void LateUpdate()
    {
        if (scrollbar != null)
        {
            scrollbar.size = fixedHandleSize;
        }
    }
}