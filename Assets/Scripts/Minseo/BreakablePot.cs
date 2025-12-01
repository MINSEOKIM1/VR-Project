using UnityEngine;

public class BreakablePot : MonoBehaviour
{
    [SerializeField] private float breakThreshold = 5f; 
    [SerializeField] private GameObject brokenPrefab;  
    [SerializeField] private GameObject[] itemsFromPot;  
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
        
        DogamManager.Instance.CollectItem("pottery");

        if (brokenPrefab != null)
        {
            var go = Instantiate(brokenPrefab, transform.position, transform.rotation);
            go.GetComponent<PotPiece>().SetRbParams(GetComponent<Rigidbody>().linearVelocity, GetComponent<Rigidbody>().angularVelocity);

            if (itemsFromPot != null)
            {
                foreach (var i in itemsFromPot)
                {
                    var a = Instantiate(i, transform.position + Vector3.up, transform.rotation);

                }
            }
        }

        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        Destroy(gameObject);
    }
}
