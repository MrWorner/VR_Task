using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StickController : MonoBehaviour
{
    [Header("Настройки огня")]
    public GameObject flameVisuals;
    public bool isLit = false;

    [Header("Настройки чирканья")]
    public float ignitionThreshold = 0.5f;
    public string matchboxTag = "Matchbox";

    [Header("Звук")]
    [Tooltip("Перетащите сюда AudioSource со звуком зажигания")]
    public AudioSource ignitionSound;

    [SerializeField] private XRGrabInteractable grabInteractable;
    private Vector3 lastPosition;
    private Vector3 currentVelocity;

    private void Awake()
    {
        if (flameVisuals != null) flameVisuals.SetActive(false);
    }

    private void Update()
    {
        currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLit) return;

        if (other.CompareTag(matchboxTag))
        {
            Vector3 boxVelocity = Vector3.zero;
            Rigidbody boxRb = other.attachedRigidbody;

            float speed = currentVelocity.magnitude;

            Debug.Log($"Чирканье (триггер)! Скорость: {speed}");

            if (speed >= ignitionThreshold)
            {
                Ignite();
            }
        }
    }

    private void Ignite()
    {
        isLit = true;
        if (flameVisuals != null) flameVisuals.SetActive(true);
       
        if (ignitionSound != null)
        {
            ignitionSound.Play();
        }

        SendHaptic(0.7f, 0.15f);
    }

    private void OnEnable() => grabInteractable.selectExited.AddListener(OnDropped);
    private void OnDisable() => grabInteractable.selectExited.RemoveListener(OnDropped);

    private void OnDropped(SelectExitEventArgs args)
    {
        isLit = false;
        if (flameVisuals != null) flameVisuals.SetActive(false);
    }

    private void SendHaptic(float intensity, float duration)
    {
        if (grabInteractable.interactorsSelecting.Count > 0)
        {
            var interactor = grabInteractable.interactorsSelecting[0];
            if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor inputInteractor)
                inputInteractor.SendHapticImpulse(intensity, duration);
        }
    }
}