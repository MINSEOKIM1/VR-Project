using System;
using UnityEngine;

public class RockDetector : MonoBehaviour
{
    public float nodamage = 0f;
    private void OnCollisionEnter(Collision other)
    {
        if (nodamage > 0) return;
        if (other.gameObject.CompareTag("Rock"))
        {
            if (other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude > 5f)
            {
                nodamage = 3f;
                BattleManager.Instance.TakeDamage(75);
            }
        }
    }
}
