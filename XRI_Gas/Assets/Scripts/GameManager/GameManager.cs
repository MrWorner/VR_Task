using UnityEngine;
using TMPro; // Для работы с текстом UI

public class GameManager : MonoBehaviour
{
    // Singleton - чтобы другие скрипты могли легко к нему обращаться
    public static GameManager Instance { get; private set; }

    [Header("Настройки победы")]
    [Tooltip("Сколько всего конфорок нужно проверить?")]
    public int totalBurnersToTest = 4;

    [Header("UI (Интерфейс)")]
    [Tooltip("Перетащите сюда панель с сообщением об успехе")]
    public GameObject successPanel;

    // Внутренние счетчики
    private int burnersTested = 0;
    private bool isLeakFound = false;
    private bool isGameWon = false;

    private void Awake()
    {
        // Настраиваем Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Прячем табличку победы при старте
        if (successPanel != null) successPanel.SetActive(false);
    }

    // Метод, который мы вызовем из конфорки
    public void ReportBurnerTested()
    {
        burnersTested++;
        Debug.Log($"Проверено конфорок: {burnersTested} / {totalBurnersToTest}");
        CheckWinCondition();
    }

    // Метод, который мы вызовем из газоанализатора
    public void ReportLeakFound()
    {
        if (!isLeakFound)
        {
            isLeakFound = true;
            Debug.Log("Утечка успешно обнаружена!");
            CheckWinCondition();
        }
    }

    // Проверяем, выполнил ли игрок ВСЕ условия
    private void CheckWinCondition()
    {
        if (isGameWon) return; // Если уже победили, ничего не делаем

        if (burnersTested >= totalBurnersToTest && isLeakFound)
        {
            isGameWon = true;
            ShowSuccessMessage();
        }
    }

    private void ShowSuccessMessage()
    {
        Debug.Log("ДИАГНОСТИКА ПРОЙДЕНА УСПЕШНО!");

        if (successPanel != null)
        {
            successPanel.SetActive(true);

            // Опционально: можно добавить звук победы
            // GetComponent<AudioSource>()?.Play();
        }
    }
}