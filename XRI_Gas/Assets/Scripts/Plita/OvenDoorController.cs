using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class OvenDoorController : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Объект двери, который будет вращаться")]
    public Transform doorTransform;

    [Tooltip("Угол в закрытом состоянии (обычно 0,0,0)")]
    public Vector3 closedRotation = Vector3.zero;

    [Tooltip("Угол в открытом состоянии (например, 90 по X)")]
    public Vector3 openRotation = new Vector3(90f, 0, 0);

    [Tooltip("Скорость открытия")]
    public float rotationSpeed = 5f;

    [Header("Звуки")]
    public AudioSource openSound;
    public AudioSource closeSound;

    private XRSimpleInteractable interactable;
    public bool isOpened = false;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        // Используем activated, чтобы дверь срабатывала от курка (Trigger)
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

        // Проигрываем соответствующий звук
        if (isOpened && openSound != null) openSound.Play();
        else if (!isOpened && closeSound != null) closeSound.Play();
    }

    private void Update()
    {
        if (doorTransform == null) return;

        // Плавный переход между углами
        Vector3 targetEuler = isOpened ? openRotation : closedRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        doorTransform.localRotation = Quaternion.Slerp(
            doorTransform.localRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}