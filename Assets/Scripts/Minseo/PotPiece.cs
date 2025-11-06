using System;
using UnityEngine;

public class PotPiece : MonoBehaviour
{
    public Rigidbody[] rbs;

    public void SetRbParams(Vector3 linearVelocityInherited, Vector3 angularVelocityInherited)
    {
        foreach (var rb in rbs)
        {
            rb.linearVelocity = linearVelocityInherited;
            rb.angularVelocity = angularVelocityInherited;
        }
    }
}
