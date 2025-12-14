using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

public class GuidanceSystem : MonoBehaviour
{
    public Transform startPoint;
    public Transform targetObject;
    public GameObject pathCylinder;

    public GameObject[] hints;
    public Dictionary<GameObject, bool> gameObjects;
    
    public float pathWidth = 5f;
    
    public float activationRange = 10f;
    
    private Renderer pathRenderer;
    
    public static GuidanceSystem Instance;

    void Start()
    {
        pathRenderer = pathCylinder.GetComponent<Renderer>();

        if (startPoint == null)
        {
            startPoint = Camera.main.transform;
        }

        gameObjects = new Dictionary<GameObject, bool>();
        foreach (GameObject hint in hints)
        {
            gameObjects.Add(hint, false);
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        pathCylinder.SetActive(false);
        StartCoroutine(FindNextPath());
    }

    IEnumerator FindNextPath()
    {
        while (true)
        {
            float min = Mathf.Infinity;
            float tmp = 0;
            GameObject closest = null;
            
            foreach (GameObject a in hints)
            {
                if (a == null) continue; 
                if (!gameObjects[a] && (tmp = Vector3.Distance(startPoint.position, a.transform.position)) < min)
                {
                    min = tmp;
                    closest = a;
                }
            }

            if (closest != null)
            {
                targetObject = closest.transform;
            }
            else
            {
                targetObject = null;
            }

            yield return new WaitForSeconds(5f);
        }
    }

    public void DoneWithItem(int n)
    {
        gameObjects[hints[n]] = true;
    }

    void Update()
    {
        if (targetObject == null) return;
        float distance = Vector3.Distance(startPoint.position, targetObject.position);

        bool shouldBeVisible = distance <= activationRange && targetObject != null;
        
        if (shouldBeVisible && !pathCylinder.activeSelf)
        {
            pathCylinder.SetActive(true);
        }
        else if (!shouldBeVisible && pathCylinder.activeSelf)
        {
            pathCylinder.SetActive(false);
        }
        
        if (pathCylinder.activeSelf)
        {
            DrawPath();
        }
    }

    void DrawPath()
    {
        Vector3 offset = new Vector3(0, -0.5f, 0.5f);
        
        Vector3 startPos = startPoint.position + startPoint.rotation * offset;

        Vector3 endPos = targetObject.position;
        
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        pathCylinder.transform.position = startPos + (direction / 2.0f);

        pathCylinder.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

        pathCylinder.transform.localScale = new Vector3(pathWidth, Mathf.Max(distance / 2.0f, 0.001f), pathWidth);
    }

    public void TogglePath(bool isVisible)
    {
        pathCylinder.SetActive(isVisible);
    }
}