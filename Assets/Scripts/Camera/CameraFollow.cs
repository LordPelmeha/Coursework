using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // —юда перетаскиваешь игрока в инспекторе
    public Vector3 offset;         // —мещение камеры (если нужно, например, выше центра игрока)

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
