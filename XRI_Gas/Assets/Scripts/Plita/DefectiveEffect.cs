using System.Collections;
using UnityEngine;

public class DefectiveEffect : MonoBehaviour
{
    [Header("Тайминги мерцания (в секундах)")]
    [Tooltip("Минимальное время (0.05 = 50 миллисекунд)")]
    public float minTime = 0.05f;

    [Tooltip("Максимальное время (0.2 = 200 миллисекунд)")]
    public float maxTime = 0.2f;

    [Header("Настройки эффекта")]
    [Tooltip("Если включено, язычки пламени мерцают вразнобой. Если выключено - всё пламя гаснет и загорается целиком.")]
    public bool independentFlicker = true;

    private Transform[] flames;

    private void Awake()
    {
        flames = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            flames[i] = transform.GetChild(i);
        }
    }

    private void OnEnable()
    {
        // Зажигаем огонь МГНОВЕННО при активации, не дожидаясь таймера
        UpdateFlames();
        StartCoroutine(FlickerRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (flames != null)
        {
            foreach (Transform flame in flames)
            {
                flame.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            // После ожидания обновляем состояние пламени
            UpdateFlames();
        }
    }

    private void UpdateFlames()
    {
        if (flames == null) return;

        if (independentFlicker)
        {
            foreach (Transform flame in flames)
            {
                // 60% шанс, что горит, 40% - что гаснет
                bool isBurning = Random.value > 0.4f;
                flame.gameObject.SetActive(isBurning);
            }
        }
        else
        {
            bool isBurning = Random.value > 0.5f;
            foreach (Transform flame in flames)
            {
                flame.gameObject.SetActive(isBurning);
            }
        }
    }
}