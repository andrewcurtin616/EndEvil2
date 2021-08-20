using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base weapon data and behavior
/// Every weapon is derived from this class
/// </summary>

public class BaseWeapon : MonoBehaviour
{
    public int damage;
    public float fireRate;
    public float reloadSpeed;
    public int currentAmmo;
    public int totalAmmo;
    public int maxAmmo;
    public int clipSize;
    float lastFire;
    public enum FiringType
    {
        Single, Burst, Automatic
    }
    public FiringType fireType;
    public bool hasSpread; //shotguns and such
    //protected float accuracy; //size of recticle and weapon spread

    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip outOfAmmoSound;
    AudioSource audioSource;

    public GameObject impactParticle;
    public GameObject impactEnemyParticle;
    public ParticleSystem muzzleFlash;

    public Vector3 relativePos = Vector3.right + Vector3.forward;

    PlayerController player;
    //GameObject weaponModel;

    //power up checks?

    Animator my_animation;

    GameManagerController GameManager;

    private void Awake/*Start*/()
    {
        /*damage = 1;
        fireRate = 1;
        currentAmmo = 25;
        totalAmmo = 100;
        maxAmmo = 100;
        clipSize = 25;*/
        //totalAmmo = maxAmmo;
        if (totalAmmo == 0)
            totalAmmo = currentAmmo * 2;
        currentAmmo = clipSize;
        lastFire = -fireRate;
        if (reloadSpeed == 0)
            reloadSpeed = fireRate;

        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (player == null)
            Debug.Log("!Warning! Could not find player");
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.Log("!Warning! BaseWeapon: " + gameObject.name + ", requires Audio Source Component");
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        if (muzzleFlash == null)
            Debug.Log("!Warning! Muzzle Flash not set on: " + gameObject.name +" Baseweapon");
        my_animation = GetComponent<Animator>();
        if(my_animation == null)
            Debug.Log("!Warning! BaseWeapon: " + gameObject.name + ", requires Animator Component");

        GameManager = GameManagerController.getInstance();
        if (audioSource != null)
            audioSource.volume = GameManager.SFXVolumeSliderValue();
    }

