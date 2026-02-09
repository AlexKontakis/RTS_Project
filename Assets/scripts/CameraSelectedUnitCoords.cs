using UnityEngine;

/// <summary>
/// Exposes the X and Z world coordinates of the first selected unit on the camera object.
/// Attach this to your Camera (or camera rig root) and assign the unit_manager reference,
/// or leave it null to auto-find. Other scripts can then read selectedX / selectedZ,
/// or you can optionally use them here to move the camera.
/// </summary>
public class CameraSelectedUnitCoords : MonoBehaviour
{
    [Header("References")]
    public unit_manager um;

    [Header("Output (read-only in Inspector)")]
    public float selectedX;
    public float selectedZ;

    [Header("Camera follow settings")]
    public bool moveCameraOnSpace = true;
    public float moveSpeed = 20f; // if <= 0, teleport instead of smooth move

    private void Start()
    {
        if (um == null)
        {
            um = FindObjectOfType<unit_manager>();
        }
    }

    private void LateUpdate()
    {
        if (um == null || um.us == null || um.us.Count == 0 || um.us[0] == null)
        {
            return;
        }

        Vector3 firstPos = um.us[0].transform.position;
        selectedX = firstPos.x;
        selectedZ = firstPos.z;

        // When spacebar is pressed, move the camera to the first selected unit's X/Z
        if (moveCameraOnSpace && Input.GetKeyDown(KeyCode.Space))
        {
            var camPos = transform.position;
            Vector3 targetPos = new Vector3(firstPos.x, camPos.y, firstPos.z);

            if (moveSpeed <= 0f)
            {
                // Instant snap
                transform.position = targetPos;
            }
            else
            {
                // Smooth move over time (single-frame start; remainder is handled next frames)
                StartCoroutine(MoveCameraSmooth(targetPos));
            }
        }
    }

    private System.Collections.IEnumerator MoveCameraSmooth(Vector3 targetPos)
    {
        while ((transform.position - targetPos).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = targetPos;
    }
}

