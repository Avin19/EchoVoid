using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class RemoveAdsAndPatch : EditorWindow
{
    [MenuItem("Tools/Cleanup/🧹 Remove All Ads and Patch References")]
    public static void CleanAds()
    {
        int removed = 0;
        string root = Directory.GetCurrentDirectory();

        string[] adFolders =
        {
            "Assets/IronSource",
            "Assets/LevelPlay",
            "Assets/Mediation",
            "Assets/Plugins/Android/IronSource",
            "Assets/Plugins/Android/LevelPlay",
            "Assets/Plugins/Android/UnityMediation",
            "Assets/Mobile Dependency Resolver",
            "Packages/com.unity.services.levelplay",
            "Packages/com.unity.services.mediation",
            "Packages/com.unity.ads",
        };

        foreach (string path in adFolders)
        {
            string fullPath = Path.Combine(root, path);
            if (Directory.Exists(fullPath))
            {
                FileUtil.DeleteFileOrDirectory(fullPath);
                Debug.Log($"🗑️ Deleted: {path}");
                removed++;
            }
        }

        // Clean PackageCache
        string packageCache = Path.Combine(root, "Library/PackageCache");
        if (Directory.Exists(packageCache))
        {
            foreach (var dir in Directory.GetDirectories(packageCache, "com.unity.services.*"))
            {
                if (dir.Contains("mediation") || dir.Contains("ads") || dir.Contains("levelplay"))
                {
                    FileUtil.DeleteFileOrDirectory(dir);
                    Debug.Log($"🧹 Removed cached package: {dir}");
                    removed++;
                }
            }
        }

        // Patch C# references
        string[] scripts = Directory.GetFiles(Path.Combine(root, "Assets"), "*.cs", SearchOption.AllDirectories);
        foreach (string script in scripts)
        {
            string text = File.ReadAllText(script);

            if (Regex.IsMatch(text, @"IronSource|UnityAds|AdManager|/* RemovedAdClass */|/* RemovedAdClass */|LevelPlay"))
            {
                text = Regex.Replace(text, @"IronSource\.[A-Za-z0-9_\.]+", "/* RemovedAdCall */");
                text = Regex.Replace(text, @"UnityAds\.[A-Za-z0-9_\.]+", "/* RemovedAdCall */");
                text = Regex.Replace(text, @"AdManager\.[A-Za-z0-9_\.]+", "/* RemovedAdCall */");
                text = Regex.Replace(text, @"/* RemovedAdClass */[A-Za-z0-9_]*", "/* RemovedAdClass */");
                text = Regex.Replace(text, @"/* RemovedAdClass */[A-Za-z0-9_]*", "/* RemovedAdClass */");
                text = Regex.Replace(text, @"LevelPlay[A-Za-z0-9_\.]+", "/* RemovedAdClass */");

                File.WriteAllText(script, text);
                Debug.Log($"🩹 Patched references in {Path.GetFileName(script)}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ Removed {removed} ad packages/folders and patched ad references.\n" +
                  "Next: Close Unity → Delete Library/Temp → Reopen and Rebuild.");
    }
}
