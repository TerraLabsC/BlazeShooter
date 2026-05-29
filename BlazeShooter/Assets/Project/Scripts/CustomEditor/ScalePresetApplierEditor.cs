using UnityEditor;
using UnityEngine;

/// <summary>
/// Добавляет в инспектор кнопки для быстрого применения масштаба.
/// </summary>
[CustomEditor(typeof(ScalePresetApplier))]
public class ScalePresetApplierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Стандартное отображение полей scalePreset1 и scalePreset2
        DrawDefaultInspector();

        ScalePresetApplier applier = (ScalePresetApplier)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Быстрое применение:", EditorStyles.boldLabel);

        // Кнопка 1
        if (GUILayout.Button("Установить масштаб 1 (0,0,0)", GUILayout.Height(30)))
        {
            // Запись в Undo, чтобы работал Ctrl+Z
            Undo.RecordObject(applier.transform, "Применить масштаб 1");
            applier.ApplyPreset1();
        }

        // Кнопка 2
        if (GUILayout.Button("Установить масштаб 2 (0.057, 0.243, 0.057)", GUILayout.Height(30)))
        {
            Undo.RecordObject(applier.transform, "Применить масштаб 2");
            applier.ApplyPreset2();
        }
    }
}