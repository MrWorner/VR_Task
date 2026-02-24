using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))] 
public class BurnerController : MonoBehaviour
{
    [Header("Связь с ручкой")]
    public StoveKnobController stoveKnob;

    [Header("Визуальные эффекты")]
    public GameObject flameVisuals;
    public GameObject defectiveFlameVisuals;

    [Header("Аудио Клипы (Звуки)")]
    public AudioClip gasHissClip;
    public AudioClip ignitionOneShotClip;
    public AudioClip defectiveFireClip; 

    [Header("Настройки")]
    public bool isDefective = false;
    public string fireTag = "Fire";

    [Header("Текущее состояние")]
    public bool isGasFlowing = false;
    public bool isLit = false;

    [SerializeField] private AudioSource audioSource;
    private bool hasBeenTested = false;

    private void Awake()
    {
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (flameVisuals != null) flameVisuals.SetActive(false);
        if (defectiveFlameVisuals != null) defectiveFlameVisuals.SetActive(false);

        if (stoveKnob != null)
        {
            isGasFlowing = stoveKnob.isOn;
        }
    }

    private void Update()
    {
        if (stoveKnob != null && stoveKnob.isOn != isGasFlowing)
        {
            SetGasState(stoveKnob.isOn);
        }
    }

    public void SetGasState(bool state)
    {
        isGasFlowing = state;

        if (isGasFlowing)
        {
            if (!isLit && gasHissClip != null)
            {
                PlaySound(gasHissClip);
            }
        }
        else
        {
            isLit = false;
            if (flameVisuals != null) flameVisuals.SetActive(false);
            if (defectiveFlameVisuals != null) defectiveFlameVisuals.SetActive(false);

            audioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGasFlowing && !isLit)
        {
            Transform[] allParts = other.GetComponentsInChildren<Transform>();
            foreach (Transform part in allParts)
            {
                if (part.CompareTag(fireTag))
                {
                    Ignite();
                    break;
                }
            }
        }
    }

    private void Ignite()
    {
        isLit = true;

        if (!hasBeenTested)
        {
            hasBeenTested = true;
            GameManager.Instance.ReportBurnerTested();
        }

        if (isDefective)
        {
            if (defectiveFlameVisuals != null) defectiveFlameVisuals.SetActive(true);
            PlaySound(defectiveFireClip);
            Debug.Log("Конфорка зажглась с ОШИБКОЙ!");
        }
        else
        {
            if (flameVisuals != null) flameVisuals.SetActive(true);
            audioSource.PlayOneShot(ignitionOneShotClip);
            Debug.Log("Конфорка зажглась нормально.");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}