using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BurnerController : MonoBehaviour
{
    [Header("Связь с ручкой")]
    [Tooltip("Перетащите сюда объект ручки управления (на котором висит StoveKnobController)")]
    public StoveKnobController stoveKnob;

    [Header("Визуал и Звук")]
    [Tooltip("Перетащите сюда объект пламени конфорки")]
    public GameObject flameVisuals;

    [Tooltip("Звук выхода газа (шипение)")]
    public AudioSource gasHissSound;

    [Tooltip("Звук горения пламени")]
    public AudioSource fireSound;

    [Header("Текущее состояние")]
    public bool isGasFlowing = false;
    public bool isLit = false;
    public string fireTag = "Fire";

    private void Start()
    {
        if (flameVisuals != null) flameVisuals.SetActive(false);

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
            if (!isLit && gasHissSound != null)
            {
                gasHissSound.Play();
            }
        }
        else
        {
            isLit = false;

            if (flameVisuals != null) flameVisuals.SetActive(false);
            if (gasHissSound != null) gasHissSound.Stop();
            if (fireSound != null) fireSound.Stop();
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

        if (flameVisuals != null) flameVisuals.SetActive(true);

        if (gasHissSound != null) gasHissSound.Stop();
        if (fireSound != null) fireSound.Play();

        Debug.Log("Конфорка успешно зажжена!");

        // GameManager.Instance.isBurnerTested = true;
        // GameManager.Instance.CheckWinCondition();
    }
}