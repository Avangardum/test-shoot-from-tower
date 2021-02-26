using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Die(GameObject shotBodyPart, Vector3 shotPosition, Vector3 shotImpulse)
    {
        _animator.enabled = false;
        shotBodyPart.GetComponent<Rigidbody>().AddForceAtPosition(shotImpulse, shotPosition, ForceMode.Impulse);
    }
}
