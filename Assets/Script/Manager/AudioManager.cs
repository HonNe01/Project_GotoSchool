using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header(" # BGM")]
    public AudioClip mainClip;
    public AudioClip battleClip;
    public AudioClip bossClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header(" # SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

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
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // ȿ���� �÷��̾� �ʱ�ȭ
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

    public void PlayBgm(string bgmName)        // ����� ��� �� ����
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

    public void EffectBgm(bool isPlay)      // ����� ����Ʈ ����
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

    public void PlaySfx(SFX sfx)            // ȿ���� ��� �� ����
    {
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

    public void ResetAudio() // ����� �ʱ�ȭ
    {
        StopBgm();
        channelIndex = 0;
        foreach (AudioSource sfxPlayer in sfxPlayers)
        {
            sfxPlayer.Stop();
        }
    }
}
