using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /// <summary>
    /// A basic spawner script for the Alpha
    /// </summary>

    public List<GameObject> enemies = new List<GameObject>();
    public bool safetySwitch = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnEnemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time == 300)// no longer than 5 minutes for testing
            safetySwitch = true;
    }

    IEnumerator SpawnEnemy()
    {
        float randTime = 5;
        randTime = Random.Range(5, 30);
        yield return new WaitForSeconds(randTime);
        while (!safetySwitch)
        {
            randTime = Random.Range(5, 30);
            if (PlayerPrefs.GetInt("HighScore") >= 500)
                randTime = Random.Range(2, 18);

            /*//check if spawnPos is occupied, if space found, spawn, otherwise skip
            Vector3 spawnPos = transform.position-Vector3.right+Vector3.forward;
            bool breakFlag = false;
            //0 3 6
            //1 4 7
            //2 5 8
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (Physics.OverlapSphere(spawnPos + Vector3.right * i-Vector3.forward*j, 1).Length <= 1)
                    {
                        spawnPos = spawnPos + Vector3.right * i;
                        breakFlag = true;
                        break;
                    }
                }
                if (breakFlag)
                    break;
            }
            if (breakFlag)
            {
                GameObject temp =
                Instantiate(enemies[Random.Range(0, enemies.Count)], spawnPos, Quaternion.identity);

                if (temp.GetComponent<AudioSource>() != null)
                    temp.GetComponent<AudioSource>().volume = GameManagerController.getInstance().SFXVolumeSliderValue();
            }*/

            //spawn if the space is free (simpler than above)
            if (Physics.OverlapSphere(transform.position,1).Length <=1)
            {
                GameObject temp =
                Instantiate(enemies[Random.Range(0, enemies.Count)], transform.position, Quaternion.identity);

                if (temp.GetComponent<AudioSource>() != null)
                    temp.GetComponent<AudioSource>().volume = GameManagerController.getInstance().SFXVolumeSliderValue();
            }

            yield return new WaitForSeconds(randTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up, 1f);
    }
}
