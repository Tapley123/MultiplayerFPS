using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Audio;
using TMPro;

[RequireComponent(typeof(CustomUITriggerEvents))]
//[RequireComponent(typeof(AudioSource))]
public class UITweaker : MonoBehaviour
{
    #region Singleton
    private static UITweaker _instance;
    public static UITweaker Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<UITweaker>(); } return _instance; } }
    #endregion

    public enum Platform { PC, VR, Console };
    private bool showVRSettings = false;
    private bool showPCSettings = false;
    private bool showConsoleSettings = false;
    public enum UIType { None, Button, Toggle, Slider, InputField, Dropdown, Scrollbar };

    #region Variables
    [Expandable] public UITweakerSettings settings;
    public Platform platform;
    [ReadOnly] public UIType uiType;
    public bool gotOnValidateValues = false;

    [BoxGroup("OnStartValues")]
    [ReadOnly] public bool _playClickSound;
    [ReadOnly] public bool _playHoverSound;
    [ReadOnly] public bool _changeScale;
    [ReadOnly] public bool _useVibration;
    [ReadOnly] public bool _changeBackgroundColor;
    [ReadOnly] public bool _changeTextColor;

    [BoxGroup("Audio")] public bool playClickSound = true;
    [BoxGroup("Audio")] public bool playHoverSound = true;
    [BoxGroup("Audio")] [ShowIf("playClickSound")] public AudioClip clickSound;
    [BoxGroup("Audio")] [ShowIf("playHoverSound")]public AudioClip hoverSound;
    [BoxGroup("Audio")] public AudioSource audioSource;

    [BoxGroup("Scaling")] public bool changeScale = true;
    [BoxGroup("Scaling")][ShowIf("changeScale")] public Transform objectToScale;
    [BoxGroup("Scaling")][ShowIf("changeScale")][Range(1, 500)] public float scalePercentageIncrease = 20;
    [BoxGroup("Scaling")][ShowIf("changeScale")] public Vector3 startScale = Vector3.zero;

    //[BoxGroup("Vibration")][ShowIf("showVRSettings")] public bool useVibration = false;
    [BoxGroup("Vibration")] public bool useVibration = false;
    [BoxGroup("Vibration")][ShowIf("useVibration")][ReadOnly] public float frequency = 0.3f;
    [BoxGroup("Vibration")][ShowIf("useVibration")][Range(0.004f, 1f)] public float vibrationAmplitude = 0.01f;
    [BoxGroup("Vibration")][ShowIf("useVibration")][Range(0.1f, 1f)] public float vibrationDuration = 0.1f;

    [Header("Background")]
    [BoxGroup("Color")] public bool changeBackgroundColor = true;
    [BoxGroup("Color")][ShowIf("changeBackgroundColor")] public Image background;
    [BoxGroup("Color")][ShowIf("changeBackgroundColor")] public Color backgroundHighlightColor = Color.red;
    [BoxGroup("Color")][ShowIf("changeBackgroundColor")] public Color backgroundPressedColor = Color.white;
    [BoxGroup("Color")][ShowIf("changeBackgroundColor")][ReadOnly][SerializeField] private Color backgroundDefaultColor;
    [Header("Text")]
    [BoxGroup("Color")] public bool changeTextColor = true;
    [BoxGroup("Color")][ShowIf("changeTextColor")] public TMP_Text text;
    [BoxGroup("Color")][ShowIf("changeTextColor")] public Color textHighlightColor = Color.white;
    [BoxGroup("Color")][ShowIf("changeTextColor")] public Color textPressedColor = Color.grey;
    [BoxGroup("Color")][ShowIf("changeTextColor")][ReadOnly][SerializeField] private Color textDefaultColor;

    [Foldout("Got Values")][ReadOnly][SerializeField] private float scaleMultiplier;
    [Foldout("Got Values")][ReadOnly][SerializeField] private CustomUITriggerEvents customTriggers;
    //[BoxGroup("Got Values")] public InputBridge inputBridge;
    #endregion

    #region Default Methods
    private void OnValidate()
    {
        if (gotOnValidateValues) { return; }

        SetupValidate();

        gotOnValidateValues = true;
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnDisable()
    {
        Unhighlight();
    }

    void InitializeAudio()
    {
        audioSource = UITweakerManager.Instance.audioSource;
        audioSource.outputAudioMixerGroup = settings.defaultSFXMixerGroup;
        audioSource.clip = settings.defaultClickSound;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Initialize()
    {
        _playClickSound = playClickSound;
        _playHoverSound = playHoverSound;
        _changeScale = changeScale;
        _useVibration = useVibration;
        _changeBackgroundColor = changeBackgroundColor;
        _changeTextColor = changeTextColor;

        //Initialize the button scale multiplier
        scaleMultiplier = 1 + (scalePercentageIncrease / 100);

        //get start scale
        startScale = objectToScale.localScale;
    }

    public void TurnOff()
    {
        playClickSound = false;
        playHoverSound = false;
        changeScale = false;
        useVibration = false;
        changeBackgroundColor = false;
        changeTextColor = false;
    }

    public void TurnBackOn()
    {
        playClickSound = _playClickSound;
        playHoverSound = _playHoverSound;
        changeScale = _changeScale;
        useVibration = _useVibration;
        changeBackgroundColor = _changeBackgroundColor;
        changeTextColor = _changeTextColor;
    }
    #endregion

    #region Behaviors
    void HideAllSettings()
    {
        showVRSettings = false;
        showPCSettings = false;
        showConsoleSettings = false;
    }
    
    public void VibrateLeftHand()
    {
        //HapticManager.Instance.inputBridge.VibrateController(hapticSettings.frequency, hapticSettings.uiButtonAmplitude, hapticSettings.uiButtonDuration, ControllerHand.Left);
    }

    public void VibrateRightHand()
    {
        //HapticManager.Instance.inputBridge.VibrateController(hapticSettings.frequency, hapticSettings.uiButtonAmplitude, hapticSettings.uiButtonDuration, ControllerHand.Right);
    }

    [Button("Reset Defaults")]
    void SetupValidate()
    {
        //set default settings
        //color
        backgroundHighlightColor = settings.defaultBackgroundHighlightColor;
        backgroundPressedColor = settings.defaultBackgroundPressedColor;
        textHighlightColor = settings.defaultTextHighlightColor;
        textPressedColor = settings.defaultTextPressedColor;
        //scaling
        scalePercentageIncrease = settings.defaultScalePercentageIncrease;
        //audio
        clickSound = settings.defaultClickSound;
        hoverSound = settings.defaultHoverSound;
        //vibration
        useVibration = settings.defaultUseVibration;
        vibrationAmplitude = settings.defaultVibrationAmplitude;
        vibrationDuration = settings.defaultVibrationDuration;

        //Initialize Events
        if (this.GetComponent<CustomUITriggerEvents>() && customTriggers == null)
        {
            customTriggers = this.GetComponent<CustomUITriggerEvents>();
            customTriggers.uiTweaker = this;
        }

        //initialize Manager
        if (UITweakerManager.Instance && audioSource == null)
        {
            InitializeAudio();
        }
        if (!UITweakerManager.Instance)
        {
            //Debug.LogError("UITweakerManager not found in scene");
            GameObject manager = Instantiate(settings.managerGo);
            manager.name = "UITweakerManager";
            InitializeAudio();
        }


        //set type
        //Button
        if (this.GetComponent<Button>() && uiType != UIType.Button)
        {
            //set the ui type
            uiType = UIType.Button;
            
        }
        //Toggle
        if (this.GetComponent<Toggle>() && uiType != UIType.Toggle)
        {
            //set the ui type
            uiType = UIType.Toggle;
        }
        //Slider
        if (this.GetComponent<Slider>() && uiType != UIType.Slider)
        {
            //set the ui type
            uiType = UIType.Slider;
        }
        //InputFlield
        if (this.GetComponent<TMP_InputField>() && uiType != UIType.InputField)
        {
            //set the ui type
            uiType = UIType.InputField;
        }
        //DropDown
        if (this.GetComponent<TMP_Dropdown>() && uiType != UIType.Dropdown)
        {
            //set the ui type
            uiType = UIType.Dropdown;
        }
        SetDefaults();


        //Hide Settings
        if (platform == Platform.PC && !showPCSettings)
        {
            HideAllSettings();
            showPCSettings = true;
        }
        if (platform == Platform.VR && !showVRSettings)
        {
            HideAllSettings();
            showVRSettings = true;
        }
        if (platform == Platform.Console && !showConsoleSettings)
        {
            HideAllSettings();
            showConsoleSettings = true;
        }
    }

    [Button]
    void UpdateChanges()
    {
        Initialize();
    }
    #endregion

    #region SetDefaults
    void SetDefaults()
    {
        switch (uiType)
        {
            //Button
            case UIType.Button:
                //set object to scale
                changeScale = true;
                objectToScale = transform;
                scalePercentageIncrease = 5;
                //background color
                changeBackgroundColor = false;
                //change text color
                changeTextColor = false;
                break;

            //Toggle
            case UIType.Toggle:
                //set object to scale
                changeScale = true;
                objectToScale = transform.GetChild(0);
                scalePercentageIncrease = 5;
                //background color
                changeBackgroundColor = false;
                //change text color
                changeTextColor = false;
                break;

            //Dropdown
            case UIType.Dropdown:
                //set object to scale
                changeScale = false;
                objectToScale = transform;
                //background color
                changeBackgroundColor = false;
                //change text color
                changeTextColor = false;
                break;

            //Input Field
            case UIType.InputField:
                //set object to scale
                changeScale = true;
                objectToScale = transform;
                scalePercentageIncrease = 5;
                //background color
                changeBackgroundColor = false;
                //change text color
                changeTextColor = false;
                break;

            //Slider
            case UIType.Slider:
                //set object to scale
                changeScale = true;
                foreach (Transform child in transform)
                    if (child == transform.GetChild(transform.childCount - 1))
                        objectToScale = child.GetChild(0);
                scalePercentageIncrease = 5;
                //background color
                changeBackgroundColor = true;
                //change text color
                changeTextColor = false;
                break;
        }

        //set objects to change color
        background = objectToScale.GetComponent<Image>();

        //set the default background color
        backgroundDefaultColor = background.color;

        //set the default start scale
        startScale = objectToScale.localScale;

        //set the text to change color
        foreach (Transform child in transform)
        {
            if (child.GetComponent<TMP_Text>())
            {
                //set the text oject to change color on
                text = child.GetComponent<TMP_Text>();
                //set the default text color
                textDefaultColor = text.color;
                return;
            }
            if (child.GetComponentInChildren<TMP_Text>())
            {
                //set the text oject to change color on
                text = child.GetComponentInChildren<TMP_Text>();
                //set the default text color
                textDefaultColor = text.color;
                return;
            }
        }
    }
    #endregion

    #region Interactions
    public void Press()
    {
        switch (uiType)
        {
            case UIType.Button:
                //Debug.Log("Pressed Button");
                break;
            case UIType.Toggle:
                //Debug.Log("Pressed Toggle");
                break;
            case UIType.Dropdown:
                //Debug.Log("Pressed DropDown");
                break;
            case UIType.InputField:
                //Debug.Log("Pressed InputFiled");
                break;
            case UIType.Slider:
                //Debug.Log("Pressed Slider");
                break;
        }

        //play sound effect
        if (playClickSound)
            audioSource.PlayOneShot(clickSound);

        //change background colour
        if (changeBackgroundColor && background)
            background.color = backgroundPressedColor;

        //change text color
        if (changeTextColor && text)
            text.color = textPressedColor;
    }

    public void Highlight()
    {
        switch (uiType)
        {
            case UIType.Button:
                //Debug.Log("Highlighted Button");
                break;
            case UIType.Toggle:
                //Debug.Log("Highlighted Toggle");
                break;
            case UIType.Dropdown:
                //Debug.Log("Highlighted DropDown");
                break;
            case UIType.InputField:
                //Debug.Log("Highlighted InputFiled");
                break;
            case UIType.Slider:
                //Debug.Log("Highlighted Slider");
                break;
        }

        //vibrate controllers
        if (useVibration)
        {
            //HapticManager.Instance.VibrateRightHand(hapticSettings.uiButtonAmplitude, hapticSettings.uiButtonDuration);
            //HapticManager.Instance.VibrateLeftHand(hapticSettings.uiButtonAmplitude, hapticSettings.uiButtonDuration);
        }

        //Increase the button scale
        if (changeScale)
            objectToScale.localScale = new Vector3(objectToScale.localScale.x * scaleMultiplier, objectToScale.localScale.y * scaleMultiplier, objectToScale.localScale.z * scaleMultiplier);

        //change the button colour
        if (changeBackgroundColor && background)
            background.color = backgroundHighlightColor;

        //change text color
        if (changeTextColor && text)
            text.color = textHighlightColor;

        //play sound effect
        if (playHoverSound)
            audioSource.PlayOneShot(hoverSound);
    }

    public void Unhighlight()
    {
        switch (uiType)
        {
            case UIType.Button:
                //Debug.Log("Unhighlighted Button");
                break;
            case UIType.Toggle:
                //Debug.Log("Unhighlighted Toggle");
                break;
            case UIType.Dropdown:
                //Debug.Log("Unhighlighted DropDown");
                break;
            case UIType.InputField:
                //Debug.Log("Unhighlighted InputFiled");
                break;
            case UIType.Slider:
                //Debug.Log("Unhighlighted Slider");
                break;
        }

        //reset the button scale
        if (changeScale)
            objectToScale.localScale = startScale;

        //change the button colour back
        if (changeBackgroundColor && background)
            background.color = backgroundDefaultColor;

        //change text color
        if (changeTextColor && text)
            text.color = textDefaultColor;
    }
    #endregion
}
