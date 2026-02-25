// НАЗНАЧЕНИЕ: Управление логикой зажигания спички при взаимодействии с коробком в VR

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using NaughtyAttributes;

namespace GasStoveSimulator.Modules.Matches
{
    public class StickController : MonoBehaviour
    {
        #region Поля: Required
        [BoxGroup("Required"), Required, SerializeField]
        private GameObject _flameVisuals;

        [BoxGroup("Required"), Required, SerializeField]
        private AudioSource _ignitionSound;

        [BoxGroup("Required"), Required, SerializeField]
        private XRGrabInteractable _grabInteractable;
        #endregion

        #region Поля
        [BoxGroup("SETTINGS"), SerializeField]
        private float _ignitionThreshold = 0.5f;

        [BoxGroup("SETTINGS"), SerializeField]
        private string _matchboxTag = "Matchbox";

        [BoxGroup("DEBUG"), SerializeField, ReadOnly]
        private bool _isLit;

        [BoxGroup("DEBUG"), SerializeField]
        private bool _coloredDebug;

        [BoxGroup("DEBUG"), SerializeField, ReadOnly]
        private Vector3 _currentVelocity;
        #endregion

        #region Свойства
        public bool IsLit => _isLit;
        #endregion

        #region Unity Методы
        private void Awake()
        {
            if (_flameVisuals == null) DebugUtils.LogMissingReference(this, nameof(_flameVisuals));
            if (_ignitionSound == null) DebugUtils.LogMissingReference(this, nameof(_ignitionSound));
            if (_grabInteractable == null) DebugUtils.LogMissingReference(this, nameof(_grabInteractable));

            if (_flameVisuals != null)
                _flameVisuals.SetActive(false);
        }

        private void OnEnable() => _grabInteractable.selectExited.AddListener(OnDropped);
        private void OnDisable() => _grabInteractable.selectExited.RemoveListener(OnDropped);

        private Vector3 _lastPosition;
        private void Update()
        {
            _currentVelocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isLit) return;

            if (other.CompareTag(_matchboxTag))
            {
                float speed = _currentVelocity.magnitude;

                ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> Попытка зажигания. Скорость: {0}", _coloredDebug, speed);

                TryIgnite(speed);
            }
        }
        #endregion

        #region Личные методы
        private bool TryIgnite(float speed)
        {
            if (speed < _ignitionThreshold) return false;

            ApplyIgnition();

            ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Объект успешно зажжен", _coloredDebug);

            return true;
        }

        private void ApplyIgnition()
        {
            _isLit = true;

            if (_flameVisuals != null)
                _flameVisuals.SetActive(true);

            if (_ignitionSound != null)
                _ignitionSound.Play();

            SendHaptic(0.7f, 0.15f);
        }

        private void OnDropped(SelectExitEventArgs args)
        {
            _isLit = false;
            if (_flameVisuals != null)
                _flameVisuals.SetActive(false);

            ColoredDebug.CLog(gameObject, "<color=orange>[SYSTEM]</color> Объект брошен, огонь потушен", _coloredDebug);
        }

        private void SendHaptic(float intensity, float duration)
        {
            if (_grabInteractable.interactorsSelecting.Count > 0)
            {
                var interactor = _grabInteractable.interactorsSelecting[0];
                if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor inputInteractor)
                    inputInteractor.SendHapticImpulse(intensity, duration);
            }
        }
        #endregion
    }
}