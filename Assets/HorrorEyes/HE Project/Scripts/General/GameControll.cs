using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class GameControll : MonoBehaviour, IUnityAdsListener
{

    [Header("General parameters")]
    [Tooltip("Player controller script here")]
    public bool m_mobileTouchInput;
    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public ItemsSpawner m_spawner;
    [HideInInspector]
    public Inventory inventory;
    private bool pause;
    public string m_loadingSceneName;
    public string m_mainMenuSceneName;
    public List<DifficultyModes> m_difficultyModes = new List<DifficultyModes>();

    [Header("Enemy Settings")]
    public Text m_eyesPillsCountText;
    [HideInInspector]
    public List<Enemy> enemy = new List<Enemy>();
    public Animation m_eyesPillsAnimationUI;
    public string m_addEyesPillsAnimation;
    public int m_eyesCount;
    public float m_maxWatchTime;
    float m_currentWatchTime;
    bool m_isOnEnemyEyesView;
    public List<EnemyModes> m_enemyModes = new List<EnemyModes>();

    [Header("Security Cameras System")]
    public AudioClip m_videcamChangeSound;
    public AudioClip m_videcamOnSound;
    public AudioClip m_videcamOffSound;
    bool m_isOnSecurityCamerasView;
    [HideInInspector]
    public Videcam[] m_videcams;
    public GameObject videcamControllPanel;
    public Text m_videcamRoomNameText;
    int m_currentVidecam;


    [Header("UI Settings")]
    public Animation fadeScreen;
    public string fadeOutAnimName;
    public string fadeInAnimName;
    public GameObject enemyEyesControllPanel;
    public GameObject gameControllPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Tips Settings")]
    [Tooltip("Tips")]
    public Text m_TipText;
    public float m_TipShowTime;

    [Header("Pictures Settings")]
    public Image m_currentPicture;
    [HideInInspector]
    public Sprite m_nextPicture;
    public Text m_currentPapersCountText;
    public int m_needPicturesCount;
    [HideInInspector]
    public int m_currenPicturestCount;
    public Animation m_paperPicturesAnimationUI;
    public string m_addPaperPictureAnimation;
    PicturePaper.effectType m_currentEffect = PicturePaper.effectType.Null;

    [Header("Start Cutscene Text Printing")]
    [TextArea(10,100)]
    public string m_cutsceneText;
    public GameObject m_cutscenePanel;
    public float m_printSpeed;
    public Text m_cutsceneTextArea;
    public AudioClip[] m_printSound;
    public AudioClip m_printSpaceSound;
    float m_gameHadrnesMultip;
    bool m_cutsceneEnded = false;
    int m_charID = 0;
    int m_charCount;
    bool m_cantPrint = false;


    string gameId = "3653578";
    string myPlacementId = "video";
    bool testMode = false;

    public static bool showAdBool = true;


    private void Awake()
    {
        enemy.Clear();
        Enemy[] enm = FindObjectsOfType<Enemy>();
        enemy.AddRange(enm);
        player = FindObjectOfType<PlayerController>();
        m_videcams = FindObjectsOfType<Videcam>();
        m_spawner = GetComponent<ItemsSpawner>();    
        inventory = GetComponent<Inventory>();
        m_charCount = m_cutsceneText.Length;
        m_cutscenePanel.SetActive(true);
        player.locked = true;
        m_TipText.text = "";
        m_TipText.gameObject.SetActive(false);
        if (enemy.Count > 0)
        {
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].gameObject.SetActive(false);
            }
        }
        enemy.Clear();
        PrepareGame();

    }

    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);

        ShowAd();


        m_spawner.SpawnPictures(m_needPicturesCount);
        m_spawner.SpawnPills();
        Time.timeScale = 1.0f;
        pausePanel.SetActive(false);
        enemyEyesControllPanel.SetActive(false);
        m_currentPapersCountText.text = m_currenPicturestCount.ToString() + "/" + m_needPicturesCount.ToString();

    }

    private void PrepareGame()
    {

        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            AudioListener.volume = 1f;
            volumeSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            player.mouseSensetivity = PlayerPrefs.GetFloat("Sensitivity");
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivitySlider.value = sensitivitySlider.maxValue / 2f;
            player.mouseSensetivity = sensitivitySlider.value;
        }

        if (PlayerPrefs.HasKey("EnemyMode"))
        {
            int EM = PlayerPrefs.GetInt("EnemyMode");

            int i = GetEnemyModeById(EM);

            if(i != -1)
            {
                enemy.AddRange(m_enemyModes[i].m_enemys);
           
            }else
            {
                Debug.Log("Wrond enemy mode id, cant find mode in Enemy Modes list");
            }
            
        }else
        {
            int EM = 0;

            int i = GetEnemyModeById(EM);

            if (i != -1)
            {
                enemy.AddRange(m_enemyModes[i].m_enemys);

            }
            else
            {
                Debug.Log("Wrond enemy mode id, cant find mode in Enemy Modes list");
            }
        }


        if (PlayerPrefs.HasKey("GameDifficulty"))
        {
            int GD = PlayerPrefs.GetInt("GameDifficulty");

            int diffId = GetDifficultyModeById(GD);

            if (diffId != -1)
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].seeDistance = m_difficultyModes[diffId].m_enemySeeDistance;
                    enemy[i].enemyFOV = m_difficultyModes[diffId].m_enemyFOV;
                    enemy[i].lostTime = m_difficultyModes[diffId].m_enemyLostTime;
                    enemy[i].walkSpeed = m_difficultyModes[diffId].m_enemyMoveSpeed;
                    m_gameHadrnesMultip = m_difficultyModes[diffId].m_XP_Multiplayer;
                }
            }else
            {
                Debug.Log("Wrond difficulty mode id, cant find mode in difficulty Modes list");
            }
        }else
        {
            int GD = 0;

            int diffId = GetDifficultyModeById(GD);

            if (diffId != -1)
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].seeDistance = m_difficultyModes[diffId].m_enemySeeDistance;
                    enemy[i].enemyFOV = m_difficultyModes[diffId].m_enemyFOV;
                    enemy[i].lostTime = m_difficultyModes[diffId].m_enemyLostTime;
                    enemy[i].walkSpeed = m_difficultyModes[diffId].m_enemyMoveSpeed;
                    m_gameHadrnesMultip = m_difficultyModes[diffId].m_XP_Multiplayer;
                }
            }
         
        }
    }

    private void Update()
    {
        ControllGame();
        UpdateStats();
        Cutscene();
    }

    private int GetEnemyModeById(int id)
    {
        for (int i = 0; i < m_enemyModes.Count; i++)
        {
            if(m_enemyModes[i].m_enemyModeId == id)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetDifficultyModeById(int id)
    {
        for (int i = 0; i < m_difficultyModes.Count; i++)
        {
            if (m_difficultyModes[i].m_DifficultyModeId == id)
            {
                return i;
            }
        }

        return -1;
    }

    private void Cutscene()
    {
        if(!m_cutsceneEnded && !m_cantPrint)
        {
            m_cutsceneTextArea.text = m_cutsceneTextArea.text + m_cutsceneText[m_charID];
           
                if (!char.IsWhiteSpace(m_cutsceneText[m_charID]))
                {
                    int r = Random.Range(0, m_printSound.Length);
                    player.AS.PlayOneShot(m_printSound[r]);
                }else
                {
                    player.AS.PlayOneShot(m_printSpaceSound);
                }
            
            m_charID += 1;
            m_cantPrint = true;
            if(m_charID >= m_charCount)
            {
                m_cutsceneEnded = true;
               
            }else
            {
                if (m_mobileTouchInput)
                {


                    for (var i = 0; i < Input.touchCount; ++i)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            StartCoroutine(WaitCharPrintTime(0.0f));

                        }
                    }

                    StartCoroutine(WaitCharPrintTime(m_printSpeed));


                }
                else
                {

                    if (CrossPlatformInputManager.GetButton("Interact"))
                    {
                        StartCoroutine(WaitCharPrintTime(0.0f));
                    }
                    else
                    {
                        StartCoroutine(WaitCharPrintTime(m_printSpeed));
                    }

                }


            }
        }
    }

    public void SkipCutscene()
    {
        StopAllCoroutines();
        m_cutsceneEnded = true;
        m_cutsceneTextArea.text = m_cutsceneText;
        StartCoroutine(WaitCutscene());
    }

    public void SetEffect(PicturePaper.effectType effectType)
    {
        if (m_currentEffect != PicturePaper.effectType.Null)
        {
            DisableCurrentEffect(m_currentEffect);
        }

        m_currentEffect = effectType;

            switch (effectType)
            {
              case PicturePaper.effectType.Null:
                break;
              case PicturePaper.effectType.AddEyePills:
                AddEyePills(1);
                    break;
                case PicturePaper.effectType.EnemyAlwaysSeePlayer:
             
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].alwaysSeePlayer = true;
                }
                break;
                case PicturePaper.effectType.EnemyCall:

                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].CallEnemy(player.transform.position);
                }
           
                    break;
                case PicturePaper.effectType.UnlimitedStamina:
                player.m_unlimitedStamina = true;
                    break;
            case PicturePaper.effectType.Random:

                int r = Random.Range(0,3); /// random from 0 to number of effects
                switch (r)
                {
                    case 0:
                        AddEyePills(1);
                        m_currentEffect = PicturePaper.effectType.AddEyePills;
                        break;
                    case 1:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].alwaysSeePlayer = true;
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyAlwaysSeePlayer;
                        break;
                    case 2:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].CallEnemy(player.transform.position);
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyCall;
                        break;
                    case 3:
                        player.m_unlimitedStamina = true;
                        m_currentEffect = PicturePaper.effectType.UnlimitedStamina;
                        break;

                }
                break;

            case PicturePaper.effectType.RandomBad:

                int rb = Random.Range(0, 1); /// random from 0 to number of effects
                switch (rb)
                {
                   
                    case 0:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].alwaysSeePlayer = true;
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyAlwaysSeePlayer;
                        break;
                    case 1:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].CallEnemy(player.transform.position);
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyCall;
                        break;
         

                }
                break;

            }   
        
    }

    private void DisableCurrentEffect(PicturePaper.effectType effectType)
    {
        switch (effectType)
        {
            case PicturePaper.effectType.Null:
                break;

            case PicturePaper.effectType.AddEyePills:             
                break;
            case PicturePaper.effectType.EnemyAlwaysSeePlayer:
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].alwaysSeePlayer = false;
                    enemy[i].ResetPatrol();
                }
                break;
            case PicturePaper.effectType.EnemyCall:
                
                break;
            case PicturePaper.effectType.UnlimitedStamina:
                player.m_unlimitedStamina = false;
                break;
            case PicturePaper.effectType.Random:
                break;
            case PicturePaper.effectType.RandomBad:
                break;

        }
    }

    private void UpdateStats()
    {
        m_eyesPillsCountText.text = m_eyesCount.ToString();
        player.m_enemySeePlayer = SomeEnemySeePlayer();
    }

    private bool SomeEnemySeePlayer()
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            if (enemy[i].seePlayer)
            {
                return true;
            }
        }

        return false;
    }

    public void AddPaperPicture()
    {
        m_currenPicturestCount += 1;
        m_paperPicturesAnimationUI.Play(m_addPaperPictureAnimation);
        m_currentPapersCountText.text = m_currenPicturestCount.ToString() + "/" + m_needPicturesCount.ToString();
    }

    public void AddEyePills(int cnt)
    {
        m_eyesCount += cnt;
        m_eyesPillsAnimationUI.Play(m_addEyesPillsAnimation);
    }

    public void ChangePaperPictureIcon()
    {
        m_currentPicture.sprite = m_nextPicture;
    }

    private void ControllGame()
    {
        if(CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            PauseGame();
        }


        if (m_isOnEnemyEyesView)
        {
           if(m_currentWatchTime >= m_maxWatchTime)
            {
                SetEnemyEyes();
            }else
            {
                m_currentWatchTime += Time.deltaTime;
            }
        }
    }

    public void SetSecurityCamerasChange(int id)
    {
        if(m_videcams.Length > 1)
        {
            if (id > 0)
            {
                if (m_currentVidecam + 1 < m_videcams.Length)
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam += 1;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
                else
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam = 0;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
            }
            else

            {
                if (m_currentVidecam > 0)
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam -= 1;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
                else
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam = m_videcams.Length - 1;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
            }
        }

        AudioSource.PlayClipAtPoint(m_videcamChangeSound,m_videcams[m_currentVidecam].transform.position);
    }

    public void SetSecurityCameras()
    {
        
            if (!m_isOnSecurityCamerasView)
            {
               if (m_videcams.Length > 0 && !player.locked)
               {
                gameControllPanel.SetActive(false);
                videcamControllPanel.SetActive(true);
                m_currentVidecam = 0;
                m_isOnSecurityCamerasView = true;
                player.locked = true;
                player.cameraTransform.gameObject.SetActive(false);
                m_videcams[m_currentVidecam].m_camera.SetActive(true);
                AudioSource.PlayClipAtPoint(m_videcamOnSound, m_videcams[m_currentVidecam].transform.position);
                m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
               }
            }
            else
            {
                AudioSource.PlayClipAtPoint(m_videcamOffSound, player.transform.position);
                gameControllPanel.SetActive(true);
                videcamControllPanel.SetActive(false);
                m_isOnSecurityCamerasView = false;
                player.locked = false;
                player.cameraTransform.gameObject.SetActive(true);
                m_videcams[m_currentVidecam].m_camera.SetActive(false);
            }
        
    }

    public void SetEnemyEyes()
    {
        if(!m_isOnEnemyEyesView && !player.locked)
        {
            if (m_eyesCount > 0)
            {
                m_eyesCount -= 1;
                gameControllPanel.SetActive(false);
                enemyEyesControllPanel.SetActive(true);
                m_currentWatchTime = 0;
                m_isOnEnemyEyesView = true;
                player.locked = true;
                player.cameraTransform.gameObject.SetActive(false);
                int eID = GetNearestEnemy();
                enemy[eID].m_enemyHeadCamera.SetActive(true);
            }
        }else
        {
            gameControllPanel.SetActive(true);
            enemyEyesControllPanel.SetActive(false);
            m_currentWatchTime = 0;
            m_isOnEnemyEyesView = false;
            player.locked = false;
            player.cameraTransform.gameObject.SetActive(true);
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].m_enemyHeadCamera.SetActive(false);
            }
           
        }
    }

    int GetNearestEnemy()
    {
        float dist = 90000f;
        int enemyId = 0;

        for (int i = 0; i < enemy.Count; i++)
        {
            float dist2 = Vector3.Distance(enemy[i].transform.position ,player.transform.position);
            if(dist2 < dist)
            {
                dist = dist2;
                enemyId = i;
            }
        }

        return enemyId;
    }

    public void PauseGame()
    {
        
            if (!pause)
            {
               if (!player.lockedByDying)
               {
                pause = true;
                pausePanel.SetActive(pause);
                Time.timeScale = 0.0f;
                player.locked = true;
               }

            }
            else
            {
            
                pause = false;
                pausePanel.SetActive(pause);
                Time.timeScale = 1.0f;
                player.locked = false;
            
            }
        
    }

    public void GameOver()
    {
       
        gameOverPanel.SetActive(true);

        ShowAd();
    }

    public void GameWin()
    {

        
        gameControllPanel.SetActive(false);
        gameWinPanel.SetActive(true);
        player.locked = true;
        player.lockedByDying = true;
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].gameObject.SetActive(false);
        }
     
    }

    public void ShowTip(int itemId, int type)
    {

        StopCoroutine(WaitForTip());

        if (type == 0)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "You need a " + inventory.m_itemsDatabase.Items[itemId].name;
            StartCoroutine(WaitForTip());
        }

        if (type == 1)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "Door is jamned!";
            StartCoroutine(WaitForTip());
        }

        if (type == 3)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "I need to find all pictures before leave this place!";
            StartCoroutine(WaitForTip());
        }

        if (type == 4)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "Used a " + inventory.m_itemsDatabase.Items[itemId].name;
            StartCoroutine(WaitForTip());
        }

        if (type == 5)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "No Power!";
            StartCoroutine(WaitForTip());
        }

        if (type == 6)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "You take a " + inventory.m_itemsDatabase.Items[itemId].name;
            StartCoroutine(WaitForTip());
        }
    }

    public void ResetPlayerStates()
    {
        if (m_isOnEnemyEyesView)     
        {
            gameControllPanel.SetActive(true);
            enemyEyesControllPanel.SetActive(false);
            m_currentWatchTime = 0;
            m_isOnEnemyEyesView = false;
            player.locked = false;
            player.cameraTransform.gameObject.SetActive(true);
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].m_enemyHeadCamera.SetActive(false);
            }

        }


        if(m_isOnSecurityCamerasView)
        {
            AudioSource.PlayClipAtPoint(m_videcamOffSound, player.transform.position);
            gameControllPanel.SetActive(true);
            videcamControllPanel.SetActive(false);
            m_isOnSecurityCamerasView = false;
            player.locked = false;
            player.cameraTransform.gameObject.SetActive(true);
            m_videcams[m_currentVidecam].m_camera.SetActive(false);
        }









    }

    public void MainMenuExit(int type)
    {
        if (type == 0)
        {
            PlayerPrefs.SetString("GameLevel", m_mainMenuSceneName);
            SceneManager.LoadScene(m_loadingSceneName);
        }

        if(type == 1) //// Game Over
        {
            PlayerPrefs.SetInt("ReturnFromGame", 1);
            PlayerPrefs.SetInt("CollectedPictures", m_currenPicturestCount);
            float plusXP = m_currenPicturestCount * 100;
            plusXP = plusXP * m_gameHadrnesMultip;
            int xp = Mathf.RoundToInt(plusXP);
            PlayerPrefs.SetInt("PLUS_XP", xp);
            PlayerPrefs.SetString("GameLevel", m_mainMenuSceneName);
            SceneManager.LoadScene(m_loadingSceneName);
        }


        if (type == 2) //// Game Win
        {
            PlayerPrefs.SetInt("ReturnFromGame", 1);
            PlayerPrefs.SetInt("CollectedPictures", m_currenPicturestCount);
            float plusXP = m_currenPicturestCount * 250;
            plusXP = plusXP * m_gameHadrnesMultip;
            int xp = Mathf.RoundToInt(plusXP);
            PlayerPrefs.SetInt("PLUS_XP", xp);
            PlayerPrefs.SetString("GameLevel", m_mainMenuSceneName);
            SceneManager.LoadScene(m_loadingSceneName);
        }
    }

    public void ScreenFade(int state)
    {
        if(state == 0)
        {
            fadeScreen.Play(fadeOutAnimName);
        }

        if(state == 1)
        {
            fadeScreen.Play(fadeInAnimName);
        }

        if(state == 2)
        {
           
            StartCoroutine(WaitKillAnim(1.2f));
        }

    }

    IEnumerator WaitForTip()
    {

        yield return new WaitForSeconds(m_TipShowTime);
        m_TipText.text = "";
        m_TipText.gameObject.SetActive(false);

    }

    private IEnumerator WaitKillAnim(float killTime)
    {
        yield return new WaitForSeconds(killTime);
        fadeScreen.Play(fadeInAnimName);
        StartCoroutine(WaitFadeAnim(fadeInAnimName));
    }

    private IEnumerator WaitCharPrintTime(float speed)
    {
        yield return new WaitForSeconds(speed);
        m_cantPrint = false;

    }

    private IEnumerator WaitCutscene()
    {
        yield return new WaitForSeconds(1f);
        fadeScreen.Play(fadeOutAnimName);
        m_cutscenePanel.SetActive(false);
        player.locked = false;
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].gameObject.SetActive(true);
        }


    }

    private IEnumerator WaitFadeAnim(string name)
    {
        yield return new WaitForSeconds(fadeScreen[name].length);
        GameOver();
        
    }

    public void ConfigureApply()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        player.mouseSensetivity = PlayerPrefs.GetFloat("Sensitivity");

    }


    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (showAdBool == true)
        {
            // If the ready Placement is rewarded, show the ad:
            if (placementId == myPlacementId)
            {
                Advertisement.Show(myPlacementId);
                showAdBool = false;
            }
        }
    }


    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}

