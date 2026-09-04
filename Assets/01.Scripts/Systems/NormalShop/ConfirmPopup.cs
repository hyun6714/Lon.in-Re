using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmPopup : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI titleText;    // 팝업 상단 제목
    [SerializeField] private TextMeshProUGUI contentText;  // 중앙 내용
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private Action onConfirmAction; // '예' 눌렀을 때 실행될 함수
    private Action onCancelAction;  // '아니오' 눌렀을 때 실행될 함수

    private void Awake()
    {
        // 버튼 리스너 연결
        if (yesBtn != null) yesBtn.onClick.AddListener(OnClickYes);
        if (noBtn != null) noBtn.onClick.AddListener(OnClickNo);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string title, string message, Action onConfirm, Action onCancel = null)
    {
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;

        if (titleText != null) titleText.text = title;
        if (contentText != null) contentText.text = message;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    private void OnClickYes()
    {
        onConfirmAction?.Invoke();
        Close();
    }

    private void OnClickNo()
    {
        onCancelAction?.Invoke();
        Close();
    }

    public void Close()
    {
        onConfirmAction = null;
        onCancelAction = null;
        gameObject.SetActive(false);
    }
}