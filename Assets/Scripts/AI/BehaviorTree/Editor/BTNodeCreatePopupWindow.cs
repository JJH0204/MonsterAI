using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class BTNodeCreatePopupWindow : EditorWindow
{
    private BTNodeView _parentNodeView;
    private int _outPortIndex;
    private BehaviorTree _tree;
    private BTGraphView _graphView;
    private TextField _nameField;
    private PopupField<string> _typeField;
    private List<Type> _nodeTypes;
    private Vector2 _popupPosition;

    public static BTNodeCreatePopupWindow ShowPopup(BTNodeView parentNodeView, int outPortIndex, Vector2 position, BehaviorTree tree, BTGraphView graphView)
    {
        var wnd = CreateInstance<BTNodeCreatePopupWindow>();
        wnd._parentNodeView = parentNodeView;
        wnd._outPortIndex = outPortIndex;
        wnd._tree = tree;
        wnd._graphView = graphView;
        wnd._popupPosition = position;
        wnd.titleContent = new GUIContent("노드 생성");
        wnd.position = new Rect(position.x, position.y, 320, 160);
        wnd.ShowPopup();
        wnd.CreateUI();
        return wnd;
    }

    private void CreateUI()
    {
        rootVisualElement.Clear();
        _nodeTypes = GetAllNodeTypes();
        var typeNames = _nodeTypes.Select(t => t.Name).ToList();
        _typeField = new PopupField<string>(typeNames, 0);
        rootVisualElement.Add(new Label("노드 타입 선택:"));
        rootVisualElement.Add(_typeField);
        _nameField = new TextField("노드 이름") { value = "NewNode" };
        rootVisualElement.Add(_nameField);
        var createBtn = new Button(OnCreateNode) { text = "생성" };
        rootVisualElement.Add(createBtn);
    }

    private List<Type> GetAllNodeTypes()
    {
        var baseType = typeof(BTNode);
        var asm = baseType.Assembly;
        return asm.GetTypes()
            .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract && t.IsClass)
            .ToList();
    }

    private void OnCreateNode()
    {
        int idx = _typeField.index;
        if (idx < 0 || idx >= _nodeTypes.Count) return;
        var nodeType = _nodeTypes[idx];
        var node = ScriptableObject.CreateInstance(nodeType) as BTNode;
        if (node == null) return;
        node.guid = System.Guid.NewGuid().ToString();
        node.position = _popupPosition;
        node.name = _nameField.value; // BTNode의 name만 입력값으로 설정
        AssetDatabase.AddObjectToAsset(node, _tree);
        AssetDatabase.SaveAssets();
        // 부모 노드에 연결
        if (_parentNodeView.Node is BTComposite composite)
        {
            if (_outPortIndex < composite.children.Count)
                composite.children[_outPortIndex] = node;
            else
                composite.children.Add(node);
            node.input = _parentNodeView.Node;
        }
        else if (_parentNodeView.Node is BTDecorator decorator)
        {
            decorator.child = node;
            node.input = _parentNodeView.Node;
        }
        EditorUtility.SetDirty(node);
        EditorUtility.SetDirty(_parentNodeView.Node);
        AssetDatabase.SaveAssets();
        _graphView?.RedrawTree();
        Close();
    }
}
