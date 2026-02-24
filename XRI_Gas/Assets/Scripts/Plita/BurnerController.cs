using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BurnerController : MonoBehaviour
{
    [Header("Связь с ручкой")]
    public StoveKnobController stoveKnob;

    [Header("Нормальная работа (Визуал и Звук)")]
    public GameObject flameVisuals;
    public AudioSource gasHissSound;
    public AudioSource fireSound;

    [Header("Неисправность (Визуал и Звук)")]
    [Tooltip("Поставьте галочку, чтобы эта конфорка работала с ошибкой")]
    public bool isDefective = false;
    [Tooltip("Перетащите сюда эффект дерганого/желтого пламени")]
    public GameObject defectiveFlameVisuals;
    [Tooltip("Перетащите сюда звук хлопков или потрескивания")]
    public AudioSource defectiveFireSound;

    [Header("Текущее состояние")]
    public bool isGasFlowing = false;
    public bool isLit = false;
    public string fireTag = "Fire";
    private bool hasBeenTested = false;
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
            if (!isLit && gasHissSound != null)
            {
                gasHissSound.Play();
            }
        }
        else
        {
            isLit = false;

            if (flameVisuals != null) flameVisuals.SetActive(false);
            if (defectiveFlameVisuals != null) defectiveFlameVisuals.SetActive(false);
            if (gasHissSound != null) gasHissSound.Stop();
            if (fireSound != null) fireSound.Stop();
            if (defectiveFireSound != null) defectiveFireSound.Stop();
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
        if (gasHissSound != null) gasHissSound.Stop();

        if (!hasBeenTested)
        {
            hasBeenTested = true;
            GameManager.Instance.ReportBurnerTested();
        }

        if (isDefective)
        {
            if (defectiveFlameVisuals != null) defectiveFlameVisuals.SetActive(true);
            if (defectiveFireSound != null) defectiveFireSound.Play();
            Debug.Log("Конфорка зажглась с ОШИБКОЙ (хлопки)!");
        }
        else
        {
            if (flameVisuals != null) flameVisuals.SetActive(true);
            if (fireSound != null) fireSound.Play();
            Debug.Log("Конфорка зажглась нормально.");
        }
    }
}