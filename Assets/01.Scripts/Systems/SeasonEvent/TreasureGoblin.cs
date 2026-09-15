using UnityEngine;
using UnityEngine.EventSystems;

public class TreasureGoblin : MonoBehaviour
{
    [Header("고블린 데이터")]
    [SerializeField] private TreasureGoblinData data;

    private TreasureGoblinEventPopup popup;

    private Vector2 dir;

    private Camera camera;

    public void Init(TreasureGoblinEventPopup popup)
    {
        this.popup = popup;
        camera = Camera.main;                

        if (dir == Vector2.zero)
            dir = Vector2.right;
    }

    private void OnEnable()
    {
        // 랜덤 방향
        dir = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        CheckScreen();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        transform.position += (Vector3)(dir * data.MoveSpeed * Time.fixedDeltaTime);
    }

    private void CheckScreen()
    {
        if (camera == null)
            return;

        Vector3 viewPosition = camera.WorldToViewportPoint(transform.position);

        if (viewPosition.x <= data.PaddingX)
        {
            dir.x = Mathf.Abs(dir.x);
        }
        else if (viewPosition.x >= 1f - data.PaddingX)
        {
            dir.x = -Mathf.Abs(dir.x);
        }

        if (viewPosition.y <= data.PaddingY)
        {
            dir.y = Mathf.Abs(dir.y);
        }
        else if (viewPosition.y >= 1f - data.PaddingY)
        {
            dir.y = -Mathf.Abs(dir.y);
        }

        dir.Normalize();
    }

    public void OnClick()
    {
        popup.OnClickGoblin();
    }
}
