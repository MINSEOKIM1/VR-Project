using UnityEngine;

public class VRHandFollow : MonoBehaviour
{
    public Transform hand; // XR Controller transform
    public Rigidbody rb;

    void FixedUpdate()
    {
        rb.MovePosition(hand.position);
        rb.MoveRotation(hand.rotation);
    }
}
