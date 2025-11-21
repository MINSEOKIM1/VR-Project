using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModalController : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] CanvasGroup canvasGroup; // optional

    void Awake()
    {
        // 안전하게 리스너 설정 (Inspector 사용 안 함)
        closeButton.onClick.AddListener(OnClickClose);
    }

    void OnClickClose()
    {
        // 중복 클릭 방지
        closeButton.interactable = false;

        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        // UI 이벤트 루프가 끝날 때까지 1프레임 대기
        yield return null;

        // (선택) 페이드아웃
        if (canvasGroup != null)
        {
            float t = 0f;
            float d = 0.25f;
            float start = canvasGroup.alpha;

            while (t < d)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, 0f, t / d);
                yield return null;
            }
        }

        // 모달 비활성화
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지
        closeButton.onClick.RemoveListener(OnClickClose);
    }
}
