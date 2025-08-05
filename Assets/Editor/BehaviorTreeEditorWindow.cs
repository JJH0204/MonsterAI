using UnityEngine;
using UnityEditor;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using System.Collections.Generic;

public class BehaviorTreeEditorWindow : EditorWindow
{
    private BehaviorTree tree;
    private Vector2 scrollPos;
    private Dictionary<BTNode, Rect> nodeRects = new Dictionary<BTNode, Rect>();
    private const float NodeWidth = 180f;
    private const float NodeHeight = 60f;
    private const float HorizontalSpacing = 40f;
    private const float VerticalSpacing = 80f;

    [MenuItem("Window/AI/Behavior Tree Viewer")]
    public static void ShowWindow()
    {
        GetWindow<BehaviorTreeEditorWindow>("Behavior Tree Viewer");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        tree = (BehaviorTree)EditorGUILayout.ObjectField("Behavior Tree", tree, typeof(BehaviorTree), false);
        if (tree == null || tree.rootNode == null)
        {
            EditorGUILayout.HelpBox("BehaviorTree를 선택하세요.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        nodeRects.Clear();
        float startX = position.width / 2 - NodeWidth / 2;
        DrawNodeRecursive(tree.rootNode, startX, 20, 0);
        DrawConnections(tree.rootNode);
        EditorGUILayout.EndScrollView();
    }

    private void DrawNodeRecursive(BTNode node, float x, float y, int depth)
    {
        if (node == null) return;
        Rect rect = new Rect(x, y, NodeWidth, NodeHeight);
        nodeRects[node] = rect;
        GUI.Box(rect, node.GetType().Name);

        if (node is BTComposite composite && composite.children != null)
        {
            float totalWidth = (composite.children.Count - 1) * (NodeWidth + HorizontalSpacing);
            float childX = x - totalWidth / 2;
            for (int i = 0; i < composite.children.Count; i++)
            {
                DrawNodeRecursive(composite.children[i], childX + i * (NodeWidth + HorizontalSpacing), y + NodeHeight + VerticalSpacing, depth + 1);
            }
        }
        else if (node is BTDecorator decorator && decorator.child != null)
        {
            DrawNodeRecursive(decorator.child, x, y + NodeHeight + VerticalSpacing, depth + 1);
        }
    }

    private void DrawConnections(BTNode node)
    {
        if (node == null || !nodeRects.ContainsKey(node)) return;
        Rect fromRect = nodeRects[node];
        Vector3 from = new Vector3(fromRect.x + fromRect.width / 2, fromRect.y + fromRect.height, 0);

        if (node is BTComposite composite && composite.children != null)
        {
            foreach (var child in composite.children)
            {
                if (child != null && nodeRects.ContainsKey(child))
                {
                    Rect toRect = nodeRects[child];
                    Vector3 to = new Vector3(toRect.x + toRect.width / 2, toRect.y, 0);
                    Handles.DrawLine(from, to);
                    DrawConnections(child);
                }
            }
        }
        else if (node is BTDecorator decorator && decorator.child != null && nodeRects.ContainsKey(decorator.child))
        {
            Rect toRect = nodeRects[decorator.child];
            Vector3 to = new Vector3(toRect.x + toRect.width / 2, toRect.y, 0);
            Handles.DrawLine(from, to);
            DrawConnections(decorator.child);
        }
    }
}

