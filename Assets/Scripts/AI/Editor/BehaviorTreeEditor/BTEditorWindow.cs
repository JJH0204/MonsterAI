using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;

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
                _graphView = new BTGraphView(_currentTree);
                _graphView.StretchToParentSize();
                rootVisualElement.Add(_graphView);
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
    }
}