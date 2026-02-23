// Assets/Editor/ScriptUsageScanner.cs
#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ScriptUsageScanner
{
    [MenuItem("Tools/Script Usage Scanner/Scan Project")]
    public static void ScanProject()
    {
        // All C# scripts under Assets (not Packages)
        var allScriptGuids = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets" });
        var allScriptPaths = allScriptGuids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p)
            .ToList();

        // Assets that can contain MonoBehaviours / references
        var scanGuids = AssetDatabase.FindAssets("t:Scene t:Prefab t:ScriptableObject", new[] { "Assets" });
        var scanPaths = scanGuids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .OrderBy(p => p)
            .ToList();

        var usedScripts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assetPath in scanPaths)
        {
            var deps = AssetDatabase.GetDependencies(assetPath, true);
            foreach (var d in deps)
            {
                if (d.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    usedScripts.Add(d);
            }
        }

        bool ignoreEditorFolder = true;
        bool IsInEditorFolder(string path)
        {
            var norm = path.Replace('\\', '/');
            return ignoreEditorFolder && norm.IndexOf("/Editor/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        var used = allScriptPaths.Where(p => usedScripts.Contains(p)).Where(p => !IsInEditorFolder(p)).ToList();
        var unused = allScriptPaths.Where(p => !usedScripts.Contains(p)).Where(p => !IsInEditorFolder(p)).ToList();
        var editorScripts = allScriptPaths.Where(IsInEditorFolder).ToList();

        var reportPath = "Assets/ScriptUsageReport.txt";
        using (var sw = new StreamWriter(reportPath, false))
        {
            sw.WriteLine("Script Usage Report");
            sw.WriteLine("Generated: " + DateTime.Now);
            sw.WriteLine();
            sw.WriteLine($"Total scripts (Assets): {allScriptPaths.Count}");
            sw.WriteLine($"Scanned assets (scenes/prefabs/SO): {scanPaths.Count}");
            sw.WriteLine($"Used scripts (by dependencies): {used.Count}");
            sw.WriteLine($"Unused scripts (candidate): {unused.Count}");
            sw.WriteLine($"Editor scripts (ignored): {editorScripts.Count}");
            sw.WriteLine();

            sw.WriteLine("=== USED ===");
            foreach (var p in used) sw.WriteLine(p);
            sw.WriteLine();

            sw.WriteLine("=== UNUSED (candidate) ===");
            foreach (var p in unused) sw.WriteLine(p);
            sw.WriteLine();

            sw.WriteLine("=== IGNORED (Editor folder) ===");
            foreach (var p in editorScripts) sw.WriteLine(p);
        }

        AssetDatabase.Refresh();

        Debug.Log(
            $"[ScriptUsageScanner] Report saved to: {reportPath}\n" +
            $"Total: {allScriptPaths.Count}, Used: {used.Count}, Unused: {unused.Count}\n" +
            "NOTE: Skripty přidávané dynamicky (AddComponent, reflection, Resources.Load) se nemusí objevit jako dependency."
        );
    }
}
#endif