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

        if (dir == Vector2.zero)
            dir = Vector2.right;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        camera = Camera.main;
        // 랜덤 방향
        dir = Random.insideUnitCircle.normalized;

        if(dir == Vector2.zero)
            dir = Vector2.right;

        ScaleCheck();
    }

    private void Update()
    {
        CheckScreen();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ScaleCheck()
    {
        Vector3 targetScale = transform.localScale;

        if (dir.x < 0)
        {
            targetScale.x = data.NonFlipX;
        }
        else
        {
            targetScale.x = data.FlipX;
        }

        transform.localScale = targetScale;
    }

    private void Move()
    {
        transform.position += (Vector3)(dir * data.MoveSpeed * Time.fixedDeltaTime);
    }

    private void CheckScreen()
    {
        if (camera == null)
            return;

        // 고블린의 월드 좌표를 뷰포트 좌표로 표시(0~1 사이)
        Vector3 viewPosition = camera.WorldToViewportPoint(transform.position);

        // 고블린의 좌표가 Padding 지점을 벗어났을 경우 강제로 방향 변경
        if (viewPosition.x <= data.PaddingX)
        {
            dir.x = Mathf.Abs(dir.x);

            Vector3 targetScale = transform.localScale;
            targetScale.x = data.FlipX;
            transform.localScale = targetScale;
        }
        else if (viewPosition.x >= 1f - data.PaddingX)
        {
            dir.x = -Mathf.Abs(dir.x);

            Vector3 targetScale = transform.localScale;
            targetScale.x = data.NonFlipX;
            transform.localScale = targetScale;
        }

        if (viewPosition.y <= data.PaddingY)
        {
            dir.y = Mathf.Abs(dir.y);
        }
        else if (viewPosition.y >= 1f - data.PaddingY)
        {
            dir.y = -Mathf.Abs(dir.y);
        }

        // 속도 유지
        dir.Normalize();
    }

    public void OnClick()
    {
        popup.OnClickGoblin();
    }
}
