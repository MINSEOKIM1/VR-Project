using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    public Transform target;
    public float distance = 1.8f;
    public float heightOffset = -0.2f;
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;

        // 카메라 정면 방향 (Y축은 무시)
        Vector3 forward = target.forward;
        forward.y = 0;
        forward.Normalize();

        // 목표 위치 계산
        Vector3 goalPos = target.position + forward * distance;
        goalPos.y += heightOffset;

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, goalPos, Time.deltaTime * followSpeed);

        // Y축만 회전 (뒤집힘 없음)
        Quaternion targetRot = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }
}
