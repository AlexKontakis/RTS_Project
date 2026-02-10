using UnityEngine;

/// <summary>
/// Positions three UI elements relative to the screen/canvas so they adapt to any resolution:
/// - leftCorner:    top-left corner
/// - rightCorner:   top-right corner
/// - bottomMiddle:  bottom-center
///
/// Attach this to a GameObject under your Canvas and assign the three RectTransforms.
/// This script sets anchors/pivots so layout is resolution-independent.
/// </summary>
public class UIAutoPosition : MonoBehaviour
{
    [Header("UI Elements to Position")]
    public RectTransform leftCorner;
    public RectTransform rightCorner;
    public RectTransform bottomMiddle;
    public RectTransform bottomLeft;

    [Header("Offsets (in pixels)")]
    public Vector2 topOffset = new Vector2(20f, 20f);   // x from side, y from top
    public float bottomOffset = 20f;                    // y from bottom

    private void Awake()
    {
        ApplyLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        // Called when the Canvas size changes (resolution / aspect changes)
        ApplyLayout();
    }

    private void ApplyLayout()
    {
        // Top-left
        if (leftCorner != null)
        {
            leftCorner.anchorMin = new Vector2(0f, 1f);
            leftCorner.anchorMax = new Vector2(0f, 1f);
            leftCorner.pivot = new Vector2(0f, 1f);
            leftCorner.anchoredPosition = new Vector2(topOffset.x, -topOffset.y);
        }

        // Top-right
        if (rightCorner != null)
        {
            rightCorner.anchorMin = new Vector2(1f, 1f);
            rightCorner.anchorMax = new Vector2(1f, 1f);
            rightCorner.pivot = new Vector2(1f, 1f);
            rightCorner.anchoredPosition = new Vector2(-topOffset.x, -topOffset.y);
        }

        // Bottom-left
        if (bottomLeft != null)
        {
            bottomLeft.anchorMin = new Vector2(0f, 0.2f);
            bottomLeft.anchorMax = new Vector2(0f, 0.2f);
            bottomLeft.pivot = new Vector2(0f, 1f);
            bottomLeft.anchoredPosition = new Vector2(topOffset.x, -topOffset.y);
        }

        // Bottom-middle
        if (bottomMiddle != null)
        {
            bottomMiddle.anchorMin = new Vector2(0.5f, 0f);
            bottomMiddle.anchorMax = new Vector2(0.5f, 0f);
            bottomMiddle.pivot = new Vector2(0.5f, 0f);
            bottomMiddle.anchoredPosition = new Vector2(0f, bottomOffset);
        }
    }
}

