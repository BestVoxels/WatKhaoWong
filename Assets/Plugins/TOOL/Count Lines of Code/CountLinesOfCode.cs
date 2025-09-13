using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CountLinesOfCode : EditorWindow
{
    //private SerializedObject serializedObject;
    //private SerializedProperty foldersToIgnoreProperty;

    //public List<string> foldersToIgnore = new List<string>();

    [MenuItem("Tools/Count Lines of Code")]
    public static void ShowWindow()
    {
        GetWindow<CountLinesOfCode>("Count Lines of Code");
    }

    private void OnEnable()
    {
        //serializedObject = new SerializedObject(this);
        //foldersToIgnoreProperty = serializedObject.FindProperty("foldersToIgnore");
    }

    private void OnGUI()
    {
        //serializedObject.Update();

        //EditorGUILayout.LabelField("Folders to Ignore (relative to Assets):");
        //EditorGUILayout.PropertyField(foldersToIgnoreProperty, true);

        //if (GUILayout.Button("Count Lines of Code"))
        //{
        //    CountLines();
        //}

        if (GUILayout.Button("Count Lines of Code Myself Only"))
        {
            CountLinesMineOnly();
        }

        //serializedObject.ApplyModifiedProperties();
    }

    private void CountLinesMineOnly()
    {
        string[] allScripts = Directory.GetFiles(Path.Combine(Application.dataPath, "_Assets", "Scripts"), "*.cs", SearchOption.AllDirectories);

        int totalLines = 0;
        int totalFiles = 0;

        foreach (string script in allScripts)
        {
            //Debug.Log($"script : {script}"); // TO SHOW SCRIPT NAME
            string[] lines = File.ReadAllLines(script);
            totalLines += lines.Length;

            totalFiles++;
        }

        Debug.Log($"Total lines of code: {totalLines}");
        Debug.Log($"Total Files: {totalFiles}");
    }

    //private void CountLines()
    //{
    //    string assetsPath = Application.dataPath;
    //    string[] allScripts = Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories);
    //    List<string> foldersToIgnorePaths = new List<string>();

    //    foreach (string folder in foldersToIgnore)
    //    {
    //        string folderPath = Path.Combine(assetsPath, folder);
    //        if (Directory.Exists(folderPath))
    //        {
    //            foldersToIgnorePaths.Add(folderPath);
    //        }
    //    }

    //    int totalLines = 0;

    //    foreach (string script in allScripts)
    //    {
    //        bool ignore = false;

    //        foreach (string folderPath in foldersToIgnorePaths)
    //        {
    //            if (script.StartsWith(folderPath))
    //            {
    //                ignore = true;
    //                break;
    //            }
    //        }

    //        if (!ignore)
    //        {
    //            string[] lines = File.ReadAllLines(script);
    //            totalLines += lines.Length;
    //        }
    //    }

    //    Debug.Log("Total lines of code: " + totalLines);
    //}
}