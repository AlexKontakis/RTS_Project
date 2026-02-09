using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FormationController : MonoBehaviour
{
    public unit_manager um;

    public enum FormationType
    {
        BoxLoose = 0,
        BoxTight = 1,
        CircleLoose = 2,
        CircleTight = 3
    }

    // Current active formation. Default: loose box.
    public FormationType currentFormationType = FormationType.BoxLoose;

    private void Start()
    {
        // Auto-find unit_manager if not assigned in the Inspector
        if (um == null)
        {
            um = FindObjectOfType<unit_manager>();
        }
    }

    private void Update()
    {
        // Require Shift + number
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!shift || um == null || um.us.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplyFormation(FormationType.BoxLoose);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplyFormation(FormationType.BoxTight);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ApplyFormation(FormationType.CircleLoose);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ApplyFormation(FormationType.CircleTight);
        }
    }

    /// <summary>
    /// Apply a formation to the currently selected units, centered on the
    /// current selection center. Also updates the currentFormationType.
    /// Used when the player presses Shift+1..4.
    /// </summary>
    public void ApplyFormation(FormationType type)
    {
        currentFormationType = type;

        if (!BuildGroups(out var ranged, out var melee, out var center))
            return;

        switch (type)
        {
            case FormationType.BoxLoose:
                ApplyBoxFormation(ranged, melee, center, 20f);      
                break;
            case FormationType.BoxTight:
                ApplyBoxFormation(ranged, melee, center, 10f);       
                break;
            case FormationType.CircleLoose:
                ApplyCircleFormation(ranged, melee, center, 24f, 20f); 
                break;
            case FormationType.CircleTight:
                ApplyCircleFormation(ranged, melee, center, 14f, 12f);   
                break;
        }
    }

    /// <summary>
    /// Apply the currently selected formation type to the selected units,
    /// using a custom center point (e.g. a right-click destination).
    /// This is used by unit_manager when issuing move orders so that units
    /// keep the last chosen formation while moving.
    /// </summary>
    public void ApplyCurrentFormationAt(Vector3 center)
    {
        if (!BuildGroups(out var ranged, out var melee, out _))
            return;

        switch (currentFormationType)
        {
            case FormationType.BoxLoose:
                ApplyBoxFormation(ranged, melee, center, 20f);
                break;
            case FormationType.BoxTight:
                ApplyBoxFormation(ranged, melee, center, 10f);
                break;
            case FormationType.CircleLoose:
                ApplyCircleFormation(ranged, melee, center, 24f, 20f);
                break;
            case FormationType.CircleTight:
                ApplyCircleFormation(ranged, melee, center, 14f, 12f);
                break;
        }
    }

    /// <summary>
    /// Build ranged and melee groups from the current selection, and compute
    /// the current selection center. Returns false if there are no units.
    /// </summary>
    private bool BuildGroups(out List<GameObject> ranged, out List<GameObject> melee, out Vector3 selectionCenter)
    {
        ranged = new List<GameObject>();
        melee = new List<GameObject>();
        selectionCenter = Vector3.zero;

        var selected = um != null ? um.us : null;
        if (selected == null || selected.Count == 0) return false;

        int centerCount = 0;

        foreach (var go in selected)
        {
            if (go == null) continue;
            var props = go.GetComponent<unit_properties>();
            if (props == null) continue;

            // accumulate for center
            selectionCenter += go.transform.position;
            centerCount++;

            bool isRanged = um.FRUSet.Contains(props.type) || um.ERUSet.Contains(props.type);

            if (isRanged)
                ranged.Add(go);
            else
                melee.Add(go);
        }

        if (centerCount > 0)
        {
            selectionCenter /= centerCount;
        }

        return ranged.Count + melee.Count > 0;
    }

    /// <summary>
    /// Box formation: units placed on a grid around the center.
    /// Ranged units are assigned to positions closest to the center,
    /// melee units to outer positions.
    /// </summary>
    private void ApplyBoxFormation(List<GameObject> ranged, List<GameObject> melee, Vector3 center, float spacing)
    {
        int total = ranged.Count + melee.Count;
        if (total == 0) return;

        // Generate grid offsets
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(total));
        List<Vector3> offsets = new List<Vector3>();

        int half = gridSize / 2;
        for (int z = -half; z <= half; z++)
        {
            for (int x = -half; x <= half; x++)
            {
                offsets.Add(new Vector3(x * spacing, 0f, z * spacing));
            }
        }

        // Sort offsets by distance to center (0,0,0) ascending
        offsets.Sort((a, b) => a.sqrMagnitude.CompareTo(b.sqrMagnitude));

        // Build ordered list: ranged first (middle), then melee (outer)
        List<GameObject> orderedUnits = new List<GameObject>();
        orderedUnits.AddRange(ranged);
        orderedUnits.AddRange(melee);

        for (int i = 0; i < total && i < offsets.Count; i++)
        {
            var go = orderedUnits[i];
            if (go == null) continue;

            var agent = go.GetComponent<NavMeshAgent>();
            if (agent == null) continue;

            Vector3 dest = center + offsets[i];
            agent.SetDestination(dest);
        }
    }

    /// <summary>
    /// Circle formation: ranged units on an inner circle, melee on an outer circle.
    /// </summary>
    private void ApplyCircleFormation(List<GameObject> ranged, List<GameObject> melee, Vector3 center, float innerRadius, float ringSpacing)
    {
        int rangedCount = ranged.Count;
        int meleeCount = melee.Count;

        // Inner circle for ranged
        if (rangedCount > 0)
        {
            for (int i = 0; i < rangedCount; i++)
            {
                var go = ranged[i];
                if (go == null) continue;

                var agent = go.GetComponent<NavMeshAgent>();
                if (agent == null) continue;

                float angle = (Mathf.PI * 2f * i) / Mathf.Max(1, rangedCount);
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * innerRadius;
                Vector3 dest = center + offset;
                agent.SetDestination(dest);
            }
        }

        // Outer circle for melee
        float outerRadius = innerRadius + ringSpacing;
        if (meleeCount > 0)
        {
            for (int i = 0; i < meleeCount; i++)
            {
                var go = melee[i];
                if (go == null) continue;

                var agent = go.GetComponent<NavMeshAgent>();
                if (agent == null) continue;

                float angle = (Mathf.PI * 2f * i) / Mathf.Max(1, meleeCount);
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * outerRadius;
                Vector3 dest = center + offset;
                agent.SetDestination(dest);
            }
        }
    }
}

