using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject fracturePrefab; // Set by the Spawner
    
    // Configurable damage/force values
    public float ExplosionRadius = 8f;
    public float ExplosionForce = 500f; 
    public float PlayerDamage = 50f;
    public GameObject ExplosionVFX; // Drag your explosion particle system here

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called by the Hazard Spawner to set the fracture pieces
    public void SetFracturePrefab(GameObject prefab)
    {
        fracturePrefab = prefab;
    }

    // Called by the Hazard Spawner to start the drop
    public void StartDrop(Vector3 target, float duration)
    {
        // Enable physics
        rb.isKinematic = false;
        rb.useGravity = true;

        // Apply high velocity to reach the ground in the given duration
        Vector3 direction = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target);
        float velocityMagnitude = distance / duration; 
        
        // CORRECTED: Use rb.velocity for setting linear movement
        rb.linearVelocity = direction * velocityMagnitude; 
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if we hit the ground (assuming you tag your ground/environment objects)
        // If the rocks are going through the ground, this tag must match the terrain tag!
        if (other.CompareTag("Ground") || other.CompareTag("Environment"))
        {
            ExplodeAndShatter();
        }
    }

    void ExplodeAndShatter()
    {
        // 1. Instantiate the Shattered Rock
        if (fracturePrefab != null)
        {
            // Instantiate the fractured object at the impact location
            GameObject shatteredRock = Instantiate(fracturePrefab, transform.position, transform.rotation);
            
            // 2. Apply Force to the single instantiated shattered object
            Rigidbody shatteredRb = shatteredRock.GetComponent<Rigidbody>();
            if (shatteredRb != null)
            {
                // Apply the explosion force directly to the single parent Rigidbody
                shatteredRb.AddExplosionForce(
                    ExplosionForce * 0.75f, // Use a portion of the main force
                    transform.position, 
                    ExplosionRadius * 0.5f, 
                    1.0f // Lift component
                );
            }

            // Clean up the fragments after a short time
            Destroy(shatteredRock, 10f); 
        }

        // 3. Instantiate Visual/Audio FX
        if (ExplosionVFX != null)
        {
            Instantiate(ExplosionVFX, transform.position, Quaternion.identity);
        }

        // 4. Damage & Force Check
        // Find all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player")) 
            {
                // TODO: Replace Debug.Log with your actual Player health code!
                // Example: hit.GetComponent<PlayerHealth>().TakeDamage(PlayerDamage);
                Debug.Log($"Player hit! Applied {PlayerDamage} damage.");
            }
            
            // Apply physics force (e.g., to push the player or other props away)
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
            }
        }
        
        // 5. Destroy the original intact rock instance
        Destroy(gameObject);
    }
    
    // NOTE: The ApplyExplosionForceToFragments method has been deleted as it is no longer used 
    // in the revised design where the fracture prefab is treated as a single physics object.
}