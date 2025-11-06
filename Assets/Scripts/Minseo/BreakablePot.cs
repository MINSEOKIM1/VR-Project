using UnityEngine;

public class BreakablePot : MonoBehaviour
{
    [SerializeField] private float breakThreshold = 5f; 
    [SerializeField] private GameObject brokenPrefab;  
    [SerializeField] private AudioClip breakSound;     

    private bool isBroken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        float impact = collision.relativeVelocity.magnitude;

        if (impact > breakThreshold)
        {
            BreakPot();
        }
    }

    private void BreakPot()
    {
        isBroken = true;

        // 깨진 프리팹 생성
        if (brokenPrefab != null)
        {
            var go = Instantiate(brokenPrefab, transform.position, transform.rotation);
            go.GetComponent<PotPiece>().SetRbParams(GetComponent<Rigidbody>().linearVelocity, GetComponent<Rigidbody>().angularVelocity);
        }

        // 사운드 재생
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // 원래 오브젝트 제거
        Destroy(gameObject);
    }
}
