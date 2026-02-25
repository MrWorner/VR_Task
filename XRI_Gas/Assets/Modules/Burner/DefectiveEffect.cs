// НАЗНАЧЕНИЕ: Управляет эффектом случайного мерцания группы объектов (например, язычков пламени).

using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Threading;
using UnityEngine;

namespace GasStoveSimulator.Modules.Burner
{
    public class DefectiveEffect : MonoBehaviour
    {
        #region Поля
        [BoxGroup("SETTINGS"), Tooltip("Минимальное время (в секундах)"), SerializeField]
        private float _minTime = 0.05f;

        [BoxGroup("SETTINGS"), Tooltip("Максимальное время (в секундах)"), SerializeField]
        private float _maxTime = 0.2f;

        [BoxGroup("SETTINGS"), Tooltip("Язычки мерцают вразнобой (true) или одновременно (false)"), SerializeField]
        private bool _independentFlicker = true;

        [BoxGroup("SETTINGS"), Tooltip("Шанс горения при независимом мерцании (было 0.4)"), SerializeField, Range(0f, 1f)]
        private float _burnThresholdIndependent = 0.4f;

        [BoxGroup("SETTINGS"), Tooltip("Шанс горения при синхронном мерцании (было 0.5)"), SerializeField, Range(0f, 1f)]
        private float _burnThresholdUnified = 0.5f;

        [BoxGroup("DEBUG")]
        [SerializeField, ReadOnly] private Transform[] _flames;

        [BoxGroup("DEBUG"), SerializeField]
        protected bool _ColoredDebug;

        private CancellationTokenSource _cancellationTokenSource;
        #endregion

        #region Unity Методы
        private void Awake()
        {
            _flames = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                _flames[i] = transform.GetChild(i);
            }

            ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> Инициализация завершена, собрано объектов: {0}", _ColoredDebug, _flames.Length);
        }

        private void OnEnable()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            UpdateFlames();
            FlickerRoutineAsync(_cancellationTokenSource.Token).Forget();
        }

        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            if (_flames != null)
            {
                foreach (Transform flame in _flames)
                {
                    if (flame != null) flame.gameObject.SetActive(false);
                }
            }

            ColoredDebug.CLog(gameObject, "<color=orange>[SYSTEM]</color> Мерцание отключено.", _ColoredDebug);
        }
        #endregion

        #region Личные методы
        private async UniTaskVoid FlickerRoutineAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    float waitTime = UnityEngine.Random.Range(_minTime, _maxTime);
                    await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

                    UpdateFlames();
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                ColoredDebug.CLog(gameObject, "<color=red>[ERROR]</color> {0}", _ColoredDebug, e.Message);
            }
        }

        private void UpdateFlames()
        {
            if (_flames == null) return;

            if (_independentFlicker)
            {
                foreach (Transform flame in _flames)
                {
                    bool isBurning = UnityEngine.Random.value > _burnThresholdIndependent;
                    flame.gameObject.SetActive(isBurning);
                }
            }
            else
            {
                bool isBurning = UnityEngine.Random.value > _burnThresholdUnified;
                foreach (Transform flame in _flames)
                {
                    flame.gameObject.SetActive(isBurning);
                }
            }
        }
        #endregion
    }
}