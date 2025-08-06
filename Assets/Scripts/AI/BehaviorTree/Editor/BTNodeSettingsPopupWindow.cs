using UnityEditor;
using UnityEngine;
using AI.BehaviorTree.Nodes;
using UnityEngine.UIElements;

public class BTNodeSettingsPopupWindow : EditorWindow
{
    private BTNode _targetNode;
    private Label _nodeTypeLabel;
    private TextField _nodeNameField;
    private System.Action _onNodeChanged;
    private BTGraphView _graphView;

    public void SetTargetNode(BTNode node, BTGraphView graphView)
    {
        _targetNode = node;
        _graphView = graphView;
        CreateUI();
    }

    private void CreateUI()
    {
        rootVisualElement.Clear();
        if (_targetNode == null)
        {
            rootVisualElement.Add(new Label("노드가 선택되지 않았습니다."));
            return;
        }

        _nodeTypeLabel = new Label($"노드 타입: {_targetNode.GetType().Name}");
        rootVisualElement.Add(_nodeTypeLabel);

        _nodeNameField = new TextField("노드 이름") { value = _targetNode.name };
        _nodeNameField.RegisterValueChangedCallback(evt =>
        {
            _targetNode.name = evt.newValue;
            EditorUtility.SetDirty(_targetNode);
            AssetDatabase.SaveAssets();
            _onNodeChanged?.Invoke();
        });
        rootVisualElement.Add(_nodeNameField);

        // Composite 노드의 경우 자식 리스트 Inspector 스타일로 표시 (읽기 전용)
        if (_targetNode is BTComposite composite)
        {
            var childList = new ListView(composite.children, 20, () => new Label(), (elem, i) =>
            {
                var child = composite.children[i];
                (elem as Label).text = child != null ? child.name : "(비어 있음)";
            });
            childList.selectionType = SelectionType.None;
            childList.style.marginTop = 8;
            childList.style.marginBottom = 8;
            rootVisualElement.Add(new Label("자식 노드 리스트 (Out 포트 수):"));
            rootVisualElement.Add(childList);

            // Out 포트 추가 버튼
            var addOutPortBtn = new Button(() =>
            {
                composite.children.Add(null); // 빈 슬롯 추가
                EditorUtility.SetDirty(_targetNode);
                AssetDatabase.SaveAssets();
                _graphView?.RedrawTree(); // 그래프 뷰 즉시 갱신
                CreateUI(); // 설정창 즉시 갱신
            }) { text = "Out 포트 추가" };
            rootVisualElement.Add(addOutPortBtn);

            // Out 포트 삭제 버튼
            var removeOutPortBtn = new Button(() =>
            {
                if (composite.children.Count == 0)
                {
                    Debug.LogWarning("삭제할 Out 포트가 없습니다.");
                    return;
                }
                var lastChild = composite.children[composite.children.Count - 1];
                if (lastChild != null)
                {
                    Debug.LogWarning("마지막 Out 포트에 연결된 자식 노드가 있으므로 삭제할 수 없습니다.");
                    return;
                }
                composite.children.RemoveAt(composite.children.Count - 1);
                EditorUtility.SetDirty(_targetNode);
                AssetDatabase.SaveAssets();
                _graphView?.RedrawTree(); // 그래프 뷰 즉시 갱신
                CreateUI(); // 설정창 즉시 갱신
            }) { text = "Out 포트 삭제" };
            rootVisualElement.Add(removeOutPortBtn);
        }
        // Decorator 노드의 경우 자식 정보만 표시 (읽기 전용)
        else if (_targetNode is BTDecorator decorator)
        {
            rootVisualElement.Add(new Label("자식 노드 (Out 포트):"));
            var childName = decorator.child != null ? decorator.child.name : "(비어 있음)";
            var childLabel = new Label(childName);
            rootVisualElement.Add(childLabel);
        }
    }
}
