using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class DeepCleanMediation : EditorWindow
{
    [MenuItem("Tools/LevelPlay/Deep Clean Old SDK")]
    public static void Clean()
    {
        string projectPath = Directory.GetCurrentDirectory();
        int deleted = 0;

        string[] dirsToDelete =
        {
            "Assets/IronSource",
            "Assets/Mediation",
            "Assets/LevelPlay",
            "Assets/Plugins/Android/IronSource",
            "Assets/Plugins/Android/LevelPlay",
            "Assets/Plugins/Android/UnityMediation",
            "Assets/Mobile Dependency Resolver",
            "Library/Bee",
            "Library/PackageCache/com.unity.services.mediation@*",
            "Packages/com.unity.services.mediation",
            "Temp",
            "Obj",
            "Build"
        };

        foreach (var pattern in dirsToDelete)
        {
            string full = Path.Combine(projectPath, pattern);
            if (pattern.Contains("*"))
            {
                var matches = Directory.GetDirectories(Path.GetDirectoryName(full), Path.GetFileName(pattern));
                foreach (var match in matches)
                    deleted += TryDelete(match);
            }
            else
                deleted += TryDelete(full);
        }

        // Delete androidbridge specifically
        string androidBridge = Path.Combine(projectPath, "Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/java/com/ironsource/unity/androidbridge");
        deleted += TryDelete(androidBridge);

        // Delete any cached gradle projects
        var gradleDirs = Directory.GetDirectories(Path.Combine(projectPath, "Library"), "Gradle*", SearchOption.AllDirectories);
        foreach (var dir in gradleDirs)
            deleted += TryDelete(dir);

        AssetDatabase.Refresh();
        Debug.Log($"✅ Deep Clean Complete. {deleted} folders/files removed.\n" +
                  "➡️ Now close Unity manually and delete your /Library and /Temp once more before reopening.");
    }

    private static int TryDelete(string path)
    {
        if (Directory.Exists(path))
        {
            FileUtil.DeleteFileOrDirectory(path);
            Debug.Log($"🧹 Deleted directory: {path}");
            return 1;
        }
        if (File.Exists(path))
        {
            FileUtil.DeleteFileOrDirectory(path);
            Debug.Log($"🧹 Deleted file: {path}");
            return 1;
        }
        return 0;
    }
}
