using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Layout
{
    public class LayoutsMenuItem : MonoBehaviour //monobehaviour is required for destroy immediate/instantiate etc.
    {
        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Layouts/Grid", false, 30001)]
        static void CreateGrid(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Grid Layout (M3D)");
            go.AddComponent<GridLayoutGroup>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Layouts/Circular", false, 30002)]
        static void CreateCircle(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Circular Layout (M3D)");
            go.AddComponent<CircularLayoutGroup>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Layouts/Linear", false, 30003)]
        static void CreateLine(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Linear Layout (M3D)");
            go.AddComponent<LinearLayoutGroup>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}