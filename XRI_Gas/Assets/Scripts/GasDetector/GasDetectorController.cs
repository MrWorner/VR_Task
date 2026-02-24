using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с UI Image
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GasDetectorController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Перетащите сюда объект Background из Canvas")]
    public Image backgroundImage;
    [Tooltip("Перетащите сюда объект Icon из Canvas")]
    public Image iconImage;

    [Header("Спрайты (Иконки)")]
    public Sprite waitingSprite;
    public Sprite okSprite;
    public Sprite alertSprite;

    [Header("Цвета фона")]
    public Color colorWaiting = Color.black;
    public Color colorOk = Color.gray;
    public Color colorAlert = Color.red;

    [Header("Звук и Настройки")]
    [Tooltip("Звук писка при обнаружении газа")]
    public AudioSource alertSound;
    [Tooltip("Тег зоны, где происходит утечка")]
    public string gasZoneTag = "GasLeakZone";

    [SerializeField] private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private bool isNearGas = false;


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
        UpdateScreen();
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        isGrabbed = false;
        UpdateScreen();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gasZoneTag))
        {
            isNearGas = true;
            UpdateScreen();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(gasZoneTag))
        {
            isNearGas = false;
            UpdateScreen();
        }
    }

    private void UpdateScreen()
    {
        if (!isGrabbed)
        {
            backgroundImage.color = colorWaiting;
            if (waitingSprite != null) iconImage.sprite = waitingSprite;

            if (alertSound != null) alertSound.Stop();
        }
        else
        {
            if (isNearGas)
            {
                backgroundImage.color = colorAlert;
                if (alertSprite != null) iconImage.sprite = alertSprite;

                if (alertSound != null && !alertSound.isPlaying) alertSound.Play();
            }
            else
            {
                backgroundImage.color = colorOk;
                if (okSprite != null) iconImage.sprite = okSprite;

                if (alertSound != null) alertSound.Stop();
            }
        }
    }
}