// НАЗНАЧЕНИЕ: Управляет газоанализатором, его визуальными состояниями (UI) и звуковыми оповещениями.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using NaughtyAttributes;
using GasStoveSimulator.Modules.Core;

namespace GasStoveSimulator.Modules.GasDetector
{
    public class GasDetectorController : MonoBehaviour
    {
        #region Поля: Required
        [BoxGroup("Required"), Required, SerializeField] private Image _backgroundImage;
        [BoxGroup("Required"), Required, SerializeField] private Image _iconImage;
        [BoxGroup("Required"), Required, SerializeField] private XRGrabInteractable _grabInteractable;
        [BoxGroup("Required"), Required, SerializeField] private AudioSource _audioSource;
        #endregion

        #region Поля
        [BoxGroup("SETTINGS/Sprites"), SerializeField] private Sprite _waitingSprite;
        [BoxGroup("SETTINGS/Sprites"), SerializeField] private Sprite _okSprite;
        [BoxGroup("SETTINGS/Sprites"), SerializeField] private Sprite _alertSprite;

        [BoxGroup("SETTINGS/Colors"), SerializeField] private Color _colorWaiting = Color.black;
        [BoxGroup("SETTINGS/Colors"), SerializeField] private Color _colorOk = Color.gray;
        [BoxGroup("SETTINGS/Colors"), SerializeField] private Color _colorAlert = Color.red;

        [BoxGroup("SETTINGS/Audio"), SerializeField]
        [Tooltip("Звук сирены при обнаружении газа (зациклен)")]
        private AudioClip _alertClip;

        [BoxGroup("SETTINGS/Audio"), SerializeField]
        [Tooltip("Короткий Beep при взятии в руку")]
        private AudioClip _grabClip;

        [BoxGroup("SETTINGS/Audio"), SerializeField]
        [Tooltip("Милый писк при выбрасывании предмета")]
        private AudioClip _dropClip;

        [BoxGroup("DEBUG")]
        [SerializeField, ReadOnly] private bool _isGrabbed = false;

        [BoxGroup("DEBUG")]
        [SerializeField, ReadOnly] private bool _isNearGas = false;

        [BoxGroup("DEBUG"), SerializeField] protected bool _ColoredDebug;
        #endregion

        #region Unity Методы
        private void Awake()
        {
            if (_backgroundImage == null) DebugUtils.LogMissingReference(this, nameof(_backgroundImage));
            if (_iconImage == null) DebugUtils.LogMissingReference(this, nameof(_iconImage));
            if (_grabInteractable == null) DebugUtils.LogMissingReference(this, nameof(_grabInteractable));
            if (_audioSource == null) DebugUtils.LogMissingReference(this, nameof(_audioSource));

            if (_audioSource != null)
            {
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1.0f;
            }
        }

        private void OnEnable()
        {
            if (_grabInteractable != null)
            {
                _grabInteractable.selectEntered.AddListener(OnGrabbed);
                _grabInteractable.selectExited.AddListener(OnDropped);
            }
        }

        private void OnDisable()
        {
            if (_grabInteractable != null)
            {
                _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
                _grabInteractable.selectExited.RemoveListener(OnDropped);
            }
        }

        private void Start()
        {
            UpdateScreen();
        }
        #endregion

        #region Публичные методы
        public void SetGasDetected(bool hasGas, bool isTrueLeak = false)
        {
            if (hasGas && isTrueLeak)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ReportLeakFound();
                }
            }

            if (_isNearGas == hasGas) return;

            _isNearGas = hasGas;
            UpdateScreen();
        }
        #endregion

        #region Личные методы
        private void OnGrabbed(SelectEnterEventArgs args)
        {
            _isGrabbed = true;
            ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Газоанализатор взят в руку", _ColoredDebug);

            if (_grabClip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_grabClip);
            }

            UpdateScreen();
        }

        private void OnDropped(SelectExitEventArgs args)
        {
            _isGrabbed = false;
            ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Газоанализатор выпущен из рук", _ColoredDebug);

            if (_dropClip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_dropClip);
            }

            UpdateScreen();
        }

        private void UpdateScreen()
        {
            if (_backgroundImage == null || _iconImage == null) return;

            if (!_isGrabbed)
            {
                _backgroundImage.color = _colorWaiting;
                if (_waitingSprite != null) _iconImage.sprite = _waitingSprite;
            }
            else
            {
                if (_isNearGas)
                {
                    _backgroundImage.color = _colorAlert;
                    if (_alertSprite != null) _iconImage.sprite = _alertSprite;
                }
                else
                {
                    _backgroundImage.color = _colorOk;
                    if (_okSprite != null) _iconImage.sprite = _okSprite;
                }
            }

            HandleAlertSound();
        }

        private void HandleAlertSound()
        {
            if (_audioSource == null) return;

            bool shouldAlarm = _isGrabbed && _isNearGas;

            if (shouldAlarm)
            {
                if (_audioSource.clip != _alertClip || !_audioSource.isPlaying)
                {
                    _audioSource.clip = _alertClip;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }
            }
            else
            {
                if (_audioSource.clip == _alertClip)
                {
                    _audioSource.Stop();
                }
            }
        }
        #endregion
    }
}