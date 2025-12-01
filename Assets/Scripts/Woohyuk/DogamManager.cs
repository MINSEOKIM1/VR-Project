using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System; // [중요] 뉴 인풋 시스템 사용
using TMPro;

public class DogamManager : MonoBehaviour
{
    [Header("Input Settings")]
    // XR Interaction Toolkit에서는 이 방식으로 버튼 입력을 받는 것이 표준입니다.
    public InputActionProperty toggleButtonAction;

    [Header("UI References")]
    public CanvasGroup dogamCanvasGroup; // 도감 캔버스 전체
    public GameObject tooltipCanvasObject; // 아까 만든 컨트롤러 툴팁 (도감 열면 끌 것)
    public TMP_Text descriptionText;

    [Header("Item Data")]
    public Sprite questionMarkSprite; // 미수집 상태 이미지 (?)
    public Image[] itemSlots; // UI에 있는 6개의 이미지 슬롯들

    public static DogamManager Instance;

    // 아이템 정보 구조체
    [System.Serializable]
    public struct ItemData
    {
        public string itemName;
        public Sprite itemImage;
        public bool isCollected;

        [TextArea(3, 5)]
        public String description;
    }

    // 우리가 관리할 6개의 아이템
    public ItemData[] items;

    // [추가된 부분 1] 컴포넌트가 켜질 때 입력 감지도 같이 켭니다.
    private void OnEnable()
    {
        if (toggleButtonAction.action != null)
            toggleButtonAction.action.Enable();
    }

    // [추가된 부분 2] 컴포넌트가 꺼질 때 입력 감지도 같이 끕니다. (메모리 낭비 방지)
    private void OnDisable()
    {
        if (toggleButtonAction.action != null)
            toggleButtonAction.action.Disable();
    }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        // 시작할 때 도감은 꺼두기
        HideDogam();
        // [중요] 시작할 때 버튼들에게 "클릭되면 할 일"을 알려줍니다.
        // 일일이 인스펙터에서 연결할 필요 없이 코드로 자동 연결합니다.
        for (int i = 0; i < itemSlots.Length; i++)
        {
            int index = i; // 람다식 클로저 문제를 피하기 위해 지역 변수에 복사
            Button btn = itemSlots[i].GetComponent<Button>();

            // 만약 버튼 컴포넌트가 없으면 자동으로 붙여줌 (안전장치)
            if (btn == null) btn = itemSlots[i].gameObject.AddComponent<Button>();

            // 버튼 클릭 시 OnSlotClicked 함수 실행 연결
            btn.onClick.AddListener(() => OnSlotClicked(index));
        }
        // UI 초기화 (처음엔 다 ?로 표시)
        UpdateUI();
    }

    private void Update()
    {
        // B키가 "이번 프레임에 눌렸는지" 체크
        // action.WasPressedThisFrame()은 버튼을 '딸깍' 누른 순간만 참이 됩니다.
        if (toggleButtonAction.action != null && toggleButtonAction.action.WasPressedThisFrame())
        {
            ToggleDogam();
        }
    }

    // 도감 열고 닫기 기능
    public void ToggleDogam()
    {
        // 현재 투명도가 0보다 크면(보이면) -> 숨기기, 아니면 -> 보이기
        if (dogamCanvasGroup.alpha > 0)
        {
            HideDogam();
        }
        else
        {
            ShowDogam();
        }
    }

    private void ShowDogam()
    {
        dogamCanvasGroup.alpha = 1; // 보이게
        dogamCanvasGroup.interactable = true; // 버튼 클릭 가능하게
        dogamCanvasGroup.blocksRaycasts = true; // 레이저 감지되게

        Debug.Log("도감 열림");
        UpdateUI();

        if (tooltipCanvasObject != null) tooltipCanvasObject.SetActive(false);
    }

    private void HideDogam()
    {
        dogamCanvasGroup.alpha = 0; // 투명하게 (하지만 존재함)
        dogamCanvasGroup.interactable = false; // 버튼 클릭 불가
        dogamCanvasGroup.blocksRaycasts = false; // 레이저 통과

        Debug.Log("도감 닫힘 (백그라운드에서 위치 추적 중)");
    }

    // 데이터에 따라 UI 그림 바꿔주는 함수
    public void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Length)
            {
                if (items[i].isCollected)
                {
                    // 수집했다면 원래 이미지 보여주기
                    itemSlots[i].sprite = items[i].itemImage;
                    itemSlots[i].color = Color.white;
                }
                else
                {
                    // 수집 못했으면 ? 이미지 보여주기
                    itemSlots[i].sprite = questionMarkSprite;
                    // 약간 어둡게 처리 (선택사항)
                    itemSlots[i].color = Color.gray;
                }
            }
        }
    }

    // 외부에서 아이템을 획득했을 때 호출할 함수
    // 예: CollectItem("검");
    public void CollectItem(string name)
    {
        
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemName == name)
            {
                if (items[i].isCollected) return;
                items[i].isCollected = true;
                // Debug.Log(name + " 획득!");
                GameManager.Instance.collectedEntries++;
                UpdateUI();
                break;
            }
        }
    }
    
    // [추가] 슬롯이 클릭되었을 때 실행되는 함수
    private void OnSlotClicked(int index)
    {
        // 데이터 범위를 벗어나는지 체크
        if (index >= items.Length) return;

        if (items[index].isCollected)
        {
            // 수집한 아이템이면 -> 진짜 설명을 보여줌
            descriptionText.text = items[index].description;
            Debug.Log($"선택함: {items[index].itemName}");
        }
        else
        {
            // 아직 수집 못했으면 -> 비밀 유지
            descriptionText.text = "아직 발견하지 못한 아이템입니다.\n(단서를 찾아보세요!)";
            Debug.Log("미수집 아이템 선택함");
        }
    }
}