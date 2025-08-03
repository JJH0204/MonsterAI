using System;
using AI.BehaviorTree.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BTNodeView : Node
{
    public BTNode node;

    public Port inputPort;
    public Port outputPort;

    public BTNodeView(BTNode node)
    {
        this.node = node;
        title = node.name;
        viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        inputPort.portName = "In";
        inputContainer.Add(inputPort);

        outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        outputPort.portName = "Out";
        outputContainer.Add(outputPort);

        RefreshExpandedState();
        RefreshPorts();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.position = newPos.position;
    }
}