using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject FX;
    public AudioClip levelComplete;
    public Color mainColor;
    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
    public Color color5;
    public Color color6;
    public static GameManager instance;
    public GameObject wall_left;
    public GameObject confettiFx;
    public GameObject BackButtonConfirmationWin;
    public GameObject AddTimeButton;
    public GameObject wall_right;
    public GameObject wall_top;
    public GameObject wall_bottom;
    public GameObject Ball;
    public GameObject f1_prefab;
    public GameObject[] f2_prefab;
    public GameObject BallParent;
    public GameObject Block;
    public GameObject Corner;
    public GameObject top_Bottom;
    public GameObject BlockParent;
    public GameObject SoundParent;
    public GameObject SoundChild;
    public GameObject ballsDownButton;
    public GameObject speedUpButton;
    public GameObject allClearScreen;
    public GameObject endGameScreen;
    public GameObject animationMessageScreen;
    public GameObject f1_gameobjectParent;
    public GameObject f2_gameobjectParent;
    public GameObject targetIndicator;
    public GameObject timerIndicator;
    public GameObject gameWonBgPanel;
    public GameObject notice;
    public int score = 0;
    public Camera camera_;
    public int maxSpeedVal = 20;
    public List<GameObject> players = new List<GameObject>();
    Vector2[] touches = new Vector2[3];
    public bool projectiling;
    public bool clampSpeedNow = false;
    public bool MainBallDown = false;
    public GameObject mainBall;
    public bool addNewLevel = false;
    public bool progressSaved = false;
    bool f1_spawned = false;
    bool f2_spawned = false;
    public Text ballCount;
    public Text checkPointsCount;
    public Text TimerText;
    public Text HighscoreText;
    public Text scoreText;
    public Text levelText;
    public Text targetText;
    public int timeLeft = 600;
    bool isGameEnded = false;
    bool isGameover = false;
    bool isLevelClear = false;
    public float deadLine = 0;
    public Vector2 base_;
    public Vector2 forceVal;
    public LineRenderer GameOverLine;
    bool TimeIndicatorShown = false;
    //string Interstitial_Ad_Id = "InterstitialAd";
    string Rewarded_Ad = "Rewarded_video";
    string Rewarded_Ad_Id = "Rewarded_Android";
    string Interstitial_Ad_Id = "Interstitial_Android";
    //string Banner_Ad_Id = "BannerAd";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Time.timeScale = 1;
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            string message = (PlayerPrefs.GetInt("mission") + 3).ToString() + "K : Target    ";
            PopUpNotice("Your target : " + (PlayerPrefs.GetInt("mission") + 3).ToString() + " K");
            targetText.text = message;
            showGameobjectForTime(targetIndicator, 10);
            if (!PlayerPrefs.HasKey("mission"))
            {
                PlayerPrefs.SetInt("mission", 0);
            }
            levelText.text = (PlayerPrefs.GetInt("mission") + 1).ToString() + ": Level    ";
            isGameover = false;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float ratio = (float)screenHeight / (float)screenWidth;
            float newBlockSize = (1.777777778f * 0.5f) / ratio;
            Block.transform.localScale = new Vector3(newBlockSize, newBlockSize, newBlockSize);
            projectiling = false;

            if (PlayerPrefs.HasKey("highScore"))
            {
                HighscoreText.text = "    HIGHSCORE: " + fromIntToString(PlayerPrefs.GetInt("highScore"));
            }
            else
            {
                HighscoreText.text = "    HIGHSCORE: 0";
            }
            if (PlayerPrefs.HasKey("myLevel"))
            {
                if (PlayerPrefs.HasKey("savedChildNum"))
                {
                    RetrieveProgress();
                }
                else
                {
                    PlayerPrefs.SetInt("myLevel", 5);
                    CreateLevel();
                }

            }
            else
            {
                PlayerPrefs.SetInt("myLevel", 5);
                CreateLevel();
            }
            spawnWalls();
            checkPointsCount.text = "Checkpoints:" + PlayerPrefs.GetInt("myLevel").ToString();
            StartCoroutine(UpdateTimerText());
        }

    }
    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isLevelClear)
            {
                CheckLevelClear();
            }
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                scoreText.text = "    SCORE: " + fromIntToString(score);
            }

            if (score >= PlayerPrefs.GetInt("highScore"))
            {
                if(!PlayerPrefs.HasKey("prevHighscore"))
                {
                    PlayerPrefs.SetInt("prevHighscore", score);
                }
                PlayerPrefs.SetInt("highScore", score);
                HighscoreText.text = "    HIGHSCORE: " + fromIntToString(PlayerPrefs.GetInt("highScore"));
                if(score > PlayerPrefs.GetInt("prevHighscore") + 500)
                {
                    PlayerPrefs.SetInt("prevHighscore", score);
                    loginWithPlayFab.instance.SendLeaderboard(PlayerPrefs.GetInt("highScore"));
                }
            }
            if (BlockParent.transform.childCount == 0)
            {
                showAllClearScreen();
            }
            if (clampSpeedNow)
            {
                foreach (GameObject player in players)
                {
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(player.GetComponent<Rigidbody2D>().velocity, maxSpeedVal);
                }
            }
            if (allBallStationary() == 1 && !progressSaved)
            {
                //SaveProgress(BlockParent);
                progressSaved = true;
            }
            if (allBallStationary() == 1)
            {
                projectiling = false;
                clampSpeedNow = false;
                if (ballsDownButton.activeInHierarchy)
                {
                    ballsDownButton.SetActive(false);
                }
                if (BlockParent.transform.childCount == 0)
                {
                    CreateLevel();
                }
                if (maxSpeedVal > 7)
                {
                    maxSpeedVal = 7;
                }
                speedUpButton.SetActive(false);
                if (players[0].GetComponent<SpriteRenderer>().color != Color.green)
                {
                    players[0].GetComponent<SpriteRenderer>().color = Color.green;
                }
                if (players[0].GetComponent<SpriteRenderer>().sortingOrder != 2)
                {
                    players[0].GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
                foreach (GameObject player in players)
                {
                    player.GetComponent<gamePlay>().collidedToF1 = false;
                }
                if (f1_gameobjectParent.transform.childCount > 0 && f1_gameobjectParent.transform.GetChild(0).CompareTag("f1done"))
                {
                    Destroy(f1_gameobjectParent.transform.GetChild(0).gameObject);
                }
                if (f2_gameobjectParent.transform.childCount > 0 && f2_gameobjectParent.transform.GetChild(0).CompareTag("f2done"))
                {
                    Destroy(f2_gameobjectParent.transform.GetChild(0).gameObject);
                }
                if (f1_spawned == true)
                {
                    f1_spawned = false;
                }
                if (f2_spawned == true)
                {
                    f2_spawned = false;
                }


            }
            if (allBallStationary() == 0)
            {
                progressSaved = false;
                clampSpeedNow = true;
                projectiling = true;
                speedUpButton.SetActive(true);
            }
            if (allBallMoving() == 0)
            {
                ballCount.gameObject.SetActive(true);
                ballCount.gameObject.transform.position = new Vector2(base_.x, base_.y + 0.22f);
                ballCount.text = "x" + numOfStationaryBalls().ToString();
            }
            if (allBallMoving() == 1)
            {
                ballCount.gameObject.SetActive(false);
            }
        }

    }
    private void Update()
    {
        backButton();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            checkEndGame();
        }

        if (isGameEnded)
        {
            PlayerPrefs.DeleteKey("myLevel");
            isGameover = true;
            endGameScreen.SetActive(true);
            AdsManager4.instance.HideBanner();
            AdsManager4.instance.showRewardedVideoAd(Rewarded_Ad);
            deleteProgress(BlockParent);
            isGameEnded = false;
            stopAllBalls();
        }
        if (Input.touchCount > 0 && !projectiling && !isGameover && !notice.activeSelf)
        {
            PlayerPrefs.SetInt("ballCount", 0);
            RaycastHit2D ray;
            RaycastHit2D ray2;
            if (Input.GetTouch(0).position.y < AddTimeButton.transform.GetChild(2).position.y)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    Touch touchInfo = Input.GetTouch(0);
                    touches[0] = Camera.main.ScreenToWorldPoint(new Vector3(touchInfo.position.x, touchInfo.position.y, -10));
                    int layerMask = ~(LayerMask.GetMask("ball"));
                    ray = Physics2D.Raycast(mainBall.transform.position, new Vector2(touches[0].x, touches[0].y + 5), Mathf.Infinity, layerMask);
                    Vector2 reflectedray = new Vector2();
                    if (ray.collider.tag == "leftWall" || ray.collider.tag == "rightWall" || ray.collider.tag == "block")
                    {
                        reflectedray = new Vector2(mainBall.transform.position.x, 2 * (ray.point.y) - mainBall.transform.position.y);
                    }
                    else if (ray.collider.tag == "topWall" || ray.collider.tag == "blockBase")
                    {
                        reflectedray = new Vector2(2 * (ray.point.x) - mainBall.transform.position.x, mainBall.transform.position.y);
                    }
                    string prevTag = ray.collider.gameObject.tag;
                    int layerMask2 = ~(LayerMask.GetMask(prevTag));
                    ray2 = Physics2D.Raycast(ray.point, reflectedray, Mathf.Infinity, layerMask2);
                    touches[1] = ray.point;
                    touches[2] = reflectedray;
                    Vector2 semifinalPoint = new Vector2(2 * reflectedray.x - touches[1].x, 2 * reflectedray.y - touches[1].y);
                    Vector2 finalPoint = new Vector2(2 * semifinalPoint.x - touches[1].x, 2 * semifinalPoint.y - touches[1].y);
                    mainBall.GetComponent<LineRenderer>().enabled = true;
                    mainBall.GetComponent<LineRenderer>().SetPosition(0, mainBall.transform.position);
                    mainBall.GetComponent<LineRenderer>().SetPosition(1, touches[1]);
                    mainBall.GetComponent<LineRenderer>().SetPosition(2, finalPoint);
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    MainBallDown = false;
                    int tmp = PlayerPrefs.GetInt("spawn");
                    PlayerPrefs.SetInt("spawn", tmp + 1);
                    changeForceVal();
                    clampSpeedNow = true;
                    foreach (GameObject player in players)
                    {
                        player.GetComponent<LineRenderer>().enabled = false;
                    }
                    projectiling = true;
                    moveAllBalls1();
                }
            }
        }
        updateCheckPointsText();
    }
    public void AddTime()
    {
        if (Advertisement.IsReady(Rewarded_Ad_Id))
        {
            Advertisement.Show(Rewarded_Ad_Id);
        }
        else
        {
            showMessageInAnimation("Rewarded AD not ready please make sure you are connected to internet.", 2);
        }
    }
    int allBallStationary()
    {
        int Val = 1;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            {
                Val = 1 * Val;
            }
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude != 0)
            {
                Val = 0 * Val;
            }
        }
        return Val;
    }
    int allBallMoving()
    {
        int Val = 1;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude != 0)
            {
                Val = 1 * Val;
            }
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            {
                Val = 0 * Val;
            }
        }
        return Val;
    }
    void animatedScreenWithMessage()
    {
        isGameEnded = true;
        animationMessageScreen.transform.GetChild(0).GetComponent<TMP_Text>().text = "Time up\n \nGame over !!!";
        animationMessageScreen.SetActive(true);
        Invoke("hideMessageAnimated", 3);
    }
    void backButton()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                if(SceneManager.GetActiveScene().buildIndex == 0)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
                else if(SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
                else if(SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
            }
        }
    }
    void checkEndGame()
    {
        if (!isGameover)
        {
            for (int i = 0; i < BlockParent.transform.childCount; i++)
            {
                if (BlockParent.transform.GetChild(i).position.y < deadLine)
                {
                    isGameEnded = true;
                }
            }
            changeColorNearDeadline();
        }
    }
    void changeColorNearDeadline()
    {
        for (int i = 0; i < BlockParent.transform.childCount; i++)
        {
            if (BlockParent.transform.GetChild(i).position.y > deadLine)
            {
                GameOverLine.transform.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < BlockParent.transform.childCount; i++)
        {
            if (BlockParent.transform.GetChild(i).position.y < deadLine + 1)
            {
                GameOverLine.transform.gameObject.SetActive(true);
                GameOverLine.SetPosition(0, new Vector2(-5, deadLine));
                GameOverLine.SetPosition(1, new Vector2(5, deadLine));
                if (BlockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color != color3)
                {
                    StartCoroutine(changeColorOfBlock(BlockParent.transform.GetChild(i).gameObject));
                }
                else if (BlockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color != color5)
                {
                    StartCoroutine(changeColorOfBlock2(BlockParent.transform.GetChild(i).gameObject));
                }
            }
        }
    }
    IEnumerator changeColorOfBlock(GameObject block)
    {
        yield return new WaitForSeconds(1f);
        if (block)
        {
            if (block.GetComponent<SpriteRenderer>().color != color3)
            {
                block.GetComponent<SpriteRenderer>().color = color3;
            }
        }
    }
    IEnumerator changeColorOfBlock2(GameObject block)
    {
        yield return new WaitForSeconds(1f);
        if (block)
        {
            if (block.GetComponent<SpriteRenderer>().color != color5)
            {
                block.GetComponent<SpriteRenderer>().color = color5;
            }
        }
    } 
    void changeForceVal()
    {
        forceVal = (-new Vector2(mainBall.transform.position.x, mainBall.transform.position.y) + touches[1]) * 1000 * Time.deltaTime;
    }
    void closeAllClearScreen()
    {
        allClearScreen.SetActive(false);
    }
    public void CreateLevel()
    {
        int Rand_f1 = Random.Range(1, 6);
        int Rand_f2 = Random.Range(1, 6);
        int maxBlockInColumn = 0;
        Vector3[] blockPos = new Vector3[10];
        for (int i = 0; i < 10; i++)
        {
            int randomSpawn = Random.Range(0, 2);
            if (randomSpawn == 1 && maxBlockInColumn < 8)
            {
                int randomSpawnF1 = Random.Range(0, 2);
                int randomSpawnF2 = Random.Range(0, 2);
                blockPos[i] = camera_.ViewportToWorldPoint(new Vector3((i * 0.1f) + 0.05f, 0.8f, camera_.nearClipPlane));
                if (Rand_f1 == 1 && !f1_spawned && randomSpawnF1 == 1 && f1_gameobjectParent.transform.childCount == 0)
                {
                    GameObject block = Instantiate(f1_prefab, blockPos[i], Quaternion.identity);
                    maxBlockInColumn++;
                    f1_spawned = true;
                    block.transform.parent = f1_gameobjectParent.transform;
                }
                else if (Rand_f2 == 1 && !f2_spawned && randomSpawnF2 == 1 && f2_gameobjectParent.transform.childCount == 0)
                {
                    int selectedChild = Random.Range(0, 2);
                    GameObject block = Instantiate(f2_prefab[selectedChild], blockPos[i], Quaternion.identity);
                    maxBlockInColumn++;
                    f2_spawned = true;
                    block.transform.parent = f2_gameobjectParent.transform;
                }
                else
                {
                    GameObject block = Instantiate(Block, blockPos[i], Quaternion.identity);
                    block.GetComponent<SpriteRenderer>().color = mainColor;
                    int ranNumChoose = Random.Range(0, 5);
                    int numCount = 1;
                    if (ranNumChoose < 4)
                    {
                        numCount = 2;
                    }
                    block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (PlayerPrefs.GetInt("myLevel") * numCount).ToString();
                    block.transform.parent = BlockParent.transform;
                    maxBlockInColumn++;
                }
            }
            if (BlockParent.transform.childCount == 0 && i == 9)
            {
                blockPos[i] = camera_.ViewportToWorldPoint(new Vector3((i * 0.1f) + 0.05f, 0.8f, camera_.nearClipPlane));
                GameObject block = Instantiate(Block, blockPos[i], Quaternion.identity);
                block.GetComponent<SpriteRenderer>().color = mainColor;
                block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("myLevel").ToString();
                block.transform.parent = BlockParent.transform;
                maxBlockInColumn++;
            }
            if (i >= 7 && maxBlockInColumn == 0)
            {
                blockPos[i] = camera_.ViewportToWorldPoint(new Vector3((i * 0.1f) + 0.05f, 0.8f, camera_.nearClipPlane));
                GameObject block = Instantiate(Block, blockPos[i], Quaternion.identity);
                block.GetComponent<SpriteRenderer>().color = mainColor;
                block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("myLevel").ToString();
                block.transform.parent = BlockParent.transform;
                maxBlockInColumn++;
            }
            if (i >= 8 && maxBlockInColumn == 1)
            {
                blockPos[i] = camera_.ViewportToWorldPoint(new Vector3((i * 0.1f) + 0.05f, 0.8f, camera_.nearClipPlane));
                GameObject block = Instantiate(Block, blockPos[i], Quaternion.identity);
                block.GetComponent<SpriteRenderer>().color = mainColor;
                block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("myLevel").ToString();
                maxBlockInColumn++;
                block.transform.parent = BlockParent.transform;
            }
            if (i >= 9 && maxBlockInColumn == 2)
            {
                blockPos[i] = camera_.ViewportToWorldPoint(new Vector3((i * 0.1f) + 0.05f, 0.8f, camera_.nearClipPlane));
                GameObject block = Instantiate(Block, blockPos[i], Quaternion.identity);
                block.GetComponent<SpriteRenderer>().color = mainColor;
                block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("myLevel").ToString();
                block.transform.parent = BlockParent.transform;
                maxBlockInColumn++;
            }
        }
    }
    void CheckLevelClear()
    {
        if (score >= (PlayerPrefs.GetInt("mission") + 3) * 1000 && PlayerPrefs.HasKey("myLevel"))
        {
            PlayerPrefs.DeleteKey("myLevel");
            deleteProgress(BlockParent);
            showMessageInAnimation("congratulations!!!\n mission passed\n level++", 1);
            gameWonBgPanel.SetActive(true);
            GameObject cnfettiFx = Instantiate(confettiFx);
            Destroy(cnfettiFx, 7);
            PlayerPrefs.SetInt("mission", PlayerPrefs.GetInt("mission") + 1);
            isLevelClear = true;
            Invoke("playWinSound", 1);
            Invoke("loadNewLevel", 7);
            PlayerPrefs.Save();
            stopAllBalls();
        }
    }
    void playWinSound()
    {
        playThisSound(levelComplete);
    }
    void deleteProgress(GameObject blockParent)
    {
        int childCount = PlayerPrefs.GetInt("savedChildNum");
        for (int i = 0; i < childCount; i++)
        {
            string childName = "child" + i.ToString();
            string childPosX = childName + "_posX";
            string childPosY = childName + "_posY";
            string childHitVal = childName + "_val";
            string childColor = childName + "_color";
            if (PlayerPrefs.HasKey("savedChildNum"))
            {
                PlayerPrefs.DeleteKey(childPosX);
                PlayerPrefs.DeleteKey(childPosY);
                PlayerPrefs.DeleteKey(childHitVal);
                PlayerPrefs.DeleteKey(childColor);
            }
        }
        PlayerPrefs.DeleteKey("savedChildNum");
    }
    public void exitApplication()
    {
        Application.Quit();
    }
    string fromIntToString(int value1)
    {
        float value = value1;
        string returnValue = "0";
        if (value < 10000)
        {
            returnValue = value.ToString();
        }
        else if (value >= 10000 && value < 1000000)
        {
            returnValue = (value / 1000).ToString("F2") + "K";
        }
        else if (value >= 1000000 && value < 1000000000)
        {
            returnValue = (value / 1000000).ToString("F2") + "B";
        }
        return returnValue;
    }
    public void GetAllBallsDown()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.transform.position = base_;
        }
        ballsDownButton.SetActive(false);
    }
    public void GetMainBall(GameObject ball)
    {
        MainBallDown = true;
        mainBall = ball;
        base_ = mainBall.transform.position;
    }
    public void gameToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    IEnumerator hideMessageInAnimation(int num, int time)
    {
        yield return new WaitForSeconds(time);
        if (num == 2)
        {
            timeLeft = (10 + (PlayerPrefs.GetInt("mission"))) * 60;
        }
        animationMessageScreen.SetActive(false);
    }
    void hideMessageAnimated()
    {
        animationMessageScreen.SetActive(false);
    }
    IEnumerator hideObjectAfterTime(GameObject gameObject, int time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    void moveAllBalls1()
    {
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(forceVal, maxSpeedVal);
        if (PlayerPrefs.GetInt("ballCount") < players.Count - 1)
        {
            PlayerPrefs.SetInt("ballCount", PlayerPrefs.GetInt("ballCount") + 1);
            Invoke("moveAllBalls2", 0.08f);
        }
        else if (PlayerPrefs.GetInt("ballCount") == players.Count - 1)
        {
            ballsDownButton.SetActive(true);
            PlayerPrefs.SetInt("ballCount", 0);
        }
    }
    void moveAllBalls2()
    {
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(forceVal, maxSpeedVal);
        if (PlayerPrefs.GetInt("ballCount") < players.Count - 1)
        {
            PlayerPrefs.SetInt("ballCount", PlayerPrefs.GetInt("ballCount") + 1);
            Invoke("moveAllBalls1", 0.08f);
        }
        else if (PlayerPrefs.GetInt("ballCount") == players.Count - 1)
        {
            ballsDownButton.SetActive(true);
            PlayerPrefs.SetInt("ballCount", 0);
        }
    }
    void loadNewLevel()
    {
        SceneManager.LoadScene(2);
        AdsManager4.instance.showInterstitial(Interstitial_Ad_Id);
    }
    int numOfStationaryBalls()
    {
        int Val = 0;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            {
                Val++;
            }
        }
        return Val;
    }
    public void OnClickNo()
    {
        BackButtonConfirmationWin.SetActive(false);
        Time.timeScale = 1;
    }
    public void OpenScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void playThisSound(AudioClip clip)
    {
        GameObject audioSource = Instantiate(GameManager.instance.SoundChild, GameManager.instance.SoundParent.transform);
        AudioSource source = audioSource.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(audioSource, 2);
    }
    void PopUpNotice(string message)
    {
        notice.SetActive(true);
        notice.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
        Invoke("HidePopMessage", 2);
    }
    void HidePopMessage()
    {
        notice.SetActive(false);
    }
    void RetrieveProgress()
    {
        int childCount = PlayerPrefs.GetInt("savedChildNum");
        timeLeft = PlayerPrefs.GetInt("timeLeft");
        score = PlayerPrefs.GetInt("score");
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                string childName = "child" + i.ToString();
                string childPosX = childName + "_posX";
                string childPosY = childName + "_posY";
                string childHitVal = childName + "_val";
                string childColor = childName + "_color";

                Vector2 position = new Vector2(PlayerPrefs.GetFloat(childPosX), PlayerPrefs.GetFloat(childPosY));
                int childHitValue = PlayerPrefs.GetInt(childHitVal);
                GameObject child = Instantiate(Block, position, Quaternion.identity);
                child.transform.parent = BlockParent.transform;
                for (int j = 0; j < 6; j++)
                {
                    if (PlayerPrefs.GetInt(childColor) == 1)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = color1;
                    }
                    else if (PlayerPrefs.GetInt(childColor) == 2)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = color2;
                    }
                    else if (PlayerPrefs.GetInt(childColor) == 3)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = color3;
                    }
                    else if (PlayerPrefs.GetInt(childColor) == 4)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = color4;
                    }
                    else if (PlayerPrefs.GetInt(childColor) == 5)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = color5;
                    }
                    else if (PlayerPrefs.GetInt(childColor) == 6)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = color6;
                    }
                    else if (PlayerPrefs.GetInt(childColor) == 7)
                    {
                        child.transform.GetComponent<SpriteRenderer>().color = mainColor;
                    }
                }
                child.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = childHitValue.ToString();
            }
        }
    }
    void showGameobjectForTime(GameObject gameObject, int time)
    {
        gameObject.SetActive(true);
        StartCoroutine(hideObjectAfterTime(gameObject, time));
    }
    void showMessageInAnimation(string message, int num)
    {
        animationMessageScreen.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
        if(num == 2)
        {
            animationMessageScreen.SetActive(true); StartCoroutine(hideMessageInAnimation(num, 3));
        }
        else
        {
            animationMessageScreen.SetActive(true); StartCoroutine(hideMessageInAnimation(num, 7));
        }
        
    }
    
    void spawnWalls()
    {
        Vector3 top = camera_.ViewportToWorldPoint(new Vector3(0.5f, 1, camera_.nearClipPlane));
        Vector3 left = camera_.ViewportToWorldPoint(new Vector3(0, 0.5f, camera_.nearClipPlane));
        Vector3 right = camera_.ViewportToWorldPoint(new Vector3(1, 0.5f, camera_.nearClipPlane));
        Vector3 bottom = camera_.ViewportToWorldPoint(new Vector3(0.5f, 0, camera_.nearClipPlane));
        Vector3 ball = camera_.ViewportToWorldPoint(new Vector3(0.5f, 0.08f, camera_.nearClipPlane));

        GameObject TopWall = Instantiate(wall_top, top, Quaternion.identity);
        Instantiate(wall_left, left, Quaternion.identity);
        Instantiate(wall_right, right, Quaternion.identity);
        GameObject bottomWall = Instantiate(wall_bottom, bottom, Quaternion.identity);

        TopWall.transform.parent = top_Bottom.transform;
        bottomWall.transform.parent = top_Bottom.transform;
        Vector2 corner_2 = camera_.ViewportToWorldPoint(new Vector3(0, 1, camera_.nearClipPlane));
        Vector2 corner_4 = camera_.ViewportToWorldPoint(new Vector3(1, 1, camera_.nearClipPlane));
        GameObject c2 = Instantiate(Corner, corner_2, Quaternion.identity);
        GameObject c4 = Instantiate(Corner, corner_4, Quaternion.identity);
        c2.transform.position = new Vector2(c2.transform.position.x, TopWall.transform.GetChild(0).transform.position.y);
        c4.transform.position = new Vector2(c4.transform.position.x, TopWall.transform.GetChild(0).transform.position.y);

        for (int i = 0; i < PlayerPrefs.GetInt("myLevel"); i++)
        {
            GameObject player = Instantiate(Ball, ball, Quaternion.identity);
            players.Add(player);
            players[i].transform.parent = BallParent.transform;
        }
        GetMainBall(players[0]);
        base_ = mainBall.transform.position;
    }
    void showAllClearScreen()
    {
        allClearScreen.SetActive(true);
        Invoke("closeAllClearScreen", 3);
    }
    void stopAllBalls()
    {
        foreach(GameObject player in players)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
    public void SaveProgress(GameObject blockParent)
    {
        int childCount = blockParent.transform.childCount;
        deleteProgress(blockParent);
        PlayerPrefs.SetInt("savedChildNum", childCount);
        PlayerPrefs.SetInt("timeLeft", timeLeft);
        PlayerPrefs.SetInt("score", score);
        for (int i = 0; i < childCount; i++)
        {
            string childName = "child" + i.ToString();
            string childPosX = childName + "_posX";
            string childPosY = childName + "_posY";
            string childHitVal = childName + "_val";
            string childColor = childName + "_color";
            if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == color1)
            {
                PlayerPrefs.SetInt(childColor, 1);
            }
            else if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == color2)
            {
                PlayerPrefs.SetInt(childColor, 2);
            }
            else if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == color3)
            {
                PlayerPrefs.SetInt(childColor, 3);
            }
            else if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == color4)
            {
                PlayerPrefs.SetInt(childColor, 4);
            }
            else if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == color5)
            {
                PlayerPrefs.SetInt(childColor, 5);
            }
            else if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == color6)
            {
                PlayerPrefs.SetInt(childColor, 6);
            }
            else if (blockParent.transform.GetChild(i).GetComponent<SpriteRenderer>().color == mainColor)
            {
                PlayerPrefs.SetInt(childColor, 7);
            }
            PlayerPrefs.SetFloat(childPosX, blockParent.transform.GetChild(i).transform.position.x);
            PlayerPrefs.SetFloat(childPosY, blockParent.transform.GetChild(i).transform.position.y);
            PlayerPrefs.SetInt(childHitVal, int.Parse(blockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text));
        }
        PlayerPrefs.Save();
    }
    IEnumerator UpdateTimerText()
    {
        yield return new WaitForSeconds(1);
        timeLeft -= 1;
        if(timeLeft <= 178 && !TimeIndicatorShown)
        {
            PopUpNotice("< 3 minutes left");
            showGameobjectForTime(timerIndicator, 15);
            TimeIndicatorShown = true;
        }
        if(timeLeft > 180 && TimeIndicatorShown)
        {
            TimeIndicatorShown = false;
            timerIndicator.SetActive(false);
        }

        if(timeLeft / 60 > 2)
        {
            if (AddTimeButton.activeSelf)
            {
                AddTimeButton.SetActive(false);
            }
            TimerText.color = Color.green;
        }
        else if(timeLeft / 60 <= 2)
        {
            if(!AddTimeButton.activeSelf)
            {
                AddTimeButton.SetActive(true);
            }
            TimerText.color = Color.red;
        }
        if(timeLeft%60 > 9)
        {
            TimerText.text =(timeLeft / 60).ToString() + ":" + (timeLeft % 60).ToString() + "  :: TIME LEFT    ";
        }
        else
        {
            TimerText.text =(timeLeft / 60).ToString() + ":0" + (timeLeft % 60).ToString() + "  :: TIME LEFT   ";
        }
        
        if(timeLeft > 0)
        {
            StartCoroutine(UpdateTimerText());
        }
        if(timeLeft == 0 && timeLeft%60 == 0)
        {
            animatedScreenWithMessage();
        }
    }
    void updateCheckPointsText()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            string preText = checkPointsCount.text;
            if (preText != "Checkpoint:" + PlayerPrefs.GetInt("myLevel").ToString())
            {
                checkPointsCount.text = "   CHECKPOINTS:" + PlayerPrefs.GetInt("myLevel").ToString();
            }
        }
    }
    public void speedUpAllBalls()
    {
        foreach (GameObject player in players)
        {
            maxSpeedVal = 10;
            player.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity * 2;
        }
        speedUpButton.SetActive(false);
    }
}
