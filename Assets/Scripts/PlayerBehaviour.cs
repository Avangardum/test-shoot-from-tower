using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PlayerBehaviour : MonoBehaviour
{
    private const bool ShouldDrawPath = false;
    
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    private Vector3 _previousPosition;
    private bool _isPreviousPositionDetermined;
    private Vector3 _previousEulerAngles;
    private bool _arePreviousEulerAnglesDetermined;
    
    private bool _isInShootingMode;
    private bool _isInShootingZone;
    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int TurnSpeedHash = Animator.StringToHash("Turn speed");
    private static readonly int IsMovingHash = Animator.StringToHash("Is moving");

    private float _currentShootingCooldown;

    [SerializeField] private LayerMask levelMask;
    [SerializeField] private LayerMask shootableMask;
    [SerializeField] private float shootingCooldown = 1;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletStartingPoint;
    [SerializeField] private float bulletSpeed;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement(); 
        DrawPath();
        Shooting();
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

        float speed = _isPreviousPositionDetermined ? (transform.position - _previousPosition).magnitude / Time.deltaTime : 0;
        float turnSpeed = _arePreviousEulerAnglesDetermined ? (transform.eulerAngles.y - _previousEulerAngles.y) / Time.deltaTime : 0;
        _animator.SetFloat(SpeedHash, speed / _navMeshAgent.speed);
        _animator.SetFloat(TurnSpeedHash, turnSpeed / _navMeshAgent.angularSpeed);
        _animator.SetBool(IsMovingHash, speed > 0 || Mathf.Abs(turnSpeed) > 0);
        
        if (_navMeshAgent.remainingDistance == 0 && _isInShootingZone)
            EnableShootingMode();

        _previousPosition = transform.position;
        _isPreviousPositionDetermined = true;
        _previousEulerAngles = transform.eulerAngles;
        _arePreviousEulerAnglesDetermined = true;
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

    private void Shooting()
    {
        if (!_isInShootingMode)
            return;

        if (Input.GetKey(KeyCode.Mouse0) && _currentShootingCooldown == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, shootableMask))
            {
                Vector3 target = raycastHit.point;
                
                _currentShootingCooldown = shootingCooldown;
                Debug.Log("BANG!");

                GameObject bullet = Instantiate(bulletPrefab, bulletStartingPoint.position, Quaternion.identity);
                bullet.transform.LookAt(target);
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
            }
        }

        _currentShootingCooldown -= Time.deltaTime;
        if (_currentShootingCooldown < 0)
        {
            _currentShootingCooldown = 0;
        }
    }

    private void EnableShootingMode()
    {
        _isInShootingMode = true;
        Debug.Log("(　-_･) ︻デ═一");
        _animator.SetFloat(SpeedHash, 0);
        _animator.SetFloat(TurnSpeedHash, 0);
        _animator.SetBool(IsMovingHash, false);
    }

    private void OnTriggerEnter(Collider other)
    {
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
