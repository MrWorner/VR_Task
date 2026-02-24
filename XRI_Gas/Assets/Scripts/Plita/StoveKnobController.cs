using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class StoveKnobController : MonoBehaviour
{
    [Header("Визуал и Вращение")]
    [Tooltip("Объект, который визуально крутится (Mesh)")]
    public Transform knobVisuals;

    [Tooltip("Углы Эйлера (XYZ) в выключенном состоянии")]
    public Vector3 offRotation = Vector3.zero;

    [Tooltip("Углы Эйлера (XYZ) во включенном состоянии")]
    public Vector3 onRotation = new Vector3(0, 0, 90f);

    [Tooltip("Скорость плавного поворота")]
    public float rotationSpeed = 10f;

    [Header("Звук и События")]
    public AudioSource clickSound;

    [Tooltip("True - включено, False - выключено")]
    public UnityEvent<bool> onStateChanged;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    public bool isOn = false;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        // Срабатывает при нажатии (Select)
        //interactable.selectEntered.AddListener(OnSelected);
        interactable.activated.AddListener(OnActivated);
    }
    private void OnDisable()
    {
        interactable.activated.RemoveListener(OnActivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        ToggleKnob();
    }


    private void ToggleKnob()
    {
        isOn = !isOn;

        if (clickSound != null)
            clickSound.Play();

        onStateChanged.Invoke(isOn);
    }

    private void Update()
    {
        if (knobVisuals == null)
            return;

        Vector3 targetEuler = isOn ? onRotation : offRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        knobVisuals.localRotation = Quaternion.Lerp(
            knobVisuals.localRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}