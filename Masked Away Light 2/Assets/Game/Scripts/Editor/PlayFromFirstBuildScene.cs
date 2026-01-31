// Assets/Editor/PlayInitPanel.cs
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorTools
{
    internal static class PlayFromFirstBuildScene
    {
        public static void Run()
        {
            // Optional: if already playing, stop.
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            // Ask to save any modified scenes.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            // First ENABLED scene in Build Settings (recommended).
            var first = EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled);

            if (first == null || string.IsNullOrEmpty(first.path))
            {
                Debug.LogError("No enabled scenes found in Build Settings. Add/enable your Initialization scene first.");
                return;
            }

            EditorSceneManager.OpenScene(first.path, OpenSceneMode.Single);

            // Start play mode next tick (ensures the scene is loaded).
            EditorApplication.delayCall += () =>
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    EditorApplication.isPlaying = true;
            };
        }
    }

    /// <summary>
    /// Dockable panel you can place anywhere in the Unity Editor layout.
    /// </summary>
    public class PlayInitPanel : EditorWindow
    {
        private const string WindowTitle = "Play Init";

        [MenuItem("Tools/Play Init Panel", priority = 2000)]
        public static void Open()
        {
            // Opens a dockable window.
            var window = GetWindow<PlayInitPanel>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(220, 80);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Play from Initialization scene", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Opens the first enabled Build Settings scene and enters Play Mode.",
                    EditorStyles.wordWrappedMiniLabel);

                EditorGUILayout.Space(8);

                using (new EditorGUI.DisabledScope(EditorApplication.isCompiling))
                {
                    if (GUILayout.Button("? Play Init Scene", GUILayout.Height(28)))
                        PlayFromFirstBuildScene.Run();
                }

                if (EditorApplication.isCompiling)
                    EditorGUILayout.HelpBox("Compiling… button disabled.", MessageType.Info);
            }

            EditorGUILayout.Space(6);

            // Optional quick info: what Unity thinks the first enabled build scene is.
            var first = EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled);
            var name = (first != null && !string.IsNullOrEmpty(first.path))
                ? System.IO.Path.GetFileNameWithoutExtension(first.path)
                : "<none enabled>";

            EditorGUILayout.LabelField("First enabled build scene:", name);
        }
    }
}
#endif
