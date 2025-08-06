using System.Collections.Generic;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BTGraphView : GraphView
{
    private BehaviorTree _tree;
    private Dictionary<string, BTNodeView> _nodeViews = new();

    public BTGraphView(BehaviorTree tree)
    {
        _tree = tree;
        var styleSheet = Resources.Load<StyleSheet>("BTStyles");
        if (styleSheet != null)
            styleSheets.Add(styleSheet);

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        // EdgeConnector는 포트에 직접 할당해야 하므로 GraphView에는 추가하지 않음

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        if (_tree == null || _tree.rootNode == null) return;
        DrawTree();
    }

    private void DrawTree()
    {
        // 기존 노드/엣지 완전 제거
        foreach (var view in _nodeViews.Values)
            RemoveElement(view);
        foreach (var edge in edges.ToList())
            RemoveElement(edge);
        _nodeViews.Clear();

        if (_tree == null || _tree.rootNode == null) return;
        var visited = new HashSet<BTNode>();
        Traverse(_tree.rootNode, visited);
        DrawEdges();
        FrameAllNodes();
    }

    public void RedrawTree()
    {
        DrawTree();
    }

    private void FrameAllNodes()
    {
        if (_nodeViews.Count == 0) return;
        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        foreach (var view in _nodeViews.Values)
        {
            Vector2 pos = view.Node.position;
            float width = view.layout.width > 0 ? view.layout.width : 200f;
            float height = view.layout.height > 0 ? view.layout.height : 100f;
            minX = Mathf.Min(minX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxX = Mathf.Max(maxX, pos.x + width);
            maxY = Mathf.Max(maxY, pos.y + height);
        }
        var bounds = new Rect(minX, minY, maxX - minX, maxY - minY);
        float viewWidth = this.layout.width > 0 ? this.layout.width : 800f;
        float viewHeight = this.layout.height > 0 ? this.layout.height : 600f;
        Vector2 center = bounds.center;
        Vector2 viewCenter = new Vector2(viewWidth / 2, viewHeight / 2);
        Vector3 targetPos = viewCenter - center;
        UpdateViewTransform(targetPos, Vector3.one);
    }

    private void Traverse(BTNode node, HashSet<BTNode> visited)
    {
        if (node == null || visited.Contains(node)) return;
        visited.Add(node);
        var view = new BTNodeView(node, _tree.rootNode, this);
        _nodeViews[node.guid] = view;
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
        foreach (var kvp in _nodeViews)
        {
            var node = kvp.Value.Node;
            var parentView = kvp.Value;
            if (node is BTComposite composite && composite.children != null)
            {
                for (int i = 0; i < composite.children.Count; i++)
                {
                    var child = composite.children[i];
                    if (child == null || !_nodeViews.ContainsKey(child.guid)) continue;
                
                    var childView = _nodeViews[child.guid];
                    if (parentView.OutputPorts.Count <= i || childView.InputPort == null) continue;
                
                    var edge = parentView.OutputPorts[i].ConnectTo(childView.InputPort);
                    AddElement(edge);
                }
            }
            else if (node is BTDecorator decorator && decorator.child != null)
            {
                if (!_nodeViews.ContainsKey(decorator.child.guid)) continue;
            
                var childView = _nodeViews[decorator.child.guid];
                if (parentView.OutputPorts.Count <= 0 || childView.InputPort == null) continue;
            
                var edge = parentView.OutputPorts[0].ConnectTo(childView.InputPort);
                AddElement(edge);
            }
        }
    }

    private void OnEdgeConnected(Edge edge)
    {
        if (edge == null || edge.input == null || edge.output == null) return;
        var parentView = edge.output.node as BTNodeView;
        var childView = edge.input.node as BTNodeView;
        if (parentView == null || childView == null) return;
        var parentNode = parentView.Node;
        var childNode = childView.Node;
        // 자기 자신 연결 금지
        if (parentNode == childNode)
        {
            Debug.LogWarning("자기 자신을 연결할 수 없습니다.");
            RemoveElement(edge);
            return;
        }
        // 순환 참조 체크
        var visited = new HashSet<BTNode>();
        bool hasCycle = CheckCycle(parentNode, childNode, visited);
        if (hasCycle)
        {
            Debug.LogWarning("순환 참조가 발생하므로 연결할 수 없습니다.");
            RemoveElement(edge);
            return;
        }
        // 실제 트리 구조에 반영
        if (parentNode is BTComposite composite)
        {
            // 이미 연결된 경우 중복 방지
            if (!composite.children.Contains(childNode))
            {
                // 빈 슬롯(null) 있으면 그 위치에 연결, 없으면 추가
                int idx = composite.children.IndexOf(null);
                if (idx >= 0)
                    composite.children[idx] = childNode;
                else
                    composite.children.Add(childNode);
                childNode.input = parentNode;
            }
        }
        else if (parentNode is BTDecorator decorator)
        {
            if (decorator.child != childNode)
            {
                decorator.child = childNode;
                childNode.input = parentNode;
            }
        }
        EditorUtility.SetDirty(parentNode);
        EditorUtility.SetDirty(childNode);
        AssetDatabase.SaveAssets();
        RedrawTree();
    }

    private void OnEdgeDisconnected(Edge edge)
    {
        if (edge == null || edge.input == null || edge.output == null) return;
        var parentView = edge.output.node as BTNodeView;
        var childView = edge.input.node as BTNodeView;
        if (parentView == null || childView == null) return;
        var parentNode = parentView.Node;
        var childNode = childView.Node;
        // 트리 구조에서 연결 해제
        if (parentNode is BTComposite composite)
        {
            int idx = composite.children.IndexOf(childNode);
            if (idx >= 0)
                composite.children[idx] = null;
            childNode.input = null;
        }
        else if (parentNode is BTDecorator decorator)
        {
            if (decorator.child == childNode)
            {
                decorator.child = null;
                childNode.input = null;
            }
        }
        EditorUtility.SetDirty(parentNode);
        EditorUtility.SetDirty(childNode);
        AssetDatabase.SaveAssets();
        RedrawTree();
    }

    private bool CheckCycle(BTNode parent, BTNode child, HashSet<BTNode> visited)
    {
        if (parent == null || child == null) return false;
        if (parent == child) return true;
        if (!visited.Add(child)) return true;
        var children = BTEditorUtils.GetChildren(child);
        foreach (var c in children)
        {
            if (CheckCycle(parent, c, visited)) return true;
        }
        return false;
    }

    // 노드 생성 팝업을 띄우는 메서드
    public void ShowNodeCreatePopup(BTNodeView parentNodeView, int outPortIndex, Vector2 position)
    {
        var wnd = BTNodeCreatePopupWindow.ShowPopup(parentNodeView, outPortIndex, position, _tree, this);
    }
}
