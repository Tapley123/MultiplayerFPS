using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Text.RegularExpressions;

public class PlayfabManager : MonoBehaviour
{
    [Foldout("UI")][SerializeField] private TMP_Text text_Error;
    private string username;

    [Header("Panels")]
    [Foldout("UI")][SerializeField] private List<GameObject> mainPanels = new List<GameObject>();
    [Foldout("UI")][SerializeField] private List<GameObject> subPanels = new List<GameObject>();

    [Header("Loading")]
    [Foldout("UI")][SerializeField] private GameObject panel_Loading;
    [Foldout("UI")][SerializeField] private TMP_Text text_Loading;

    [Header("Login/Create")]
    [Foldout("UI")][SerializeField] private GameObject panel_LoginCreate;
    [Foldout("UI")][SerializeField] private TMP_InputField inputField_Email;
    [Foldout("UI")][SerializeField] private TMP_InputField inputField_Password;

    [Header("Set Username")]
    [Foldout("UI")][SerializeField] private GameObject panel_SetUsername;
    [Foldout("UI")][SerializeField] private TMP_InputField inputField_Username;

    [Header("Logged In")]
    [Foldout("UI")][SerializeField] private GameObject panel_LoggedIn;
    [Foldout("UI")][SerializeField] private TMP_Text text_Username;

    private void Awake()
    {
        Initilize();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Initilize()
    {
        ClearError();
        
        //Load Saved Data
        XMLManager.Instance.LoadItems(SaveType.Player);
        XMLManager.Instance.LoadItems(SaveType.DefaultStorage);
        //If the login data has been stored
        if(XMLManager.Instance.playerDB.savedLogin)
        {
            //Activate load screen while you attempt to auto-login
            Load($"Attempting to auto-login");
            //Attempt to login with the saved email and password
            Login(XMLManager.Instance.playerDB.email, XMLManager.Instance.playerDB.password);
        }
        //The login data has not previously been saved
        else
        {
            SwapPanel(panel_LoginCreate);
        }
    }

    #region Behaviors
    void ClearError()
    {
        text_Error.text = string.Empty;
    }

    void Error(string errorMsg)
    {
        text_Error.text = errorMsg;
    }

    void SwapPanel(GameObject panelToActivate)
    {
        //disable all of the panels
        foreach (GameObject p in mainPanels)
        {
            p.SetActive(false);
        }

        //activate the one panel you want
        panelToActivate.SetActive(true);
    }

    void SwapSubPanel(GameObject subPanelToActivate)
    {
        //disable all of the panels
        foreach (GameObject p in subPanels)
        {
            p.SetActive(false);
        }

        //activate the one panel you want
        subPanelToActivate.SetActive(true);
    }

    // Regular expression pattern to validate an email
    private static readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        // Use Regex to check if the email matches the pattern
        return Regex.IsMatch(email, emailPattern);
    }
    #endregion

    #region Loading
    void Load(string loadingMsg)
    {
        //opens the loading screen panel
        SwapPanel(panel_Loading);
        //changes the loading screen text to display what is happening
        text_Loading.text = loadingMsg;
    }
    #endregion

    #region Login / Create
    public void Button_CreateAccount()
    {
        Debug.Log($"Called Create Account");
        ClearError();

        //email validation
        if (IsValidEmail(inputField_Email.text))
        {
            Debug.Log($"Email is valid");
        }
        //email is invalid
        else
        {
            Error($"Email Address in invalid!");
            return;
        }

        //password validation
        if (inputField_Password.text.Length >= 6 && inputField_Password.text.Length <= 100)
        {
            Debug.Log($"Password is valid");
        }
        //password is invalid
        else
        {
            Error($"Password invalid: must be between 6 and 100 characters long!");
            return;
        }

        //passed all of the checks so attempt to make an account
        RegisterPlayFabUser(inputField_Email.text, inputField_Password.text);
    }

    public void Button_Login()
    {
        Debug.Log($"Called Login");
        ClearError();

        //email validation
        if (IsValidEmail(inputField_Email.text))
        {
            Debug.Log($"Email is valid");
        }
        //email is invalid
        else
        {
            Error($"Email Address in invalid!");
            return;
        }

        //password validation
        if (inputField_Password.text.Length >= 6 && inputField_Password.text.Length <= 100)
        {
            Debug.Log($"Password is valid");
        }
        //password is invalid
        else
        {
            Error($"Password invalid: must be between 6 and 100 characters long!");
            return;
        }

        Login(inputField_Email.text, inputField_Password.text);
    }

    void RegisterPlayFabUser(string email, string password)
    {
        Debug.Log($"Trying to register: {email}...");

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        },
        result =>
        {
            Debug.Log($"You registered an account with the email: {email}. \n You should try auto login from here!");
            Login(email, password);
        },
        error =>
        {
            Debug.Log($"There was an error registering the account: {error.GenerateErrorReport()}");
            Error($"There was an error registering the account: {error.GenerateErrorReport()}");
        });
    }

    void Login(string email, string password)
    {
        Debug.Log($"Trying to login with email({email}), password({password})");
        ClearError();

        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest()
        {
            Email = email,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        },
        result =>
        {
            Debug.Log($"You logged in with {email}");
            //***** CHECK IF USERNAME IS SET ****** If yes go to logged in, if not go to set username
            Load($"Checking username");
            CheckUserName();
        },
        error =>
        {
            Debug.Log($"There was an error logging in: {error.GenerateErrorReport()}");
            //swap to the login panel in case you came from autologin (loading screen)
            SwapPanel(panel_LoginCreate);
            Error($"There was an error logging in: {error.GenerateErrorReport()}");
        });
    }
    #endregion

    #region Username
    void CheckUserName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
        {

        },
        result =>
        {
            Debug.Log($"You got the account info: {result}");

            //if there is no username set
            if (string.IsNullOrEmpty(result.AccountInfo.Username))
            {
                Debug.Log($"Username not set");
                SwapPanel(panel_SetUsername);
            }
            else
            {
                Debug.Log($"Username already created");

                //store the username
                username = result.AccountInfo.Username;
                //display the username
                text_Username.text = username;
                //switch to the logged in panel
                SwapPanel(panel_LoggedIn);
            }

            //result.AccountInfo.TitleInfo.DisplayName;
        },
        error =>
        {
            Debug.Log($"There was an error getting the account info: {error.GenerateErrorReport()}");

            //make sure you are at the login create panel if your request failed
            SwapPanel(panel_LoginCreate);

            //show error message on screen
            Error(error.GenerateErrorReport());
        });
    }

    public void Button_SetUsername()
    {
        Debug.Log($"Called Set Username");

        //Check if possible******
    }
    #endregion

    #region LoggedIn
    public void Button_BackFromLoggedIn()
    {
        Debug.Log($"Called Back From Logged in");

        SwapPanel(panel_LoginCreate);
    }
    #endregion
}
