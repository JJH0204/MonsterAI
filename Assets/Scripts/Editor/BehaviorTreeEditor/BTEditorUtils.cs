using System.Collections.Generic;
using AI.BehaviorTree;
using UnityEditor;
using UnityEngine;
using AI.BehaviorTree.Nodes;

public static class BTEditorUtils
{
    public static T CreateNodeAsset<T>(string name = null) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        asset.name = name ?? typeof(T).Name;
        AssetDatabase.AddObjectToAsset(asset, Selection.activeObject);
        AssetDatabase.SaveAssets();
        return asset;
    }

    public static string GenerateGUID() => System.Guid.NewGuid().ToString();

    public static Color GetColorByNodeType(System.Type type)
    {
        if (type == typeof(BTSequence)) return new Color(0.21f, 0.36f, 0.49f);      // 파랑
        if (type == typeof(BTSelector)) return new Color(0.42f, 0.36f, 0.48f);      // 보라
        if (type == typeof(BTAction))   return new Color(0.75f, 0.42f, 0.52f);      // 핑크
        return Color.gray;
    }
    
    public static BTRoot CreateRootNode(BehaviorTree tree, string name = null)
    {
        var node = ScriptableObject.CreateInstance<BTRoot>();
        
        if (node == null)
        {
            Debug.LogError($"[CreateNode] Failed to create ScriptableObject of type {typeof(BTRoot)}. Make sure it is a non-abstract ScriptableObject.");
            return null;
        }
        
        node.name = name ?? typeof(BTRoot).Name;
        node.guid = System.Guid.NewGuid().ToString();
        node.position = Vector2.zero;

        // 트리의 에셋 내부에 서브 에셋으로 등록
        // AssetDatabase.AddObjectToAsset(node, tree);      // TODO: Fix null Error
        // AssetDatabase.SaveAssets();

        return node;
    }
    
    public static T CreateNode<T>(BehaviorTree tree, string name = null) where T : BTNode
    {
        var node = ScriptableObject.CreateInstance<T>();
        
        if (node == null)
        {
            Debug.LogError($"[CreateNode] Failed to create ScriptableObject of type {typeof(T)}. Make sure it is a non-abstract ScriptableObject.");
            return null;
        }
        
        node.name = name ?? typeof(T).Name;
        node.guid = System.Guid.NewGuid().ToString();
        node.position = Vector2.zero;

        // 트리의 에셋 내부에 서브 에셋으로 등록
        AssetDatabase.AddObjectToAsset(node, tree);
        AssetDatabase.SaveAssets();

        return node;
    }

    // input Node == Parent Node
    public static BTComposite GetParent(BTNode node)
    {
        return node.input as BTComposite;
    }

    public static List<BTNode> GetChildren(BTNode node)
    {
        var result = new List<BTNode>();
        if (node is BTComposite composite)
            result.AddRange(composite.children);
        else if (node is BTDecorator decorator && decorator.child != null)
            result.Add(decorator.child);
        return result;
    }
}