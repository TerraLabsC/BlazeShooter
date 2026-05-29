using UnityEditor;
using UnityEngine;

/// <summary>
/// Отдельное окно для массового применения и запоминания масштабных пресетов
/// ко всем объектам с компонентом ScalePresetApplier.
/// </summary>
public class ScalePresetGlobalWindow : EditorWindow
{
    [MenuItem("Tools/Scale Presets Global Window")]
    public static void ShowWindow()
    {
        GetWindow<ScalePresetGlobalWindow>("Scale Presets (Global)");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Применить пресет ко ВСЕМ объектам на сцене:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("(у которых есть компонент ScalePresetApplier)", EditorStyles.miniLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Применить Пресет 1 ко всем - скейл 0", GUILayout.Height(40)))
        {
            ApplyPresetToAll(presetIndex: 1);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Применить Пресет 2 ко всем - скейл объектов", GUILayout.Height(40)))
        {
            ApplyPresetToAll(presetIndex: 2);
        }

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Запоминание текущего масштаба:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Сохранить scale всех объектов в Preset2", EditorStyles.miniLabel);
        GUILayout.Space(5);

        if (GUILayout.Button("Запомнить все scale в Preset2", GUILayout.Height(40)))
        {
            RememberAllScalesToPreset2();
        }
    }

    private void ApplyPresetToAll(int presetIndex)
    {
        ScalePresetApplier[] allAppliers = FindObjectsOfType<ScalePresetApplier>(true);

        if (allAppliers.Length == 0)
        {
            Debug.LogWarning("На сцене нет объектов с компонентом ScalePresetApplier.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        foreach (ScalePresetApplier applier in allAppliers)
        {
            Undo.RecordObject(applier.transform, "Mass Scale Change");

            if (presetIndex == 1)
                applier.ApplyPreset1();
            else
                applier.ApplyPreset2();
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"Пресет {presetIndex} применён к {allAppliers.Length} объектам.");
    }

    private void RememberAllScalesToPreset2()
    {
        ScalePresetApplier[] allAppliers = FindObjectsOfType<ScalePresetApplier>(true);

        if (allAppliers.Length == 0)
        {
            Debug.LogWarning("На сцене нет объектов с компонентом ScalePresetApplier.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        foreach (ScalePresetApplier applier in allAppliers)
        {
            // Записываем изменение самого компонента (меняем scalePreset2)
            Undo.RecordObject(applier, "Remember Scale to Preset2");
            applier.scalePreset2 = applier.transform.localScale;
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"Текущий масштаб {allAppliers.Length} объектов сохранён в Preset2.");
    }
}