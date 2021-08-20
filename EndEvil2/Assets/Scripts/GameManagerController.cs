using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mediates communication between all single instance scripts
/// Every single instance script has a reference to this
/// </summary>

public class GameManagerController
{
    public static GameManagerController instance = null;

    PlayerController player;
    UIController userInterface;
    UndeadManager undeadManager;

    public static GameManagerController getInstance()
    {
        if (instance == null)
            instance = new GameManagerController();

        return instance;
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
        if (this.player != null)
            Debug.Log("player set");
        else
            Debug.Log("Error: player not set");
    }

    public void SetUserInterface(UIController userInterface)
    {
        this.userInterface = userInterface;
        if (this.userInterface != null)
            Debug.Log("userInterface set");
        else
            Debug.Log("Error: userInterface not set");
    }

    public void SetUndeadManager(UndeadManager undeadManager)
    {
        this.undeadManager = undeadManager;
        if (this.undeadManager != null)
            Debug.Log("undeadManager set");
        else
            Debug.Log("Error: undeadManager not set");
    }


    /*** Here we can add calls to and from scripts ***/
    public Vector3 GetPlayerPos()
    {
        return player.transform.position;
    }

    public void DamagePlayer(int damage)
    {
        player.TakeDamage(damage);
    }

    public void UpdateHealth()
    {
        userInterface.UpdateHealth(player.health);
    }

    public void UpdateAmmo(int currentAmmo, int totalAmmo)
    {
        userInterface.UpdateAmmo(currentAmmo, totalAmmo);
    }
    public void UpdateGrenades(int count)
    {
        userInterface.UpdateGrenades(count);
    }
    public void UpdateScore(int points)
    {
        userInterface.UpdateScore(points);
    }


    public void PausePlayer()
    {
        if (player == null)
            return;
        player.stopControl = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.audioSource.Stop();

        GameObject.Find("MusicHolder").GetComponent<AudioSource>().volume = 0.1f *MusicVolumeSliderValue();
        GameObject.Find("MusicHolder").GetComponent<AudioSource>().pitch = 0.9f;
    }
    public void UnpausePlayer()
    {
        if (player == null)
            return;
        player.stopControl = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameObject.Find("MusicHolder").GetComponent<AudioSource>().volume = 0.25f* MusicVolumeSliderValue();
        GameObject.Find("MusicHolder").GetComponent<AudioSource>().pitch = 1f;

    }

    public bool AutoReloadToggleOn()
    {
        return userInterface.autoReloadToggle.isOn;
    }
    public bool WalkingAnimToggleOn()
    {
        return userInterface.walkingAnimToggle.isOn;
    }
    public float SFXVolumeSliderValue()
    {
        return userInterface.sfxVolumeSlider.value;
    }
    public float MusicVolumeSliderValue()
    {
        return userInterface.musicVolumeSlider.value;
    }
    public void ChangeSFXVolume()
    {
        foreach(AudioSource audioSource in GameObject.FindObjectsOfType<AudioSource>())
        {
            if (audioSource.gameObject.name == "MusicHolder")
                continue;
            if (audioSource.gameObject.name == "Player")
                audioSource.volume = 0.25f * SFXVolumeSliderValue();
            else
                audioSource.volume = SFXVolumeSliderValue();
        }
    }
    public void ChangeMusicVolume()
    {
        GameObject.Find("MusicHolder").GetComponent<AudioSource>().volume = 0.1f * MusicVolumeSliderValue();
    }

    public void EndGameWin()
    {
        PausePlayer();

        foreach(Spawner spawner in GameObject.Find("Spawners").GetComponentsInChildren<Spawner>())
        {
            spawner.safetySwitch = true;
        }

        foreach(Enemy enemy in GameObject.FindObjectsOfType<Enemy>())
        {
            enemy.TakeDamage(5);
            enemy.TakeDamage(5);
            enemy.TakeDamage(5);
            enemy.TakeDamage(5);
            enemy.TakeDamage(5);
        }

        PlayerPrefs.SetInt("HighScore", System.Int32.Parse(userInterface.ScoreText.text));

        //New HighScore
        userInterface.NewHighScore();
        //tell player they unlocked endless mode
        userInterface.EndlessModeText.SetActive(true);

    }

    public void GameOver()
    {
        PausePlayer();

        foreach (Spawner spawner in GameObject.Find("Spawners").GetComponentsInChildren<Spawner>())
        {
            spawner.safetySwitch = true;
        }

        //New HighScore or Your Score
        userInterface.NewHighScore();

    }
}
