using UnityEngine;

[CreateAssetMenu(fileName = "UIManagerData", menuName = "Manager/UIManagerData")]
public class UIManagerData : ScriptableObject
{
    [Header("이벤트 남은 시간 텍스트")]
    [SerializeField] private string nextEventText = "{0}일 {1}시간";

    public string NextEventText => nextEventText;
}
