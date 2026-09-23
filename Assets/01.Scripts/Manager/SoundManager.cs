using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField, Range(0f, 1f)] private float bgmMasterScale = 0.5f;

    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private List<AudioSource> sfxSources = new List<AudioSource>();

    [Header("사운드 데이터")]
    [SerializeField] private SoundData soundData;

    private Dictionary<SFXType, SFXData> sfxDict = new Dictionary<SFXType, SFXData>();
    private Dictionary<BGMType, AudioClip> bgmDict = new Dictionary<BGMType, AudioClip>();

    private int sfxIndex = 0;

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

        PlayBGM(BGMType.Main);
    }

    private void InitSoundDictionary()
    {
        if (soundData == null || soundData.sfxList == null)
        {
            return;
        }

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
        if (sfxSources.Count == 0) return;

        if (sfxDict.TryGetValue(type, out SFXData data))
        {
            if (data.clip != null)
            {
                AudioSource currentSource = sfxSources[sfxIndex];

                sfxIndex++;
                if (sfxIndex >= sfxSources.Count)
                {
                    sfxIndex = 0;
                }

                currentSource.pitch = randomizePitch ? Random.Range(0.94f, 1.06f) : 1.0f;
                currentSource.PlayOneShot(data.clip, data.volume);
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
        if (bgmSource == null || clip == null)
        {
            return;
        }
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }

        float targetVolume = GetBGMVolume() * bgmMasterScale;

        // 이미 노래가 나오고 있다면 볼륨 줄인 뒤 교체
        if (bgmSource.isPlaying)
        {
            bgmSource.DOKill();
            bgmSource.DOFade(0f, 0.4f).SetUpdate(true).OnComplete(() =>
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop;
                bgmSource.Play();
                bgmSource.DOFade(targetVolume, 0.4f).SetUpdate(true);
            });
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = targetVolume;
            bgmSource.Play();
        }
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
        PlayerPrefs.SetFloat("SFXVolume", volume);
        foreach (var source in sfxSources)
        {
            if (source != null) source.volume = volume;
        }
    }

    public float GetBGMVolume() => PlayerPrefs.GetFloat("BGMVolume", 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat("SFXVolume", 1f);
}