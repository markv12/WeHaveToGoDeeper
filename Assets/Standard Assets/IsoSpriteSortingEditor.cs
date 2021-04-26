#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

#pragma warning disable 618

[CustomEditor(typeof(IsoSpriteSorting))]
public class IsoSpriteSortingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        IsoSpriteSorting myScript = (IsoSpriteSorting)target;
        if (GUILayout.Button("Sort Visible Scene"))
        {
            myScript.SortScene();
        }
    }
}
#endif
