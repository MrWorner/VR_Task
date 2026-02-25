// НАЗНАЧЕНИЕ: Утилита для массового переключения логов на сцене.
// ЗАВИСИМОСТИ: NaughtyAttributes, UnityEditor.

using UnityEngine;
using NaughtyAttributes; // 
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

public class ColoredDebugToggler : MonoBehaviour
{
    #region Поля
    [BoxGroup("DEBUG"), SerializeField] private bool _ColoredDebug;
    #endregion

    #region Публичные методы
    [Button("Включить ВСЕ логи")]
    public void EnableAllLogs() => ToggleAllLogs(true);

    [Button("Выключить ВСЕ логи")]
    public void DisableAllLogs() => ToggleAllLogs(false);
    #endregion

    #region Личные методы
    private void ToggleAllLogs(bool isEnabled)
    {
        var components = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        int changedCount = 0;

        ColoredDebug.CLog(gameObject, "<color=cyan>[INFO]</color> ColoredDebugToggler: Поиск...", _ColoredDebug);

        foreach (var comp in components)
        {
            if (comp == null) continue;

            FieldInfo field = comp.GetType().GetField("_ColoredDebug", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(bool))
            {
                field.SetValue(comp, isEnabled);
                EditorUtility.SetDirty(comp);
                changedCount++;
            }
        }

        ColoredDebug.CLog(gameObject, "<color=lime>[ACTION]</color> Изменено компонентов: {0}. Состояние: {1}", _ColoredDebug, changedCount, isEnabled);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
    #endregion
}
#endif