using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BTNodeView : Node
{
    public BTNode Node;
    public Port InputPort;
    public List<Port> OutputPorts = new List<Port>();
    private BTGraphView _graphView;
    private int _lastDragPortIndex = -1;

    public BTNodeView(BTNode node, BTNode rootNode, BTGraphView graphView)
    {
        Node = node;
        _graphView = graphView;
        title = node.name ?? node.GetType().Name;
        viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        if (node != rootNode)
        {
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = "In";
            // InputPort.AddManipulator(new EdgeConnector(new EdgeConnectorListener()));
            InputPort.AddManipulator(new EdgeConnector<Edge>(new BTEdgeConnectorListener()));
            inputContainer.Add(InputPort);
        }

        OutputPorts.Clear();
        outputContainer.Clear();
        if (node is BTComposite composite && composite.children != null)
        {
            for (int i = 0; i < composite.children.Count; i++)
            {
                var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                outPort.portName = $"Out {i + 1}";
                var edgeConnector = new EdgeConnector<Edge>(new EdgeConnectorListenerWithPopup(this, i));
                outPort.AddManipulator(edgeConnector);
                outputContainer.Add(outPort);
                OutputPorts.Add(outPort);
            }
        }
        else if (node is BTDecorator decorator)
        {
            var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outPort.portName = "Out";
            outPort.AddManipulator(new EdgeConnector<Edge>(new BTEdgeConnectorListener()));
            outputContainer.Add(outPort);
            OutputPorts.Add(outPort);
        }

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

        var label = node switch
        {
            BTSequence => "sequence",
            BTSelector => "selector",
            BTAction => "action",
            BTCondition => "condition",
            BTDecorator => "decorator",
            _ => ""
        };
        
        AddToClassList("node");
        
        if (!string.IsNullOrEmpty(label))
            AddToClassList(label);

        RefreshExpandedState();
        RefreshPorts();

        // 클릭 이벤트 등록
        RegisterCallback<MouseDownEvent>(OnNodeClicked);
    }

    private void OnNodeClicked(MouseDownEvent evt)
    {
        if (Node is BTSelector || Node is BTSequence)
        {
            BTEditorWindow.ShowNodeSettingsWindow(Node, _graphView);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position = newPos.position;
        UnityEditor.EditorUtility.SetDirty(Node);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}

// EdgeConnectorListener 확장: 빈 공간에 드롭 시 노드 생성 팝업 호출
public class EdgeConnectorListenerWithPopup : IEdgeConnectorListener
{
    private BTNodeView _nodeView;
    private int _portIndex;
    public EdgeConnectorListenerWithPopup(BTNodeView nodeView, int portIndex)
    {
        _nodeView = nodeView;
        _portIndex = portIndex;
    }
    public void OnDrop(GraphView graphView, Edge edge)
    {
        graphView.AddElement(edge);
    }
    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        // 빈 공간에 드롭 시 노드 생성 팝업 호출
        var graphView = edge.GetFirstAncestorOfType<BTGraphView>();
        if (graphView != null)
        {
            graphView.ShowNodeCreatePopup(_nodeView, _portIndex, position);
            graphView.RemoveElement(edge);
        }
    }
}