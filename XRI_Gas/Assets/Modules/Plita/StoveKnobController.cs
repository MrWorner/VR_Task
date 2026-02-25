// НАЗНАЧЕНИЕ: Контроллер поворотной ручки плиты для VR

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using NaughtyAttributes;
using DG.Tweening;

namespace GasStoveSimulator.Modules.Stove
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class StoveKnobController : MonoBehaviour
    {
        [BoxGroup("Events")]
        public UnityEvent<bool> OnStateChanged;

        #region Поля: Required
        [BoxGroup("Required"), Required, SerializeField]
        private XRSimpleInteractable _interactable;

        [BoxGroup("Required"), Required, SerializeField]
        private Transform _knobVisuals;
        #endregion

        #region Поля
        [BoxGroup("SETTINGS"), SerializeField]
        private Vector3 _offRotation = Vector3.zero;

        [BoxGroup("SETTINGS"), SerializeField]
        private Vector3 _onRotation = new Vector3(0, 0, 90f);

        [BoxGroup("SETTINGS"), SerializeField]
        private float _rotateDuration = 0.3f;

        [BoxGroup("SETTINGS"), SerializeField]
        private AudioSource _clickSound;

        [BoxGroup("DEBUG"), SerializeField, ReadOnly]
        private bool _isOn;

        [BoxGroup("DEBUG"), SerializeField]
        protected bool _ColoredDebug;
        #endregion

        #region Свойства
        public bool IsOn => _isOn;
        #endregion

        #region Unity Методы
        private void Awake()
        {
            if (_interactable == null)
                DebugUtils.LogMissingReference(this, nameof(_interactable));

            if (_knobVisuals == null)
                DebugUtils.LogMissingReference(this, nameof(_knobVisuals));
        }

        private void OnEnable()
        {
            if (_interactable != null)
                _interactable.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (_interactable != null)
                _interactable.activated.RemoveListener(OnActivated);
        }
        #endregion

        #region Публичные методы
        [Button("Toggle Knob")]
        public void ToggleKnob()
        {
            _isOn = !_isOn;

            PlayClickSound();
            AnimateRotation();

            OnStateChanged?.Invoke(_isOn);

            ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Состояние ручки изменено: {0}", _ColoredDebug, _isOn);
        }
        #endregion

        #region Личные методы
        private void OnActivated(ActivateEventArgs args)
        {
            ToggleKnob();
        }

        private void PlayClickSound()
        {
            if (_clickSound != null)
                _clickSound.Play();
        }

        private void AnimateRotation()
        {
            if (_knobVisuals == null) return;

            Vector3 targetEuler = _isOn ? _onRotation : _offRotation;

            var seq = DOTween.Sequence();
            seq.Append(_knobVisuals.DOLocalRotate(targetEuler, _rotateDuration).SetEase(Ease.OutBack));
            seq.OnComplete(() =>
            {
                ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> Поворот ручки завершен.", _ColoredDebug);
            });
        }
        #endregion
    }
}