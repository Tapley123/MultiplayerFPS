using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using NaughtyAttributes;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//https://www.youtube.com/watch?v=6vl1IYMpwVQ

public enum SaveType { DefaultStorage, Player };
public class XMLManager : MonoBehaviour
{
    #region Singleton
    private static XMLManager _instance;
    public static XMLManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<XMLManager>(); } return _instance; } }
    #endregion

    [BoxGroup("Settings")][Expandable] public AudioSetting audioSettings;

    //Defaults
    [BoxGroup("Defaults")] public string defaultsFileName = "DefaultsDatabase";
    [BoxGroup("Defaults")] public DefaultStorage defaultsDB;

    //Player
    [BoxGroup("Player")] public string playerFileName = "PlayerDatabase";
    [BoxGroup("Player")] public PlayerStorage playerDB;

    private void Awake()
    {
        //Persist through scenes
        DontDestroyOnLoad(this);

        //Makes sure there is only one instance of this class
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        //Load all of the data in you save files when you start the game
        LoadAllData();
    }

    void LoadAllData()
    {
        //Loop through all the save types
        foreach (SaveType savetype in Enum.GetValues(typeof(SaveType)))
        {
            LoadItems(savetype);
        }
    }

    public void ClearData(SaveType savetype)
    {
        switch (savetype)
        {
            case SaveType.DefaultStorage:
                //clear all of the data assosiated with this save type
                defaultsDB.RestoreDefaults();
                //Save the cleared data
                SaveItems(savetype);
                break;

            case SaveType.Player:
                //clear all of the data assosiated with this save type
                playerDB.RestoreDefaults();
                //Save the cleared data
                SaveItems(savetype);
                break;
        }
    }

    //save function
    public void SaveItems(SaveType saveType)
    {
        //create xml serializer
        XmlSerializer serializer = null;
        FileStream stream = null;

        switch (saveType)
        {
            case SaveType.DefaultStorage:
                //Assign xml serializer type
                serializer = new XmlSerializer(typeof(DefaultStorage));
                //Assign file stream path
                stream = new FileStream(Application.persistentDataPath + $"/{defaultsFileName}.xml", FileMode.Create);
                //Serialize
                serializer.Serialize(stream, defaultsDB);
                break;

            case SaveType.Player:
                //Assign xml serializer type
                serializer = new XmlSerializer(typeof(PlayerStorage));
                //Assign file stream path
                stream = new FileStream(Application.persistentDataPath + $"/{playerFileName}.xml", FileMode.Create);
                //Serialize
                serializer.Serialize(stream, playerDB);
                break;
        }
        //close stream
        stream.Close();
    }

    //load function
    public void LoadItems(SaveType saveType)
    {
        XmlSerializer serializer = null;
        FileStream stream = null;

        switch (saveType)
        {
            case SaveType.DefaultStorage:
                //check if the file path exists
                if (File.Exists(Application.persistentDataPath + $"/{defaultsFileName}.xml"))
                {
                    //Assign xml serializer type
                    serializer = new XmlSerializer(typeof(DefaultStorage));
                    //Assign file stream path
                    stream = new FileStream(Application.persistentDataPath + $"/{defaultsFileName}.xml", FileMode.Open);
                    //Deserialize
                    defaultsDB = serializer.Deserialize(stream) as DefaultStorage;
                    //close stream
                    stream.Close();
                }
                break;

            case SaveType.Player:
                //check if the file path exists
                if (File.Exists(Application.persistentDataPath + $"/{playerFileName}.xml"))
                {
                    //Assign xml serializer type
                    serializer = new XmlSerializer(typeof(PlayerStorage));
                    //Assign file stream path
                    stream = new FileStream(Application.persistentDataPath + $"/{playerFileName}.xml", FileMode.Open);
                    //Deserialize
                    playerDB = serializer.Deserialize(stream) as PlayerStorage;
                    //close stream
                    stream.Close();
                }
                break;
        }
    }

    #region Defaults
    //To use this fuction
    // 1) load the data from the xml file to a DefaultStorage variable
    // 2) change the data you want to change
    // 3) send the variable here
    public void UpdateDefaults(DefaultStorage newdata)
    {
        //load the data
        LoadItems(SaveType.DefaultStorage);

        //if the first time bool is different from the new data
        if (defaultsDB.firstTime != newdata.firstTime)
        {
            defaultsDB.firstTime = newdata.firstTime;
        }

        //save to xml
        SaveItems(SaveType.DefaultStorage);
    }

    [Button]
    public void ResetDefaultValues()
    {
        //load the data
        LoadItems(SaveType.DefaultStorage);

        //reset the data
        defaultsDB.RestoreDefaults();

        //save to xml
        SaveItems(SaveType.DefaultStorage);
    }

    [Button]
    public void UpdateDefaultValuesFromVar()
    {
        //save to xml
        SaveItems(SaveType.DefaultStorage);
    }
    #endregion
}


#region Default Storage
[System.Serializable]
public class DefaultStorage
{
    public bool firstTime = true;

    //Audio Settings
    public bool volumeSaved = false;
    [Range(-80f, 0f)] public float masterVol = 0;
    [Range(-80f, 0f)] public float musicVol = 0;
    [Range(-80f, 0f)] public float sfxVol = 0;
    [Range(-80f, 0f)] public float voiceVol = 0;
    [Range(-80f, 0f)] public float enemyVol = 0;

    //Comfort Settings
    public bool comfortSaved = false;
    public bool devOptionsSaved = false;
    public bool vignetteOn;
    public bool positionSaved = false;
    public int positionForwardMultiplier = 0;
    public int positionUpMultiplier = 0;

    //Dev Settings
    public bool showDebugMenu = false;

    public void RestoreDefaults()
    {
        firstTime = true;
        volumeSaved = false;
        comfortSaved = false;
    }
}
#endregion

#region Player Sorage
[System.Serializable]
public class PlayerStorage
{
    public bool savedLogin = false;

    //Login
    public string email;
    public string password;

    public void RestoreDefaults()
    {
        savedLogin = false;
        email = "";
        password = "";
    }
}
#endregion
