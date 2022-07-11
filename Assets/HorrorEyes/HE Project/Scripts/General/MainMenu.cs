using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Settings")]
    [Tooltip("Slider to controll volume")]
    public Slider volumeSlider;
    [Tooltip("Slider to controll sensitivity")]
    public Slider sensitivitySlider;
    public GameObject m_lockImageGameObject;
    public Button m_playButton;
    public GameObject m_startGameWindow;
    public GameObject m_setupGameWindow;
    public GameObject m_returnedFormGameWindow;
    public Text m_plus_Xp_text;
    public Text m_collectedPicturesCountText;

    [Header("Game Setup")]
    public string m_loadingSceneName;
    public LevelSetup[] m_levels;
    public EnemySetup[] m_enemys;
    public DifficultSetup[] m_difficults;
    int levelID = 0;
    int enemyID = 0;
    int difficultID = 0;

    [Header("XP Settings")]
    public Text m_xp_text;
    private static int m_XP;

    [Header("Level Settings")]
    public Text m_levelNameText;
    public Text m_levelDescriptionText;
    public GameObject m_buyLevelIconGO;
    public Text m_levelPriceText;
    public GameObject m_level_NotEnoughXP;
    bool m_selectedLevelIsLocked;


    [Header("Enemy Settings")]
    public Text m_enemyNameText;
    public Text m_enemyDescriptionText;
    public GameObject m_buyEnemyIconGO;
    public Text m_enemyPriceText;
    public GameObject m_enemy_NotEnoughXP;
    bool m_selectedEnemyIsLocked;


    [Header("Difficult Settings")]
    public Text m_difficultNameText;
    public Text m_difficultDescriptionText;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("ReturnFromGame"))
        {
            int m_RFG = PlayerPrefs.GetInt("ReturnFromGame");
            if (m_RFG == 1)
            {

                m_returnedFormGameWindow.SetActive(true);
                m_startGameWindow.SetActive(false);

                if (PlayerPrefs.HasKey("PLUS_XP"))
                {
                 
                    int m_plus_XP = PlayerPrefs.GetInt("PLUS_XP");
                    m_XP += m_plus_XP;
                    PlayerPrefs.SetInt("XP", m_XP);
                    m_plus_Xp_text.text = "+ " + m_plus_XP.ToString();

                }else
                {
                    m_XP += 0;
                    PlayerPrefs.SetInt("XP", m_XP);
                    m_plus_Xp_text.text = "+ " + 0;
                }

                if(PlayerPrefs.HasKey("CollectedPictures"))
                {
                   m_collectedPicturesCountText.text = "You Collect " + PlayerPrefs.GetInt("CollectedPictures") + " Pictures!";
                }

                PlayerPrefs.SetInt("ReturnFromGame", 0);
                PlayerPrefs.SetInt("CollectedPictures", 0);
            }
        }


        if (PlayerPrefs.HasKey("XP"))
        {
            m_XP = PlayerPrefs.GetInt("XP");
        }else
        {
            m_XP = 0;
        }
        m_xp_text.text = m_XP.ToString() + " XP";


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
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivitySlider.value = sensitivitySlider.maxValue / 2f;
        }

        GameSetup();
    }

    private void GameSetup()
    {
        m_levelNameText.text = m_levels[levelID].name;
        m_levelDescriptionText.text = m_levels[levelID].m_description;

        m_enemyNameText.text = m_enemys[enemyID].name;
        m_enemyDescriptionText.text = m_enemys[enemyID].m_description;

        m_difficultNameText.text = m_difficults[difficultID].name;
        m_difficultDescriptionText.text = m_difficults[difficultID].m_description;

        m_levels[levelID].m_levelShowGameobject.SetActive(true);

        for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
        {
            m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(true);
        }


        UpdateGameInfo();
    }

    public void ChangeLevel(int value)
    {
        

        if (value > 0)
        {
            if(levelID < m_levels.Length -1)
            {
                m_levels[levelID].m_levelShowGameobject.SetActive(false);
                levelID += 1;
            }
        }

        if (value < 0)
        {
            if (levelID > 0)
            {
                m_levels[levelID].m_levelShowGameobject.SetActive(false);
                levelID -= 1;
            }
        }


        m_levelNameText.text = m_levels[levelID].name;
        m_levelDescriptionText.text = m_levels[levelID].m_description;

        m_levels[levelID].m_levelShowGameobject.SetActive(true);

        UpdateGameInfo();
    }

    public void ChangeEnemy(int value)
    {

        if (value > 0)
        {
            if (enemyID < m_enemys.Length - 1)
            {
                for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
                {
                    m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(false);
                }
                enemyID += 1;
            }
        }

        if (value < 0)
        {
            if (enemyID > 0)
            {
                for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
                {
                    m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(false);
                }
                enemyID -= 1;
            }
        }

        for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
        {
            m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(true);
        }
        m_enemyNameText.text = m_enemys[enemyID].name;
        m_enemyDescriptionText.text = m_enemys[enemyID].m_description;
        UpdateGameInfo();
    }

    public void ChangeDifficulty(int value)
    {


        if (value > 0)
        {
            if (difficultID < m_difficults.Length - 1)
            {
                difficultID += 1;
            }
        }

        if (value < 0)
        {
            if (difficultID > 0)
            {
                difficultID -= 1;
            }
        }

        m_difficultNameText.text = m_difficults[difficultID].name;
        m_difficultDescriptionText.text = m_difficults[difficultID].m_description;

    }

    public void BuyMode(int modeID) /// buy 1 - level, 2 - enemy
    {
        if(modeID == 1)
        {
            if(m_selectedLevelIsLocked)
            {
                if(m_XP >= m_levels[levelID].m_price)
                {
                    m_XP -= m_levels[levelID].m_price;
                    m_levels[levelID].m_lockState = 1;
                    PlayerPrefs.SetInt(m_levels[levelID].m_sceneName + "LockState", 1);
                    PlayerPrefs.SetInt("XP",m_XP);
                    m_setupGameWindow.SetActive(true);
                    UpdateGameInfo();
                }else
                {
                    m_level_NotEnoughXP.SetActive(true);
                }
            }
        }

        if (modeID == 2)
        {
            if (m_selectedEnemyIsLocked)
            {
                if (m_XP >= m_enemys[enemyID].m_price)
                {
                    m_XP -= m_enemys[enemyID].m_price;
                    m_enemys[enemyID].m_lockState = 1;
                    PlayerPrefs.SetInt("Enemy" + m_enemys[enemyID].m_enemyModeId + "LockState", 1);
                    PlayerPrefs.SetInt("XP", m_XP);
                    m_setupGameWindow.SetActive(true);
                    UpdateGameInfo();
                }
                else
                {
                    m_enemy_NotEnoughXP.SetActive(true);
                }
            }
        }
    }

    private void UpdateGameInfo()
    {
        int m_levelLockState = 0;
        int m_enemyLockState = 0;

        m_xp_text.text = m_XP.ToString() + " XP";

        if (PlayerPrefs.HasKey(m_levels[levelID].m_sceneName + "LockState"))
        {
            m_levelLockState = PlayerPrefs.GetInt(m_levels[levelID].m_sceneName + "LockState");
            m_levels[levelID].m_lockState = m_levelLockState;

            switch (m_levelLockState)
            {
                case 0:
                    m_buyLevelIconGO.SetActive(true);
                    m_selectedLevelIsLocked = true;
                    break;
                case 1:
                    m_buyLevelIconGO.SetActive(false);
                    m_selectedLevelIsLocked = false;
                    break;
            }
        }
        else
        {
            if (m_levels[levelID].m_lockByDefault)
            {
                m_selectedLevelIsLocked = true;
                m_buyLevelIconGO.SetActive(true);
            }
            else
            {
                m_buyLevelIconGO.SetActive(false);
                m_selectedLevelIsLocked = false;
            }
        }



        if (PlayerPrefs.HasKey("Enemy" + m_enemys[enemyID].m_enemyModeId + "LockState"))
        {
            m_enemyLockState = PlayerPrefs.GetInt("Enemy" + m_enemys[enemyID].m_enemyModeId + "LockState");
            m_enemys[enemyID].m_lockState = m_enemyLockState;
            switch (m_enemyLockState)
            {
                case 0:
                    m_buyEnemyIconGO.SetActive(true);
                    m_selectedEnemyIsLocked = true;
                    break;
                case 1:
                    m_buyEnemyIconGO.SetActive(false);
                    m_selectedEnemyIsLocked = false;
                    break;
            }
        }
        else
        {
            if (m_enemys[enemyID].m_lockByDefault)
            {
                m_buyEnemyIconGO.SetActive(true);
                m_selectedEnemyIsLocked = true;
            }
            else
            {
                m_selectedEnemyIsLocked = false;
                m_buyEnemyIconGO.SetActive(false);
            }
        }

        if (m_selectedLevelIsLocked || m_selectedEnemyIsLocked)
        {
            m_lockImageGameObject.SetActive(true);
            m_playButton.interactable = false;
        }else
        {
            m_lockImageGameObject.SetActive(false);
            m_playButton.interactable = true;
        }

        m_levelPriceText.text = m_levels[levelID].m_price.ToString();
        m_enemyPriceText.text = m_enemys[enemyID].m_price.ToString();
    }

    public void ApplyConfig()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
     
    }

    public void ClearAllSaves()
    {
        m_XP = 0;
        m_xp_text.text = m_XP.ToString();
        PlayerPrefs.DeleteAll();
        UpdateGameInfo();
    }

    public void StartGame()
    {
        if (!m_selectedEnemyIsLocked && !m_selectedLevelIsLocked)
        {
            PlayerPrefs.SetInt("EnemyMode", m_enemys[enemyID].m_enemyModeId);
            PlayerPrefs.SetString("GameLevel", m_levels[levelID].m_sceneName);
            PlayerPrefs.SetInt("GameDifficulty", m_difficults[difficultID].m_difficultId);
            SceneManager.LoadScene(m_loadingSceneName, LoadSceneMode.Single);
        }
    }

    public void rate()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.Lordly.Granny3");
    }

    public void QuitGame()
    {      
        Application.Quit();
    }

    

   
}
