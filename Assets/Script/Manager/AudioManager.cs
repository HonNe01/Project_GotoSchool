using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header(" # BGM")]
    public AudioClip mainClip;
    public AudioClip battleClip;
    public AudioClip bossClip;
    public float bgmVolume;
    [Range(0, 1)] public float bgmSlider = 1f;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header(" # SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    [Range(0, 1)] public float sfxSlider = 1f;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    private float sfxBufferTime = 0.1f;
    private Dictionary<SFX, float> lastSFXtime = new Dictionary<SFX, float>();


    public enum SFX { Dead, Hit, LevelUp = 3, Melee, Range = 6, Boom, Damaged = 9, Tick = 11, Bus, Win, Lose, Select }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            Init();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++) {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(string bgmName)        // 배경음 재생 및 정지
    {
        switch (bgmName)
        {
            case "Start":
            case "Map":
                bgmPlayer.clip = mainClip;

                break;
            case "Stage":
                bgmPlayer.clip = battleClip;
            
                break;
            case "Boss":
                bgmPlayer.clip = bossClip;

                break;
        }
        if (bgmPlayer.clip != null) {
            bgmPlayer.Play();
        }
    }

    public void StopBgm()
    {
        bgmPlayer.Stop();
    }

    public void EffectBgm(bool isPlay)      // 배경음 이펙트 적용
    {
        if (bgmEffect == null)
        {
            bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();
        }

        if (bgmEffect != null)
        {
            bgmEffect.enabled = isPlay;
        }
    }

    public void UpdateBgmVolume()
    {
        if (bgmPlayer != null)
            bgmPlayer.volume = bgmVolume * bgmSlider;
    }

    public void UpdateSfxVolume()
    {
        if (sfxPlayers != null)
            foreach (var sfxPlayer in sfxPlayers)
                sfxPlayer.volume = sfxVolume * sfxSlider;
    }

    public void PlaySfx(SFX sfx)            // 효과음 재생 및 정지
    {
        // 사운드 체크
        float now = Time.time;
        if (lastSFXtime.TryGetValue(sfx, out float lastTime))
        {
            if (now - lastTime < sfxBufferTime) return;
        }
        lastSFXtime[sfx] = now;

        // 사운드 재생
        for (int index = 0; index < sfxPlayers.Length; index++) {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            if (sfx == SFX.Hit || sfx == SFX.Melee || sfx == SFX.Boom || sfx == SFX.Damaged) {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void ResetAudio() // 오디오 초기화
    {
        StopBgm();
        channelIndex = 0;
        foreach (AudioSource sfxPlayer in sfxPlayers)
        {
            sfxPlayer.Stop();
        }
    }
}
