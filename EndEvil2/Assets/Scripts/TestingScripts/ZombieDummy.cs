using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieDummy : BaseEnemyTest
{
    NavMeshAgent agent;
    Transform player;
    Vector3 target;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(player.position);
        
    }
}
