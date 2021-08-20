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
    bool temp;
    float temp2;

    LineRenderer line;

    private void Awake()
    {
        //Random spin
        GetComponent<Rigidbody>().AddRelativeTorque
           (Random.Range(500, 1500), 0, Random.Range(500, 1500));
        GameManager = GameManagerController.getInstance();
        line = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //throw, sound, wait to explode
        GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * forwardForce + Vector3.up * upForce);
        GetComponent<AudioSource>().volume = GameManager.SFXVolumeSliderValue();
        if (spawnSound != null)
            GetComponent<AudioSource>().PlayOneShot(spawnSound);
        StartCoroutine("Explode");
    }

    private void Update()
    {
        if (transform.position.y > temp2)
            temp2 = transform.position.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactSound != null)
            GetComponent<AudioSource>().PlayOneShot(impactSound);

        if (!temp)
        {
            Debug.Log(transform.position.z + ":::" + temp2);
            temp = true;
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeAfter); //explode after 3 seconds

        //Raycast downwards to check ground
        RaycastHit checkGround;
        if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50) &&
            explosionPrefab != null)
        {
            //Instantiate metal explosion prefab on ground
            GameObject temp = Instantiate(explosionPrefab, checkGround.point,
                Quaternion.FromToRotation(Vector3.forward, checkGround.normal));
            Destroy(temp, 3);
        }

        if (explosionSound != null)
            GetComponent<AudioSource>().PlayOneShot(explosionSound);

        //Use overlapshere to check for nearby colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            //do something for every 'hit' in the sphere
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

        //Destroy the grenade object on explosion
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        Destroy(gameObject, 3);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        //draw sphere at end point, midpoint at max height, and line from pos to mid to end
        Gizmos.color = Color.green;
        /*Vector3 midPoint = Vector3.zero;
        Vector3 endPoint = Vector3.zero;
        Gizmos.DrawLine(transform.position, midPoint);
        Gizmos.DrawLine(midPoint,endPoint);
        Gizmos.DrawSphere(midPoint, 0.25f);
        Gizmos.DrawSphere(endPoint, 0.25f);*/
        foreach(Vector3 pos in DrawTrajectory())
        {
            Gizmos.DrawSphere(pos, 0.25f);
        }


        //at 1500 forward and 300 up facing forward with no angle, results 29.93961:::2.774872
        // from -10 and 1; giving a height of about 1.75 and distance of about 30

        //heigt is 1 to 5.996618 at up of 500
        //0                                 -  0
        //100 1.059207  ~ 94.410252198      -  0.059207 
        //200 1.769698  ~ 113.01363283      -  0.710491
        //300 2.774872  ~ 108.1130949       -  1.005174
        //400 4.182027  ~ 95.64739778102    -  1.407155
        //500 5.996618  ~ 83.38033204716    -  1.814591
        //600 (8.219168)                    -~ 2.2----- = 2.22255

        //for height, every 100 adds about 0.4?

        //at 1500 forward and 300 up, goes from -10 to about 30 so 40 units forward with no angle
        //and from 29.93961:::2.774872
        //
        //parabola equation y= a(x-h)^2 +k
        //y=a(x-29.93961/2)^2 + 2.774872
        //0=a(0-29.93961/2)^2 + 2.774872
        // a = -2.774872/(29.93961/2)^2
        // a =-2.774872/224.095061738025
        // a= -0.01238256648084428943739591515569


        //max height equation: (v^2*sin^2(angle))/2g
    }

    private List<Vector3> DrawTrajectory()
    {
        List<Vector3> curvePoints = new List<Vector3>();
        curvePoints.Add(transform.position);

        // Initial values for trajectory
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocity = transform.forward*forwardForce+Vector3.up*upForce;

        for(int i = 0; i <200; i++)
        {
            if (currentPosition.y <= 0)
                break;

            float t = .5f / currentVelocity.magnitude;

            currentVelocity = currentVelocity + i * Physics.gravity;
            currentPosition = currentPosition + t * currentVelocity;
            
            curvePoints.Add(currentPosition);
        }
        
        return curvePoints;
    }
}
