using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public float volume = 0.3f;
    public List<SoundStruct> sounds;
    public float musicVolume = 0.1f;
    public AudioSource dayMusic;
    public AudioSource nightMusic;
    DayTime lastSeenDayTime = DayTime.Day;

    Dictionary<SoundType, AudioSource> audioSources = new();

    public static AudioManager I;

    void Awake() {
        if (I != null) Destroy(this);
        else I = this;
        foreach (SoundType soundType in Enum.GetValues(typeof(SoundType))) {
            if (!sounds.Any(s => s.soundType == soundType)) continue;
            AudioSource audioSource = new GameObject($"Audio Source {soundType}", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.transform.parent = transform;
            audioSource.volume = volume;
            audioSource.resource = sounds.Find(s => s.soundType == soundType).audio;
            audioSources.Add(soundType, audioSource);
        }
        dayMusic.volume = musicVolume;
        dayMusic.Play();
        nightMusic.volume = 0;
        nightMusic.Play();
    }

    void Update() {
        DayTime currentDayTime = GameManager.I.GetCurrentDayTime();
        if (lastSeenDayTime != currentDayTime) {
            if (currentDayTime == DayTime.Night) {
                dayMusic.DOFade(0, 2f);
                nightMusic.DOFade(musicVolume, 2f);
            } else if (currentDayTime == DayTime.Day) {
                dayMusic.DOFade(musicVolume, 2f);
                nightMusic.DOFade(0, 2f);
            }
            lastSeenDayTime = currentDayTime;
        }
    }

    public void PlaySound(SoundType soundType) {
        if (!audioSources.Keys.Contains(soundType)) return;
        audioSources[soundType].Play();
    }

    public void PlayUIClick() { PlaySound(SoundType.UIClick); }
}

public enum SoundType {
    UIHover,
    Explosion,
    GrenadeLauncher,
    Shotgun,
    Summon,
    Riffle,
    Submachinegun,
    ZombieBite,
    ZombieStep,
    UIClick
}

[Serializable]
public struct SoundStruct {
    public SoundType soundType;
    public AudioResource audio;
}
