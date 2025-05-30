using UnityEngine;

/// <summary>
/// Contr�le la cam�ra avec les touches WASD ou fl�ch�es, et permet un zoom avec la molette.
/// La vitesse de d�placement s'ajuste automatiquement selon le niveau de zoom.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Vitesse de base � un zoom normal.
    /// </summary>
    public float baseMoveSpeed = 5f;

    /// <summary>
    /// Zoom de r�f�rence � partir duquel la vitesse est calcul�e.
    /// </summary>
    public float referenceZoom = 5f;

    /// <summary>
    /// Vitesse de zoom avec la molette.
    /// </summary>
    public float zoomSpeed = 5f;

    /// <summary>
    /// Zoom minimum autoris�.
    /// </summary>
    public float minZoom = 2f;

    /// <summary>
    /// Zoom maximum autoris�.
    /// </summary>
    public float maxZoom = 20f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    /// <summary>
    /// G�re le d�placement de la cam�ra en fonction du zoom actuel.
    /// </summary>
    void HandleMovement()
    {
        Vector3 move = new Vector3();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move.y += 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move.y -= 1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move.x -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move.x += 1;

        // Calculer une vitesse ajust�e selon le zoom actuel
        float zoomFactor = cam.orthographicSize / referenceZoom;
        float adjustedSpeed = baseMoveSpeed * zoomFactor;

        transform.position += move * adjustedSpeed * Time.deltaTime;
    }

    /// <summary>
    /// G�re le zoom avec la molette de la souris.
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
