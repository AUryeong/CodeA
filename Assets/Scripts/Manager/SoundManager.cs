using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    BGM,
    SFX
}

public class SoundManager : Singleton<SoundManager>
{
    private class AudioInfo
    {
        public AudioSource audioSource;
        public float audioVolume;
    }

    private readonly string path = "Sounds/";
    private readonly Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private readonly Dictionary<ESoundType, AudioInfo> audioInfos =
        new Dictionary<ESoundType, AudioInfo>();

    protected override bool IsDontDestroying => true;

    protected override void OnCreated()
    {
        var clips = Resources.LoadAll<AudioClip>(path);
        foreach (var clip in clips)
            audioClips.Add(clip.name, clip);

        var audioInfo = AddAudioInfo(ESoundType.BGM);
        audioInfo.audioVolume = SaveManager.Instance.GameData.bgmSound;
        audioInfo.audioSource.loop = true;

        AddAudioInfo(ESoundType.SFX).audioVolume = SaveManager.Instance.GameData.sfxSound;
    }

    public void UpdateVolume(ESoundType soundType, float sound)
    {
        audioInfos[soundType].audioVolume = sound;
        audioInfos[soundType].audioSource.volume = sound;
    }

    private AudioInfo AddAudioInfo(ESoundType soundType)
    {
        var audioSourceObj = new GameObject(nameof(soundType));
        audioSourceObj.transform.SetParent(transform);

        var audioInfo = new AudioInfo
        {
            audioSource = audioSourceObj.AddComponent<AudioSource>(),
        };
        audioInfos.Add(soundType, audioInfo);
        return audioInfo;
    }

    public AudioClip PlaySound(string soundName, ESoundType soundType = ESoundType.BGM, float multipleVolume = 1,
        float pitch = 1)
    {
        if (!audioClips.ContainsKey(soundName))
        {
            Debug.Log("그 소리 없음!");
            return null;
        }

        var clip = audioClips[soundName];
        var audioInfo = audioInfos[soundType];
        var audioSource = audioInfo.audioSource;

        audioSource.pitch = pitch;

        if (soundType.Equals(ESoundType.BGM))
        {
            audioSource.clip = clip;
            audioSource.volume = audioInfo.audioVolume * multipleVolume;
            audioSource.Play();
        }
        else //SFX
            audioSource.PlayOneShot(clip, audioInfo.audioVolume * multipleVolume);

        return clip;
    }
}