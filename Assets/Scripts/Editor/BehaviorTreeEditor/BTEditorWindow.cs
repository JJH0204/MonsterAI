using AI.BehaviorTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BTEditorWindow : EditorWindow
{
    private BTGraphView _graphView;
    private BehaviorTree _tree;

    [MenuItem("Tools/Behavior Tree Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<BTEditorWindow>();
        window.titleContent = new GUIContent("Behavior Tree Editor");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraphView()
    {
        _graphView = new BTGraphView { name = "Behavior Tree Graph" };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    // private void GenerateToolbar()
    // {
    //     var toolbar = new Toolbar();
    //
    //     var newTreeButton = new Button(() => _graphView.CreateNewTree()) { text = "New Tree" };
    //     toolbar.Add(newTreeButton);
    //
    //     var addActionNode = new Button(() =>
    //     {
    //         var node = new BTNodeView("Action");
    //         node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));
    //         _graphView.AddElement(node);
    //     }) { text = "Add Action Node" };
    //     toolbar.Add(addActionNode);
    //
    //     rootVisualElement.Add(toolbar);
    // }
    
    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var loadBtn = new Button(() => _graphView.PopulateView(_tree)) { text = "Load Tree" };
        toolbar.Add(loadBtn);

        var saveBtn = new Button(() => _graphView.SaveTree()) { text = "Save Tree" };
        toolbar.Add(saveBtn);

        rootVisualElement.Add(toolbar);
    }
}