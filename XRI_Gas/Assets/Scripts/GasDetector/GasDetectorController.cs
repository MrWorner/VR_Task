using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(AudioSource))]
public class GasDetectorController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public Image backgroundImage;
    public Image iconImage;

    [Header("Спрайты (Иконки)")]
    public Sprite waitingSprite;
    public Sprite okSprite;
    public Sprite alertSprite;

    [Header("Цвета фона")]
    public Color colorWaiting = Color.black;
    public Color colorOk = Color.gray;
    public Color colorAlert = Color.red;

    [Header("Аудио Клипы")]
    [Tooltip("Звук сирены при обнаружении газа (зациклен)")]
    public AudioClip alertClip;
    [Tooltip("Короткий Beep при взятии в руку")]
    public AudioClip grabClip;
    [Tooltip("Милый писк при выбрасывании предмета")]
    public AudioClip dropClip;

    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;
    private bool isGrabbed = false;
    private bool isNearGas = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; 
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

    private void Start()
    {
        UpdateScreen();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        if (grabClip != null)
        {
            audioSource.PlayOneShot(grabClip);
        }

        UpdateScreen();
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        isGrabbed = false;

        if (dropClip != null)
        {
            audioSource.PlayOneShot(dropClip);
        }

        UpdateScreen();
    }

    public void SetGasDetected(bool hasGas)
    {
        if (isNearGas == hasGas) return;

        isNearGas = hasGas;

        if (isNearGas)
        {
            if (GameManager.Instance != null) GameManager.Instance.ReportLeakFound();
        }

        UpdateScreen();
    }

    private void UpdateScreen()
    {
        if (!isGrabbed)
        {
            backgroundImage.color = colorWaiting;
            if (waitingSprite != null) iconImage.sprite = waitingSprite;
        }
        else
        {
            if (isNearGas)
            {
                backgroundImage.color = colorAlert;
                if (alertSprite != null) iconImage.sprite = alertSprite;
            }
            else
            {
                backgroundImage.color = colorOk;
                if (okSprite != null) iconImage.sprite = okSprite;
            }
        }

        HandleAlertSound();
    }

    private void HandleAlertSound()
    {
        bool shouldAlarm = isGrabbed && isNearGas;

        if (shouldAlarm)
        {
            if (audioSource.clip != alertClip || !audioSource.isPlaying)
            {
                audioSource.clip = alertClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip == alertClip)
            {
                audioSource.Stop();
            }
        }
    }
}