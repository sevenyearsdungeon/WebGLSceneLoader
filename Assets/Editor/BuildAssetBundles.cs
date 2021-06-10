using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Diagnostics;

public static class BuildAssetBundles
{
    [MenuItem("Assets/Build Bundles", false, 1999)]
    static void BuildAllAssetBundles(MenuCommand command)
    {
        // Debug.Log(string.Join("\n", Selection.objects.Select(o => o.ToString())));
        BuildPipeline.BuildAssetBundles("Assets/Bundles/windows", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("Assets/Bundles/webgl", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
        string dest = @"C:\Users\nicho\Desktop\WebApp\scenes";
        string source = Path.Combine(Application.dataPath, "Bundles");

        Process p = new Process();
        p.StartInfo.Arguments = string.Format("/C Robocopy {0} {1} /mir /xf *.meta", source, dest);
        p.StartInfo.FileName = "CMD.EXE";
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.Start();
    }

}

