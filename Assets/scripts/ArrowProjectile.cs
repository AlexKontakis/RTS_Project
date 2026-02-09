using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unified arrow projectile script that handles:
/// - Smooth parabolic movement from a fire point to a target snapshot
/// - Lifetime/despawn
/// - Damage application and "sticking" into hit units
/// 
/// Attach this to your arrow prefab and have your archer script
/// set the target (and optionally firePoint) before or right after instantiation.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ArrowProjectile : MonoBehaviour
{
    [Header("Damage")]
    public int DMG = 10;
    /// <summary>
    /// "Friendly" arrow damages Enemy units, "Enemy" arrow damages Friendly units.
    /// </summary>
    public string type = "Friendly";

    [Header("Flight")]
    /// <summary>
    /// Optional explicit fire point (e.g. bow). If null, the projectile's current position is used.
    /// </summary>
    public Transform firePoint;
    /// <summary>
    /// Target transform to shoot at. Its position is snapshotted at fire time.
    /// </summary>
    public Transform target;
    /// <summary>
    /// How long (in seconds) the arrow takes to travel from start to target.
    /// </summary>
    public float flightDuration = 1.0f;
    /// <summary>
    /// Maximum vertical height (relative to straight line) of the arc.
    /// </summary>
    public float arcHeight = 2.0f;

    [Header("Lifetime")]
    public float maxLifetime = 5.0f;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _t; // 0 → 1 over the course of the flight
    private float _lifeTimer;
    public bool _hasHit;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            // Ensure there is always a rigidbody so physics events will fire
            _rb = gameObject.AddComponent<Rigidbody>();
           
        }

        // We will move the projectile manually. Must be kinematic so physics doesn't override our position.
        _rb.useGravity = false;
        _rb.isKinematic = false;
    }

    private void Start()
    {
        // Determine start position
        _startPos = firePoint != null ? firePoint.position : transform.position;

        // Snapshot the target position at fire time so the arrow flies
        // along a clean arc instead of constantly chasing a moving target.
        if (target != null)
        {
            _targetPos = target.position;
        }
        else
        {
            // Fallback: shoot forward a bit if no target is provided
            _targetPos = _startPos + transform.forward * 10f;
        }

        // Start the arrow at the start position
        transform.position = _startPos;
        _t = 0f;
        _lifeTimer = 0f;
    }

    private void Update()
    {
        
        if (_hasHit)
            return;

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

    private void OnCollisionEnter(Collision other)
    {
        if (_hasHit)
            return;

        HandleHit(other.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit)
            return;

        HandleHit(other);
    }

    private void HandleHit(Collider otherCollider)
    {
        // Allow colliders on child objects
        var unit = otherCollider.GetComponentInParent<unit_properties>();
        if (unit == null)
            return;

        // Same damage rules as old Projectile script
        if (type == "Enemy" && unit.faction == "Friendly")
        {
            unit.HP -= DMG;
            StickIntoTarget(otherCollider.transform);
        }
        else if (type == "Friendly" && unit.faction == "Enemy")
        {
            unit.HP -= DMG;
            StickIntoTarget(otherCollider.transform);
        }
    }

    private void StickIntoTarget(Transform hitTransform)
    {
        _hasHit = true;

        

        if (_rb != null)
        {
            _rb.isKinematic = true;
        }

        // Parent arrow to the hit object so it moves with it
        transform.SetParent(hitTransform, true);
    }
}

