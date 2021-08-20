using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject target;
    Animator my_animator;
    public float enemyHitPoints = 20;
    float lastAttack = 0f;
    float attackRate = 1.25f;
    public float attackRange = 2.5f;

    UndeadManager undeadManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        my_animator = GetComponent<Animator>();
        undeadManager = UndeadManager.getInstance();
        //undeadManager.numberOfEnemies++;
    }

    private void Update()
    {
        if (!agent.enabled)
            return;

        if (agent.isStopped)
        {
            //always face player while attacking
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

            //if player goes out of range, stop attacking and start chasing
            if (Vector3.Distance(transform.position, target.transform.position) >= attackRange)
            {
                if (agent.isStopped)
                    agent.isStopped = false;
                if (my_animator.GetBool("Attack"))
                    my_animator.SetBool("Attack", false);
               
                //Debug.Log("should animate");
            }

            //if player is in attack range, damage them at a certain rate
            //Note: we could instead ask the animator if it finished or started the attack anim for the attack rate timing
            if (lastAttack + attackRate < Time.time 
                && Vector3.Distance(transform.position, target.transform.position) < attackRange+0.5f)
            {
                //Debug.Log("HitPlayer");
                //target.GetComponent<PlayerController>().TakeDamage(5);
                target.GetComponent<PlayerController>().TakeDamage(5,transform.position);
                lastAttack = Time.time;
            }

        }
        else
        {
            //start moving only when attack anim is finished so we aren't sliding
            if (!my_animator.IsInTransition(0) && !my_animator.GetBool("Attack"))
                GoToTarget();
           
            //if target is in range, stop and start attacking
            if (Vector3.Distance(transform.position, target.transform.position) < attackRange)
            {
                agent.isStopped = true;
                my_animator.SetBool("Attack", true);
            }
        }

        //testing for attack rate based on animation
        /*Debug.Log(my_animator.GetCurrentAnimatorStateInfo(0).IsName("SwingNormal")
            && my_animator.GetCurrentAnimatorStateInfo(0).normalizedTime ==0.5f);*/
        /*if (my_animator.GetCurrentAnimatorStateInfo(0).IsName("SwingNormal")
            && my_animator.GetCurrentAnimatorStateInfo(0).normalizedTime%1>=0.5f
            && my_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 1f)
            Debug.Log("Wow");*/
        


    }//end update

    private void GoToTarget()
    {
        agent.SetDestination(target.transform.position);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange/1.75f);
    }

    public void TakeDamage(int damage)
    {
        enemyHitPoints -= +5;
        Debug.Log(gameObject.name + " has been hit" + enemyHitPoints);
        //when enemy is dead animation activates to isDead
        if (enemyHitPoints == 0)
        {
         
            my_animator.SetBool("isDead", true);
         
            agent.isStopped = true;
            agent.enabled = false;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 15f);
        }
    }
}
