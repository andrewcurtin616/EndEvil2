using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI elements
/// </summary>

public class UIController : MonoBehaviour
{
    GameManagerController GameManager;
    public Text ScoreText;
    Text CurrentAmmoText;
    Text TotalAmmoText;
    Text GrenadeText;
    Slider healthSlider;
    Image DamagePanel;
    bool damageFlash = false;
    AudioSource my_audio;
    RectTransform Heart;
    bool beatFlag = false;
    bool dead;
    Text YouDiedText;

    bool win;

    GameObject OptionsMenu;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Toggle autoReloadToggle;
    public Toggle walkingAnimToggle;

    //moved from UIManager
    GameObject[] pauseObjects;

    public GameObject NewHighScoreText;
    public GameObject EndlessModeText;

    // Start is called before the first frame update
    void /*Start*/Awake()
    {
        GameManager = GameManagerController.getInstance();
        GameManager.SetUserInterface(GetComponent<UIController>());

        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        ScoreText.text = ""+0;

        CurrentAmmoText = GameObject.Find("CurrentAmmoText").GetComponent<Text>();
        TotalAmmoText = GameObject.Find("TotalAmmoText").GetComponent<Text>();
        GrenadeText = GameObject.Find("GrenadeText").GetComponent<Text>();

        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        healthSlider.value = 100;
        DamagePanel = GameObject.Find("DamagePanel").GetComponent<Image>();
        my_audio = GetComponent<AudioSource>();
        Heart = GameObject.Find("Heart").GetComponent<RectTransform>();
        YouDiedText = GameObject.Find("YouDiedText").GetComponent<Text>();

        OptionsMenu = GameObject.Find("OptionsMenu");
        sfxVolumeSlider = GameObject.Find("SFXVolumeSlider").GetComponent<Slider>();
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        ChangeSFXVolume();
        musicVolumeSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        ChangeMusicVolume();
        autoReloadToggle = GameObject.Find("AutoReloadToggle").GetComponent<Toggle>();
        autoReloadToggle.isOn = PlayerPrefs.GetInt("AutoReloadToggle", 1) != 0;
        walkingAnimToggle = GameObject.Find("WalkingAnimToggle").GetComponent<Toggle>();
        walkingAnimToggle.isOn = PlayerPrefs.GetInt("WalkingAnimToggle", 1) != 0;
        OptionsMenu.SetActive(false);

        //moved from UIManager
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePaused();

        NewHighScoreText = GameObject.Find("NewHighScoreText");
        NewHighScoreText.SetActive(false);
        EndlessModeText = GameObject.Find("EndlessModeText");
        EndlessModeText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (win)
            return;
        if (dead)
        {
            DamagePanel.color = Color.Lerp(DamagePanel.color, Color.red, Time.deltaTime);
            return;
        }

        if (damageFlash)
        {
            DamagePanel.color = Color.red;
        }
        else
        {
            DamagePanel.color = Color.Lerp(DamagePanel.color, Color.clear, 5 * Time.deltaTime);
        }
        damageFlash = false;

        if (healthSlider.value > 0)
        {
            if (beatFlag)
            {
                //Heart.localScale = Vector3.Lerp(Heart.localScale, Vector3.one * 2, 0.5f);
                Heart.localScale += Vector3.one *
                    /*healthSlider.value / 100*/100 / healthSlider.value * Time.deltaTime;
                if (Heart.localScale.x >= 1.5)
                    beatFlag = false;
            }
            else
            {
                Heart.localScale = Vector3.Lerp(Heart.localScale, Vector3.one,
                    /*healthSlider.value / 100*/100 / healthSlider.value * Time.deltaTime);
                //Heart.localScale -= Vector3.one * healthSlider.value /100 * Time.deltaTime;
                if (Heart.localScale.x <= 1.1f)
                    beatFlag = true;
            }
        }

        //moved from UIManager
        //uses the p button to pause and unpause the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                //Time.timeScale = 0; //moved to showPause
                showPaused();
            }
            else if (Time.timeScale == 0)
            {
                Debug.Log("high");
                //Time.timeScale = 1; //moved to hidePause
                hidePaused();
                HideOptionsMenu();
            }
        }

    }//end update

    public void UpdateScore(int points)
    {
        int temp = int.Parse(ScoreText.text) + points;
        ScoreText.text = ""+temp;

        if (PlayerPrefs.GetInt("HighScore",0) >= 500)
            return;

        if (temp >= 500)
        {
            StartCoroutine("YouWin");
            StartCoroutine("TempWinLose");
            GameManager.EndGameWin();
            win = true;
        }
    }

    IEnumerator TempWinLose()
    {
        yield return new WaitForSeconds(.75f);
        showPaused();
        GameObject.Find("OptionsButton").SetActive(false);
        Time.timeScale = 1;
        GameObject.Find("PlayButton").SetActive(false);
        GameObject.Find("PauseText").SetActive(false);
    }

    IEnumerator YouWin()
    {
        Text YouWinText = GameObject.Find("YouWinText").GetComponent<Text>();
        for (float i = 0; i < 1; i += 0.05f)
        {
            Color c = YouWinText.color;
            c.a = i;
            YouWinText.color = c;
            yield return new WaitForSeconds(.175f);
        }
    }

    public void UpdateAmmo(int current, int total)
    {
        CurrentAmmoText.text = "" + current;
        TotalAmmoText.text = "" + total;
    }

    public void UpdateGrenades(int count)
    {
        GrenadeText.text = "" + count;
    }

    public void UpdateHealth(int newHealth)
    {
        if (my_audio.clip != null && healthSlider.value > newHealth)
            my_audio.Play();

        healthSlider.value = newHealth;
        if (healthSlider.value <= 0)
        {
            dead = true;
            DamagePanel.color = Color.clear;
            StartCoroutine("Died");
            StartCoroutine("TempWinLose");
        }
        damageFlash = true;
        
    }

    IEnumerator Died()
    {
        for(float i = 0; i < 1; i += 0.05f)
        {
            Color c = YouDiedText.color;
            c.a = i;
            YouDiedText.color = c;
            yield return new WaitForSeconds(.175f);
        }
    }


    public void ShowOptionsMenu()
    {
        OptionsMenu.SetActive(true);
    }
    public void HideOptionsMenu()
    {
        OptionsMenu.SetActive(false);
    }

    public void ChangeSFXVolume()
    {
        GameManager.ChangeSFXVolume();
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }
    public void ChangeMusicVolume()
    {
        GameManager.ChangeMusicVolume();
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    public void AutoReloadToggleChange()
    {
        PlayerPrefs.SetInt("AutoReloadToggle", autoReloadToggle.isOn ? 1 : 0);
    }

    public void WalkingAnimToggleChange()
    {
        PlayerPrefs.SetInt("WalkingAnimToggle", walkingAnimToggle.isOn ? 1 : 0);
    }

    public void NewHighScore()
    {
        NewHighScoreText.SetActive(true);
        if(System.Int32.Parse(ScoreText.text) > PlayerPrefs.GetInt("HighScore",0))
        {
            PlayerPrefs.SetInt("HighScore", System.Int32.Parse(ScoreText.text));
            NewHighScoreText.GetComponent<Text>().text =
                "New HighScore: " + System.Int32.Parse(ScoreText.text);
        }
        else
        {
            NewHighScoreText.GetComponent<Text>().text =
                "Your score: " + System.Int32.Parse(ScoreText.text);
        }
    }

    //moved from UIManager
    //Reloads the Level
    public void Restart()
    {
        //Application.LoadLevel(Application.loadedLevel);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("scene name");
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene 2");
    }

    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        Time.timeScale = 0;
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
        GameManager.PausePlayer();
    }

    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        Time.timeScale = 1;
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
        GameManager.UnpausePlayer();
    }

    //loads inputted level
    public void LoadLevel(string level)
    {
        Application.LoadLevel(level);
    }

    public void exitGame()
    {
        Application.Quit(); //NOTE; will only work on build, not in editor
    }
}
