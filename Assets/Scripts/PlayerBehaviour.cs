using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerBehaviour : MonoBehaviour
{
    private const bool ShouldDrawPath = false;
    
    private NavMeshAgent _navMeshAgent;
    private bool _isInShootingMode;
    private bool _isInShootingZone;

    [SerializeField] private LayerMask levelMask;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Movement(); 
        DrawPath();
        
    }

    private void Movement()
    {
        if (_isInShootingMode)
            return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, levelMask))
            {
                _navMeshAgent.SetDestination(raycastHit.point);
            }
        }

        if (_navMeshAgent.remainingDistance == 0 && _isInShootingZone)
            EnableShootingMode();
    }

    private void DrawPath()
    {
        if (!ShouldDrawPath)
            return;
        for (int i = 1; i < _navMeshAgent.path.corners.Length; i++)
        {
            Debug.DrawLine(_navMeshAgent.path.corners[i], _navMeshAgent.path.corners[i-1], Color.blue);
        }
    }

    private void EnableShootingMode()
    {
        _isInShootingMode = true;
        Debug.Log("(　-_･) ︻デ═一");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(1);
        if (other.CompareTag("Shooting Zone"))
        {
            _isInShootingZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shooting Zone"))
        {
            _isInShootingZone = false;
        }
    }
}
