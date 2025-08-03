using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using UnityEditor;

public class BTGraphView : GraphView
{
    private BehaviorTree tree;
    public BTGraphView()
    {
        var styleSheet = Resources.Load<StyleSheet>("BTStyles");
        if (styleSheet != null)
            styleSheets.Add(styleSheet);
        else
            Debug.LogWarning("BTStyles stylesheet not found. Please ensure it exists in the Resources folder.");
        // styleSheets.Add();
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        GridBackground grid = new();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    private BTNodeView GenerateEntryPointNode()
    {
        var node = new BTNodeView(BTEditorUtils.CreateNode<BTNode>(tree, "Root Node"));
        node.SetPosition(new Rect(100, 200, 200, 150));
        AddElement(node);
        return node;
    }

    public void CreateNewTree()
    {
        DeleteElements(graphElements);
        AddElement(GenerateEntryPointNode());
    }
    public void PopulateView(BehaviorTree tree)
    {
        this.tree = tree;

        // 1. 기존 노드 제거
        graphElements.ToList().ForEach(RemoveElement);

        // 2. 트리 노드 순회하며 GraphView 노드 생성
        if (tree.rootNode == null)
            tree.rootNode = BTEditorUtils.CreateNode<BTRoot>(tree);

        CreateNodeView(tree.rootNode);
        tree.rootNode.OnValidateNode();

        foreach (var node in tree.GetAllNodes())
            CreateNodeView(node);

        // 3. 연결 정보 복원
        foreach (var node in tree.GetAllNodes())
        {
            var children = BTEditorUtils.GetChildren(node);
            foreach (var child in children)
                AddEdge(FindNodeView(node), FindNodeView(child));
        }
    }

    public void SaveTree()
    {
        // Editor에서 노드 위치, GUID 업데이트
        foreach (var nodeView in nodes.ToList().Cast<BTNodeView>())
        {
            nodeView.node.position = nodeView.GetPosition().position;
            EditorUtility.SetDirty(nodeView.node);
        }

        EditorUtility.SetDirty(tree);
        AssetDatabase.SaveAssets();
    }

    private BTNodeView CreateNodeView(BTNode node)
    {
        var view = new BTNodeView(node);
        AddElement(view);
        return view;
    }

    private BTNodeView FindNodeView(BTNode node)
    {
        return GetNodeByGuid(node.guid) as BTNodeView;
    }

    private void AddEdge(BTNodeView parentView, BTNodeView childView)
    {
        var edge = parentView.outputPort.ConnectTo(childView.inputPort);
        AddElement(edge);
    }
}