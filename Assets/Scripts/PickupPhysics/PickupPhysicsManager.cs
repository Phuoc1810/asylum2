using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupPhysicsManager : MonoBehaviour
{
    [Header("Hold Positions")]
    [SerializeField] private Transform boxDirectorKeyHoldPosition;
    [SerializeField] private Transform noteKnock;
    [SerializeField] private Transform noteDrawer;
    [SerializeField] private Transform newspaper;

    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("UI reference")]
    [SerializeField] private GameObject inspectionInforPanel;
    [SerializeField] private TextMeshProUGUI inspectionInforText;
    [SerializeField] private TextMeshProUGUI itemsInformationText;

    [Header("Item Scaling")]
    [SerializeField] private float boxDirectorKeyScale = 0.6f;
    [SerializeField] private float noteKnockScale = 0.5f;
    [SerializeField] private float noteDrawerScale = 0.6f;
    [SerializeField] private float newspaperScale = 0.5f;
    [SerializeField] private float otherItemScale = 1f;

    [SerializeField] private Transform targetHoldPosition;

    private GameObject currentItem;
    private Interactable.InteracType currentItemType;
    private Camera playerCamera;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Vector3 originalScale;

    private bool isMovingToHoldPosition = false;
    private float movementProgress = 0f;


    public void StartPickupItem(GameObject item, Camera camera, Interactable.InteracType itemType)
    {
        if (item == null) return;
        if (camera == null) return;

        currentItem = item;
        currentItemType = itemType;
        playerCamera = camera;

        if (targetHoldPosition == null) return;

        SaveOriginalItemState();
        ApplyItemScale();
        DisableItemPhysics();
        BeginItemMovement();
    }
    public void UpdateItemPickup()
    {
        if (currentItem == null) return;

        if (isMovingToHoldPosition)
        {
            UpdateItemMovement();
        }
        else
        {
            HandleItemRotation();
        }
    }
    private void SaveOriginalItemState()
    {
        originalPosition = currentItem.transform.position;
        originalRotation = currentItem.transform.rotation;
        originalParent = currentItem.transform.parent;
        originalScale = currentItem.transform.localScale;
    }
    private void ApplyItemScale()
    {
        switch (currentItemType)
        {
            case Interactable.InteracType.BoxDirectorKey:
                currentItem.transform.localScale = originalScale * boxDirectorKeyScale;
                break;
            case Interactable.InteracType.NoteKnock:
                currentItem.transform.localScale = originalScale * noteKnockScale;
                break;
            case Interactable.InteracType.NoteDrawer:
                currentItem.transform.localScale = originalScale * noteDrawerScale;
                break;
            case Interactable.InteracType.Newspaper:
                currentItem.transform.localScale = originalScale * newspaperScale;
                break;
            case Interactable.InteracType.Note1:
                currentItem.transform.localScale = originalScale * otherItemScale;
                break;
                
        }
    }
    private void DisableItemPhysics()
    {
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
    private void BeginItemMovement()
    {
        isMovingToHoldPosition = true;
        movementProgress = 0f;
    }
    private void UpdateItemMovement()
    {
        if (targetHoldPosition == null || currentItem == null) return;

        movementProgress += Time.deltaTime * moveSpeed;

        Vector3 targetPosition = targetHoldPosition.position;
        Quaternion targetRotation = targetHoldPosition.rotation;

        currentItem.transform.position = Vector3.Lerp(
            originalPosition,
            targetPosition,
            movementProgress
            );

        currentItem.transform.rotation = Quaternion.Slerp(
            originalRotation,
            targetRotation,
            movementProgress
            );

        if (movementProgress >= 1)
        {
            CompleteItemMovement();
        }
    }
    private void CompleteItemMovement()
    {
        if (currentItem == null || playerCamera == null) return;

        isMovingToHoldPosition = false;
        currentItem.transform.SetParent(playerCamera.transform);
        currentItem.transform.localPosition = targetHoldPosition.localPosition;
        currentItem.transform.localRotation = targetHoldPosition.localRotation;

        ShowInspectionPanel(true);

        UpdateFuntion();
    }
    private void HandleItemRotation()
    {
        if (currentItem == null) return;

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentItem.transform.Rotate(-mouseY, mouseX, 0, Space.Self);
        }
    }
    private void ShowInspectionPanel(bool show)
    {
        if (inspectionInforPanel != null)
        {
            inspectionInforPanel.SetActive(show);
        }
    }
    public void StopInspecting()
    {
        if (currentItem == null)
        {
            return;
        }

        ShowInspectionPanel(false);

        currentItem.transform.SetParent(originalParent);
        DontDestroyOnLoad(currentItem);
        currentItem.transform.position = originalPosition;
        currentItem.transform.rotation = originalRotation;
        currentItem.transform.localScale = originalScale;

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        currentItem = null;
        playerCamera = null;

        InteractableController controller = FindObjectOfType<InteractableController>();
        if (controller != null)
        {
            controller.OnInspectionComplete();
        }
    }
    private void UpdateFuntion()
    {
        if (currentItemType == Interactable.InteracType.BoxDirectorKey)
        {
            BoxKnockPuzzle puzzle = currentItem.GetComponent<BoxKnockPuzzle>();
            if (puzzle != null)
            {
                puzzle.StartInspecting();
            }
        }
        if (inspectionInforText != null && itemsInformationText != null)
        {
            if (currentItemType == Interactable.InteracType.NoteKnock)
            {
                inspectionInforText.text = "3-3-2-1 ?";
            }
            else if (currentItemType == Interactable.InteracType.NoteDrawer)
            {
                inspectionInforText.text = "Bức vẽ kì lạ";
                itemsInformationText.text = "Một bức vẽ kì lạ với né vẽ nguệch ngoặc, và một cái cây bị ngược? Mà khoan! Một cánh tay phải?";
            }
            else if( currentItemType == Interactable.InteracType.Newspaper)
            {
                inspectionInforText.text = "Một tờ báo";
                itemsInformationText.text = "Bệnh viện Saint Morrow được chọn làm cơ sở thử nghiệm phục hồi cảm xúc ở bệnh nhân rối loạn tâm thần. Dự án mang tên Asylum.";
            }
            else if (currentItemType == Interactable.InteracType.Note1)
            {
                inspectionInforText.text = "Ghi chú lạ";
                itemsInformationText.text = "Phòng bảo trì bên cạnh có cửa bị lỏng, không ai dám chạm vào sau lần đó… Cần một chiếc tua vít để sửa lại";
            }
            else if (currentItemType == Interactable.InteracType.Note2)
            {
                inspectionInforText.text = "Ghi chú lạ";
                itemsInformationText.text = "Có báo cáo ghi lại rằng: Đã có đứa trẻ nhìn thấy mã số trên tường ở gần phòng an ninh. Đứa bé có đèn UV. Thật nguy hiểm, may thay là nó không biết rằng đó là mã số cái hộp gỗ ở trong phòng.";
            }
            else if (currentItemType == Interactable.InteracType.Note3)
            {
                inspectionInforText.text = "Hồ sơ bệnh nhân số 01";
                itemsInformationText.text = "Ngày 15/02/1997  \r\nGiới tính: Nam  \r\nTình trạng: Bệnh nhân được chuyển đến từ khu nội trú trong tình trạng hoang mang cực độ, liên tục kêu “ánh sáng trong đầu tôi đang cháy”.  \r\n\r\nChẩn đoán:  \r\nQuét X-Quang cho thấy cấu trúc não bộ xuất hiện vùng xáo trộn mạnh quanh thùy trán.  \r\nPhản ứng cảm xúc giảm dần, nhịp tim và sóng não vẫn ổn định ở mức thấp.  \r\nKhông còn phản ứng khi tiếp xúc với kích thích âm thanh hoặc ánh sáng mạnh.  \r\n\r\nGhi chú của bác sĩ phụ trách:  \r\n> “Không còn sợ hãi. Không còn cảm xúc.  \r\n> Chúng ta đã tiến gần hơn đến sự tĩnh lặng tuyệt đối.”  \r\n\r\nKý tên:  \r\n**Dr. ███████**  \r\nKhoa Nghiên cứu Thần kinh  \r\nSaint Morrow Hospital  \r\n\r\n⚠️ Hồ sơ này được niêm phong theo quy định nội bộ.  \r\n**Mã lưu trữ: 1502**   " +"";
            }
            else if (currentItemType == Interactable.InteracType.Note4)
            {
                inspectionInforText.text = "Hồ sơ điều trị";
                itemsInformationText.text = "BỆNH VIỆN SAINT MORROW  \r\nKHOA TÂM THẦN LÂM SÀNG  \r\nHỒ SƠ BỆNH ÁN – KHÔNG XÁC ĐỊNH  \r\n\r\nTên bệnh nhân: [DỮ LIỆU BỊ MỜ]  \r\nGiới tính: Không rõ  \r\nTuổi: Không xác định  \r\n\r\nTình trạng ban đầu:  \r\nBệnh nhân được tìm thấy trong tư thế ngồi co rút bên góc phòng, da tái nhợt, mắt mở to không chớp.  \r\nLiên tục lặp lại cụm từ: “Tôi vẫn nghe họ… Tôi vẫn nghe họ…”  \r\n\r\nKết quả kiểm tra:  \r\nPhản xạ thần kinh gần như mất hẳn, đồng tử không phản ứng với ánh sáng.  \r\nSóng não dao động bất thường, ghi nhận những đỉnh cao tương tự trạng thái hoảng loạn,  \r\nnhưng nhịp tim duy trì ở mức thấp ổn định – không tương thích với sợ hãi tự nhiên.  \r\n\r\nGhi chú của bác sĩ phụ trách:  \r\n> “Cảm xúc của bệnh nhân bị tước bỏ, nhưng ký ức vẫn còn.”  \r\n\r\nTình trạng hiện tại: Không xác định. ";
            }
            else
            {
                inspectionInforText.text = "";
            }
        }
    }
}