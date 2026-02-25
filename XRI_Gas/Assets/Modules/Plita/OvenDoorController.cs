// НАЗНАЧЕНИЕ: Контроллер дверцы духовки для VR-взаимодействия.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using NaughtyAttributes;
using DG.Tweening;

[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(AudioSource))]
public class OvenDoorController : MonoBehaviour
{
    #region Поля: Required
    [BoxGroup("Required"), Required, SerializeField]
    private Transform _doorTransform;

    [BoxGroup("Required"), Required, SerializeField]
    private XRSimpleInteractable _interactable;

    [BoxGroup("Required"), Required, SerializeField]
    private AudioSource _audioSource;
    #endregion

    #region Поля
    [BoxGroup("SETTINGS"), SerializeField]
    private Vector3 _closedRotation = Vector3.zero;

    [BoxGroup("SETTINGS"), SerializeField]
    private Vector3 _openRotation = new Vector3(90f, 0, 0);

    [BoxGroup("SETTINGS"), SerializeField]
    [Tooltip("Длительность анимации (вместо скорости)")]
    private float _animationDuration = 0.5f;

    [BoxGroup("SETTINGS"), SerializeField]
    private AudioClip _openClip;

    [BoxGroup("SETTINGS"), SerializeField]
    private AudioClip _closeClip;

    [BoxGroup("DEBUG"), SerializeField, ReadOnly]
    private bool _isOpened;

    [BoxGroup("DEBUG"), SerializeField]
    protected bool _ColoredDebug;
    #endregion

    #region Свойства
    public bool IsOpened => _isOpened;
    #endregion

    #region Unity Методы
    private void Awake()
    {
        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
        }
        else
        {
            ColoredDebug.CLog(gameObject, "<color=red>[ERROR]</color> Отсутствует ссылка на AudioSource!", _ColoredDebug);
        }

        if (_doorTransform == null || _interactable == null)
        {
            ColoredDebug.CLog(gameObject, "<color=red>[ERROR]</color> Не все обязательные ссылки (Required) назначены!", _ColoredDebug);
        }
    }

    private void OnEnable()
    {
        if (_interactable != null)
            _interactable.activated.AddListener(OnDoorActivated);
    }

    private void OnDisable()
    {
        if (_interactable != null)
            _interactable.activated.RemoveListener(OnDoorActivated);
    }
    #endregion

    #region Публичные методы
    [Button("Toggle Door")]
    public void ToggleDoor()
    {
        _isOpened = !_isOpened;

        PlaySound();
        AnimateDoor();

        ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Дверца переключена. IsOpened: {0}", _ColoredDebug, _isOpened);
    }
    #endregion

    #region Личные методы
    private void OnDoorActivated(ActivateEventArgs args)
    {
        ToggleDoor();
    }

    private void PlaySound()
    {
        if (_audioSource == null) return;

        AudioClip clipToPlay = _isOpened ? _openClip : _closeClip;

        if (clipToPlay != null)
            _audioSource.PlayOneShot(clipToPlay);
    }

    private void AnimateDoor()
    {
        if (_doorTransform == null) return;

        Vector3 targetEuler = _isOpened ? _openRotation : _closedRotation;

        var seq = DOTween.Sequence();
        seq.Append(_doorTransform.DOLocalRotate(targetEuler, _animationDuration));
        seq.OnComplete(() =>
        {
            ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> Анимация дверцы завершена.", _ColoredDebug);
        });
    }
    #endregion
}