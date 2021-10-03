using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyTest : MonoBehaviour
{
    public int health = 100;
    public GameObject impactParticle;

    bool onFire;
    float fireTime;
    bool slowed;
    float slowTime;

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (onFire)
        {

        }
    }

    public void TakeDamage(int damage, int damageType, RaycastHit hit)
    {
        health -= damage;
        if(impactParticle != null && damageType == 0)
        {
            GameObject temp = Instantiate(impactParticle);
            temp.transform.position = hit.point;
            temp.transform.forward = hit.normal;
            Destroy(temp, 1.5f);
        }
        if (damageType == 1)
        {
            StartCoroutine("HellFire");

        }
        else if (damageType == 2)
            StartCoroutine("BoneChill");
        else if (damageType == 3)
            StartCoroutine("SoulSteal");
    }

    IEnumerator HellFire()
    {
        if (!onFire)
        {
            onFire = true;
            fireTime = Time.time + 0.75f;
            while (Time.time < fireTime)
            {
                health -= 5;
                yield return new WaitForSeconds(0.5f);
            }
            onFire = false;
        }
        else if(Time.time + 5f > fireTime)
        {
            fireTime += 1.5f;
        }
    }

    IEnumerator BoneChill()
    {
        if (!slowed)
        {
            slowed = true;
            //change speed
            slowTime = Time.time + 1.25f;
            while (Time.time < slowTime)
            {
                yield return null;
            }
            slowed = false;
            //return speed to normal
        }
        else if (Time.time + 5f > slowTime)
        {
            slowTime += 0.75f;
        }
    }

    IEnumerator SoulSteal()
    {
        yield return null;
    }
}
