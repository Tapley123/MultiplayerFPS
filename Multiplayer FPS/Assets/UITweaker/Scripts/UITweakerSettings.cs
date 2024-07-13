using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "UITweakerSettings", menuName = "UITweakerSettings")]
public class UITweakerSettings : ScriptableObject
{
    [Header("Managers")]
    [BoxGroup("Manager")]public GameObject managerGo;

    [Header("Defaults")]
    //Colours
    [BoxGroup("Color")][Range(1, 500)] public float defaultScalePercentageIncrease = 20;
    [BoxGroup("Color")]public Color defaultBackgroundHighlightColor;
    [BoxGroup("Color")]public Color defaultBackgroundPressedColor;
    [BoxGroup("Color")]public Color defaultTextHighlightColor;
    [BoxGroup("Color")]public Color defaultTextPressedColor;
    //Audio
    [BoxGroup("Audio")] public AudioClip defaultClickSound;
    [BoxGroup("Audio")] public AudioClip defaultHoverSound;
    [BoxGroup("Audio")] public AudioMixerGroup defaultSFXMixerGroup;
    //vibration
    [BoxGroup("Vibration")] public bool defaultUseVibration = false;
    [BoxGroup("Vibration")][ReadOnly] public float vibrationFrequency = 0.3f;
    [BoxGroup("Vibration")][Range(0.004f, 1f)] public float defaultVibrationAmplitude = 0.01f;
    [BoxGroup("Vibration")][Range(0.1f, 1f)] public float defaultVibrationDuration = 0.1f;
}
