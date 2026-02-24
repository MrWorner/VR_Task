using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
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

    [SerializeField] private XRSimpleInteractable interactable;
    public bool isOn = false;

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnKnobInteracted);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnKnobInteracted);
    }

    private void OnKnobInteracted(SelectEnterEventArgs args)
    {
        isOn = !isOn;

        if (clickSound != null)
        {
            clickSound.Play();
        }

        onStateChanged.Invoke(isOn);
    }

    private void Update()
    {
        if (knobVisuals == null) return;

        Vector3 targetEuler = isOn ? onRotation : offRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        knobVisuals.localRotation = Quaternion.Lerp(knobVisuals.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}