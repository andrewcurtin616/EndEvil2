using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public float forwardForce;
    public float upForce;
    public float explodeAfter;
    public float explosionRadius;
    public GameObject explosionPrefab;
    public AudioClip spawnSound;
    public AudioClip impactSound;
    public AudioClip explosionSound;
    GameManagerController GameManager;
    [HideInInspector]
    public int count;

    private void Awake()
    {
        GetComponent<Rigidbody>().AddRelativeTorque
           (Random.Range(500, 1500), 0, Random.Range(500, 1500));
        GameManager = GameManagerController.getInstance();
    }

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * forwardForce + Vector3.up * upForce);
        GetComponent<AudioSource>().volume = GameManager.SFXVolumeSliderValue();
        if (spawnSound != null)
            GetComponent<AudioSource>().PlayOneShot(spawnSound);
        StartCoroutine("Explode");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactSound != null)
            GetComponent<AudioSource>().PlayOneShot(impactSound);

        if(collision.gameObject.CompareTag("Enemy") && name[0] == 'S')
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.SetParent(collision.transform);
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeAfter);

        //Raycast downwards to check ground
        /*RaycastHit checkGround;
        if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50) &&
            explosionPrefab != null)
        {
            //Instantiate metal explosion prefab on ground
            GameObject temp = Instantiate(explosionPrefab, checkGround.point,
                Quaternion.FromToRotation(Vector3.forward, checkGround.normal));
            Destroy(temp, 3);
        }*/
        GameObject temp = Instantiate(explosionPrefab, transform.position,
                Quaternion.identity);
        Destroy(temp, 3);

        if (explosionSound != null)
            GetComponent<AudioSource>().PlayOneShot(explosionSound);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.tag == "Enemy")
            {
                Debug.Log("Calling undead hit");
                //check for kill, (using fives for now instead of damage)
                if (hit.gameObject.GetComponent<Enemy>().enemyHitPoints <= 5)
                    GameManager.UpdateScore(15);
                hit.gameObject.GetComponent<Enemy>().TakeDamage(30);
                hit.gameObject.GetComponent<Enemy>().TakeDamage(30);
                hit.gameObject.GetComponent<Enemy>().TakeDamage(30); //temporary for now
                hit.gameObject.GetComponent<Enemy>().TakeDamage(30);
                hit.gameObject.GetComponent<Enemy>().TakeDamage(30);
                GameManager.UpdateScore(20);
            }
        }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        Destroy(gameObject, 3);
    }
}
