using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GasSensorTip : MonoBehaviour
{
    [Tooltip("Перетащите сюда корневой объект GasDetector")]
    public GasDetectorController mainController;

    private List<Collider> activeZones = new List<Collider>();

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GasLeakZone") || other.CompareTag("BurnerZone"))
        {
            if (!activeZones.Contains(other))
            {
                activeZones.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (activeZones.Contains(other))
        {
            activeZones.Remove(other);
        }
    }

    private void Update()
    {
        if (mainController == null) return;

        bool detectGas = false;

        for (int i = activeZones.Count - 1; i >= 0; i--)
        {
            Collider zone = activeZones[i];

            if (zone == null || !zone.gameObject.activeInHierarchy)
            {
                activeZones.RemoveAt(i);
                continue;
            }

            if (zone.CompareTag("GasLeakZone"))
            {
                detectGas = true;
                break;
            }
            else if (zone.CompareTag("BurnerZone"))
            {
                BurnerController burner = zone.GetComponent<BurnerController>();

                if (burner != null && burner.isGasFlowing && !burner.isLit)
                {
                    detectGas = true;
                    break;
                }
            }
        }

        mainController.SetGasDetected(detectGas);
    }
}