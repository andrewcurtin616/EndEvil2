using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls player
/// Currently requires rigidbody with xyz rotation constrained
/// Camera is attached to player object as a child
/// </summary>

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 1; //5
    public float rotSpeedX = 1; //3
    public float rotSpeedY = 1; //3
    public float upperVLimit = 25; //25
    public float lowerVLimit = -25;
    float verticalCounter;
    [HideInInspector]
    public Camera viewCamera;
    bool isGrounded;
    public float jumpForce; //225
    bool canMove;
    float sprintEnergy;
    bool isSprinting;
    bool isAiming;
    float jumpTimer;
    float grenadeTimer;

    GameManagerController GameManager;
    public List<BaseWeapon> weapons = new List<BaseWeapon>();
    BaseWeapon currentWeapon;
    //reference to child "FPSMarker" for weapon model and animations
    public GameObject grenade;
    [HideInInspector]
    public int grenadeCount = 5;

    [HideInInspector]
    public int health = 10;

    bool walkingAnimFlag = false;
    float walkingAnimValue = 0.005f;
    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip aimSound;
    public AudioClip deathSound;

    public bool stopControl;
    public bool walkingAnimation = true;

    // Start is called before the first frame update
    void /*Start*/Awake()
    {
        rb = GetComponent<Rigidbody>();
        viewCamera = Camera.main;

        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined; // keep confined in the game window
        Cursor.lockState = CursorLockMode.Locked; // keep confined to center of screen

        canMove = true;
        sprintEnergy = 100;

        if (weapons.Count > 0)
        {
            currentWeapon = weapons[0];
            currentWeapon.transform.SetParent(viewCamera.transform);
            currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            currentWeapon.transform.localPosition = currentWeapon.relativePos;
        }
        else
            Debug.Log("!Warning! no weapons set");

        GameManager = GameManagerController.getInstance();
        GameManager.SetPlayer(GetComponent<PlayerController>());
        

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = walkSound;
        //Time.timeScale = 0.05f; //for testing
    }

    private void Start()
    {
        if (currentWeapon != null)
            GameManager.UpdateAmmo(currentWeapon.currentAmmo, currentWeapon.totalAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        if (stopControl)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if ((h != 0 || v != 0) && canMove)
        {
            if (!isAiming) //movement animation
            {
                if (/*walkingAnimation*/GameManager.WalkingAnimToggleOn()) //able to turn on and off
                {
                    if (walkingAnimFlag)
                    {
                        viewCamera.transform.localPosition += Vector3.up * walkingAnimValue;
                        if (viewCamera.transform.localPosition.y >= 0.55)
                            walkingAnimFlag = false;
                    }
                    else
                    {
                        viewCamera.transform.localPosition += Vector3.down * walkingAnimValue;
                        if (viewCamera.transform.localPosition.y <= 0.45)
                            walkingAnimFlag = true;
                    }
                }
            
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }

            RaycastHit hit; //for slopes
            Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f);
            Vector3 temp;
            if (isSprinting && v > 0)
            {
                temp = transform.forward * v * speed * 2f + transform.right * h * speed;
                walkingAnimValue = 0.0075f;
                if (audioSource.clip != runSound)
                    audioSource.clip = runSound;
            }
            else
            {
                temp = transform.forward * v * speed + transform.right * h * speed;
                walkingAnimValue = 0.005f;
                if (audioSource.clip != walkSound)
                    audioSource.clip = walkSound;
            }

            rb.velocity = new Vector3(temp.x, rb.velocity.y, temp.z);
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, hit.normal); //<-handles slopes
        }
        else if (canMove)
        {
            rb.velocity = Vector3.up * rb.velocity.y;
            if (audioSource.isPlaying && !isAiming)
                audioSource.Stop();
        }


        //horizontal, controls both camera and player
        if(Input.GetAxis("Mouse X") != 0)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotSpeedX);
        }

        //vertical, with limiters
        //[INVERSE]
        /*if ((Input.GetAxis("Mouse Y") > 0 && verticalCounter < upperVLimit) ||
            (Input.GetAxis("Mouse Y") < 0 && verticalCounter > lowerVLimit))
        {
            viewCamera.transform.Rotate(Vector3.right * rotSpeedY * Input.GetAxis("Mouse Y"), Space.Self);
            verticalCounter += Input.GetAxis("Mouse Y");
        }*/

        if ((Input.GetAxis("Mouse Y") < 0 && verticalCounter < upperVLimit) ||
            (Input.GetAxis("Mouse Y") > 0 && verticalCounter > lowerVLimit))
        {
            viewCamera.transform.Rotate(Vector3.right * rotSpeedY * -Input.GetAxis("Mouse Y"), Space.Self);
            verticalCounter -= Input.GetAxis("Mouse Y");
        }

        //Debug.Log(verticalCounter);


        //jumping
        if (!isGrounded && jumpTimer+0.25f<Time.time)
            if (checkGround())
                canMove = true;

        if (Input.GetKeyDown(KeyCode.Space) && checkGround() && !isAiming)
        {
            isGrounded = false;
            canMove = false;
            rb.velocity /= 2;
            rb.AddForce(Vector3.up * jumpForce);
            jumpTimer = Time.time;
            if (audioSource.isPlaying)
                audioSource.Stop();
        }


        //Aim down sights
        if (Input.GetMouseButtonDown(1))
        {
            speed = 2.5f;
            StartCoroutine("StartAim");
            isAiming = true;
            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.PlayOneShot(aimSound);
            //increase weapon accuracy
            //shrink crosshair
        }
        if (Input.GetMouseButtonUp(1) || (viewCamera.fieldOfView != 60 && !Input.GetMouseButton(1)))
        {
            speed = 5;
            StartCoroutine("StopAim");
            isAiming = false;
            //decrease weapon accuracy
            //expand crosshair
        }


        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isAiming)
        {
            isSprinting = true;
            //speed = 10;
        }
        if (isSprinting)
        {
            if (v > 0)
                sprintEnergy -= 0.2f;
            if (sprintEnergy <= 0 || isAiming)
            { isSprinting = false; speed = 5; }
        }
        if (!isSprinting && sprintEnergy != 100)
            sprintEnergy += 0.4f;
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            speed = 5;
        }


        //firing
        //Use mouse down instead, then check for hold in base weapon?
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Firing");
            currentWeapon.Fire();
            //cancel sprint, have Fire() return bool, if true then cancel sprint?
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(2))
        {
            currentWeapon.Reload();
            
            //same here? have Reload() return bool, if true then cancel sprint?
            /*if (isSprinting) 
            { isSprinting = false; speed = 5; }*/
        }


        //Switching weapons
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel") > 0)
            && weapons.Count > 1) 
        {
            if (weapons.Count - 1 == weapons.IndexOf(currentWeapon))
            { SwitchWeapons(0); }

            else
            { SwitchWeapons(weapons.IndexOf(currentWeapon) + 1); }

            //inform UI thourhg GM
            //GameManager.UpdateAmmo(currentWeapon.currentAmmo, currentWeapon.totalAmmo);
        }
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") < 0)
            && weapons.Count > 1)
        {
            if (0 == weapons.IndexOf(currentWeapon))
            { SwitchWeapons(weapons.Count - 1); }

            else
            { SwitchWeapons(weapons.IndexOf(currentWeapon) - 1); }

            //inform UI thourhg GM
            //GameManager.UpdateAmmo(currentWeapon.currentAmmo, currentWeapon.totalAmmo);
        }


        //grenade
        //spawn grenade and pass in camera forward vector for arc
        if (Input.GetKeyDown(KeyCode.G) && grenade != null && grenadeCount > 0 &&
            grenadeTimer+0.25f < Time.time)
        {
            Instantiate(grenade, transform.position+transform.forward,
                viewCamera.transform.rotation);
            grenadeCount--;
            GameManager.UpdateGrenades(grenadeCount);
            grenadeTimer = Time.time;
        }

        //melee
        /*Never added*/
    }

    bool checkGround()
    {
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.01f);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.25f);

        return isGrounded;
    }

    IEnumerator StartAim()
    {
        int i = 0;
        while (true)
        {
            if (viewCamera.fieldOfView > 30)
            {
                viewCamera.fieldOfView -= 2;
                currentWeapon.transform.localPosition -= Vector3.right * 0.05f;
            }
            else
            {
                viewCamera.fieldOfView = 30;
                currentWeapon.transform.localPosition = new Vector3(0,
                    currentWeapon.transform.localPosition.y, currentWeapon.transform.localPosition.z);
                break;
            }
            i++;
            if (i > 100 || !Input.GetMouseButton(1))
                break;
            yield return null;
        }
        StopCoroutine("StartAim");
    }
    IEnumerator StopAim()
    {
        int i = 0;
        while (true)
        {
            if (viewCamera.fieldOfView < 60)
            {
                viewCamera.fieldOfView += 3;
                currentWeapon.transform.localPosition += Vector3.right * 0.075f;
            }
            else
            {
                viewCamera.fieldOfView = 60;
                currentWeapon.transform.localPosition = new Vector3(0.75f,
                    currentWeapon.transform.localPosition.y, currentWeapon.transform.localPosition.z);
                break;
            }
            i++;
            if (i > 100 /*|| Input.GetMouseButton(1)*/)
                break;
            yield return null;
        }
        StopCoroutine("StopAim");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "AmmoPickupTest1" || other.gameObject.name[0]=='A')
        {
            other.gameObject.GetComponent<AudioSource>().Play();
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            other.gameObject.GetComponent<SphereCollider>().enabled = false;
            Destroy(other.gameObject, 1);
            foreach (BaseWeapon weapon in weapons)
                weapon.totalAmmo = weapon.maxAmmo;

            if (grenadeCount > 6)
                grenadeCount = 8;
            grenadeCount += 2;
            GameManager.UpdateAmmo(currentWeapon.currentAmmo, currentWeapon.totalAmmo);
            GameManager.UpdateGrenades(grenadeCount);
        }

        if(other.gameObject.name == "DamageTest1")
        {
            TakeDamage(8, other.transform.position);
        }

        if (other.gameObject.name == "HealthPickupTest1" || other.gameObject.name[0] == 'H')
        {
            other.gameObject.GetComponent<AudioSource>().Play();
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            other.gameObject.GetComponent<SphereCollider>().enabled = false;
            Destroy(other.gameObject, 1);
            if (health > 80)
                health = 100;
            else
                health += 20;
            GameManager.UpdateHealth();
        }
    }

    public void SwitchWeapons(int indexOfNext)
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.SetParent(null);
            currentWeapon.transform.position = Vector3.up * -10;
        }

        currentWeapon.CancelWeapon();

        currentWeapon = weapons[indexOfNext];
        currentWeapon.transform.SetParent(viewCamera.transform);
        currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentWeapon.transform.localPosition = currentWeapon.relativePos;

        currentWeapon.SwitchIn();Debug.Log("Weapon Switched");
        GameManager.UpdateAmmo(currentWeapon.currentAmmo, currentWeapon.totalAmmo);

        //Switching weapons should cancel aiming
        StopCoroutine("StartAim");
        speed = 5;
        StartCoroutine("StopAim");
        isAiming = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        GameManager.UpdateHealth();

        if (health <= 0)
        {
            Dead();
        }
    }

    public void TakeDamage(int damage, Vector3 attackPos)
    {
        if (stopControl)
            return;
        health -= damage;
        //rock camera from attackPos
        //use localRotation, to not mess with walning anim
        //use coroutine
        //StartCoroutine("DamageRock", attackPos);
        rb.AddForce(transform.position - attackPos, ForceMode.Impulse);
        GameManager.UpdateHealth();
        if (health <= 0)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            if (deathSound != null)
                audioSource.PlayOneShot(deathSound);
            if (deathSound != null)
                Debug.Log("Yes");
            else
                Debug.Log("No");
            Dead();
        }
    }

    void Dead()
    {
        stopControl = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GetComponent<CapsuleCollider>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;
        
        
        StartCoroutine("DeathAnim");

        GameObject temp = Instantiate(currentWeapon.gameObject.GetComponentInChildren<MeshRenderer>().gameObject,
            currentWeapon.transform.position, currentWeapon.transform.rotation);
        temp.transform.localScale = Vector3.one;
        temp.AddComponent<MeshCollider>().convex = true;
        temp.AddComponent<Rigidbody>().AddForce((Vector3.up+transform.forward)*50f);
        temp.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 50f);
        Destroy(currentWeapon.gameObject);

        //tell gm we dead
        GameManager.GameOver();
    }

    IEnumerator DeathAnim()
    {
        int i = 0;
        while (true)
        {
            transform.Rotate(transform.forward, -0.8f);
            //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.forward * -90, Time.deltaTime);
            i++;

            if (viewCamera.fieldOfView < 70)
                viewCamera.fieldOfView += 2;
            else
                viewCamera.fieldOfView = 70;

            yield return null;
            if (i >= 100 || transform.localEulerAngles.z <=-90)
                break;
        }
    }


    void DisplayWeaponTest()
    {
        GameObject temp = Instantiate(currentWeapon.gameObject.GetComponentInChildren<MeshRenderer>().gameObject,
            currentWeapon.transform.position, currentWeapon.transform.rotation);

        
    }

}
