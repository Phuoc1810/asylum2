using UnityEngine;

public class DrawerFocusController : MonoBehaviour
{
    [Header("Focus Setting")]
    [SerializeField] private Transform focusPoint;
    [SerializeField] private float focusSpeed = 3f;

    [Header("Drawer Components")]
    [SerializeField] private Animator drawerAnimator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawerOpenSound;
    [SerializeField] private AudioClip drawerCloseSound;

    private enum DrawerState
    {
        Normal,
        FocusMode,
        Solved
    }

    private DrawerState currentState = DrawerState.Normal;
    private bool isInFocusMode = false;
    private bool isPuzzleSolved = false;

    private Transform playerCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Transform originalCameraParent;

    private bool isTransitioning = false;
    private float transitionProgress = 0f;
    private Vector3 transitionStartPos;
    private Quaternion transitionStartRot;

    private const string OPEN_TRIGGER = "Open";
    private const string CLOSE_TRIGGER = "Close";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeDrawer();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTransitioning)
        {
            UpdateCameraTransition();
        }
        if (isInFocusMode && !isPuzzleSolved)
        {
            HandleForcusModeInput();
        }
    }
    private void InitializeDrawer()
    {
        if (drawerAnimator == null)
        {
            drawerAnimator = GetComponent<Animator>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (drawerAnimator != null)
        {
            drawerAnimator.SetTrigger(OPEN_TRIGGER);
        }
        if(drawerAnimator!=null && isTransitioning)
        {
            drawerAnimator.SetTrigger(CLOSE_TRIGGER);
        }

        currentState = DrawerState.Normal;
    }
    public void StartFocusMode(Camera camera)
    {
        if (isPuzzleSolved || isInFocusMode || focusPoint == null) return;

        playerCamera = camera.transform;

        originalCameraPosition = playerCamera.position;
        originalCameraRotation = playerCamera.rotation;
        originalCameraParent = playerCamera.parent;

        BeginCameraTransition(focusPoint.position, focusPoint.rotation);

        if (drawerAnimator != null)
        {
            drawerAnimator.SetTrigger(CLOSE_TRIGGER);
        }
        PlaySound(drawerCloseSound);

        currentState = DrawerState.FocusMode;
        isInFocusMode = true;
    }
    private void HandleForcusModeInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExitForcusMode(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            SolvedPuzzle();
        }
    }
    private void ExitForcusMode(bool isSolved)
    {
        if (!isInFocusMode) return;

        BeginCameraTransition(originalCameraPosition, originalCameraRotation);

        if (!isSolved)
        {
            if (drawerAnimator != null)
            {
                drawerAnimator.SetTrigger(OPEN_TRIGGER);
            }

            PlaySound(drawerOpenSound);

            currentState = DrawerState.Normal;
        }

        isInFocusMode = false;

        InteractableController controller = FindObjectOfType<InteractableController>();
        if (controller != null)
        {
            controller.OnDrawerPuzzleComplete();
        }
    }
    private void SolvedPuzzle()
    {
        isPuzzleSolved = true;

        if (drawerAnimator != null)
        {
            drawerAnimator.SetTrigger(OPEN_TRIGGER);
        }
        PlaySound(drawerOpenSound);

        ExitForcusMode(true);

        Collider drawerCollider = GetComponent<Collider>();
        if (drawerCollider != null)
        {
            drawerCollider.enabled = false;
        }

        currentState = DrawerState.Solved;

        InteractableController controller = FindObjectOfType<InteractableController>();
        if (controller != null)
        {
            controller.OnDrawerPuzzleComplete();
        }
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
        transitionProgress += Time.deltaTime * focusSpeed;

        Vector3 targetPos = isInFocusMode ? focusPoint.position : originalCameraPosition;
        Quaternion targetRot = isInFocusMode ? focusPoint.rotation : originalCameraRotation;

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
    public bool IsInFocusMode => isInFocusMode;
    public bool IsSolved => isPuzzleSolved;
    private void OnValidate()
    {
        if (drawerAnimator == null)
        {
            drawerAnimator = GetComponent<Animator>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}
