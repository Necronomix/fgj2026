using System.IO;
using UnityEditor;
using UnityEngine;

public static class OpenPersistentDatapath
{
    [MenuItem("Tools/Open Persistent Datapath")]
    private static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        EditorUtility.RevealInFinder(path);
    }
}
