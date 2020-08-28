using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PlayFabController : MonoBehaviour
{

    //Variables
    public static PlayFabController PFC;

    private string userPlayFabID;
    private string userName;
    private string userEmail;
    private string userPassword;
    private string userGender;      //Changes may occur while developping further
    private string userBirthDate;
    private string userCountry;     //Changes may occur while developping further
    private string userPhone;       //Changes may occur while developping further
    private string userAddress;     //Changes may occur while developping further

    private string regexPattern1 = @"([a-zA-Z0-9\-\\_]{1,20})[@]([a-zA-Z0-9]{2,10})[.]([a-zA-Z0-9]{2,10})";
    private string regexPattern2 = @"([a-zA-Z0-9\-\\_]{1,20})[@]([a-zA-Z0-9]{2,10})[.]([a-zA-Z0-9]{2,10})[.]([a-zA-Z0-9]{2,10})";
    private Regex reg1;
    private Regex reg2;

    public GameObject mainMenuPanel;
    public GameObject loginMenuPanel;
    public GameObject signupMenuPanel;
    public GameObject gameMenuPanel;


    //This function is called when the object becomes enabled and active.
    public void OnEnable()
    {
        if (PlayFabController.PFC == null)
        {
            PlayFabController.PFC = this;
        }
        else
        {
            if (PlayFabController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        reg1 = new Regex(regexPattern1);
        reg2 = new Regex(regexPattern2);

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "6751C";
        }

        //AUTO LOGIN WHEN GAME STARTS!
        //if (PlayerPrefs.HasKey("EMAIL") || PlayerPrefs.HasKey("USERNAME"))
        //{
        //    userName = PlayerPrefs.HasKey("USERNAME").ToString();
        //    userEmail = PlayerPrefs.HasKey("EMAIL").ToString();
        //    userPassword = PlayerPrefs.HasKey("PASSWORD").ToString();

        //    var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        //    PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnFailure);
        //}
        



    }





    // -------------------------------------- Getter START REGION! ----------------------------------------------------
    #region Getters
    public void GetUserEmail(string emailIn)
    {
        if(reg1.IsMatch(emailIn)) 
        {
            userEmail = emailIn;
            Debug.Log("useremail reg1: " + reg1.IsMatch(emailIn));
        }
        else if(reg2.IsMatch(emailIn))
        {
            userEmail = emailIn;
            Debug.Log("useremail reg2: " + reg1.IsMatch(emailIn));
        }
        else
        {
            userEmail = "";
        }
        Debug.Log("user email: " + userEmail);
    }
    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }
    public void GetUserName(string usernameIn)
    {
        if (reg1.IsMatch(usernameIn))
        {
            userName = "";
            Debug.Log("username reg1: " + reg1.IsMatch(usernameIn));
        }
        else if (reg2.IsMatch(usernameIn))
        {
            userName = "";
            Debug.Log("username reg2: " + reg1.IsMatch(usernameIn));
        }
        else
        {
            userName = usernameIn;
        }
        Debug.Log("user name: " + userName);
    }
    public void GetUserGender(string userGenderIn)
    {
        userGender = userGenderIn;
    }
    public void GetUserBirthDate(string userBirthDateIn)
    {
        userBirthDate = userBirthDateIn;
    }
    public void GetUserCountry(string userCountryIn)
    {
        userCountry = userCountryIn;
    }
    public void GetUserPhone(string userPhoneIn)
    {
        userPhone = userPhoneIn;
    }
    public void GetUserAddress(string userAddressIn)
    {
        userAddress = userAddressIn;
    }
    #endregion Getters
    // -------------------------------------- Getter END REGION! ----------------------------------------------------





    // -------------------------------------- Login & Register START REGION! ----------------------------------------------------
    #region Login & Register
    public void OnClickLogin()
    {
        //change menus
        mainMenuPanel.SetActive(false);
        loginMenuPanel.SetActive(true);
        signupMenuPanel.SetActive(false);
        gameMenuPanel.SetActive(false);
    }
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Successful !");

        //TO SIGN AUTOMATICCALY!
        //PlayerPrefs.SetString("USERNAME", userName);
        //PlayerPrefs.SetString("Email", userEmail);
        //PlayerPrefs.SetString("PASSWORD", userPassword);

        //FIX MENUS
        loginMenuPanel.SetActive(false);
        gameMenuPanel.SetActive(true);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Text>().text = "Welcome: " + userName.ToString();
        //GameObject.Find("WelcomeTxt").GetComponent<Text>().text = "Welcome: " + userName.ToString();
    }

    
    public void OnClickRegister()
    {
        //change menus
        mainMenuPanel.SetActive(false);
        loginMenuPanel.SetActive(false);
        signupMenuPanel.SetActive(true);
        gameMenuPanel.SetActive(false);
    }
    public void Register()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnFailure);
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registration Successful !");
        //ADD PLAYER DATA (GENDER, COUNTRY, etc...)
        SetUserData();

        //ADD PLAYER DEFAULT STATISTICS
        SetStatistics();


        //FIX MENUS
        signupMenuPanel.SetActive(false);
        loginMenuPanel.SetActive(true);
    }
    #endregion Login & Register
    // -------------------------------------- Login & Register END REGION! ----------------------------------------------------

    private void OnFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call.");
        Debug.LogError(error.GenerateErrorReport());
    }





    // ------------------------ PlayerStatistics REGION ------------------------------------------------------------------
    #region PlayerStatistics
    //Player Statistics Variables (to be moved later)
    public int playerLevel;
    public int playerHealth = 100;

    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }
    public void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName.ToString() + "): " + eachStat.Value.ToString());
            switch (eachStat.StatisticName)
            {
                case "Player Level":
                    playerLevel = eachStat.Value;
                    break;
                case "Player Health":
                    playerHealth = eachStat.Value;
                    break;
            }
        }
    }

    public void SetStatistics()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "Player Level", Value = Random.Range(1,10) },
            new StatisticUpdate { StatisticName = "Player Health", Value = playerHealth }
            }
        },
            result => { Debug.Log("player statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }


    // Build the request object and access the API
    public void StartCloudUpdateStatistics()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStatistics", // Arbitrary function name (must exist in the uploaded cloud.js file)
            FunctionParameter = new { levelIn = playerLevel, healthIn = playerHealth, applesIn = 0 }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateStatistics, OnFailure);
    }

    // OnCloudHelloWorld defined in the next code block
    private static void OnCloudUpdateStatistics(ExecuteCloudScriptResult result)
    {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
        PlayFab.Json.JsonObject jsonResult = (PlayFab.Json.JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    #endregion PlayerStatistics
    // ------------------------ PlayerStatistics REGION ------------------------------------------------------------------




    // ------------------------ PlayerData REGION ------------------------------------------------------------------
    #region PlayerData
    //Player Data Variables (to be moved later)


    public void GetUserData(string myPlayFabeId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabeId,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("Gender")) Debug.Log("No Gender");
            else Debug.Log("Gender: " + result.Data["Gender"].Value);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"Gender", userGender},
            {"BirthDate", userBirthDate},
            {"Country", userCountry},
            {"Phone", userPhone},
            {"Address", userAddress}
            }
        },
        result => Debug.Log("Successfully updated user data"),
            error => {
                Debug.Log("Got error setting user data Ancestor to Arthur");
                Debug.Log(error.GenerateErrorReport());
            });
    }

    #endregion PlayerData
    // ------------------------ PlayerData REGION ------------------------------------------------------------------

    #region Commented region
    /*
    private void OnLoginSuccess2(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    private void OnLoginFailure2(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());

        // Sign In Automatically
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnFailure);
        }
        else
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnFailure);
#endif

#if UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnFailure);
#endif
        }
        //PlayerPrefs.DeleteAll();

    }


    // Update is called once per frame
    void Update()
    {

    }



    // ------------------------ Mobile Code REGION ------------------------------------------------------------------
    #region Mobile Code
    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    // Check if this function can be the same as loginSuccess !!
    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made a successful Mobile Login");
        loginMenuPanel.SetActive(false);
        userPlayFabID = result.PlayFabId;
    }

    #endregion Mobile Code
    // ------------------------ Mobile Code REGION ------------------------------------------------------------------

    // ------------------------ Getters REGION ------------------------------------------------------------------
    #region Getters
    public void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName.ToString() + "is your new Display name!");
    }

    
    //public void OnEmailContact(AddOrUpdateContactEmailResult result)
    //{
    //    Debug.Log(result.CustomData.ToString() + "is your new Contact Email!");
    //}
    //public void OnAvatarUrl(UpdateAvatarUrlRequest result)
    //{
    //    Debug.Log(result.ImageUrl.ToString() + "is your new Avatar Url!");
    //}

    #endregion Getters
    // ------------------------ Getters REGION ------------------------------------------------------------------
    
    */
    #endregion Commented region


    //Call Android Toast
    public void ShowToast(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ShowToast : " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message);
#endif
    }

    //Android DatePicker Dialog Demo
    public void TestDatePicker()
    {
#if UNITY_EDITOR
        Debug.Log("TestDatePicker called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowDatePickerDialog("", "yyyy/M/d", this.gameObject.name, "ShowToast");
#endif
    }


    //To call even when the application is closed -> public
    public void OnDestroy()     
    {
#if UNITY_EDITOR
        Debug.Log("AndroidPlugin.Release called");
#elif UNITY_ANDROID
        AndroidPlugin.Release();
#endif
    }

}
