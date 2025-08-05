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
    private BehaviorTree _tree;

    public BTNode outputPort;
    public Node inputPort;

    #region 생성자

    // 그래프 뷰 생성자
    public BTGraphView()
    {
        var styleSheet = Resources.Load<StyleSheet>("BTStyles");    // uss 파일을 /Assets/Resources 아래에서 찾는다.
        
        if (styleSheet != null)
            styleSheets.Add(styleSheet);    // 스타일 적용
        else
            Debug.LogWarning("BTStyles stylesheet not found. Please ensure it exists in the Resources folder.");
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        // 배경 그리드 설정
        GridBackground grid = new();
        Insert(0, grid);
        grid.StretchToParentSize();
        
        // Root Node 생성
        // AddElement(GenerateEntryPointNode());
    }

    #endregion

    // private BTNodeView GenerateEntryPointNode()
    // {
    //     var temp = BTEditorUtils.CreateRootNode(_tree, "Root");
    //     var node = new BTNodeView(temp);
    //     node.SetPosition(new Rect(100, 200, 200, 150));
    //     AddElement(node);
    //     return node;
    // }

    // public void CreateNewTree()
    // {
    //     DeleteElements(graphElements);
    //     AddElement(GenerateEntryPointNode());
    // }
    
    public void PopulateView(BehaviorTree tree)
    {
        _tree = tree;
        DeleteElements(graphElements); // 기존 노드 제거
        if (_tree.rootNode == null)
            _tree.rootNode = BTEditorUtils.CreateNode<BTNode>(_tree, "Root");
        CreateNodeView(_tree.rootNode);
        _tree.rootNode.OnValidateNode();
        foreach (var node in _tree.GetAllNodes())
        {
            var nodeView = CreateNodeView(node);
            nodeView.SetPosition(new Rect(node.position.x, node.position.y, 200, 150));
        }
    }

    // public void SaveTree()
    // {
    //     // Editor에서 노드 위치, GUID 업데이트
    //     foreach (var nodeView in nodes.ToList().Cast<BTNodeView>())
    //     {
    //         nodeView.node.position = nodeView.GetPosition().position;
    //         EditorUtility.SetDirty(nodeView.node);
    //     }
    //
    //     EditorUtility.SetDirty(_tree);
    //     AssetDatabase.SaveAssets();
    // }

    public BTNodeView CreateNodeView(BTNode node)
    {
        var view = new BTNodeView(node);
        AddElement(view);
        return view;
    }

    // private BTNodeView FindNodeView(BTNode node)
    // {
    //     return GetNodeByGuid(node.guid) as BTNodeView;
    // }
    //
    // private void AddEdge(BTNodeView parentView, BTNodeView childView)
    // {
    //     var edge = parentView.outputPort.ConnectTo(childView.inputPort);
    //     AddElement(edge);
    // }
    
    // public void LoadFromBehaviorTree(BehaviorTree tree)
    // {
    //     _tree = tree;
    //
    //     // TODO: 트리를 순회하면서 에디터 뷰에 노드 생성
    // }
}