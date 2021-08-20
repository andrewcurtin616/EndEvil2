using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject AmmoPrefab;
    GameObject currentSpawn;
    public bool safetySwitch;
    // Start is called before the first frame update
    void Start()
    {
        if (AmmoPrefab == null)
            Debug.Log("!Warning! AmmoSpawner is empty");
        StartCoroutine("SpawnAmmo");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnAmmo()
    {
        float spawnTime = 65;
        yield return new WaitForSeconds(spawnTime);
        currentSpawn = Instantiate(AmmoPrefab, Vector3.up * 1.5f, Quaternion.identity);
        while (!safetySwitch)
        {
            if (currentSpawn == null)
            {
                spawnTime = Random.Range(45, 60);
                yield return new WaitForSeconds(spawnTime);
                currentSpawn = Instantiate(AmmoPrefab, Vector3.up * 1.5f, Quaternion.identity);
            }
            yield return null;
        }
    }
}
