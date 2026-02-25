// НАЗНАЧЕНИЕ: Управляет логикой работы конфорки, эффектами пламени и звуками. 
// ЗАВИСИМОСТИ: StoveKnobController, GameManager, AudioSource. 
// ПРИМЕЧАНИЕ: Поддерживает логирование через ColoredDebug и работает без GetComponent. 

using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class BurnerController : MonoBehaviour
{
    #region Поля: Required
    [BoxGroup("Required"), Required, SerializeField]
    private StoveKnobController _stoveKnob;

    [BoxGroup("Required"), Required, SerializeField]
    private GameObject _flameVisuals;

    [BoxGroup("Required"), Required, SerializeField]
    private GameObject _defectiveFlameVisuals;

    [BoxGroup("Required"), Required, SerializeField]
    private AudioClip _gasHissClip;

    [BoxGroup("Required"), Required, SerializeField]
    private AudioClip _ignitionOneShotClip;

    [BoxGroup("Required"), Required, SerializeField]
    private AudioClip _defectiveFireClip;

    [BoxGroup("Required"), Required, SerializeField]
    private AudioSource _audioSource;
    #endregion 

    #region Поля
    [BoxGroup("SETTINGS"), SerializeField]
    private bool _isDefective = false;

    [BoxGroup("SETTINGS"), SerializeField]
    private string _fireTag = "StickFlame";

    [BoxGroup("DEBUG")]
    [SerializeField, ReadOnly] private bool _isGasFlowing = false;

    [BoxGroup("DEBUG")]
    [SerializeField, ReadOnly] private bool _isLit = false;

    [BoxGroup("DEBUG")]
    [SerializeField, ReadOnly] private bool _hasBeenTested = false;

    [BoxGroup("DEBUG"), SerializeField]
    protected bool _ColoredDebug;
    #endregion

    #region Свойства
    public bool IsDefective => _isDefective;
    public bool IsGasFlowing => _isGasFlowing;
    public bool IsLit => _isLit;
    #endregion 

    #region Unity Методы
    private void Awake()
    {
        if (_audioSource == null) DebugUtils.LogMissingReference(this, nameof(_audioSource));
        if (_stoveKnob == null) DebugUtils.LogMissingReference(this, nameof(_stoveKnob));

        if (_audioSource != null)
        {
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (_flameVisuals != null) _flameVisuals.SetActive(false);
        if (_defectiveFlameVisuals != null) _defectiveFlameVisuals.SetActive(false);

        if (_stoveKnob != null)
        {
            _isGasFlowing = _stoveKnob.isOn;
        }
    }

    private void Update()
    {
        if (_stoveKnob != null && _stoveKnob.isOn != _isGasFlowing)
        {
            SetGasState(_stoveKnob.isOn);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isGasFlowing && !_isLit)
        {
            // Рекурсивная проверка тега без использования тяжелого GetComponentInChildren
            if (TryFindFireTag(other.transform, out bool hasFire) && hasFire)
            {
                Ignite();
            }
        }
    }
    #endregion 

    #region Публичные методы
    public void SetGasState(bool state)
    {
        _isGasFlowing = state;

        if (_isGasFlowing)
        {
            if (!_isLit && _gasHissClip != null)
            {
                PlaySound(_gasHissClip);
            }
        }
        else
        {
            _isLit = false;
            if (_flameVisuals != null) _flameVisuals.SetActive(false);
            if (_defectiveFlameVisuals != null) _defectiveFlameVisuals.SetActive(false);

            if (_audioSource != null) _audioSource.Stop();
        }
    }
    #endregion 

    #region Личные методы
    private void Ignite()
    {
        _isLit = true;

        if (!_hasBeenTested)
        {
            _hasBeenTested = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReportBurnerTested();
            }
        }

        if (_isDefective)
        {
            if (_defectiveFlameVisuals != null) _defectiveFlameVisuals.SetActive(true);
            PlaySound(_defectiveFireClip);

            ColoredDebug.CLog(gameObject, "<color=orange>[SYSTEM]</color> Конфорка зажглась с ОШИБКОЙ!", _ColoredDebug);
        }
        else
        {
            if (_flameVisuals != null) _flameVisuals.SetActive(true);
            if (_audioSource != null) _audioSource.Stop();
            if (_audioSource != null) _audioSource.PlayOneShot(_ignitionOneShotClip);

            ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> Конфорка зажглась нормально.", _ColoredDebug);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || _audioSource == null) return;
        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private bool TryFindFireTag(Transform rootTransform, out bool result)
    {
        result = false;

        if (rootTransform.CompareTag(_fireTag))
        {
            result = true;
            return true;
        }

        for (int i = 0; i < rootTransform.childCount; i++)
        {
            if (TryFindFireTag(rootTransform.GetChild(i), out result) && result)
            {
                return true;
            }
        }

        return false;
    }
    #endregion 
}