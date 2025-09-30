using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class BoxKnockPuzzle : MonoBehaviour
{
    [Header("Puzzle Setting")]
    [SerializeField] private int[] correctPattern = new int[] { 3, 3, 2, 1 };
    [SerializeField] private float groupTimerout = 0.5f;

    [Header("Box Component")]
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private GameObject directorKey;

    [Header("Audio")]
    [SerializeField] private AudioClip knockSound;
    [SerializeField] private AudioClip completeOpenSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Animation Setting")]
    [SerializeField] private float openAnimationDelay = 0.5f;

    private int[] playerPattern = new int[4];
    private int currentGroupIndex = 0;
    private int knocksInCurrentGroup = 0;

    private float lastKnockTime = 0f;
    private bool isWaitingForTimeout = false;

    private bool isInspecting = false;
    private bool isPuzzleSolved = false;

    private const string OPEN_TRIGGER = "Open";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePuzzle();
    }
    private void InitializePuzzle()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if(boxAnimator == null)
        {
            boxAnimator = GetComponent<Animator>();
        }
        ResetPattern();
    }
    // Update is called once per frame
    void Update()
    {
        if (!isInspecting || isPuzzleSolved) return;

        HandleKnockInput();

        HandleGroupTimeOut();
    }
    private void HandleKnockInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickingOnBox())
            {
                RegisterKnock();
            }
        }
    }
    private bool IsClickingOnBox()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }
    private void RegisterKnock()
    {
        knocksInCurrentGroup++;

        PlayKnockSound();

        lastKnockTime = Time.time;
        isWaitingForTimeout = true;
    }
    private void HandleGroupTimeOut()
    {
        if (!isWaitingForTimeout) return;

        float timeSinceLastKnock = Time.time - lastKnockTime;
        if (timeSinceLastKnock >= groupTimerout)
        {
            FinishCurrentGroup();
        }
    }
    private void FinishCurrentGroup()
    {
        if (currentGroupIndex >= 4)
        {
            ResetPattern();
            ExitInspectMode();
            return;
        }

        playerPattern[currentGroupIndex] = knocksInCurrentGroup;

        knocksInCurrentGroup = 0;
        isWaitingForTimeout = false;
        currentGroupIndex++;

        if (currentGroupIndex >= 4)
        {
            ValidatePattern();
        }
    }
    private void ValidatePattern()
    {
        bool isCorrect = true;

        for(int i = 0; i < 4; i++)
        {
            if (playerPattern[i] != correctPattern[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            OnPatternCorrect();
        }
        else
        {
            OnPatternWrong();
        }
    }
    private void OnPatternCorrect()
    {
        isPuzzleSolved = true;
        if (boxAnimator != null)
        {
            boxAnimator.SetTrigger(OPEN_TRIGGER);
            PlayCompleteOpenBoxSound();
        }

        Collider boxCollider = GetComponent<Collider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        Invoke(nameof(ExitInspectMode), openAnimationDelay);
    }
    private void OnPatternWrong()
    {
        ResetPattern();
        ExitInspectMode();
    }
    private void ExitInspectMode()
    {
        PickupPhysicsManager pickupManager = FindObjectOfType<PickupPhysicsManager>();

        if (pickupManager != null)
        {
            pickupManager.StopInspecting();
        }
        isInspecting = false;
    }
    private void PlayKnockSound()
    {
        if (audioSource != null && knockSound != null)
        {
            audioSource.PlayOneShot(knockSound);
        }
    }
    private void PlayCompleteOpenBoxSound()
    {
        if (audioSource != null && completeOpenSound != null)
        {
            audioSource.PlayOneShot(completeOpenSound);
        }
    }
    private void ResetPattern()
    {
        playerPattern = new int[4];
        currentGroupIndex = 0;
        knocksInCurrentGroup = 0;
        lastKnockTime = 0;
        isWaitingForTimeout = false;
    }
    public void StartInspecting()
    {
        if (isPuzzleSolved)
        {
            return;
        }
        isInspecting = true;
        ResetPattern();
    }
    public void StopInspecting()
    {
        isInspecting = false;
        ResetPattern();
    }
    private void DisplayCurrentPattern()
    {
        string pattern = "Current pattern";
        for(int i = 0; i < currentGroupIndex; i++)
        {
            pattern += playerPattern[i];
            if (i < currentGroupIndex - 1) pattern += ", ";
        }
        pattern += "]";
        if (knocksInCurrentGroup > 0)
        {
            pattern += $"+{knocksInCurrentGroup}(current group)";
        }
        Debug.Log(pattern);
    }
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
    }
}
