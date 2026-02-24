using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class StickController : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    [Tooltip("Перетащите сюда объект Flame Orange")]
    public GameObject flameVisuals;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (flameVisuals != null)
        {
            flameVisuals.SetActive(false);
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnDropped);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnDropped);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (flameVisuals != null)
        {
            flameVisuals.SetActive(true);
        }
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        if (flameVisuals != null)
        {
            flameVisuals.SetActive(false);
        }
    }
}