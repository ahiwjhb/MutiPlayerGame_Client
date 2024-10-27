#nullable enable
using System.IO;
using System.Text;
using UnityEngine;

namespace Core.AssetLoader
{
    public static class PathTool
    {
        public static string StreamingAssetsPath {
            get {
                string path;

#if UNITY_EDITOR || UNITY_STANDALONE
                path = Application.streamingAssetsPath;
#elif UNITY_IPHONE
                path =  Application.dataPath + "/Raw";
#elif UNITY_ANDROID
                path =  Application.dataPath + "!/assets";            
#endif
                return path;
            }
        }

        public static string StreamingAssetsWWWPath {
            get {
                string url;

#if UNITY_EDITOR || UNITY_STANDALONE
                url = "file://";
#elif UNITY_IPHONE
            url =     "file://";
#elif UNITY_ANDROID
            url = "jar:file://";
#endif
                return url + StreamingAssetsPath;
            }
        }

        public static string AssetBundleSaveFolderName {
            get {
                switch (Application.platform) {

                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        return "WindowsABPackage";
                    case RuntimePlatform.IPhonePlayer:
                        return "IPhoneABPackage";
                    case RuntimePlatform.Android:
                        return "AndroidABPackage";
                    default:
                        return "AB";
                }
            }
        }
    }
}
