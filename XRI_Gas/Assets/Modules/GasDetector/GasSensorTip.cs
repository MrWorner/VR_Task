// НАЗНАЧЕНИЕ: Отслеживание зон утечки газа и конфорок для детектора газа.

using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Collider))]
public class GasSensorTip : MonoBehaviour
{
    #region Поля: Required
    [BoxGroup("Required"), Required, SerializeField]
    private GasDetectorController _mainController;
    #endregion

    #region Поля
    [BoxGroup("SETTINGS"), SerializeField]
    private string _gasLeakZoneTag = "GasLeakZone";

    [BoxGroup("SETTINGS"), SerializeField]
    private string _burnerZoneTag = "BurnerZone";

    [BoxGroup("DEBUG"), SerializeField, ReadOnly]
    private List<Collider> _activeZones = new List<Collider>();

    [BoxGroup("DEBUG"), SerializeField]
    protected bool _ColoredDebug;
    #endregion

    #region Свойства
    #endregion

    #region Unity Методы
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_gasLeakZoneTag) || other.CompareTag(_burnerZoneTag))
        {
            if (!_activeZones.Contains(other))
            {
                _activeZones.Add(other);
                ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Вход в зону: {0}", _ColoredDebug, other.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_activeZones.Contains(other))
        {
            _activeZones.Remove(other);
            ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Выход из зоны: {0}", _ColoredDebug, other.name);
        }
    }

    private void Update()
    {
        if (_mainController == null) return;

        bool detectGas = false;

        for (int i = _activeZones.Count - 1; i >= 0; i--)
        {
            Collider zone = _activeZones[i];

            if (zone == null || !zone.gameObject.activeInHierarchy)
            {
                _activeZones.RemoveAt(i);
                continue;
            }

            if (zone.CompareTag(_gasLeakZoneTag))
            {
                detectGas = true;
                break;
            }
            else if (zone.CompareTag(_burnerZoneTag))
            {
                if (zone.TryGetComponent(out BurnerController burner))
                {
                    if (burner.IsGasFlowing && !burner.IsLit)
                    {
                        detectGas = true;
                        break;
                    }
                }
            }
        }

        _mainController.SetGasDetected(detectGas);
    }
    #endregion
}