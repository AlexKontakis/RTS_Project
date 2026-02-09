using UnityEngine;

/// <summary>
/// Clamps a camera's world position within configurable XYZ bounds.
/// Attach this to your camera (or the camera rig root) and set min/max values in the Inspector.
/// Works with any movement script by correcting the position each frame.
/// </summary>
public class CameraBounds : MonoBehaviour
{
    [Header("World-space bounds")]
    public float minX = -50f;
    public float maxX = 50f;

    public float minY = 5f;
    public float maxY = 50f;

    public float minZ = -50f;
    public float maxZ = 50f;

    private void LateUpdate()
    {
        var pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}

