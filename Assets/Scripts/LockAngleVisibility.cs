using UnityEngine;

public class LockAngleVisibility : MonoBehaviour
{
    public GameObject lockObject;
    public Transform playerCamera;
    public float maxAngle = 30f;

    void Update()
    {
        if (lockObject == null || playerCamera == null) return;
        // Project positions onto XY plane (ignore Z)
        Vector2 lockPos2D = new Vector2(lockObject.transform.position.x, lockObject.transform.position.z);
        Vector2 cameraPos2D = new Vector2(playerCamera.position.x, playerCamera.position.z);

        // Direction vector from camera to lock in 2D
        Vector2 toLock2D = (lockPos2D - cameraPos2D).normalized;

        // Lock's forward in 2D (project its forward vector)
        Vector2 lockForward2D = new Vector2(lockObject.transform.forward.x, -lockObject.transform.forward.y).normalized;

        // Angle between vectors in 2D
        float angle = Vector2.Angle(lockForward2D, toLock2D);
        print("lockpos2d.x:" + lockForward2D.x);
        print("lockpos2d.z:" + lockForward2D.y);
        bool isVisible = angle <= maxAngle;

        lockObject.SetActive(isVisible);
    }
}