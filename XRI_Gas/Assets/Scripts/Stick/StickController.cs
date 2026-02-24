using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class StickController : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    [Tooltip("Перетащите сюда объект Flame Orange")]
    public GameObject flameVisuals;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        // Получаем компонент XRI, который уже висит на объекте Stick
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Гарантируем, что на старте огонь выключен, если спичку никто не держит
        if (flameVisuals != null)
        {
            flameVisuals.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Подписываемся на события взятия и отпускания объекта
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnDropped);
    }

    private void OnDisable()
    {
        // Обязательно отписываемся от событий, чтобы избежать ошибок при удалении объекта
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnDropped);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Игрок взял спичку -> Включаем пламя
        if (flameVisuals != null)
        {
            flameVisuals.SetActive(true);
        }
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        // Игрок отпустил спичку -> Выключаем пламя
        if (flameVisuals != null)
        {
            flameVisuals.SetActive(false);
        }
    }
}