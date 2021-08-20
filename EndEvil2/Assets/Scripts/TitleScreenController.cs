using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour
{

    Button PlayButton;
    Button ControlsButton;
    Button CreditsButton;
    Button SettingsButton;
    Button ExitButton;
    Button BackButton;

    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioClip startSound;
    AudioSource audioSource;

    GameObject ControlsDisplay;
    GameObject CreditsDisplay;
    GameObject SettingsDisplay;

    Image FadePanel;
    AudioSource music;

    bool enterGame;

    Text HighScoreText;
    Text EndlessText;

    GameObject OptionsMenu;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Toggle autoReloadToggle;
    public Toggle walkingAnimToggle;

    // Start is called before the first frame update
    void Start()
    {
        PlayButton = GameObject.Find("PlayButton").GetComponent<Button>();
        ControlsButton = GameObject.Find("ControlsButton").GetComponent<Button>();
        CreditsButton = GameObject.Find("CreditsButton").GetComponent<Button>();
        SettingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
        ExitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        BackButton = GameObject.Find("BackButton").GetComponent<Button>();
        BackButton.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();

        ControlsDisplay = GameObject.Find("ControlsDisplay");
        CreditsDisplay = GameObject.Find("CreditsDisplay");
        ControlsDisplay.SetActive(false);
        CreditsDisplay.SetActive(false);

        FadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
        FadePanel.gameObject.SetActive(false);

        music = GameObject.Find("Ambience").GetComponent<AudioSource>();

        //PlayerPrefs.SetInt("HighScore", 0);

        HighScoreText = GameObject.Find("HighScoreText").GetComponent<Text>();
        HighScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore",0);
        EndlessText = GameObject.Find("EndlessText").GetComponent<Text>();
        if (PlayerPrefs.GetInt("HighScore") < 500)
            EndlessText.gameObject.SetActive(false);


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
        //OptionsMenu.SetActive(false);
        SettingsDisplay = GameObject.Find("SettingsDisplay");
        SettingsDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonClick()
    {
        enterGame = true;
        HideMenu();
        StartCoroutine("EnterExitFade");
        audioSource.PlayOneShot(startSound);
    }
    public void CrontrolsButtonClick()
    {
        HideMenu();
        BackButton.gameObject.SetActive(true);
        audioSource.PlayOneShot(selectSound);
        ControlsDisplay.SetActive(true);
    }

    public void CreditsButtonClick()
    {
        HideMenu();
        BackButton.gameObject.SetActive(true);
        audioSource.PlayOneShot(selectSound);
        CreditsDisplay.SetActive(true);
    }

    public void SettingsButtonClick()
    {
        HideMenu();
        BackButton.gameObject.SetActive(true);
        audioSource.PlayOneShot(selectSound);
        SettingsDisplay.SetActive(true);
    }

    public void ChangeSFXVolume()
    {
        //set sounds in title screen
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume");
    }
    public void ChangeMusicVolume()
    {
        //set music in title screen
        GameObject.Find("Ambience").GetComponent<AudioSource>().volume = 0.25f * musicVolumeSlider.value;
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

    public void ExitButtonClick()
    {
        HideMenu();
        audioSource.PlayOneShot(selectSound);
        StartCoroutine("EnterExitFade");
    }

    void HideMenu()
    {
        PlayButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        CreditsButton.gameObject.SetActive(false);
        SettingsButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);
        
        HighScoreText.gameObject.SetActive(false);
        EndlessText.gameObject.SetActive(false);
    }
    public void ShowMenu()
    {
        //back button
        BackButton.gameObject.SetActive(false);
        ControlsDisplay.SetActive(false);
        CreditsDisplay.SetActive(false);
        SettingsDisplay.SetActive(false);
        
        audioSource.PlayOneShot(selectSound);
        PlayButton.gameObject.SetActive(true);
        ControlsButton.gameObject.SetActive(true);
        CreditsButton.gameObject.SetActive(true);
        SettingsButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);

        HighScoreText.gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("HighScore") >= 500)
            EndlessText.gameObject.SetActive(true);
    }

    public void HoverNoise()
    {
        audioSource.PlayOneShot(hoverSound);
    }

    IEnumerator EnterExitFade()
    {
        int i = 0;
        FadePanel.gameObject.SetActive(true);
        while (true)
        {
            //add some motion
            if (enterGame && Camera.main.fieldOfView < 140)
            {
                Camera.main.fieldOfView++;
                Camera.main.GetComponent<Rotator>().rotateSpeed += 0.01f;
            }

            //fade screen and music
            FadePanel.color = Color.Lerp(FadePanel.color, Color.black, 0.01f);
            music.volume -= 0.00125f;

            i++;
            yield return null;
            if (i > 200)
            {

                break;
            }
        }
        if (enterGame)
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene 2");
        else
            Application.Quit();
        Debug.Log("Hit");
    }
}
