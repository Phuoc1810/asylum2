using UnityEngine;

public class HeadBobbingController : MonoBehaviour
{
    [SerializeField] private bool _enabled = true; // Biến để kiểm soát trạng thái của Head Bobbing
    private bool isMoving = false;

    [SerializeField, Range(0, 0.1f)] float _amplitude = 0.15f; // Biên độ của Head Bobbing
    [SerializeField, Range(0, 30f)] float _frequency = 10f; // Tần số của Head Bobbing

    [SerializeField, Range(0, 0.1f)] float _amplitudeForRun = 0.01f;
    [SerializeField, Range(0, 30f)] float _frequenceForRun = 12f;

    [SerializeField] private Transform _cameraTransform = null; // Biến để tham chiếu đến Transform của Camera
    [SerializeField] private Transform _cameraHodler = null; // Biến để tham chiếu đến Transform của Camera Holder

    private float _toggleSpeed = 0.2f; // Tốc độ chuyển đổi trạng thái của Head Bobbing
    private Vector3 startPos; // Biến để lưu vị trí ban đầu của Camera
    private CharacterController _controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        startPos = _cameraTransform.localPosition; // Lưu vị trí ban đầu của Camera
    }
    // Update is called once per frame
    void Update()
    {
        if(!_enabled) return; // Nếu Head Bobbing không được bật, thoát khỏi hàm Update
        CheckMotion(); // Kiểm tra chuyển động của CharacterController
        ResetPosition(); // Đặt lại vị trí của Camera về vị trí ban đầu nếu cần
        _cameraTransform.LookAt(FocusTarget()); // Đặt Camera nhìn về phía mục tiêu
    }
    private void PlayMotion(Vector3 motion)
    {
        _cameraTransform.localPosition += motion; // Cập nhật vị trí của Camera bằng cách cộng thêm chuyển động
    }
    private void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude; // Lấy tốc độ di chuyển của CharacterController
        isMoving = speed >= _toggleSpeed && _controller.isGrounded;
        if (!isMoving) return;

        PlayMotion(FootStepMotion()); // Nếu tốc độ lớn hơn tốc độ chuyển đổi, gọi hàm PlayMotion với chuyển động của bước chân
    }
    private Vector3 FootStepMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        bool isRunning = speed >= 3;
        float currentAplitude = isRunning ? _amplitudeForRun : _amplitude;
        float currentFrequecy = isRunning ? _frequenceForRun : _frequency;
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * currentFrequecy) * currentAplitude;
        pos.x += Mathf.Cos(Time.time * currentFrequecy * 0.5f) * currentAplitude * 2;
        return pos;
    }
    private void ResetPosition()
    {
        if (!isMoving && _cameraTransform.localPosition != startPos)
        {
            _cameraTransform.localPosition = Vector3.Lerp(
                _cameraTransform.localPosition, 
                startPos, 
                5 * Time.deltaTime
                );
        }
    }
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHodler.localPosition.y, transform.position.z);
        pos += _cameraHodler.forward * 5f; // Dịch chuyển vị trí của Camera Holder về phía trước một chút
        return pos;
    }
}
