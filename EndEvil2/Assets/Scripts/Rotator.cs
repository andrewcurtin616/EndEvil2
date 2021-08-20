using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up, 0.75f,Space.World);
        transform.RotateAround(Vector3.zero, Vector3.up, -rotateSpeed);
        //transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + 0.5f, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.up*transform.position.y, transform.position.z);
    }
}
