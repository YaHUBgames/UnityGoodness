using UnityEngine;
using UnityEditor;

namespace EditorGoodness
{
    /// <summary>
    /// Hierarchy Window Group Header
    /// http://diegogiacomelli.com.br/unitytips-hierarchy-window-group-header
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyWindowGroupHeader
    {
        static HierarchyWindowGroupHeader()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject is null)
                return;

            OnSpecialTextToColor(gameObject, "---", selectionRect, Color.gray);
            OnSpecialTextToColor(gameObject, "-white-", selectionRect, Color.white);
            OnSpecialTextToColor(gameObject, "-black-", selectionRect, Color.black);
            OnSpecialTextToColor(gameObject, "-red-", selectionRect, Color.red);
            OnSpecialTextToColor(gameObject, "-green-", selectionRect, Color.green);
            OnSpecialTextToColor(gameObject, "-blue-", selectionRect, Color.blue);
            OnSpecialTextToColor(gameObject, "-yellow-", selectionRect, Color.yellow);
            OnSpecialTextToColor(gameObject, "-magenta-", selectionRect, Color.magenta);
            OnSpecialTextToColor(gameObject, "-cyan-", selectionRect, Color.cyan);
            OnSpecialTextToColor(gameObject, "-orange-", selectionRect, new Color(1f, 0.4f, 0f, 1f));
        }

        static void OnSpecialTextToColor(GameObject _gameObject, string _startsWith, Rect _selectionRect, Color _color)
        {
            if (_gameObject.name.StartsWith(_startsWith, System.StringComparison.Ordinal))
            {
                EditorGUI.DrawRect(_selectionRect, _color);
                EditorGUI.DropShadowLabel(_selectionRect, _gameObject.name.Replace(_startsWith, "").ToUpperInvariant());
            }
        }
    }
}
