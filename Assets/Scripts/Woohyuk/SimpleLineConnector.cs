using UnityEngine;

public class SimpleLineConnector : MonoBehaviour
{
    public Transform startPoint; // 툴팁 (보통 이 스크립트가 붙은 객체)
    public Transform endPoint;   // B 버튼 위치 (Point_B_Button)
    
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        // 선의 점 개수는 2개 (시작점, 끝점)
        lr.positionCount = 2;
    }

    void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            // 매 프레임마다 시작점과 끝점의 위치를 갱신
            lr.SetPosition(0, startPoint.position);
            lr.SetPosition(1, endPoint.position);
        }
    }
}