using UnityEditor;
using UnityEngine;
using AI.BehaviorTree.Nodes;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class BTNodeSettingsPopupWindow : VisualElement
{
    private BTNode _targetNode;
    private Label _nodeTypeLabel;
    private TextField _nodeNameField;
    private System.Action _onNodeChanged;
    private BTGraphView _graphView;
    private Editor _nodeEditor;
    private IMGUIContainer _inspectorContainer;

    public BTNodeSettingsPopupWindow()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("BTStyles"));
        style.flexDirection = FlexDirection.Column;
        AddToClassList("settings-popup"); // 설정 팝업 스타일 적용
    }

    public void SetTargetNode(BTNode node, BTGraphView graphView)
    {
        _targetNode = node;
        _graphView = graphView;
        CreateUI();
    }

    private void CreateUI()
    {
        Clear();
        if (_targetNode == null)
        {
            var label = new Label("노드가 선택되지 않았습니다.");
            label.AddToClassList("unity-label");
            Add(label);
            return;
        }
        // UIElements 기반 커스텀 인스팩터 예시
        var nameField = new TextField("노드 이름") { value = _targetNode.name };
        nameField.isDelayed = true;
        nameField.AddToClassList("unity-text-field");
        nameField.RegisterValueChangedCallback(evt =>
        {
            if (_targetNode.name != evt.newValue)
            {
                _targetNode.name = evt.newValue;
                EditorUtility.SetDirty(_targetNode);
                AssetDatabase.SaveAssets();
                _graphView?.RedrawTree();
                nameField.Focus();
            }
        });
        Add(nameField);

        // 자식 노드 UI (Composite, Decorator)
        if (_targetNode is BTComposite composite)
        {
            var childrenLabel = new Label("자식 노드 목록");
            childrenLabel.AddToClassList("unity-label");
            Add(childrenLabel);
            for (int i = 0; i < composite.children.Count; i++)
            {
                var childField = new ObjectField($"자식 {i + 1}")
                {
                    objectType = typeof(BTNode),
                    value = composite.children[i],
                    allowSceneObjects = false
                };
                childField.AddToClassList("unity-object-field");
                int idx = i;
                childField.RegisterValueChangedCallback(evt =>
                {
                    composite.children[idx] = evt.newValue as BTNode;
                    EditorUtility.SetDirty(_targetNode);
                    AssetDatabase.SaveAssets();
                    _graphView?.RedrawTree();
                    childField.Focus();
                });
                Add(childField);
            }
            // 자식 노드 추가 버튼
            var addChildBtn = new Button(() =>
            {
                composite.children.Add(null);
                EditorUtility.SetDirty(_targetNode);
                AssetDatabase.SaveAssets();
                _graphView?.RedrawTree();
            }) { text = "자식 노드 추가" };
            addChildBtn.AddToClassList("unity-button");
            Add(addChildBtn);
            
            // 자식 노드 삭제 버튼
            var removeChildBtn = new Button(() =>
            {
                if (composite.children.Count > 0)
                {
                    composite.children.RemoveAt(composite.children.Count - 1);
                    EditorUtility.SetDirty(_targetNode);
                    AssetDatabase.SaveAssets();
                    _graphView?.RedrawTree();
                }
            }) { text = "자식 노드 삭제" };
            removeChildBtn.AddToClassList("unity-button");
            Add(removeChildBtn);
        }
        else if (_targetNode is BTDecorator decorator)
        {
            var childField = new ObjectField("자식 노드")
            {
                objectType = typeof(BTNode),
                value = decorator.child,
                allowSceneObjects = false
            };
            childField.AddToClassList("unity-object-field");
            childField.RegisterValueChangedCallback(evt =>
            {
                decorator.child = evt.newValue as BTNode;
                EditorUtility.SetDirty(_targetNode);
                AssetDatabase.SaveAssets();
                _graphView?.RedrawTree();
                childField.Focus();
            });
            Add(childField);

            // 쿨타임 데코레이터
            if (decorator is BTCooldown cooldown)
            {
                var timeField = new FloatField("Cooldown Time") { value = cooldown.cooldownTime };
                timeField.RegisterValueChangedCallback(evt =>
                {
                    cooldown.cooldownTime = evt.newValue;
                    EditorUtility.SetDirty(cooldown);
                    AssetDatabase.SaveAssets();
                });
                Add(timeField);
            }

            // 딜레이 타임 데코레이터
            if (decorator is BTDelay delay)
            {
                var timeField = new FloatField("Delay Time") { value = delay.delayTime };
                timeField.RegisterValueChangedCallback(evt =>
                {
                    delay.delayTime = evt.newValue;
                    EditorUtility.SetDirty(delay);
                    AssetDatabase.SaveAssets();
                });
                Add(timeField);
            }
        }
        else if (_targetNode is BTAction action)
        {
            // 액션 노드의 경우, 스크립트 필드 추가
        }
        else if (_targetNode is BTCondition condition)
        {
            // 조건 노드의 경우, 조건 필드 추가
        }
    }
}
