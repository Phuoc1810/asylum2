using UnityEngine;

public class PasswordLockController : MonoBehaviour
{
    [Header("Focus Setting")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float zoomSpeed = 3f;

    [Header("Box Components")]
    [SerializeField] private Animator boxAnimator;

    [Header("UI")]
    [SerializeField] private GameObject passwordPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip openBoxSound;

    [Header("Password Settings")]
    [SerializeField] private string correctPassword = "1524";

    [Header("World State")]
    [SerializeField] private string puzzleStateId = "password_box_solved";

    private bool isZoomed = false;
    private bool isPuzzleSolved = false;

    private Transform playerCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private bool isTransitioning = false;
    private float transitionProgress = 0f;
    private Vector3 transitionStartPos;
    private Quaternion transitionStartRot;

    private const string OPEN_TRIGGER = "Open";

    void Start()
    {
        InitializeLock();
        LoadPuzzleState();
    }

    void Update()
    {
        if (isTransitioning)
        {
            UpdateCameraTransition();
        }

        if (isZoomed && !isPuzzleSolved)
        {
            HandleInput();
        }
    }

    private void InitializeLock()
    {
        if (boxAnimator == null)
        {
            boxAnimator = GetComponent<Animator>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (passwordPanel != null)
        {
            passwordPanel.SetActive(false);
        }
    }

    private void LoadPuzzleState()
    {
        if (WorldStateService.Instance != null && !string.IsNullOrEmpty(puzzleStateId))
        {
            if (WorldStateService.Instance.HasFlag(puzzleStateId))
            {
                isPuzzleSolved = true;

                if (boxAnimator != null)
                {
                    boxAnimator.SetTrigger(OPEN_TRIGGER);
                }

                Collider boxCollider = GetComponent<Collider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                }
            }
        }
    }

    public void StartZoomMode(Camera camera)
    {
        if (isPuzzleSolved || isZoomed || targetPoint == null) return;

        playerCamera = camera.transform;

        originalCameraPosition = playerCamera.position;
        originalCameraRotation = playerCamera.rotation;

        BeginCameraTransition(targetPoint.position, targetPoint.rotation);

        if (passwordPanel != null)
        {
            passwordPanel.SetActive(true);
        }

        isZoomed = true;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExitZoomMode(false);
        }
    }

    private void ExitZoomMode(bool isSolved)
    {
        if (!isZoomed) return;

        BeginCameraTransition(originalCameraPosition, originalCameraRotation);

        if (passwordPanel != null)
        {
            passwordPanel.SetActive(false);
        }

        isZoomed = false;

        InteractableController controller = FindObjectOfType<InteractableController>();
        if (controller != null)
        {
            controller.OnPuzzleComplete();
        }
    }

    public void CheckPassword(string inputPassword)
    {
        if (inputPassword == correctPassword)
        {
            OnPasswordCorrect();
        }
        else
        {
            OnPasswordWrong();
        }
    }

    private void OnPasswordCorrect()
    {
        isPuzzleSolved = true;

        if (WorldStateService.Instance != null && !string.IsNullOrEmpty(puzzleStateId))
        {
            WorldStateService.Instance.SetFlag(puzzleStateId, true);
        }

        PlaySound(unlockSound);

        if (boxAnimator != null && openBoxSound != null)
        {
            boxAnimator.SetTrigger(OPEN_TRIGGER);
            PlaySound(openBoxSound);
        }

        Collider boxCollider = GetComponent<Collider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        ExitZoomMode(true);
    }

    private void OnPasswordWrong()
    {
        PlaySound(errorSound);
    }

    private void BeginCameraTransition(Vector3 targetPos, Quaternion targetRot)
    {
        isTransitioning = true;
        transitionProgress = 0f;
        transitionStartPos = playerCamera.position;
        transitionStartRot = playerCamera.rotation;
    }

    private void UpdateCameraTransition()
    {
        if (playerCamera == null) return;

        transitionProgress += Time.deltaTime * zoomSpeed;

        Vector3 targetPos = isZoomed ? targetPoint.position : originalCameraPosition;
        Quaternion targetRot = isZoomed ? targetPoint.rotation : originalCameraRotation;

        playerCamera.position = Vector3.Lerp(
            transitionStartPos,
            targetPos,
            transitionProgress
        );

        playerCamera.rotation = Quaternion.Slerp(
            transitionStartRot,
            targetRot,
            transitionProgress
        );

        if (transitionProgress >= 1)
        {
            CompleteTransition();
        }
    }

    private void CompleteTransition()
    {
        isTransitioning = false;
        transitionProgress = 0f;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public bool IsZoomed => isZoomed;
    public bool IsSolved => isPuzzleSolved;

    private void OnValidate()
    {
        if (boxAnimator == null)
        {
            boxAnimator = GetComponent<Animator>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (string.IsNullOrEmpty(puzzleStateId))
        {
            puzzleStateId = gameObject.name + "_solved";
        }
    }
}