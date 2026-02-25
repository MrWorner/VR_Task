using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace GasStoveSimulator.Common
{
    public class SimpleMaterialSwap : MonoBehaviour
    {
        public Renderer targetRenderer;
        public Material highlightMaterial;

        private Material _originalMaterial;
        private XRBaseInteractable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<XRBaseInteractable>();
            if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();

            if (targetRenderer != null)
                _originalMaterial = targetRenderer.sharedMaterial;
        }

        private void OnEnable()
        {
            _interactable.hoverEntered.AddListener(x => SetHighlight(true));
            _interactable.hoverExited.AddListener(x => SetHighlight(false));
        }

        private void SetHighlight(bool on)
        {
            if (targetRenderer == null) return;

            targetRenderer.material = on ? highlightMaterial : _originalMaterial;
        }
    }
}