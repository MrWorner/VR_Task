using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(AudioSource))] 
public class OvenDoorController : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Объект двери или Pivot, который будет вращаться")]
    public Transform doorTransform;
    public Vector3 closedRotation = Vector3.zero;
    public Vector3 openRotation = new Vector3(90f, 0, 0);
    public float rotationSpeed = 5f;

    [Header("Звуки (Аудио клипы)")]
    public AudioClip openClip;
    public AudioClip closeClip;

    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private AudioSource audioSource;
    public bool isOpened = false;

    private void Awake()
    {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnEnable()
    {
        interactable.activated.AddListener(OnDoorActivated);
    }

    private void OnDisable()
    {
        interactable.activated.RemoveListener(OnDoorActivated);
    }

    private void OnDoorActivated(ActivateEventArgs args)
    {
        ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpened = !isOpened;

        if (isOpened && openClip != null)
        {
            audioSource.PlayOneShot(openClip);
        }
        else if (!isOpened && closeClip != null)
        {
            audioSource.PlayOneShot(closeClip);
        }
    }

    private void Update()
    {
        if (doorTransform == null) return;

        Vector3 targetEuler = isOpened ? openRotation : closedRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        doorTransform.localRotation = Quaternion.Slerp(
            doorTransform.localRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}