using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ShaderGraph;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

[ScriptedImporter(3, Extension)]
class ShaderSubGraphImporter : ScriptedImporter
{
    public const string Extension = "shadersubgraph";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        var graph = JsonUtility.FromJson<GraphData>(textGraph);

        if (graph == null)
            return;

        graph.isSubGraph = true;

        var sourceAssetDependencyPaths = new List<string>();
        foreach (var node in graph.GetNodes<AbstractMaterialNode>())
            node.GetSourceAssetDependencies(sourceAssetDependencyPaths);

        var graphAsset = ScriptableObject.CreateInstance<MaterialSubGraphAsset>();
        graphAsset.subGraph = graph;

        Texture2D texture = Resources.Load<Texture2D>("Icons/Subgraph@64");
        ctx.AddObjectToAsset("MainAsset", graphAsset, texture);
        ctx.SetMainObject(graphAsset);

        foreach (var sourceAssetDependencyPath in sourceAssetDependencyPaths.Distinct())
            ctx.DependsOnSourceAsset(sourceAssetDependencyPath);
    }
}
