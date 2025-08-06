using System;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BTNodeView : Node
{
    public BTNode node;
    public Port inputPort;
    public List<Port> outputPorts = new List<Port>();

    public BTNodeView(BTNode node, BTNode rootNode)
    {
        this.node = node;
        // 항상 에셋 이름을 타이틀로 사용
        if (node is UnityEngine.Object obj)
        {
            title = obj.name;
        }
        else
        {
            title = node.name ?? node.GetType().Name;
        }
        viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        // 루트 노드가 아닌 경우 inputPort 생성
        if (node != rootNode)
        {
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "In";
            inputContainer.Add(inputPort);
        }

        // Composite: 자식 수만큼 출력 포트, Decorator: 1개(있으면), Leaf: 0개
        if (node is AI.BehaviorTree.Nodes.BTComposite composite && composite.children != null)
        {
            for (int i = 0; i < composite.children.Count; i++)
            {
                var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                outPort.portName = $"Out {i + 1}";
                outputContainer.Add(outPort);
                outputPorts.Add(outPort);
            }
        }
        else if (node is AI.BehaviorTree.Nodes.BTDecorator decorator && decorator.child != null)
        {
            var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outPort.portName = "Out";
            outputContainer.Add(outPort);
            outputPorts.Add(outPort);
        }
        // Leaf 노드는 출력 포트 없음

        // 노드 상태에 따라 스타일 적용
        switch (node.state)
        {
            case NodeState.Success:
                AddToClassList("bt-node-success");
                break;
            case NodeState.Failure:
                AddToClassList("bt-node-failure");
                break;
            case NodeState.Running:
                AddToClassList("bt-node-running");
                break;
        }

        // 노드 타입별 스타일 클래스 적용 (컨디션, 액션, 루트 포함)
        if (node is BTSequence)
        {
            AddToClassList("node");
            AddToClassList("sequence");
        }
        else if (node is BTSelector)
        {
            AddToClassList("node");
            AddToClassList("selector");
        }
        else if (node is BTAction)
        {
            AddToClassList("node");
            AddToClassList("action");
        }
        else if (node is BTCondition)
        {
            AddToClassList("node");
            AddToClassList("condition");
        }
        else if (node is BTDecorator)
        {
            AddToClassList("node");
            AddToClassList("decorator");
        }
        else
        {
            AddToClassList("node");
        }

        RefreshExpandedState();
        RefreshPorts();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.position = newPos.position;

        // 자동 저장 로직 추가
        UnityEditor.EditorUtility.SetDirty(node);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}