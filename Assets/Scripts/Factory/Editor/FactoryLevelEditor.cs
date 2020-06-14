using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FactoryLevelEditor : EditorWindow
{
    [MenuItem("Tools/Factory Level Editor")]
    public static void Init()
    {
        GetWindow<FactoryLevelEditor>();
    }

    private static readonly GUIContent Title = new GUIContent("Factory Level Editor");

    private FlatConveyorBelt.SpecialBeltType _beltType;
    private bool _isSpecialConveyor;
    private bool _isCurved;


    private void OnGUI()
    {
        titleContent = Title;

        _isCurved = GUILayout.Toggle(_isCurved, "Only curved (only flat when off)");
        _isSpecialConveyor = GUILayout.Toggle(_isSpecialConveyor, "Special conveyor (only non-special when off)");

        if (_isSpecialConveyor)
        {
            _beltType = (FlatConveyorBelt.SpecialBeltType) EditorGUILayout.EnumPopup("Special belt type", _beltType);
        }

        if (GUILayout.Button("Filter"))
        {
            if (_isCurved)
            {
                Filter<FlatConveyorBeltCurve>();
            }
            else
            {
                Filter<FlatConveyorBelt>();
            }
        }

        EditorGUILayout.Separator();

        if (GUILayout.Button("Select ALL belts"))
        {
            Selection.objects = Resources.FindObjectsOfTypeAll<FlatConveyorBelt>()
                .Where(b => b.gameObject.scene == SceneManager.GetActiveScene()).Select(b => b.gameObject).ToArray();
        }
    }

    private void Filter<T>() where T : FlatConveyorBelt
    {
        var allBelts = Resources.FindObjectsOfTypeAll<T>();

        var filteredObjects = (IEnumerable<T>) allBelts;

        if (_isSpecialConveyor)
            filteredObjects = filteredObjects.Where(b => { return b.IsSpecialConveyor && b.BeltType == _beltType; });


        filteredObjects = filteredObjects.Where(b => b.GetType() == typeof(T));

        Selection.objects = filteredObjects
            .Where(b => b.hideFlags != HideFlags.NotEditable &&
                        b.gameObject.scene == SceneManager.GetActiveScene())
            .Select(b => b.gameObject).ToArray();
    }
}