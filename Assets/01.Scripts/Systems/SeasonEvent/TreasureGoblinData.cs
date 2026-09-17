using UnityEngine;

[CreateAssetMenu(fileName = "TreasureGoblinData", menuName = "Event/TreasureGoblinData")]
public class TreasureGoblinData : ScriptableObject
{
    [Header("스피드")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("최대 이동 공간 제한")]
    [SerializeField] private float paddingX = 0.02f;
    [SerializeField] private float paddingY = 0.3f;

    [Header("방향")]
    [SerializeField] private float nonFlipX = 1f;
    [SerializeField] private float flipX = -1f;

    public float MoveSpeed => moveSpeed;

    public float PaddingX => paddingX;
    public float PaddingY => paddingY;

    public float NonFlipX => nonFlipX;
    public float FlipX => flipX;
}
