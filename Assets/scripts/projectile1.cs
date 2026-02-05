using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile1 : MonoBehaviour
{
    // Optional explicit fire point (e.g. bow). If null, the projectile's current position is used.
    public Transform firePoint;

    // Target to shoot at (set from Archer_fire before instantiation)
    public GameObject target;

    // How long (in seconds) the projectile takes to travel from start to target.
    public float flightDuration = 1.0f;

    // Maximum vertical height (relative to straight line) of the arc.
    public float arcHeight = 2.0f;

    // Optional safety lifetime (seconds) before the projectile is destroyed.
    public float maxLifetime = 5.0f;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _t; // 0 → 1 over the course of the flight
    private float _lifeTimer;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            // We move manually, so disable physical forces.
            _rb.useGravity = true;
            _rb.isKinematic = false;
        }
    }

    private void Start()
    {
        // Determine start position (match ArrowProjectile behaviour)
        _startPos = firePoint != null ? firePoint.position : transform.position;

        // Snapshot the target position at fire time so the projectile flies
        // along a clean arc instead of constantly chasing a moving target.
        if (target != null)
        {
            _targetPos = target.transform.position;
        }
        else
        {
            // Fallback: shoot forward a bit if no target is provided
            _targetPos = _startPos + transform.forward * 10f;
        }

        // Start at the start position
        transform.position = _startPos;
        _t = 0f;
        _lifeTimer = 0f;
    }

    private void Update()
    {
        // Lifetime / safety destroy
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Advance along the normalized time 0 → 1
        if (flightDuration <= 0.0001f)
            flightDuration = 0.0001f;

        _t += Time.deltaTime / flightDuration;
        _t = Mathf.Clamp01(_t);

        // Base straight-line interpolation
        Vector3 straightPos = Vector3.Lerp(_startPos, _targetPos, _t);

        // Add a vertical parabolic offset: 4 * h * t * (1 - t)
        float heightOffset = 4f * arcHeight * _t * (1f - _t);
        Vector3 curvedPos = straightPos + Vector3.up * heightOffset;

        // Compute velocity direction for orientation before moving
        Vector3 velocity = curvedPos - transform.position;

        transform.position = curvedPos;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        }

        // If we've essentially reached the end of the path and haven't hit anything, 
        // let the lifetime timer handle cleanup.
    }
}
