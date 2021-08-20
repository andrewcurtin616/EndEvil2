using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Animator _animator;
    GameObject _player;

    void Awake()
    {
        _player =
                GameObject.FindGameObjectWithTag("Player");
        _animator = GetComponent<Animator>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            _animator.SetBool("IsNearPlayer", true);
            //_animator.SetBool("Idle", true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _player)
        {
            _animator.SetBool("IsNearPlayer", false);
            //_animator.SetBool("Idle", false);
        }
    }

}