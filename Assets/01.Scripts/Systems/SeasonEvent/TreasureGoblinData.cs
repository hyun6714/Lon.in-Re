using UnityEngine;

[CreateAssetMenu(fileName = "TreasureGoblinData", menuName = "Event/TreasureGoblinData")]
public class TreasureGoblinData : ScriptableObject
{
    [Header("스피드")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("카메라 여백")]
    [SerializeField] private float padding = 0.3f;

    public float MoveSpeed => moveSpeed;

    public float Padding => padding;
}
