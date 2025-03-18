using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player or object to follow
    public Vector3 offset = new Vector3(0f, 0f, -5f);

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }


        transform.position = target.position + offset;
    }
}