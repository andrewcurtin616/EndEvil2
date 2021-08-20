using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    Animator _animator;
    private NavMeshAgent _nav;
    public float dist;
    private Transform _player;
    //GameObject target;
   
    // Start is called before the first frame update
    void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
        _player =
            GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 lookVector = _player.transform.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist >= 10)
        {
        
            
            //  transform.LookAt( _player.position);
        }
        _nav.SetDestination(_player.position);
    }

}
