using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Copies the required K4A binaries in the Editor and Build project.
/// </summary>
[InitializeOnLoadAttribute]
public class UnityEnvironment : MonoBehaviour
{
    private const string Package = "Mastering_Azure_Kinect";
    private const string Plugins = "Plugins";
    private const string Plugins64 = "x86_64";

    private static readonly string[] Binaries = new string[]
    {
        "cublas64_100.dll",
        "cudart64_100.dll",
        "onnxruntime.dll",
        "dnn_model_2_0.onnx",
#if UNITY_2018 || UNITY_2017
        "depthengine_2_0.dll",
        "k4a.dll",
        "k4abt.dll",
        "k4arecord.dll",
        "vcomp140.dll"
#endif
    };

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

                foreach (string binary in Binaries)
                {
                    if (binary == name)
                    {
                        string path = Path.Combine(destination, name);

                        File.Copy(file, path, true);
                    }
                }
            }
        }
        catch
        {
            // Ignored
        }
    }
}
