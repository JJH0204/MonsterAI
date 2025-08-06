using System.Collections.Generic;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BTEditorWindow : EditorWindow
{
    private BTGraphView _graphView;
    private BehaviorTree _currentTree;
    private BTNode _selectedNode;

    [MenuItem("Window/AI/Behavior Tree Editor")]
    public static void Open()
    {
        GetWindow<BTEditorWindow>("Behavior Tree");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
        Refresh();
    }
    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        rootVisualElement.Clear();

        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;
        container.style.flexGrow = 1;

        var topBar = new VisualElement();
        topBar.style.height = 40;
        topBar.style.flexShrink = 0;
        topBar.style.flexDirection = FlexDirection.Row;

        var saveButton = new Button(SaveTreeNodes) { text = "Save Tree" };
        topBar.Add(saveButton);

        // var nodeTypes = new List<string> { "BTSequence", "BTSelector", "BTAction", "BTCondition", "BTDecorator" };
        // var nodeTypeField = new PopupField<string>(nodeTypes, 0);
        // nodeTypeField.style.width = 120;
        // topBar.Add(nodeTypeField);
        //
        // var addNodeButton = new Button(() => AddNode(nodeTypeField.value)) { text = "노드 추가" };
        // topBar.Add(addNodeButton);

        var deleteNodeButton = new Button(DeleteSelectedNode) { text = "노드 삭제" };
        deleteNodeButton.SetEnabled(_selectedNode != null);
        topBar.Add(deleteNodeButton);

        var deleteOrphanNodeButton = new Button(DeleteOrphanNode) { text = "고아 노드 삭제" };
        deleteOrphanNodeButton.SetEnabled(true);
        topBar.Add(deleteOrphanNodeButton);

        var deleteAllOrphanNodesButton = new Button(DeleteAllOrphanNodes) { text = "모든 고아 노드 일괄 삭제" };
        deleteAllOrphanNodesButton.SetEnabled(true);
        topBar.Add(deleteAllOrphanNodesButton);

        container.Add(topBar);

        if (Selection.activeObject is BehaviorTree selectedTree)
        {
            _currentTree = selectedTree;
            _graphView = new BTGraphView(_currentTree);
            _graphView.style.flexGrow = 1;
            _graphView.RegisterCallback<MouseDownEvent>(OnGraphViewMouseDown);
            container.Add(_graphView);
        }
        else
        {
            var label = new Label("BehaviorTree ScriptableObject를 선택하세요.")
            {
                style = { unityTextAlign = TextAnchor.MiddleCenter, fontSize = 14, marginTop = 20 }
            };
            container.Add(label);
        }
        rootVisualElement.Add(container);
    }

    private void OnGraphViewMouseDown(MouseDownEvent evt)
    {
        if (_graphView == null) return;
        var selection = _graphView.selection;
        _selectedNode = null;
        foreach (var item in selection)
        {
            if (item is BTNodeView nodeView)
            {
                _selectedNode = nodeView.Node;
                break;
            }
        }
        Refresh(); // 버튼 활성화 갱신
        // 노드 설정창 갱신을 위해 그래프 뷰 전달
        if (_selectedNode is BTSelector || _selectedNode is BTSequence)
        {
            ShowNodeSettingsWindow(_selectedNode, _graphView);
        }
    }

    private void AddNode(string nodeType)
    {
        if (_currentTree == null) return;
        BTNode newNode = null;
        switch (nodeType)
        {
            case "BTSequence":
                newNode = BTEditorUtils.CreateNode<BTSequence>(_currentTree);
                break;
            case "BTSelector":
                newNode = BTEditorUtils.CreateNode<BTSelector>(_currentTree);
                break;
            case "BTAction":
                newNode = BTEditorUtils.CreateNode<BTAction>(_currentTree);
                break;
            case "BTCondition":
                newNode = BTEditorUtils.CreateNode<BTCondition>(_currentTree);
                break;
            case "BTDecorator":
                newNode = BTEditorUtils.CreateNode<BTDecorator>(_currentTree);
                break;
        }
        if (newNode != null)
        {
            Debug.Log($"노드 추가: {newNode.name}");
            Refresh(); // 그래프 갱신
        }
    }

    // 트리의 모든 노드 정보를 저장하는 함수
    private void SaveTreeNodes()
    {
        if (_currentTree == null || _currentTree.rootNode == null) return;
        var visited = new HashSet<BTNode>();
        SaveNodeRecursive(_currentTree.rootNode, visited);
        EditorUtility.SetDirty(_currentTree);
        AssetDatabase.SaveAssets();
        Debug.Log("[BTEditorWindow] 트리 노드 정보가 저장되었습니다.");
    }

    private void SaveNodeRecursive(BTNode node, HashSet<BTNode> visited)
    {
        if (node == null || !visited.Add(node)) return;
        EditorUtility.SetDirty(node);
        if (node is BTComposite composite && composite.children != null)
        {
            foreach (var child in composite.children)
                SaveNodeRecursive(child, visited);
        }
        else if (node is BTDecorator decorator && decorator.child != null)
        {
            SaveNodeRecursive(decorator.child, visited);
        }
    }

    private void DeleteSelectedNode()
    {
        if (_selectedNode == null || _currentTree == null) return;
        // 부모에서 참조 제거
        if (_selectedNode.input is BTComposite parentComposite)
        {
            parentComposite.children.Remove(_selectedNode);
        }
        else if (_selectedNode.input is BTDecorator parentDecorator)
        {
            if (parentDecorator.child == _selectedNode)
                parentDecorator.child = null;
        }
        // 트리의 루트 노드라면 rootNode도 null로
        if (_currentTree.rootNode == _selectedNode)
        {
            _currentTree.rootNode = null;
        }
        // 에셋에서 제거
        AssetDatabase.RemoveObjectFromAsset(_selectedNode);
        DestroyImmediate(_selectedNode, true);
        AssetDatabase.SaveAssets();
        _selectedNode = null;
        Refresh();
    }

    private bool IsOrphanNode(BTNode node)
    {
        if (node == null) return false;
        if (node.input != null) return false;
        if (node is BTComposite composite && composite.children != null && composite.children.Count > 0)
            return false;
        if (node is BTDecorator decorator && decorator.child != null)
            return false;
        return true;
    }

    private void DeleteOrphanNode()
    {
        if (_selectedNode == null || !IsOrphanNode(_selectedNode)) return;
        AssetDatabase.RemoveObjectFromAsset(_selectedNode);
        DestroyImmediate(_selectedNode, true);
        AssetDatabase.SaveAssets();
        _selectedNode = null;
        Refresh();
    }

    private void DeleteAllOrphanNodes()
    {
        if (_currentTree == null) return;
        string treePath = AssetDatabase.GetAssetPath(_currentTree);
        var assets = AssetDatabase.LoadAllAssetsAtPath(treePath);
        int deleteCount = 0;
        foreach (var asset in assets)
        {
            if (asset is BTNode node && IsOrphanNode(node))
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                DestroyImmediate(node, true);
                deleteCount++;
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"삭제된 고아 노드 수: {deleteCount}");
        Refresh();
    }

    public static void ShowNodeSettingsWindow(BTNode node, BTGraphView graphView)
    {
        var wnd = GetWindow<BTNodeSettingsPopupWindow>(true, "노드 설정", true);
        wnd.SetTargetNode(node, graphView);
        wnd.Show();
    }
}