using UnityEditor;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
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

            if (Selection.activeObject is BehaviorTree selectedTree)
            {
                _currentTree = selectedTree;

                _graphView = new BTGraphView();
                _graphView.StretchToParentSize();
                // _graphView.LoadFromBehaviorTree(_currentTree);
                rootVisualElement.Add(_graphView);
                if (selectedTree != null)
                {
                    // TODO: 노드 연결 정보 불러오기
                    LoadGraphFromObject();
                    LoadGraphConnections();
                }
                else
                {
                    CreateDefaultGraph();
                }
            }
            else
            {
                Label label = new Label("BehaviorTree ScriptableObject를 선택하세요.");
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.style.fontSize = 14;
                label.style.marginTop = 20;
                rootVisualElement.Add(label);
            }
        }
        
        // 빈 그래프 생성 및 기본 루트 노드 생성
        private void CreateDefaultGraph()
        {
            var temp = BTEditorUtils.CreateRootNode(_currentTree, "Root");
            var node = new BTNodeView(temp);
            node.SetPosition(new Rect(100, 200, 200, 150));
            _graphView.AddElement(node);
        }
        
        // 선택된 오브젝트에서 그래프를 불러오기
        private void LoadGraphFromObject()
        {
            var selectedObject = Selection.activeObject;        // 에디터에서 선택한 객체 정보를 가져온다.
            
            if (selectedObject is not BehaviorTree tree)        // 선택한 객체가 BT가 아니면 
            {
                Debug.LogWarning("Selected object is not a BehaviorTree. Please select a valid BehaviorTree asset.");
                return;
            }
            
            // var tree = treeRunner.Tree;
            if (tree == null)                               // 선택한 BT가 비어있으면
            {
                Debug.LogWarning("Selected BehaviorTreeRunner does not have a valid BehaviorTree assigned.");
                return;
            }
            
            _currentTree = tree;
            // _graphView.PopulateView(tree);                  // 그래프 뷰에 트리 데이터 채우기
        
            // 선택된 트리의 노드들을 그래프 뷰에 추가
            var nodes = tree.GetAllNodes();
            foreach (var node in nodes)
            {
                var nodeView = new BTNodeView(node);
                nodeView.SetPosition(new Rect(node.position.x, node.position.y, 200, 150));
                _graphView.AddElement(nodeView);
            }
        }
        
        // TODO: 그래프 연결 정보 불러오기
        private void LoadGraphConnections()
        {
            if (_currentTree == null) return;

            foreach (var node in _currentTree.GetAllNodes())
            {
                var nodeView = _graphView.CreateNodeView(node);
                nodeView.SetPosition(new Rect(node.position.x, node.position.y, 200, 150));

                // 입력 포트 연결
                if (node.input != null)
                {
                    
                }
                
                // 출력 포트 연결
                if (node.outputs != null)
                {
                    foreach (var outputEdge in node.outputs)
                    {
                        // 출력 포트가 있는 경우 연결
                        // var outputPort = nodeView.outputPort;
                        // if (outputPort == null) continue;
                        // var inputPort = _graphView.GetNodeByGuid(outputEdge.guid)?.inputPort;
                        //
                        // if (inputPort == null) continue;
                        // var edge = new Edge
                        // {
                        //     output = outputPort,
                        //     input = inputPort
                        // };
                        //
                        // edge.output.Connect(edge);
                        // edge.input.Connect(edge);
                        // _graphView.AddElement(edge);
                    }
                }
            }
        }
    }
}