    public void Fire()
    {
        if (lastFire + fireRate > Time.time ||
            (my_animation  !=null && !my_animation.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
            return;

        if(currentAmmo == 0)
        {
            if (outOfAmmoSound != null)
            {
                audioSource.PlayOneShot(outOfAmmoSound);

                if (GameManager.AutoReloadToggleOn())
                {
                    lastFire = Time.time;
                    Invoke("Reload", 0.5f);
                }
            }
            return;
        }

        CastShot();

        if (fireType == FiringType.Automatic)
        {
            StartCoroutine("AutoFire");
        }

        if (fireType == FiringType.Burst)
        {
            StartCoroutine("BurstFire");
        }

        //lastFire = Time.time;
    }

    void CastShot()
    {
        //calculate rays based on accuracy
        //cast rays and message zombie if hit
        //create spark on surface and flash on muzzle?
        //play a "rock back" animation on gun with speed based on fire rate?

        currentAmmo--;
        GameManager.UpdateAmmo(currentAmmo, totalAmmo);

        if(muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        if (my_animation != null)
        {
            //my_animation.speed = 1 / fireRate;
            //if burst then [my_animation.speed = 1 / 0.05f;] use burst fire rate
            my_animation.speed = fireType == FiringType.Burst ? 1 / 0.05f : 1 / fireRate;
            my_animation.SetTrigger("Fire");
        }

        RaycastHit hit;
        //Physics.Raycast(player.transform.position, player.transform.forward, out hit);
        if (Physics.Raycast(player.viewCamera.transform.position, player.viewCamera.transform.forward, out hit))
        {
            if (hit.collider.name == "UndeadTest1")
            {
                Debug.Log("Hit Zombie");
                GameManager.UpdateScore(damage);
            }

            if (hit.collider.tag == "Enemy")
            {
                Debug.Log("Calling undead hit");
                //check for kill, (using fives for now instead of damage)
                if (hit.collider.gameObject.GetComponent<Enemy>().enemyHitPoints <= 5)
                    GameManager.UpdateScore(15);
                hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                GameManager.UpdateScore(damage);
                GameObject temp = GameObject.Instantiate(impactEnemyParticle);
                temp.transform.position = hit.point;
                temp.transform.forward = hit.normal;
                Destroy(temp, 1.5f);
            }

            else if (impactParticle != null)
            {
                GameObject temp = GameObject.Instantiate(impactParticle);
                temp.transform.position = hit.point;
                temp.transform.forward = hit.normal;
                Destroy(temp, 1.5f);
            }
        }//end initial cast

        if (hasSpread)
            for(int i = 0; i < 4; i++)
            {
                float randX = Random.Range(-2,2);
                float randY = Random.Range(-2, 2);
                if (Physics.Raycast(player.viewCamera.transform.position,
                    player.viewCamera.transform.forward + Vector3.right*randX/50+Vector3.up*randY/50,
                    out hit))
                {
                    if (hit.collider.name == "UndeadTest1")
                    {
                        Debug.Log("Hit Zombie");
                        GameManager.UpdateScore(damage);
                    }

                    if (hit.collider.tag == "Enemy")
                    {
                        Debug.Log("Calling undead hit");
                        //check for kill, (using fives for now instead of damage)
                        if (hit.collider.gameObject.GetComponent<Enemy>().enemyHitPoints <= 5)
                            GameManager.UpdateScore(15);
                        hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                        GameManager.UpdateScore(damage);
                        GameObject temp = GameObject.Instantiate(impactEnemyParticle);
                        temp.transform.position = hit.point;
                        temp.transform.forward = hit.normal;
                        Destroy(temp, 1.5f);
                    }

                    else if (impactParticle != null)
                    {
                        GameObject temp = GameObject.Instantiate(impactParticle);
                        temp.transform.position = hit.point;
                        temp.transform.forward = hit.normal;
                        Destroy(temp, 1.5f);
                    }
                }
            }//end hasSpread for loop

        lastFire = Time.time;
        //Debug.Log("Fired");
    }

    IEnumerator AutoFire()
    {
        int i = 0;//<-safety (-_-)7
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForSeconds(fireRate);

            if (currentAmmo == 0)
            {
                if (outOfAmmoSound != null)
                {
                    audioSource.PlayOneShot(outOfAmmoSound);
                    if (GameManager.AutoReloadToggleOn())
                    {
                        lastFire = Time.time;
                        Invoke("Reload", 0.5f);
                    }
                }
                break;
            }
            i++;
            if (i > 1000)
                break;

            CastShot();
        }

        StopCoroutine("AutoFire");
    }

    IEnumerator BurstFire()
    {

        int i = 0;//<-safety (-_-)7
        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            if (currentAmmo == 0)
            {
                if (outOfAmmoSound != null)
                {
                    audioSource.PlayOneShot(outOfAmmoSound);
                    if (GameManager.AutoReloadToggleOn())
                    {
                        lastFire = Time.time;
                        Invoke("Reload", 0.5f);
                    }
                }
                break;
            }
            i++;
            if (i > 3)
                break;

            CastShot();
        }

        StopCoroutine("BurstFire");
    }

    public void Reload()
    {

        /*************************TESTING ONLY**************************/

        /*if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        currentAmmo = clipSize;
        //GameManager.UpdateAmmo(currentAmmo, totalAmmo);
        return;*/

        /*************************TESTING ONLY**************************/

        

        if (totalAmmo <= 0 || currentAmmo >= clipSize ||
            (my_animation != null && !my_animation.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
            return;

        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        if (my_animation != null)
        {
            my_animation.speed = 1/reloadSpeed;
            CancelWeapon();
            my_animation.SetTrigger("Reload");
        }

        if (totalAmmo >= clipSize)
        {
            totalAmmo -= (clipSize - currentAmmo);
            currentAmmo = clipSize;
        }
        else
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }

        GameManager.UpdateAmmo(currentAmmo, totalAmmo);
    }

    public void AddAmmo()
    {
        //totalAmmo += (int)(maxAmmo / 2.5f);
        totalAmmo += (int)(clipSize * 1.25f);
        if (totalAmmo > maxAmmo)
            totalAmmo = maxAmmo;

        //update ammo UI only if this gun is out
        if (transform.position != Vector3.up * -10)
            GameManager.UpdateAmmo(currentAmmo, totalAmmo);

        if (reloadSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }

    public void CancelWeapon()
    {
        StopCoroutine("AutoFire");
        StopCoroutine("BurstFire");
        //play switch out here?
    }

    public void SwitchIn()
    {
        if (my_animation == null)
            return;

        //may need to increase this
        my_animation.speed = 1f;
        my_animation.CrossFade("SwitchIn", 0, 0);
    }
}
