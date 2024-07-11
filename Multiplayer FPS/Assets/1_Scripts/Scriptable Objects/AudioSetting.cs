using UnityEngine;
using UnityEngine.Audio;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Settings/AudioSettings")]
public class AudioSetting : ScriptableObject
{
    [Header("Defaults")]
    [BoxGroup("Defaults")][Range(-80f, 0f)] public int defaultMasterVol = 0;
    [BoxGroup("Defaults")][Range(-80f, 0f)] public int defaultMusicVol = 0;
    [BoxGroup("Defaults")][Range(-80f, 0f)] public int defaultSFXVol = 0;
    [BoxGroup("Defaults")][Range(-80f, 0f)] public int defaultVoiceVol = 0;
    [BoxGroup("Defaults")][Range(-80f, 0f)] public int defaultEnemyVol = 0;

    [Header("Mixer Groups")]
    [BoxGroup("Mixer Groups")] public AudioMixer audioMixer;
    [BoxGroup("Mixer Groups")] public AudioMixerGroup masterMixer;
    [BoxGroup("Mixer Groups")] public AudioMixerGroup musicMixer;
    [BoxGroup("Mixer Groups")] public AudioMixerGroup sfxMixer;
    [BoxGroup("Mixer Groups")] public AudioMixerGroup voiceMixer;

    [Header("Music")]
    [BoxGroup("Music")] public AudioClip[] musicTracks;

    [Header("UI")]
    [BoxGroup("UI")] public AudioClip uiClick;
    [BoxGroup("UI")] public AudioClip uiHover;

    [Header("Weapons")]
    [BoxGroup("Weapons")] public AudioClip wandShoot;
}
