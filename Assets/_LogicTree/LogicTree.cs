using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LogicTree : MonoBehaviour {
    public List<LogicNode> nodeList=new List<LogicNode>();

    public void AddChildNode<T>(Vector2 initPos) where T:LogicNode
    {
        GameObject nodeObj=new GameObject(typeof(T).Name);
        nodeObj.transform.parent = transform;
        nodeObj.transform.localPosition = Vector3.zero;
        LogicNode node = nodeObj.AddComponent<T>();
        node.NodeName= nodeObj.name;
        node.Position = initPos;
#if UNITY_EDITOR
        node.OnCreatedInEditor();
#endif
        nodeList.Add(node);
    }

    public void RemoveChildNode(LogicNode node) 
    {
        if (nodeList.Contains(node))
        {
            node.Dispose();
            nodeList.Remove(node);
        }
    }

    public void OnUpdate()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            LogicNode node = nodeList[i];
            if (node.state == ENodeState.WaitForDelay)
            {
                node.OnWaitForDelay();
            }else if (node.state == ENodeState.Running)
            {
                node.OnRunning();
            }
        }
    }


}
