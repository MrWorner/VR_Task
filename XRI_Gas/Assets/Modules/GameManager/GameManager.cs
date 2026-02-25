// НАЗНАЧЕНИЕ: Управляет логикой победы и общим состоянием диагностики.

using UnityEngine;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    #region Поля: Required
    [BoxGroup("Required"), Required, SerializeField]
    [Tooltip("Перетащите сюда панель с сообщением об успехе")]
    private GameObject _successPanel;
    #endregion

    #region Поля
    [BoxGroup("SETTINGS"), SerializeField]
    [Tooltip("Сколько всего конфорок нужно проверить?")]
    private int _totalBurnersToTest = 5;

    [BoxGroup("DEBUG")]
    [SerializeField, ReadOnly] private int _burnersTested = 0;

    [BoxGroup("DEBUG")]
    [SerializeField, ReadOnly] private bool _isLeakFound = false;

    [BoxGroup("DEBUG")]
    [SerializeField, ReadOnly] private bool _isGameWon = false;

    [BoxGroup("DEBUG"), SerializeField] protected bool _ColoredDebug;
    #endregion

    #region Unity Методы
    private void Awake()
    {
        if (_instance != null && _instance != this)
            DebugUtils.LogInstanceAlreadyExists(this, _instance);
        else
            _instance = this;

        if (_successPanel == null)
            DebugUtils.LogMissingReference(this, nameof(_successPanel));
    }

    private void Start()
    {
        if (_successPanel != null)
            _successPanel.SetActive(false);
    }
    #endregion

    #region Публичные методы
    public void ReportBurnerTested()
    {
        _burnersTested++;
        ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Проверено конфорок: {0} / {1}", _ColoredDebug, _burnersTested, _totalBurnersToTest);
        CheckWinCondition();
    }

    public void ReportLeakFound()
    {
        if (!_isLeakFound)
        {
            _isLeakFound = true;
            ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Утечка успешно обнаружена!", _ColoredDebug);
            CheckWinCondition();
        }
    }
    #endregion

    #region Личные методы
    private void CheckWinCondition()
    {
        if (_isGameWon) return;

        if (_burnersTested >= _totalBurnersToTest && _isLeakFound)
        {
            _isGameWon = true;
            ShowSuccessMessage();
        }
    }

    private void ShowSuccessMessage()
    {
        ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> ДИАГНОСТИКА ПРОЙДЕНА УСПЕШНО!", _ColoredDebug);

        if (_successPanel != null)
        {
            _successPanel.SetActive(true);
        }
    }
    #endregion
}