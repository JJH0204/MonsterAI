using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using System.Collections.Generic;

public class BTGraphView : GraphView
{
    private BehaviorTree _tree;
    private Dictionary<string, BTNodeView> nodeViews = new();

    public BTGraphView(BehaviorTree tree)
    {
        _tree = tree;
        Debug.Log("[BTGraphView] Initializing GraphView...");

        // Load and apply stylesheet
        var styleSheet = Resources.Load<StyleSheet>("BTStyles");
        if (styleSheet != null)
        {
            styleSheets.Add(styleSheet);
            Debug.Log("[BTGraphView] Stylesheet loaded successfully.");
        }
        else
        {
            Debug.LogError("[BTGraphView] Failed to load stylesheet 'BTStyles'. Ensure it is in the Resources folder.");
        }

        // Setup zoom and manipulators
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add grid background
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Draw tree
        if (_tree == null)
        {
            Debug.LogError("[BTGraphView] BehaviorTree is null. Cannot draw tree.");
            return;
        }

        if (_tree.rootNode == null)
        {
            Debug.LogWarning("[BTGraphView] BehaviorTree has no root node. Tree is empty.");
            return;
        }

        Debug.Log("[BTGraphView] Drawing tree...");
        DrawTree();
    }

    private void DrawTree()
    {
        if (_tree == null || _tree.rootNode == null) return;
        var visited = new HashSet<BTNode>();
        Traverse(_tree.rootNode, visited);
        DrawEdges();
    }

    private void Traverse(BTNode node, HashSet<BTNode> visited)
    {
        if (node == null || visited.Contains(node)) return;
        visited.Add(node);
        var view = new BTNodeView(node, _tree.rootNode);
        nodeViews[node.guid] = view;
        AddElement(view);
        if (node is BTComposite composite && composite.children != null)
        {
            foreach (var child in composite.children)
                Traverse(child, visited);
        }
        else if (node is BTDecorator decorator && decorator.child != null)
        {
            Traverse(decorator.child, visited);
        }
    }

    private void DrawEdges()
    {
        foreach (var kvp in nodeViews)
        {
            var node = kvp.Value.node;
            var parentView = kvp.Value;

            Debug.Log($"[DrawEdges] Processing node: {node.name} (GUID: {node.guid}, Type: {node.GetType()})");

            // Handle Composite nodes
            if (node is BTComposite composite && composite.children != null)
            {
                Debug.Log($"[DrawEdges] Node is a Composite: {node.name}, Children Count: {composite.children.Count}");
                for (int i = 0; i < composite.children.Count; i++)
                {
                    var child = composite.children[i];
                    if (child == null)
                    {
                        Debug.LogWarning($"[DrawEdges] Composite node {node.name} has a null child at index {i}.");
                        continue;
                    }

                    if (!nodeViews.ContainsKey(child.guid))
                    {
                        Debug.LogWarning($"[DrawEdges] Child node {child.name} (GUID: {child.guid}) not found in nodeViews.");
                        continue;
                    }

                    var childView = nodeViews[child.guid];
                    if (parentView.outputPorts.Count > i && childView.inputPort != null)
                    {
                        Debug.Log($"[DrawEdges] Connecting {node.name} to {child.name}.");
                        var edge = parentView.outputPorts[i].ConnectTo(childView.inputPort);
                        AddElement(edge);
                    }
                    else
                    {
                        Debug.LogWarning($"[DrawEdges] Failed to connect {node.name} to {child.name}. Output port or input port is missing.");
                    }
                }
            }
            // Handle Decorator nodes
            else if (node is BTDecorator decorator)
            {
                Debug.Log($"[DrawEdges] Node is a Decorator: {node.name}, Has Child: {decorator.child != null}");
                if (decorator.child == null)
                {
                    Debug.LogWarning($"[DrawEdges] Decorator node {node.name} has a null child.");
                    continue;
                }

                if (!nodeViews.ContainsKey(decorator.child.guid))
                {
                    Debug.LogWarning($"[DrawEdges] Child node {decorator.child.name} (GUID: {decorator.child.guid}) not found in nodeViews.");
                    continue;
                }

                var childView = nodeViews[decorator.child.guid];
                if (parentView.outputPorts.Count > 0 && childView.inputPort != null)
                {
                    Debug.Log($"[DrawEdges] Connecting {node.name} to {decorator.child.name}.");
                    var edge = parentView.outputPorts[0].ConnectTo(childView.inputPort);
                    AddElement(edge);
                }
                else
                {
                    Debug.LogWarning($"[DrawEdges] Failed to connect {node.name} to {decorator.child.name}. Output port or input port is missing.");
                }
            }
            else
            {
                Debug.Log($"[DrawEdges] Node does not match Composite or Decorator: {node.name}, Type: {node.GetType()}");
            }
        }
    }
}
