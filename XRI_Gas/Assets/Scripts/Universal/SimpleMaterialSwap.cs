using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SimpleMaterialSwap : MonoBehaviour
{
    public Renderer targetRenderer;
    [Tooltip("Создайте в проекте яркий материал и перетащите сюда")]
    public Material highlightMaterial;

    private Material originalMaterial;
    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
            originalMaterial = targetRenderer.sharedMaterial;
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(x => SetHighlight(true));
        interactable.hoverExited.AddListener(x => SetHighlight(false));
    }

    private void SetHighlight(bool on)
    {
        if (targetRenderer == null) return;

        targetRenderer.material = on ? highlightMaterial : originalMaterial;
    }
}