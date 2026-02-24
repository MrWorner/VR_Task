using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; // Добавлено для работы с курком
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class StoveKnobController : MonoBehaviour
{
    [Header("Визуал и Вращение")]
    [Tooltip("Перетащите сюда объект, который должен визуально крутиться (Mesh)")]
    public Transform knobVisuals;

    [Tooltip("Углы Эйлера (XYZ) в выключенном состоянии")]
    public Vector3 offRotation = Vector3.zero;

    [Tooltip("Углы Эйлера (XYZ) во включенном состоянии (например, 90 по оси Z)")]
    public Vector3 onRotation = new Vector3(0, 0, 90f);

    [Tooltip("Скорость плавного поворота ручки")]
    public float rotationSpeed = 10f;

    [Header("Звук и События")]
    public AudioSource clickSound;

    [Tooltip("Это событие сработает при переключении. True - включено, False - выключено")]
    public UnityEvent<bool> onStateChanged;

    [Header("Управление (Указательный палец / Курок)")]
    [Tooltip("Действие курка левой руки (обычно XRI LeftHand/Activate)")]
    public InputActionReference leftTriggerAction;
    [Tooltip("Действие курка правой руки (обычно XRI RightHand/Activate)")]
    public InputActionReference rightTriggerAction;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    public bool isOn = false;

    // Переменная, которая запоминает, смотрим ли мы/касаемся ли мы сейчас ручки
    private bool isHovered = false;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        // Подписываемся на НАВЕДЕНИЕ (когда луч или рука касается коллайдера ручки)
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);

        // Подписываемся на нажатие курков
        if (leftTriggerAction != null) leftTriggerAction.action.performed += OnTriggerPressed;
        if (rightTriggerAction != null) rightTriggerAction.action.performed += OnTriggerPressed;
    }

    private void OnDisable()
    {
        // Отписываемся при выключении, чтобы избежать ошибок
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);

        if (leftTriggerAction != null) leftTriggerAction.action.performed -= OnTriggerPressed;
        if (rightTriggerAction != null) rightTriggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovered = true; // Мы навели луч или руку на ручку
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovered = false; // Мы убрали луч или руку
    }

    // Этот метод срабатывает ТОЛЬКО когда мы физически прожимаем курок на контроллере
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        // Если мы в этот момент смотрим на ручку (навели на нее)
        if (isHovered)
        {
            isOn = !isOn;

            if (clickSound != null)
            {
                clickSound.Play();
            }

            onStateChanged.Invoke(isOn);
        }
    }

    private void Update()
    {
        if (knobVisuals == null) return;

        Vector3 targetEuler = isOn ? onRotation : offRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        knobVisuals.localRotation = Quaternion.Lerp(knobVisuals.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}