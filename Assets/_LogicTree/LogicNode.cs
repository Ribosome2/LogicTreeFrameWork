using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;

public enum ENodeState //节点的实际运行状态应该按照这个枚举定义的顺序执行
{
    Waiting=0,
    WaitForDelay=1,
    Running=2,
    Finished=3,
}

public class LogicNode : MonoBehaviour
{
    private const int NODE_WIDTH = 100;
    private const int NODE_HEIGHT = 80;
    private const int INPUT_WIDTH = 15;
    
    public List<LogicNode> linkNodes=new List<LogicNode>(); 

    private string mNodeName;
    public string NodeName
    {
        get { return mNodeName; }
        set
        {
            if (mNodeName != value)
            {
                gameObject.name = value;
            }
            mNodeName = value;
        }
    }

    public float delay;

    public  LogicNode parentNode;
    public ENodeState state;

    public void AddLink(LogicNode node)
    {
        if (!linkNodes.Contains(node))
        {
            if (node.parentNode)  //目前一个节点只允许有一个父节点
            {
                node.parentNode.RemoveLink(node);
            }
            node.parentNode = this;
            linkNodes.Add(node);
        }
    }

    public void RemoveLink(LogicNode node)
    {
        if (linkNodes.Contains(node))
        {
            linkNodes.Remove(node);
            node.parentNode = null;
        }
    }
#if UNITY_EDITOR
    public virtual void OnCreatedInEditor()
    {

    }
    

    public bool IsUiHit(Vector2 pos)
    {
        return frameRect.Contains(pos);
        Debug.Log("hit");
        return true;
    }
    public Rect frameRect
    {
        get
        {
           return new Rect(Position.x, Position.y, NODE_WIDTH, NODE_HEIGHT);
        }
    }

    public Rect inputRect
    {
        get
        {
             return  new Rect(frameRect.xMin + NODE_WIDTH * 0.5f - INPUT_WIDTH * 0.5f, frameRect.yMin - INPUT_WIDTH, INPUT_WIDTH, INPUT_WIDTH);
        }
    }

    public Rect outPutRect
    {
        get
        {
            return new Rect(frameRect.xMin + NODE_WIDTH * 0.5f - INPUT_WIDTH * 0.5f, frameRect.yMin+NODE_HEIGHT, INPUT_WIDTH, INPUT_WIDTH);
        }
    }

    public Vector2 InputPos
    {
        get
        {
            Rect rect = inputRect;
            return  new Vector2(rect.x+INPUT_WIDTH*0.5f,rect.y+INPUT_WIDTH*0.5f);
        }
    }

    public Vector2 OutputPos
    {
        get
        {
            Rect rect = outPutRect;
            return new Vector2(rect.x + INPUT_WIDTH * 0.5f, rect.yMax  );
        }
    }
#endif
    public Vector2 Position;

    public void Dispose()
    {
       Destroy(gameObject);
    }


    #region 运行时候的函数

    public virtual void OnStartNode()
    {
        state = ENodeState.WaitForDelay;
    }

    /// <summary>
    /// 只要是 运行就会调用的函数，过了延时时间之后会去调用OnRun
    /// </summary>
    public virtual void OnWaitForDelay()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            state = ENodeState.Running;
        }
    }

    /// <summary>
    /// 实际执行的函数
    /// </summary>
    public virtual void OnRunning()
    {
        
    }

    public virtual void FinishNode()
    {
        state = ENodeState.Finished;
        for (int i = 0; i < linkNodes.Count; i++)
        {
            linkNodes[i].OnStartNode();
        }
    }
    #endregion


#if UNITY_EDITOR
    public virtual void DrawProperty()
    {
        GUILayout.Label(NodeName);
        if (GUILayout.Button("Pin对象"))
        {
            EditorGUIUtility.PingObject(gameObject);
        }
        delay = EditorGUILayout.FloatField("延时", delay);
        
    }
#endif
}
