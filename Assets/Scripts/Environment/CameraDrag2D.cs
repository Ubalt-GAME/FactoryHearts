using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDrag2D : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 1f;

    [Header("World Bounds (inside the walls)")]
    public float minX = -13f;
    public float maxX = 13f;
    public float minY = -8f;
    public float maxY = 8f;

    private Camera cam;
    private Vector3 lastMouseWorldPos;
    private bool isDragging = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMouseDrag();
    }

    void HandleMouseDrag()
    {
        // Start dragging on left mouse down
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        // Stop dragging when released
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        // Current mouse position in world space
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        // How far did we move since last frame?
        Vector3 delta = lastMouseWorldPos - mouseWorldPos;

        // Apply movement only on X/Y, leave Z alone
        Vector3 newPos = transform.position + new Vector3(delta.x, delta.y, 0f) * dragSpeed;

        // Clamp camera so we don't go past the borders
        newPos = ClampToBounds(newPos);

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        lastMouseWorldPos = mouseWorldPos;
    }

    Vector3 ClampToBounds(Vector3 pos)
    {
        float vertExtent = cam.orthographicSize;                // half height
        float horzExtent = vertExtent * cam.aspect;             // half width

        // These are the allowed center positions for the camera
        float minCamX = minX + horzExtent;
        float maxCamX = maxX - horzExtent;
        float minCamY = minY + vertExtent;
        float maxCamY = maxY - vertExtent;

        pos.x = Mathf.Clamp(pos.x, minCamX, maxCamX);
        pos.y = Mathf.Clamp(pos.y, minCamY, maxCamY);

        return pos;
    }

    void OnDrawGizmosSelected()
    {
        // Just to visualize the bounds in the Scene view
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}