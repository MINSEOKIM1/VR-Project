using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawn : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public GameObject bubblePrefab;

    [Header("Spawn Settings")]
    public int bubbleCount = 10;
    public float minDistance = 1.5f;
    public Vector2 heightOffsetRange = new Vector2(0.3f, 0.6f);
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    public bool randomYawRotation = true;
    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;
        if (terrain == null || bubblePrefab == null)
        {
            Debug.LogWarning("terrain or bubblePrefab not setted");
            return;
        }

        SpawnBubbles();
    }

    void SpawnBubbles()
    {
        TerrainData td = terrain.terrainData;
        Vector3 terrainPos = terrain.GetPosition();
        Vector3 terrainSize = td.size;

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = bubbleCount * 20;

        while (spawned < bubbleCount && attempts < maxAttempts)
        {
            attempts++;

            float x = terrainPos.x + Random.Range(0f, terrainSize.x);
            float z = terrainPos.z + Random.Range(0f, terrainSize.z);

            float y = terrain.SampleHeight(new Vector3(x, 0f, z)) + terrainPos.y;
            float yOffset = Random.Range(heightOffsetRange.x, heightOffsetRange.y);
            Vector3 pos = new Vector3(x, y + yOffset, z);

            bool tooClose = false;
            foreach (var p in usedPositions)
            {
                if ((p - pos).sqrMagnitude < minDistance * minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            Quaternion rot = randomYawRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            GameObject bubble = Instantiate(bubblePrefab, pos, rot, transform);
            float s = Random.Range(scaleRange.x, scaleRange.y);
            bubble.transform.localScale = new Vector3(s, s, s);

            usedPositions.Add(pos);
            spawned++;
        }

        Debug.Log($"[Spawner] Spawned {spawned} bubbles after {attempts} attempts.");
    }
}
