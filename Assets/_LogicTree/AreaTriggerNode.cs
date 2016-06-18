using UnityEngine;
using System.Collections;
/// <summary>
/// 范围触发器节点，由当前对象身上的碰撞盒来定义
/// </summary>

public class AreaTriggerNode : LogicNode {
#if UNITY_EDITOR
    //作为一个碰撞触发器，我们要求这个GameObject做一些特殊的初始化
    public override void OnCreatedInEditor()
    {
        base.OnCreatedInEditor();
        //本来可以加一个RequireComponet来强制加Collider，但是我们允许根据需求加不同类型的
        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
    }
#endif



    void OnTriggerEnter(Collider other)
    {
        CheckPlayerTrigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        CheckPlayerTrigger(other);
    }

    void CheckPlayerTrigger(Collider other)
    {
        if (state == ENodeState.Running)
        {
            if (other.gameObject.tag == "Player") //这里我们只关系玩家的触发
            {
                FinishNode();
            }
        }
    }

	
}
