using UnityEngine;
using System.Collections;

public class HazardSpawner : MonoBehaviour
{
    // --- Prefab & Core Settings ---
    
    // CHANGED: Reference to the Cylinder Prefab
    [Tooltip("Drag the RedZoneCylinder Prefab here.")]
    public GameObject RedZoneCylinderPrefab; 

    // **Size 15:** Drag Rock1-15 into this array (MUST match the fracture array order)
    public GameObject[] AsteroidPrefabs;

    // **Size 15:** Drag RockFracture1-15 into this array
    public GameObject[] FracturePrefabs;

    [Header("Warning & Impact Timing")]
    public float WarningTime = 5.0f; // Time the player has to react
    public float ImpactTime = 1.5f;  // Time for the asteroid to fall
    public float SpawnHeight = 50f;
    public float ZoneRadius = 10f;   // Visual radius of the indicator

    [Header("Hazard Frequency")]
    [Tooltip("The MINIMUM time (seconds) between two hazard drops.")]
    public float MinDropInterval = 30f;

    [Tooltip("The MAXIMUM time (seconds) between two hazard drops.")]
    public float MaxDropInterval = 60f;

    [Header("Player Target Settings")]
    [Tooltip("Drag the main Player GameObject (or XR Origin) here.")]
    public Transform PlayerTransform;

    [Tooltip("The MAX distance from the player the strike can occur.")]
    public float MaxStrikeDistance = 50f;


    void Start()
    {
        // Start the cycle after a short initial delay
        StartCoroutine(DropCycle(5f)); 
    }

    IEnumerator DropCycle(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        if (PlayerTransform == null)
        {
            Debug.LogError("PlayerTransform is not assigned in HazardSpawner! Cannot drop asteroid.");
            yield break; // Stop the coroutine if no player is found
        }
        
        if (RedZoneCylinderPrefab == null)
        {
            Debug.LogError("RedZoneCylinderPrefab is not assigned! Cannot show warning.");
            yield break;
        }

        while (true)
        {
            // 1. Calculate a random position around the player
            Vector2 randomOffset2D = Random.insideUnitCircle * MaxStrikeDistance;
            Vector3 randomOffset3D = new Vector3(randomOffset2D.x, 0f, randomOffset2D.y);

            Vector3 playerPos = PlayerTransform.position;
            Vector3 strikePosition = playerPos + randomOffset3D;

            // Ensure the strike Y position is at ground level (assuming 0)
            strikePosition.y = 0f;

            // 2. Start the Warning Sequence at this area around the player
            StartCoroutine(WarningSequence(strikePosition));

            // 3. Wait for the next drop cycle interval
            yield return new WaitForSeconds(Random.Range(MinDropInterval, MaxDropInterval));
        }
    }

    IEnumerator WarningSequence(Vector3 targetPosition)
    {
        // Calculate the height for the cylinder (from ground to SpawnHeight)
        float cylinderHeight = SpawnHeight; 
        
        // Position the cylinder's center: halfway up the height
        Vector3 cylinderPosition = targetPosition + Vector3.up * (cylinderHeight / 2f);

        // 1. Instantiate the Red Zone CYLINDER Indicator
        GameObject indicator = Instantiate(RedZoneCylinderPrefab, cylinderPosition, Quaternion.identity);

        // 2. Scale the indicator to match the intended radius and height
        // (Unity's default Cylinder mesh has a height of 2, so we divide the desired height by 2)
        indicator.transform.localScale = new Vector3(ZoneRadius * 2, cylinderHeight / 2f, ZoneRadius * 2);

        // 3. Wait for the warning duration
        yield return new WaitForSeconds(WarningTime);

        // 4. Trigger the Impact
        StartCoroutine(ImpactSequence(targetPosition, indicator));
    }

    IEnumerator ImpactSequence(Vector3 targetPosition, GameObject indicator)
    {
        // 1. Select a random rock/fracture pair
        if (AsteroidPrefabs.Length == 0 || FracturePrefabs.Length == 0)
        {
            Debug.LogError("Asteroid or Fracture Prefabs array is empty!");
            yield break;
        }

        int randomIndex = Random.Range(0, AsteroidPrefabs.Length);

        GameObject selectedAsteroidPrefab = AsteroidPrefabs[randomIndex];
        GameObject selectedFracturePrefab = FracturePrefabs[randomIndex];

        // 2. Calculate the spawn position high above the target
        Vector3 spawnPosition = targetPosition + Vector3.up * SpawnHeight;

        // 3. Instantiate the Asteroid
        GameObject asteroidInstance = Instantiate(selectedAsteroidPrefab, spawnPosition, Random.rotation);

        // 4. Configure the Asteroid Controller
        AsteroidController controller = asteroidInstance.GetComponent<AsteroidController>();
        if (controller != null)
        {
            controller.SetFracturePrefab(selectedFracturePrefab);
            controller.StartDrop(targetPosition, ImpactTime);
        }

        // 5. Wait for the impact
        yield return new WaitForSeconds(ImpactTime);

        // 6. Clean up the indicator
        Destroy(indicator);
    }
}