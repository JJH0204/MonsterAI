using UnityEditor;
using AI.BehaviorTree;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public class BTEditorWindow : EditorWindow
    {
        private BTGraphView _graphView;
        private BehaviorTree _currentTree;

        [MenuItem("Window/AI/Behavior Tree Editor")]
        // public static void ShowWindow()
        public static void Open()
        {
            var window = GetWindow<BTEditorWindow>("Behavior Tree Editor");
            window.titleContent = new GUIContent("Behavior Tree Graph");
            window.Show();
        }

        // private void OnEnable()
        // {
        //     // Selection.selectionChanged += OnSelectionChanged;
        //     rootVisualElement.Clear();  // 기존 root를 비워 중복 방지
        //
        //     var selectedObject = Selection.activeObject;
        //     
        //     _graphView = new BTGraphView();
        //     _graphView.StretchToParentSize();
        //     rootVisualElement.Add(_graphView);
        //     
        //     if (selectedObject != null)
        //     {
        //         // TODO: 노드 연결 정보 불러오기
        //         // LoadGraphFromObject();
        //     }
        //     else
        //     {
        //         CreateDefaultGraph();
        //     }
        // }
        
        // 빈 그래프 생성 및 기본 루트 노드 생성
        private void CreateDefaultGraph()
        {
            var temp = BTEditorUtils.CreateRootNode(_currentTree, "Root");
            var node = new BTNodeView(temp);
            node.SetPosition(new Rect(100, 200, 200, 150));
            _graphView.AddElement(node);
        }
    }
}