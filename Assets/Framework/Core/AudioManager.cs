using System.Collections;
using UnityEngine;

/// <summary>
/// 音频管理器
/// </summary>
public class AudioManager : MonoBehaviour {

    private static bool GetLocalMusicMute () {
        return PlayerPrefs.GetInt("AudioManager_musicMute", 0) > 0;
    }

    private static void SetLocalMusicMute (bool value) {
        PlayerPrefs.SetInt("AudioManager_musicMute", value ? 1 : 0);
    }

    private static bool GetLocalEffectsMute () {
        return PlayerPrefs.GetInt("AudioManager_EffectsMute", 0) > 0;
    }

    private static void SetLocalEffectsMute (bool value) {
        PlayerPrefs.SetInt("AudioManager_EffectsMute", value ? 1 : 0);
    }

    public bool musicMute { get; private set; }

    public bool effectsMute { get; private set; }


    /// <summary>
    /// 设置循环播放的音乐的静音状态
    /// </summary>
    /// <param name="value"></param>
    public void SetMusicMute (bool value) {
        musicMute = value;
        SetLocalMusicMute(value);

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        for (int i = 0, len = audioSources.Length; i < len; i++) {
            AudioSource audioSource = audioSources[i];
            if (!audioSource.loop) continue;
            audioSource.mute = value;
        }
    }

    /// <summary>
    /// 设置音效的静音状态
    /// </summary>
    /// <param name="value"></param>
    public void SetEffectsMute (bool value) {
        effectsMute = value;
        SetLocalEffectsMute(value);

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        for (int i = 0, len = audioSources.Length; i < len; i++) {
            AudioSource audioSource = audioSources[i];
            if (audioSource.loop) continue;
            audioSource.mute = value;
        }
    }

    /// <summary>
    /// 循环播放一个音乐
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="positionTransform"> 用于绑定位置的 Transform </param>
    /// <param name="volume"> 音量 </param>
    /// <returns></returns>
    public AudioSource PlayMusic (AudioClip clip, Transform positionTransform, float volume) {
        GameObject gameObj = new GameObject("Play music (AudioManager)");
        if (positionTransform) {
            gameObj.transform.parent = positionTransform;
        }

        AudioSource audioSource = gameObj.AddComponent<AudioSource>();
        audioSource.mute = musicMute;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.Play();
        return audioSource;
    }

    /// <summary>
    /// 循环播放一个音乐
    /// </summary>
    /// <param name="clip"> 音频剪辑 </param>
    /// <param name="positionTransform"> 用于绑定位置的 Transform </param>
    /// <returns></returns>
    public AudioSource PlayMusic (AudioClip clip, Transform positionTransform) {
        return PlayMusic(clip, positionTransform, 1f);
    }

    /// <summary>
    /// 循环播放一个音乐
    /// </summary>
    /// <param name="clip"> 音频剪辑 </param>
    /// <param name="position"> 播放音乐的位置 </param>
    /// <param name="volume"> 音量 </param>
    /// <returns></returns>
    public AudioSource PlayMusic (AudioClip clip, Vector3 position, float volume) {
        GameObject gameObj = new GameObject("Play music at point (AudioManager)");
        gameObj.transform.position = position;

        AudioSource audioSource = gameObj.AddComponent<AudioSource>();
        audioSource.mute = musicMute;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.Play();
        return audioSource;
    }

    /// <summary>
    /// 循环播放一个音乐
    /// </summary>
    /// <param name="clip"> 音频剪辑 </param>
    /// <param name="position"> 播放音乐的位置 </param>
    public AudioSource PlayMusic (AudioClip clip, Vector3 position) {
        return PlayMusic(clip, position, 1f);
    }

    /// <summary> 一次性播放音效 </summary>
    public AudioSource PlayEffect (AudioClip clip, Transform positionTransform, float volume) {
        GameObject gameObj = new GameObject("Play effect (AudioManager)");
        if (positionTransform) {
            gameObj.transform.parent = positionTransform;
        }

        AudioSource audioSource = gameObj.AddComponent<AudioSource>();
        audioSource.mute = effectsMute;
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.Play();

        StartCoroutine(DestroyAudioSourceOnComplete(audioSource));

        return audioSource;
    }

    /// <summary> 一次性播放音效 </summary>
    public AudioSource PlayEffect (AudioClip clip, Transform positionTransform) {
        return PlayEffect(clip, positionTransform, 1f);
    }

    /// <summary> 一次性播放音效 </summary>
    public AudioSource PlayEffect (AudioClip clip, Vector3 position, float volume) {
        GameObject gameObj = new GameObject("Play effect at point (AudioManager)");
        gameObj.transform.position = position;

        AudioSource audioSource = gameObj.AddComponent<AudioSource>();
        audioSource.mute = effectsMute;
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.Play();

        StartCoroutine(DestroyAudioSourceOnComplete(audioSource));

        return audioSource;
    }

    /// <summary> 一次性播放音效 </summary>
    public AudioSource PlayEffect (AudioClip clip, Vector3 position) {
        return PlayEffect(clip, position, 1f);
    }

    private IEnumerator DestroyAudioSourceOnComplete (AudioSource audioSource) {
        while (audioSource.time < audioSource.clip.length) {
            yield return null;
        }
        Destroy(audioSource.gameObject);
    }

    private void Awake () {
        SetMusicMute(GetLocalMusicMute());
        SetEffectsMute(GetLocalEffectsMute());
    }

}
