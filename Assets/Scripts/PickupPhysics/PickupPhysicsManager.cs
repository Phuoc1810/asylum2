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
            else if (currentItemType == Interactable.InteracType.Newspaper)
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
                itemsInformationText.text = "Ngày 15/02/1997  \r\nGiới tính: Nam  \r\nTình trạng: Bệnh nhân được chuyển đến từ khu nội trú trong tình trạng hoang mang cực độ, liên tục kêu “ánh sáng trong đầu tôi đang cháy”.  \r\n\r\nChẩn đoán:  \r\nQuét X-Quang cho thấy cấu trúc não bộ xuất hiện vùng xáo trộn mạnh quanh thùy trán.  \r\nPhản ứng cảm xúc giảm dần, nhịp tim và sóng não vẫn ổn định ở mức thấp.  \r\nKhông còn phản ứng khi tiếp xúc với kích thích âm thanh hoặc ánh sáng mạnh.  \r\n\r\nGhi chú của bác sĩ phụ trách:  \r\n> “Không còn sợ hãi. Không còn cảm xúc.  \r\n> Chúng ta đã tiến gần hơn đến sự tĩnh lặng tuyệt đối.”  \r\n\r\nKý tên:  \r\n**Dr. ███████**  \r\nKhoa Nghiên cứu Thần kinh  \r\nSaint Morrow Hospital  \r\n\r\n⚠️ Hồ sơ này được niêm phong theo quy định nội bộ.  \r\n**Mã lưu trữ: 1502**   " + "";
            }
            else if (currentItemType == Interactable.InteracType.Note4)
            {
                inspectionInforText.text = "Hồ sơ điều trị";
                itemsInformationText.text = "BỆNH VIỆN SAINT MORROW  \r\nKHOA TÂM THẦN LÂM SÀNG  \r\nHỒ SƠ BỆNH ÁN – KHÔNG XÁC ĐỊNH  \r\n\r\nTên bệnh nhân: [DỮ LIỆU BỊ MỜ]  \r\nGiới tính: Không rõ  \r\nTuổi: Không xác định  \r\n\r\nTình trạng ban đầu:  \r\nBệnh nhân được tìm thấy trong tư thế ngồi co rút bên góc phòng, da tái nhợt, mắt mở to không chớp.  \r\nLiên tục lặp lại cụm từ: “Tôi vẫn nghe họ… Tôi vẫn nghe họ…”  \r\n\r\nKết quả kiểm tra:  \r\nPhản xạ thần kinh gần như mất hẳn, đồng tử không phản ứng với ánh sáng.  \r\nSóng não dao động bất thường, ghi nhận những đỉnh cao tương tự trạng thái hoảng loạn,  \r\nnhưng nhịp tim duy trì ở mức thấp ổn định – không tương thích với sợ hãi tự nhiên.  \r\n\r\nGhi chú của bác sĩ phụ trách:  \r\n> “Cảm xúc của bệnh nhân bị tước bỏ, nhưng ký ức vẫn còn.”  \r\n\r\nTình trạng hiện tại: Không xác định. ";
            }
            else if (currentItemType == Interactable.InteracType.Note5)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Màn hình số 3 lại nhiễu. Tôi thấy có thứ gì đó đi ngang qua hành lang tầng trệt dù camera hỏng từ hôm qua.\r\nTôi đã hỏi bên kỹ thuật, họ bảo không ai đi tuần giờ đó cả.\r\n\r\nMỗi đêm tiếng bước chân lại gần hơn — như thể nó biết khi nào tôi đang nhìn.\r\nTôi thử tắt hết camera, nhưng tiếng rít kim loại vẫn vang lên trong loa.\r\n\r\nCửa tầng hầm vẫn khóa, nhưng sáng nay... có vết kéo dài đến tận cửa phòng tôi.\r\n\r\nTôi sẽ nộp đơn xin nghỉ. Nếu ai đó đọc được mảnh giấy này —";
            }
            else if (currentItemType == Interactable.InteracType.Note6)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Tôi không nhớ mình là ai.\r\nHọ tiêm thứ gì đó mỗi tối, nói rằng để “làm dịu nỗi sợ”.\r\nNhưng mỗi khi nhắm mắt, tôi lại nghe thấy tiếng kim loại gõ vào tường…\r\nnhư ai đó đang cố thoát ra từ bên trong.\r\n\r\nĐêm qua, giường bên cạnh trống trơn.\r\nHọ bảo bệnh nhân đã “khỏi bệnh” và được chuyển xuống tầng dưới.\r\nNhưng tôi thấy anh ta vẫn ngồi ở góc phòng khi đèn tắt —\r\nchỉ khác là… không còn thở nữa.\r\n\r\nNếu ai đọc được dòng này,\r\nđừng để họ tiêm thứ đó vào anh.\r\n\r\nTôi sẽ trốn ra ngoài trước khi họ quay lại.";
            }
            else if (currentItemType == Interactable.InteracType.Note7)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Đã kiểm tra lại hộp cầu chì lần thứ ba trong tuần.\r\nDây không cháy, nguồn vẫn ổn… nhưng điện cứ tự tắt vào lúc 2:47 sáng.\r\nTôi thử để lại đèn hành lang bật suốt đêm, mà sáng ra tất cả đều tắt — như có ai đi quanh tòa nhà cúp từng cái một.\r\n\r\nBác sĩ trưởng nói “không sao đâu”, bảo đó là do nhiễu điện từ thí nghiệm tầng dưới.\r\nNhưng nếu thật sự là do thí nghiệm, thì tại sao tôi nghe thấy tiếng bước chân sau khi cầu dao bật lên?\r\n\r\nTôi đã tháo cầu chì ra để đảm bảo an toàn,\r\nnếu ai cần bật lại — hãy chắc rằng anh đang ở một mình.";
            }
            else if (currentItemType == Interactable.InteracType.Note8)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Tôi nghe tiếng ai đó gọi tên mình trong đêm, nhưng khi bật đèn lên thì căn phòng trống rỗng.\r\nCái giường cạnh tường bị lõm xuống như có người vừa ngồi, còn ga giường thì ướt lạnh.\r\n\r\nBác sĩ nói đó chỉ là “phản ứng hồi tưởng sau liệu pháp”, nhưng tôi không còn nhớ mình đã làm gì.\r\nCó lần tôi soi gương... nhưng trong gương, người nhìn lại không phải tôi.\r\n\r\nNếu đèn tắt, đừng nhìn thẳng vào gương.";
            }
            else if (currentItemType == Interactable.InteracType.Note9)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Bệnh nhân mã số 103B vẫn phản ứng tiêu cực với ánh sáng mạnh.\r\nKhi bật đèn lên, đồng tử giãn ra cực nhanh — gần như toàn phần.\r\nHắn gào “tắt đi” rồi cào vào tường đến rách cả móng tay.\r\n\r\nTôi đã báo cáo lên cấp trên, nhưng họ bảo cứ tiếp tục “điều chỉnh ánh sáng”.\r\nTôi không dám bước vào phòng đó nữa.\r\n\r\nHệ thống đèn ở đây... nó sáng lên ngay cả khi đã rút cầu chì.";
            }
            else if (currentItemType == Interactable.InteracType.Note10)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Hôm nay họ mang tới một bệnh nhân mới — người đàn ông tóc bạc, mắt mở trừng, im lặng suốt nhiều giờ.\r\nTrên tay hắn khắc chữ “ASYLUM”. Không biết là ai làm.\r\n\r\nTôi hỏi bác sĩ, ông ta chỉ nói “đó là một phần của thử nghiệm”.\r\n\r\nKhi tôi rời phòng, hắn quay đầu nhìn tôi, môi khẽ mấp máy.\r\nTôi nghĩ hắn nói “tôi đã thấy anh rồi”.\r\n\r\nTôi chưa bao giờ quay lại phòng đó nữa.";
            }
            else if (currentItemType == Interactable.InteracType.Note11)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Họ bắt đầu hoảng loạn khi không còn cảm nhận được sợ hãi.\r\nMột người cười trong khi bị tra tấn, một người khác nhìn trân trân vào tường đến chết đói.\r\nTôi đã cảnh báo họ rằng cảm xúc không thể bị triệt tiêu – nó chỉ chuyển hóa thành thứ khác…\r\n\r\nNhưng Hội đồng không nghe.\r\nHọ chỉ muốn kết quả.\r\n\r\nNếu tôi biến mất, hãy đốt toàn bộ dữ liệu tầng hầm.\r\nĐừng để “bọn họ” tìm thấy.";
            }
            else if (currentItemType == Interactable.InteracType.Note12)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Ngày 12 tháng 9\r\nMẫu thử cuối cùng phản ứng mạnh hơn dự kiến.\r\nKhông còn khả năng đồng bộ ý thức.\r\nCảm xúc bị bóp méo, trí nhớ phân mảnh.\r\n\r\nTôi nghe giọng nói trong tường — giọng của chính mình.\r\nCó lẽ quá trình này đã bắt đầu ảnh hưởng đến tôi.\r\nNếu bản ghi này được đọc…\r\n\r\nHãy rời khỏi đây.\r\n“Saint Morrow” không phải là bệnh viện. Nó là cỗ máy nghiền tâm trí.";
            }
            else if (currentItemType == Interactable.InteracType.Note13)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Tôi đã phải trốn ở đây suốt đêm.\r\nTụi nó đi qua hành lang, tiếng móng chân kéo lê trên sàn... như đang tìm ai đó.\r\nTôi nghe tiếng thở sát ngay cửa, rồi im bặt.\r\n\r\nKhi tôi mở mắt, mấy cái hộp thuốc bị xếp lại — theo hàng thẳng tắp, hướng về phía cửa ra.\r\n\r\nTôi không dám chạm vào gì nữa.\r\nNếu ai đọc được dòng này… đừng để đèn pin ở chế độ UV lâu quá.\r\nCó thứ gì đó nhìn thấy qua ánh sáng đó… mà tôi thì không muốn biết là gì.";
            }
            else if (currentItemType == Interactable.InteracType.Note14)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Họ bảo tôi xuống đây để dọn dẹp đống rác y tế cũ.\r\nNhưng trong những thùng hàng này không phải rác.\r\nCó thứ gì đó vẫn còn ấm khi tôi chạm vào.\r\n\r\nMỗi lần cúp điện, tôi nghe tiếng gõ từ bên trong mấy thùng kim loại… chậm, rồi nhanh dần.\r\nTôi đã niêm phong hết, ghi “Không được mở” — nhưng họ vẫn đem đi.\r\n\r\nNếu ai đọc được dòng này, đừng đứng gần mấy thùng chứa phía tường bên phải.";
            }
            else if (currentItemType == Interactable.InteracType.Note15)
            {
                inspectionInforText.text = "Mẫu giấy ghi chú";
                itemsInformationText.text = "Nguồn điện tầng hầm được dẫn thẳng vào phòng lab.\r\nKhông có cầu chì dự phòng, nên bất kỳ lần gạt cầu dao nào cũng kích hoạt toàn hệ thống.\r\n\r\nKhi dòng điện ổn định, ta có thể nghe thấy âm thanh… như tiếng nhịp tim dưới sàn.\r\nMột số nhân viên bảo đó chỉ là tiếng máy phát, nhưng tôi biết — máy không thể “thở”.\r\n\r\nĐừng chạm vào cầu dao nếu đèn hành lang còn nhấp nháy.\r\nNó không chỉ bật điện… nó đánh thức thứ gì đó.";
            }
            //else if (currentItemType == Interactable.InteracType.Note10)
            //{
            //    inspectionInforText.text = "Mẫu giấy ghi chú";
            //    itemsInformationText.text = "";
            //}
            else
            {
                inspectionInforText.text = "";
            }
        }
    }
}