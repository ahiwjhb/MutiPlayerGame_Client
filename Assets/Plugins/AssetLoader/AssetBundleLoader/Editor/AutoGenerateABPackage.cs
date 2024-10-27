#nullable enable
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Core.AssetLoader.AssetBundleLoader.Editor
{
    /// <summary>
    /// 负责自动导出AB包文件
    /// </summary>
    public partial class AutoGenerateABPackage : AssetModificationProcessor
    {
        private static string NeedReloadKey => "GZJ_AUTOMAKEABPACKAGE_ISNEEDRELOADKEY";

        private static string AllowAutoGenerateKey => "GZJ_AUTOMAKEABPACKAGE_ALLOWENERATE";

        private static string ResourcePath => "Resources";  //要读取的资源相对于Asset文件夹的位置

        //设置标签时的缓存，用于生成AssetName类， AB标签 To 该标签下的资源路径集合
        private static readonly Dictionary<string, List<string>> _abLableToAssetPaths = new();  

        private static bool NeedReload {
            get => EditorPrefs.HasKey(NeedReloadKey) && EditorPrefs.GetBool(NeedReloadKey);
            set => EditorPrefs.SetBool(NeedReloadKey, value);
        }

        private static bool AllowAutoGenerate {
            get => EditorPrefs.HasKey(AllowAutoGenerateKey) && EditorPrefs.GetBool(AllowAutoGenerateKey);
            set => EditorPrefs.SetBool(AllowAutoGenerateKey, value);
        }

        [InitializeOnEnterPlayMode]
        public static void AutoOutputPackge() {
            if (NeedReload && AllowAutoGenerate) {
                GenrateABPackage();
                Debug.Log("已自动生成AB包资源, 如果出现空引用异常 和 Miss Script的情况重新运行就好了");
            }
        }

        [MenuItem("AssetLoader/生成资源路径.cs文件", priority = 100)]
        public static void GenrateAssetNameClass() {
            foreach (var childDirectory in new DirectoryInfo(Path.Combine(Application.dataPath, ResourcePath)).GetDirectories()) {
                SetAssetABLable(childDirectory, childDirectory.Name);
            }
            CreateAssetNameClass();
            CompilationPipeline.RequestScriptCompilation();
            AssetDatabase.Refresh();
            Debug.Log("生成资源路径文件.cs 成功");
        }

        [MenuItem("AssetLoader/AB包/生成AB包和配置文件", priority = 100)]
        public static void GenrateABPackage() {
            //foreach (string lableName in GenerateLableEnumClass()) {
            //    abLableToAssetName.Add(lableName, new List<string>());
            //}
            GenrateAssetNameClass();
            OutputPackage();
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(NeedReloadKey, false);
            Debug.Log("加载AB包和配置文件成功");
        }

        [MenuItem("AssetLoader/AB包/开启or关闭 AB包自动生成和配置文件功能", priority = -1)]
        public static void OpenOrCloseAutoGenerate() {
            if (AllowAutoGenerate) {
                Debug.Log("已关闭AB包自动生成功能");
            }
            else {
                Debug.Log("已开启AB包自动生成功能");
            }
            AllowAutoGenerate = !AllowAutoGenerate;
        }

        /// <summary>
        /// 为名称为AesstBundleLable枚举值的文件夹中的文件加上AB标签(不区分大小写)
        /// </summary>
        /// <param name="currentDirectory">当前文件夹</param>
        /// <param name="relativePath">若不为Empty则为当前文件夹的文件添加AB标签</param>
        /// <exception cref="FormatException">资源使用中文命名将发生异常</exception>
        private static void SetAssetABLable(DirectoryInfo currentDirectory, string relativePath) {
            foreach (var file in currentDirectory.GetFiles()) {
                if (file.Extension != ".meta") {
                    string assetFilePath = file.FullName[file.FullName.IndexOf("Assets")..];
                    if (Regex.IsMatch(file.Name, @"[\u4e00-\u9fa5]")) {
                        throw new FormatException("资源文件夹不允许使用中文命名 " + assetFilePath);
                    }
                    string abLable = relativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    AssetImporter.GetAtPath(assetFilePath).assetBundleName = abLable;
                    _abLableToAssetPaths.TryAdd(abLable, new List<string>());
                    string assetPath = assetFilePath[(assetFilePath.IndexOf(ResourcePath) + ResourcePath.Length)..].TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); ;
                    _abLableToAssetPaths[abLable].Add(assetPath[..assetPath.IndexOf('.')]);
                }
            }

            foreach (var childDirectory in currentDirectory.GetDirectories()) {
                SetAssetABLable(childDirectory, Path.Combine(relativePath, childDirectory.Name));
            }
        }

        private static void OutputPackage() {
            string path = Path.Combine(PathTool.StreamingAssetsPath, PathTool.AssetBundleSaveFolderName);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            BuildAssetBundleOptions buildOp = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            BuildPipeline.BuildAssetBundles(path, buildOp, GetBuildTargetForPlatform());
        }

        private static BuildTarget GetBuildTargetForPlatform() {
#if UNITY_ANDROID
            return BuildTarget.Android;
#elif UNITY_IOS
            return BuildTarget.iOS;
#elif UNITY_STANDALONE_WIN
            return BuildTarget.StandaloneWindows;
#elif UNITY_STANDALONE_OSX
            return BuildTarget.StandaloneOSX;
#else
            return BuildTarget.NoTarget;
#endif
        }

        #region 以下函数由AssetModificationProcessor父类进行回调

        private const int NO_FIND = -1;

        private static void OnWillCreateAsset(string path) {
            if (path.IndexOf(ResourcePath) != NO_FIND) {
                NeedReload = true;
            }
        }

        private static string[] OnWillSaveAssets(string[] paths) {
            bool cache = NeedReload;
            foreach (var path in paths) {
                if (path.IndexOf(ResourcePath) != NO_FIND) {
                    cache = true;
                }
            }
            NeedReload = cache;
            return paths;
        }

        private static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath) {
            if (newPath.IndexOf(ResourcePath) != NO_FIND) {
                NeedReload = true;
            }
            return AssetMoveResult.DidNotMove;
        }

        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options) {
            if (path.IndexOf(ResourcePath) != NO_FIND) {
                NeedReload = true;
            }
            return AssetDeleteResult.DidNotDelete;
        }
        #endregion
    }

    //为资源名和类型动态生成CSharp文件
    public partial class AutoGenerateABPackage
    {
        /// <summary>
        /// 生成存储在AB包的资源名称的常量集合
        /// </summary>
        private static void CreateAssetNameClass() {
            CodeCommentStatement comment = new CodeCommentStatement(
                "-----------------------------------------------------------------------------\r\n" +
                "<auto-generated>\r\n" +
               $"   该文件由 {nameof(AutoGenerateABPackage)}.{nameof(CreateAssetNameClass)} 方法生成\r\n" +
                "   记录了储存在AB包内的资源名称\r\n" +
                "   具有记忆变量名修改的功能，可以修改文件的变量名称\r\n" +
                "</auto-generated>\r\n" +
                "------------------------------------------------------------------------------"
            );
            string className = "AssetPath";
            string guid = AssetDatabase.FindAssets(nameof(IAssetLoader))[0];
            string savePath = AssetDatabase.GUIDToAssetPath(guid).Replace(nameof(IAssetLoader), className);
            if (!File.Exists(savePath)) {
                File.Create(savePath).Close();
            }

            var compileUnit = new CodeCompileUnit();
            var nameSpace = new CodeNamespace("Core.AssetLoader");
            var containerClass = new CodeTypeDeclaration(className) {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };

            nameSpace.Types.Add(containerClass);
            nameSpace.Comments.Add(comment);
            compileUnit.Namespaces.Add(nameSpace);

            var dict = new Dictionary<string, CodeTypeDeclaration> {
                { string.Empty, containerClass }
            };

            // TODO: 待优化
            foreach (string lableName in _abLableToAssetPaths.Keys) {
                string[] path = lableName.Split('/');
                if (dict.ContainsKey(path[0]) == false) {
                    var classDeclaration = new CodeTypeDeclaration(path[0]) {
                        Attributes = MemberAttributes.Public,
                        IsClass = true
                    };
                    containerClass.Members.Add(classDeclaration);
                    dict.TryAdd(path[0], classDeclaration);
                }
                for(int i = 1; i < path.Length; ++i) {
                    var classDeclaration = new CodeTypeDeclaration(path[i]) {
                        Attributes = MemberAttributes.Public,
                        IsClass = true
                    };

                    dict[string.Join('/', path.Take(i))].Members.Add(classDeclaration);
                    if (dict.ContainsKey(string.Join('/', path.Take(i + 1))) == false) {
                        dict.TryAdd(string.Join('/', path.Take(i + 1)), classDeclaration);
                    }
                }
            }

            foreach(var(lableName, assetPathList) in _abLableToAssetPaths) {
                var memoryMapping = GetAessetPathToCodeVariableNameMapping(savePath, lableName);
                var classDeclaration = dict[lableName];
                foreach (var assetPath in assetPathList) {
                    string variableName = memoryMapping.ContainsKey(assetPath) ? memoryMapping[assetPath] : Path.GetFileNameWithoutExtension(assetPath);
                    CodeMemberField memberField = new CodeMemberField(typeof(string), variableName);
                    memberField.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                    memberField.InitExpression = new CodePrimitiveExpression(assetPath);
                    classDeclaration.Members.Add(memberField);
                }
            }

            using (var writer = new StreamWriter(savePath)) {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
            }

            _abLableToAssetPaths.Clear();
        }

        private static Dictionary<string,string> GetAessetPathToCodeVariableNameMapping(string savePath, string lableName) {
            string[] nestedClassNames = lableName.Split('/');
            var mapping = new Dictionary<string, string>();
            int index = 0;
            using (var reader = new StreamReader(savePath)) {
                string line;
                while (index < nestedClassNames.Length && (line = reader.ReadLine()) != null) {
                    if (line.IndexOf("public class " + nestedClassNames[index]) != NO_FIND) {
                        index++;
                    }
                }

                if (index == nestedClassNames.Length) {
                    while ((line = reader.ReadLine()) != null) {
                        if (line.IndexOf('}') != NO_FIND) {
                            break;
                        }
                        index = line.IndexOf("const string");
                        if (index != NO_FIND) {
                            line = line[(index + "const string".Length)..^1];
                            string[] keyAndValue = line.Split('=');
                            string variableName = keyAndValue[0].Trim(), assetPath = keyAndValue[1].Trim('\x20', '\"');
                            mapping.Add(assetPath, variableName);
                        }
                    }
                }

                return mapping;
            }
        }

        private static void AddNestedClass(ref CodeTypeDeclaration ownerClass,string[] nestedClassNames, string[] assetPaths, Dictionary<string,string> assetPathToCodeVariableNameMapping) {
            var owner = ownerClass;
            foreach(var className in nestedClassNames) {
                var classDeclaration = new CodeTypeDeclaration(className) {
                    Attributes = MemberAttributes.Public,
                    IsClass = true
                };
                owner.Members.Add(classDeclaration);
                owner = classDeclaration;
            }

            foreach (var assetPath in assetPaths) {
                string variableName = assetPathToCodeVariableNameMapping.ContainsKey(assetPath) ? assetPathToCodeVariableNameMapping[assetPath] : Path.GetFileNameWithoutExtension(assetPath);
                CodeMemberField memberField = new CodeMemberField(typeof(string), variableName);
                memberField.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                memberField.InitExpression = new CodePrimitiveExpression(assetPath);
                owner.Members.Add(memberField);
            }
        }
    }
}

