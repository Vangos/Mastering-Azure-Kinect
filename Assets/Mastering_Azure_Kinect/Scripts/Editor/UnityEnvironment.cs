using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoadAttribute]
public class UnityEnvironment : MonoBehaviour
{
    private const string Package = "Mastering_Azure_Kinect";
    private const string Plugins = "Plugins";
    private const string Plugins64 = "x86_64";

    static UnityEnvironment()
    {
        string source = Path.Combine(Application.dataPath, Package, Plugins, Plugins64);
        string destination = Directory.GetParent(Application.dataPath).FullName;

        CopyFiles(source, destination);
    }

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.StandaloneWindows64)
        {
            Debug.LogError("Only Windows x64 is supported.");
            return;
        }

        string source = Path.Combine(Application.dataPath, Package, Plugins, Plugins64);
        string destination = Directory.GetParent(pathToBuiltProject).FullName;

        CopyFiles(source, destination);
    }

    private static void CopyFiles(string source, string destination)
    {
        try
        {
            foreach (string file in Directory.GetFiles(source))
            {
                string name = Path.GetFileName(file);
                string extension = Path.GetExtension(file);

                if (extension == ".onnx" || extension == ".dll")
                {
                    string path = Path.Combine(destination, name);

                    File.Copy(file, path, true);
                }
            }

            AssetDatabase.Refresh();
        }
        catch
        {
            // Ignored
        }
    }
}
