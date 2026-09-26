#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class MultiSceneBuilder
{
    // Update these paths if your scenes are in a different subfolder inside Assets/
    private const string MeadowScenePath = "Assets/Scenes/MeadowScene.unity";
    private const string CavernScenePath = "Assets/Scenes/CavernScene.unity";

    private const string MeadowBuildOutput = "Builds/MeadowDragon/MeadowDragon.exe";
    private const string CavernBuildOutput = "Builds/CavernDragon/CavernDragon.exe";

    [MenuItem("MythicHome/Build Both Executables (Meadow & Cavern)")]
    public static void BuildBothScenes()
    {
        BuildSingleScene(MeadowScenePath, MeadowBuildOutput, "Meadow Dragon");
        BuildSingleScene(CavernScenePath, CavernBuildOutput, "Cavern Dragon");
        EditorUtility.RevealInFinder(Path.GetFullPath("Builds"));
    }

    [MenuItem("MythicHome/Build Meadow Dragon Only")]
    public static void BuildMeadowOnly()
    {
        BuildSingleScene(MeadowScenePath, MeadowBuildOutput, "Meadow Dragon");
    }

    [MenuItem("MythicHome/Build Cavern Dragon Only")]
    public static void BuildCavernOnly()
    {
        BuildSingleScene(CavernScenePath, CavernBuildOutput, "Cavern Dragon");
    }

    private static void BuildSingleScene(string sceneAssetPath, string outputExePath, string label)
    {
        if (!File.Exists(sceneAssetPath))
        {
            Debug.LogWarning($"[MultiSceneBuilder] Scene not found at '{sceneAssetPath}'. Please update the path constants in MultiSceneBuilder.cs to match your scene names.");
            return;
        }

        string dir = Path.GetDirectoryName(outputExePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { sceneAssetPath },
            locationPathName = outputExePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Debug.Log($"[MultiSceneBuilder] Building {label} -> {outputExePath}...");
        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"<color=green>[MultiSceneBuilder]</color> {label} build succeeded! ({report.summary.totalSize / (1024 * 1024)} MB)");
        }
        else
        {
            Debug.LogError($"[MultiSceneBuilder] {label} build failed: {report.summary.result}");
        }
    }
}
#endif
