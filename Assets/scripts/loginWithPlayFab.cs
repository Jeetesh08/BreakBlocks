using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class loginWithPlayFab : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform rowsParent;
    public TMP_InputField UserDisplayName;
    public GameObject BackButtonConfirmationWin;
    public Text usersUsername;
    public TMP_Text GlanceText;
    string LoggedInPlayfabId;
    public bool loggedIn = false;
    public static loginWithPlayFab instance;
    public GameObject inputNamePanel;
    string statisticName = "leaderboard_ballQueue"; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Login();
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (!PlayerPrefs.HasKey("myName"))
            {
                inputNamePanel.SetActive(true);
            }
            PlayerPrefs.SetString("displayName", PlayerPrefs.GetString("myName"));
        }
        
        Invoke("sendHighscore", 3f);
        //Invoke("updateGlanceText", 3f);
        
    }
    void backButton()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
                else if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
                else if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
            }
        }
    }
    public void SaveNewNameUser()
    {
        inputNamePanel.SetActive(false);
        if (UserDisplayName.text != "")
        {
            PlayerPrefs.SetString("myName", UserDisplayName.text);
            updateName();
        }
        else
        {
            PlayerPrefs.SetString("myName", "UnknownPlayer");
            updateName();
        }
    }
    public void updateGlanceText()
    {
        sendHighscore();
        GetLeaderboard();
        GetLeaderboardAroundPlayer();
        if(PlayerPrefs.GetInt("highestLevel") < PlayerPrefs.GetInt("rankOneLevels"))
        {
            GlanceText.text = "Rank 1 : " + PlayerPrefs.GetInt("rankOneLevels").ToString() + " levels.\n Your rank : " + PlayerPrefs.GetInt("myRank").ToString() + "\n Your levels : " + PlayerPrefs.GetInt("highestLevel").ToString();
        }
        else if(PlayerPrefs.GetInt("highestLevel") == PlayerPrefs.GetInt("rankOneLevels") && PlayerPrefs.GetInt("highestLevel") > 1)
        {
            GlanceText.text = "Congrats!!!\nyou're at the top in leaderboard";
        }
    }
    void updateName()
    {
        PlayerPrefs.SetString("displayName", PlayerPrefs.GetString("myName"));
        OnUpdatePlayerName();
    }
    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
    public void showLeaderBoard(GameObject leaderBoard)
    {
        leaderBoard.SetActive(true);
    }
    public void closeLeaderboard(GameObject Leaderboard)
    {
        Leaderboard.SetActive(false);
    }
    void sendHighscore()
    {
        SendLeaderboard(PlayerPrefs.GetInt("highScore", 0));
    }

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        loggedIn = true;
        OnUpdatePlayerName();
        LoggedInPlayfabId = result.PlayFabId;
        PlayerPrefs.SetString("loginStatus", "Logged in Successfully");
        Debug.Log("successfully logged in with custom id");
    }

    void lateLogin()
    {
        Login();
    }
    void OnError(PlayFabError error)
    {
        loggedIn = false;
        PlayerPrefs.SetString("loginStatus", "Log in failed due to network error");
        Debug.Log("failed to logged in with custom id");
    }
    void OnError2(PlayFabError error)
    {
        Debug.Log("failed to send or retrieve leaderboard");
    }
    public void SendLeaderboard(int Highscore)
    {
        
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName,
                    Value = (int)Highscore
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError2);
    }
    public void GetLeaderboard()
    {
        Invoke("sendHighscore", 0f);
        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError2);
    }
    public void playThisSound(AudioClip clip)
    {
        GameObject audioSource = Instantiate(GameManager.instance.SoundChild, GameManager.instance.SoundParent.transform);
        AudioSource source = audioSource.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        Destroy(audioSource, 2);

    }
    void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        int i = 0;
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            
            //instaUsername[i] = item.DisplayName;
            i++;
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            if(item.Position == 0)
            {
                PlayerPrefs.SetInt("rankOneLevels", item.StatValue);
                texts[1].text = item.DisplayName ;
            }
            else if(item.Position == 1)
            {
                texts[1].text = item.DisplayName ;
            }
            else if (item.Position == 2)
            {
                texts[1].text = item.DisplayName ;
            }
            else
            {
                texts[1].text = item.DisplayName;
            }



            if (item.StatValue < 10000)
            {
                texts[2].text = item.StatValue.ToString("0");
            }
            else if (item.StatValue >= 10000 && item.StatValue < 1000000)
            {
                texts[2].text = ((float)(item.StatValue) / (1000)).ToString("F1") + "K";
            }
            else if (item.StatValue >= 1000000 && item.StatValue < 100000000)
            {
                texts[2].text = ((float)(item.StatValue) / (1000)).ToString("F1") + "M";
            }
            else
            {
                texts[2].text = item.StatValue.ToString("0");
            }

            if(item.PlayFabId == LoggedInPlayfabId)
            {
                if(item.StatValue >= PlayerPrefs.GetInt("highScore", 0))
                {
                    PlayerPrefs.SetInt("highScore", item.StatValue);
                }
            }
        }
    }
    public void GetLeaderboardAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = statisticName,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, OnError2);
    }

    void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult result)
    {
        int i = 0;
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {

            //instaUsername[i] = item.DisplayName;
            i++;
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;

            if (item.StatValue < 10000)
            {
                texts[2].text = item.StatValue.ToString("0");
            }
            else if (item.StatValue >= 10000 && item.StatValue < 100000)
            {
                texts[2].text = ((float)(item.StatValue) / (1000)).ToString("F1") + "k";
            }
            else if (item.StatValue >= 100000 && item.StatValue < 1000000)
            {
                texts[2].text = ((float)(item.StatValue) / (1000)).ToString("F1") + "k";
            }
            else if (item.StatValue >= 1000000 && item.StatValue < 100000000)
            {
                texts[2].text = ((float)(item.StatValue) / (1000)).ToString("F1") + "M";
            }
            else
            {
                texts[2].text = item.StatValue.ToString("0");
            }

            if(item.PlayFabId == LoggedInPlayfabId)
            {
                PlayerPrefs.SetInt("myRank", item.Position + 1);
                texts[1].color = Color.white;
            }
        }
    }

    public void OnUpdatePlayerName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = PlayerPrefs.GetString("displayName")

        }, result =>
        {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => OnUpdatePlayerName()); ;
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("successfully sent highscore");
    }
}
