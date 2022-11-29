using System.IO;
using UnityEngine;

namespace AM1.BaseFrame.Editor
{
    [System.Serializable]
    public class BaseFrameLocalSettings
    {
        public static string SettingFilePath => "Assets/AM1/BaseFrame/LocalSettings/Settings.json";

        /// <summary>
        /// 新しい状態を生成する先のフォルダー
        /// </summary>
        public string stateFolder = "Assets";

        /// <summary>
        /// 新しいシーンの作成先フォルダー
        /// </summary>
        public string sceneFolder = "Assets/Scenes";

        /// <summary>
        /// 設定を読み込む。
        /// </summary>
        public void Load()
        {
            if (File.Exists(SettingFilePath))
            {
                string settings = File.ReadAllText(SettingFilePath);
                if (settings.Length > 0)
                {
                    var fromFile = JsonUtility.FromJson<BaseFrameLocalSettings>(settings);
                    this.stateFolder = fromFile.stateFolder;
                    this.sceneFolder = fromFile.sceneFolder;
                    return;
                }
            }

            // 現在の状態を保存
            Save();
        }

        /// <summary>
        /// 既定の先へ保存
        /// </summary>
        public void Save()
        {
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(SettingFilePath, json);
        }
    }

    public static class AM1BaseFrameUtils
    {
        static BaseFrameLocalSettings baseFrameLocalSettings;

        /// <summary>
        /// 設定を返す。
        /// </summary>
        public static BaseFrameLocalSettings LocalSettings
        {
            get
            {
                if (baseFrameLocalSettings == null)
                {
                    // ファイルがないので作成
                    baseFrameLocalSettings = new BaseFrameLocalSettings();
                    baseFrameLocalSettings.Load();
                }
                return baseFrameLocalSettings;
            }
        }

        /// <summary>
        /// Returns the relative path of the package.
        /// </summary>
        public static string packageRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackagePath))
                    m_PackagePath = GetPackageRelativePath();

                return m_PackagePath;
            }
        }
        [SerializeField]
        private static string m_PackagePath;

        /// <summary>
        /// Returns the fully qualified path of the package.
        /// </summary>
        public static string packageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackageFullPath))
                    m_PackageFullPath = GetPackageFullPath();

                return m_PackageFullPath;
            }
        }
        [SerializeField]
        private static string m_PackageFullPath;


        // Static Fields Related to locating the TextMesh Pro Asset
        private static string folderPath = "Not Found";

        private static string GetPackageRelativePath()
        {
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/jp.am1.baseframe");
            if (Directory.Exists(packagePath))
            {
                return "Packages/jp.am1.baseframe";
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                // Search default location for development package
                if (Directory.Exists(packagePath + "/Assets/AM1BaseFrame"))
                {
                    return "Assets/AM1BaseFrame";
                }

                // Search for potential alternative locations in the user project
                string[] matchingPaths = Directory.GetDirectories(packagePath, "AM1BaseFrame", SearchOption.AllDirectories);
                packagePath = ValidateLocation(matchingPaths, packagePath);
                if (packagePath != null) return packagePath;
            }

            return null;
        }

        private static string GetPackageFullPath()
        {
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/jp.am1.baseframe");
            if (Directory.Exists(packagePath))
            {
                return packagePath;
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                // Search default location for development package
                if (Directory.Exists(packagePath + "/Assets/AM1BaseFrame"))
                {
                    return packagePath + "/Assets/AM1BaseFrame";
                }

                /*
                // Search for default location of normal TextMesh Pro AssetStore package
                if (Directory.Exists(packagePath + "/Assets/AM1BaseFrame"))
                {
                    return packagePath + "/Assets/AM1BaseFrame";
                }
                */

                // Search for potential alternative locations in the user project
                /*
                string[] matchingPaths = Directory.GetDirectories(packagePath, "AM1BaseFrame", SearchOption.AllDirectories);
                string path = ValidateLocation(matchingPaths, packagePath);
                if (path != null) return packagePath + path;
                */
            }

            return null;
        }

        /// <summary>
        /// Method to validate the location of the asset folder by making sure the GUISkins folder exists.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static string ValidateLocation(string[] paths, string projectPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                // Check if any of the matching directories contain a GUISkins directory.
                if (Directory.Exists(paths[i] + "/Editor Resources"))
                {
                    folderPath = paths[i].Replace(projectPath, "");
                    folderPath = folderPath.TrimStart('\\', '/');
                    return folderPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Path.RelativePath()が2020にはないのでカプセル化
        /// </summary>
        /// <param name="abspath">基準となるパス</param>
        /// <param name="target">対象のパス</param>
        /// <returns>相対パス</returns>
        public static string GetRelativePath(string abspath, string target)
        {
            var getRelativePath = typeof(Path).GetMethod("GetRelativePath");
            if (getRelativePath != null)
            {
                return getRelativePath.Invoke(null, new object[] { abspath, target }) as string;
            }

            // 自前実装
            string fullAbs = Path.GetFullPath(abspath);
            string fullTarget = Path.GetFullPath(target);
            if (!fullTarget.StartsWith(fullAbs))
            {
                Debug.Log($"プロジェクトフォルダーのAssetsフォルダー外が指定されました。");
                return target;
            }

            string rel = fullTarget.Substring(fullAbs.Length);
            while (rel.StartsWith("\\"))
            {
                rel = rel.Substring(1);
            }
            return rel;
        }

        /// <summary>
        /// 指定のテンプレートファイルを指定のパスに保存する。
        /// ただし、すでにファイルがあった場合は保存しない。
        /// </summary>
        /// <param name="templateName">テンプレートファイル名</param>
        /// <param name="savePath">保存先パス</param>
        public static void SaveTemplateNotExists(string templateName, string savePath)
        {
            if (File.Exists(savePath))
            {
                Debug.Log($"{savePath}がすでにあるのでキャンセルします。");
                return;
            }

            string tempPath = Path.Combine(packageRelativePath, "Package Resources");
            tempPath = Path.Combine(tempPath, templateName);
            string text = File.ReadAllText(tempPath);
            File.WriteAllText(savePath, text);
        }
    }
}