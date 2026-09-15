using System.Collections.Generic;
using Unity.VectorGraphics.Editor;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField, Range(0f, 1f)] private float bgmMasterScale = 0.5f;

    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("사운드 데이터")]
    [SerializeField] private SoundData soundData;

    private Dictionary<SFXType, SFXData> sfxDict = new Dictionary<SFXType, SFXData>();
    private Dictionary<BGMType, AudioClip> bgmDict = new Dictionary<BGMType, AudioClip>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitSoundDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    private void InitSoundDictionary()
    {
        if (soundData == null || soundData.sfxList == null) return;

        // SFX 딕셔너리 캐싱
        sfxDict.Clear();
        if (soundData.sfxList != null)
        {
            foreach (var sfx in soundData.sfxList)
            {
                if (!sfxDict.ContainsKey(sfx.sfxType))
                {
                    sfxDict.Add(sfx.sfxType, sfx);
                }
            }
        }

        // BGM 딕셔너리 캐싱
        bgmDict.Clear();
        if (soundData.bgmList != null)
        {
            foreach (var bgm in soundData.bgmList)
            {
                if (!bgmDict.ContainsKey(bgm.bgmType))
                {
                    bgmDict.Add(bgm.bgmType, bgm.clip);
                }
            }
        }
    }

    public void PlaySFX(SFXType type, bool randomizePitch = false)
    {
        if (sfxSource == null) return;

        if (sfxDict.TryGetValue(type, out SFXData data))
        {
            if (data.clip != null)
            {
                sfxSource.pitch = randomizePitch ? Random.Range(0.94f, 1.06f) : 1.0f;
                sfxSource.PlayOneShot(data.clip, data.volume);
            }
        }
    }

    // Enum 타입으로 재생
    public void PlayBGM(BGMType type, bool loop = true)
    {
        if (bgmDict.TryGetValue(type, out AudioClip clip))
        {
            PlayBGM(clip, loop);
        }
    }

    // 실제 오디오 소스 재생
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource == null) return;
        bgmSource.volume = volume * bgmMasterScale;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null) return;
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public float GetBGMVolume() => PlayerPrefs.GetFloat("BGMVolume", 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat("SFXVolume", 1f);
}