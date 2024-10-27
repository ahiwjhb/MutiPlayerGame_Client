using UnityEngine;
using Core.AssetLoader;
using MultiPlayerGame;
using Network.MessageChannel;
using Network.Protocol;
using UnityEditor;
using System.IO;
using System.Diagnostics.CodeAnalysis;

public class Test : MonoBehaviour
{
    [SerializeField] DefaultAsset asset;

    [ContextMenu(nameof(Func))]
    private void Func() {
        string path = Path.Combine(Application.dataPath[..Application.dataPath.IndexOf("Assets")], AssetDatabase.GetAssetPath(asset));
        DirectoryInfo directory = new DirectoryInfo(path);
        ModifyCs(directory);
    }

    private void ModifyCs([DisallowNull]DirectoryInfo directory) {
        foreach (var fileInfo in directory.GetFiles("*.cs")) {

            string alltext = string.Empty;
            using (var reader = new StreamReader(fileInfo.FullName)) {
                alltext = reader.ReadToEnd();
            }

            using(var writer  = new StreamWriter(fileInfo.FullName)) {
                writer.WriteLine("#nullable enable");
                writer.Write(alltext);
            }  
        }

        foreach (var dir in directory.GetDirectories()) {
            ModifyCs(dir);
        }
    }
}
