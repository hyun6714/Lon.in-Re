using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundPopup : NormalPopupBase
{
    public override UIName Name => UIName.Popup_Sound;

    [Header("버튼")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveBtn;

    [Header("음량 조절")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("음량 수치 텍스트")]
    [SerializeField] private TMP_Text bgmNumText;
    [SerializeField] private TMP_Text sfxNumText;

    [Header("메인 씬 Dim")]
    [SerializeField] private GameObject mainSceneDim;

    private void Awake()
    {
        if (exitBtn != null)
        {
            exitBtn.onClick.AddListener(CloseSoundPopup);
        }

        if (saveBtn != null)
        {
            saveBtn.onClick.AddListener(SaveVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(UpdateBGMText);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(UpdateSFXText);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (SoundManager.instance == null)
        {
            return;
        }

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(
                SoundManager.instance.GetBGMVolume()
            );

            UpdateBGMText(bgmSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(
                SoundManager.instance.GetSFXVolume()
            );

            UpdateSFXText(sfxSlider.value);
        }
    }

    private void UpdateBGMText(float value)
    {
        if (bgmNumText != null)
        {
            bgmNumText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }

    private void UpdateSFXText(float value)
    {
        if (sfxNumText != null)
        {
            sfxNumText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }

    private void CloseSoundPopup()
    {
        if (mainSceneDim != null)
        {
            mainSceneDim.SetActive(false);
        }

        ClosePopup();
    }

    private void SaveVolume()
    {
        if (SoundManager.instance == null)
        {
            return;
        }

        if (bgmSlider != null)
        {
            SoundManager.instance.SetBGMVolume(bgmSlider.value);
        }

        if (sfxSlider != null)
        {
            SoundManager.instance.SetSFXVolume(sfxSlider.value);
        }

        PlayerPrefs.Save();

        if (mainSceneDim != null)
        {
            mainSceneDim.SetActive(false);
        }

        ClosePopup();
    }
}