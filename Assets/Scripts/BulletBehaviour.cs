using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifetime = 10;
    [SerializeField] private float impulse = 1;
    
    private int _enemyLayer;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == _enemyLayer)
        {
            other.gameObject.GetComponentInParent<EnemyBehaviour>()
                .Die(other.gameObject, other.GetContact(0).point, transform.forward * impulse);
        }
        Destroy(gameObject);
    }
}
