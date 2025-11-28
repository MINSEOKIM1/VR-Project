using UnityEngine;

public class GuidanceSystem : MonoBehaviour
{
    public Transform startPoint;
    public Transform targetObject;
    public GameObject pathCylinder;
    
    public float pathWidth = 5f;
    
    public float activationRange = 10f;
    
    private Renderer pathRenderer;

    void Start()
    {
        pathRenderer = pathCylinder.GetComponent<Renderer>();

        if (startPoint == null)
        {
            startPoint = Camera.main.transform;
        }

        pathCylinder.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(startPoint.position, targetObject.position);

        bool shouldBeVisible = distance <= activationRange;
        
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