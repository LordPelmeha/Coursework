using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    [Header("Настройки зума")]
    [Tooltip("Скорость изменения размера камеры при прокрутке")]
    [SerializeField] private float zoomSpeed = 5f;
    [Tooltip("Минимальное значение приближения")]
    [SerializeField] private float minZoom = 3f;
    [Tooltip("Максимальное значение отдаления")]
    [SerializeField] private float maxZoom = 15f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f))
            return;
        float newSize = cam.orthographicSize - scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }
}
