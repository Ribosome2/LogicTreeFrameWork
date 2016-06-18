using UnityEngine;
using System.Collections;
using UnityEditor;

public class LogicTreeEditWindow : EditorWindow {
    
    [MenuItem("Tools/LogicTree/OpenWindow")]
    static void OpenWindow()
    {
        GetWindow<LogicTreeEditWindow>();
    }

    enum  DragFlag
    {
        None,
        NodeFrame, //拖动节点框
        InputNode,//正在拖动输入节点，可能连接到其他节点的输出
        OutputNode,//正在拖动输出节点，可能连接到其他节点的输入
    }

    private DragFlag dragFlag = DragFlag.None;
    private LogicTree mLogicTree;

    private LogicTree CurLogicTree
    {
        get
        {
            if (mLogicTree == null)
            {
                mLogicTree = GameObject.FindObjectOfType<LogicTree>();

                if (mLogicTree == null)
                {
                    GameObject treeObj=new GameObject("Tree_Undifine");
                    mLogicTree = treeObj.AddComponent<LogicTree>();
                }
            }
            return mLogicTree;
        }
    }

    private LogicNode curDragControlNode;  //当前拖动控制的节点记录

    private LogicNode curSelectedNode;    //当前选中的节点


    void OnGUI()
    {
        GUILayout.Label("DragFlag "+dragFlag);
        if (CurLogicTree)
        {
            DrawNodesOnTheSide();
            DrawCreatedNodes();
            HandleInput();
        }
    }


    void HandleInput()
    {
        Event curEvent = Event.current;
        GUILayout.Label("Mouse pos " + curEvent.mousePosition);
        if (curEvent != null && curEvent.type!=EventType.Repaint)
        {


            #region 鼠标左键按下
            if (curEvent.isMouse && curEvent.button == 0 && curEvent.type == EventType.MouseDown)
            {
                for (int i = 0; i < CurLogicTree.nodeList.Count; i++)
                {
                    LogicNode node = CurLogicTree.nodeList[i];
                    if (node.IsUiHit(curEvent.mousePosition))
                    {
                        dragFlag = DragFlag.NodeFrame;
                        curDragControlNode = node;
                        curSelectedNode = node;
                        curEvent.Use();
                        return;
                    }

                    if (node.inputRect.Contains(curEvent.mousePosition))
                    {
                        //如果是已经有父节点了，就暂时移除父节点对这个节点的连接，如果放开位置还在这个输入区就会重新连上，不在的话就是移除
                        if (node.parentNode)
                        {
                            dragFlag = DragFlag.OutputNode;
                            curDragControlNode = node.parentNode;
                            node.parentNode.RemoveLink(node);
                        }
                        else   //没有选中就等着连接到其他的输出区
                        {
                            dragFlag = DragFlag.InputNode;
                            curDragControlNode = node;
                        }
                        curEvent.Use();
                        return;
                    }

                    if (node.outPutRect.Contains(curEvent.mousePosition))
                    {
                        dragFlag = DragFlag.OutputNode;
                        curDragControlNode = node;
                        curEvent.Use();
                        return;
                    }

                }

                //没有合适的选中，就是node
                dragFlag = DragFlag.None;
                curDragControlNode = null;

            } 
            #endregion


            #region 鼠标左键抬起
            if (curEvent.isMouse && curEvent.button == 0 && curEvent.type == EventType.MouseUp)
            {
                for (int i = 0; i < CurLogicTree.nodeList.Count; i++)
                {
                    LogicNode node = CurLogicTree.nodeList[i];
                    if (node == curDragControlNode) //不用处理跟按下的时候相同的节点
                    {
                        continue;
                    }
                    if (dragFlag == DragFlag.InputNode) //按下时候记录的是拖动节点的输入，我们就只关心是不是在其它节点的输出放开
                    {
                        if (node.outPutRect.Contains(curEvent.mousePosition))
                        {
                            node.AddLink(curDragControlNode);
                            Repaint();
                            break;
                        }
                    }else if (dragFlag == DragFlag.OutputNode)//按下的是节点输出，只需要关注是不是输入节点
                    {
                        if (node.inputRect.Contains(curEvent.mousePosition))
                        {
                            curDragControlNode.AddLink( node);
                            break;
                        }
                    }
                }

                //没有合适的选中，就是node
                dragFlag = DragFlag.None;
                curDragControlNode = null;

            }
            #endregion


           
            
            #region 鼠标右键拖动
            if (curEvent.isMouse && curEvent.button == 1 && curEvent.type == EventType.MouseDrag)
            {
                for (int i = 0; i < CurLogicTree.nodeList.Count; i++)
                {
                    LogicNode node = CurLogicTree.nodeList[i];
                    node.Position += curEvent.delta;
                }
                Repaint();
            } 
            #endregion

           
            #region 鼠标左键拖动
            if (curEvent.isMouse && curEvent.button == 0 && curEvent.type == EventType.MouseDrag)
            {
                if (dragFlag == DragFlag.NodeFrame && curDragControlNode.IsUiHit(curEvent.mousePosition))
                {
                    curDragControlNode.Position += curEvent.delta;
                    curEvent.Use();
                    Repaint();
                }
            } 
            #endregion
        }
    }

    void DrawCreatedNodes()
    {
        for (int i = 0; i < CurLogicTree.nodeList.Count; i++)
        {
            LogicNode node = CurLogicTree.nodeList[i];
            DrawSingleNode(node);
        }


        if (dragFlag == DragFlag.InputNode || dragFlag == DragFlag.OutputNode)
        {
            Vector2 start = dragFlag == DragFlag.InputNode
                ? curDragControlNode.InputPos
                : curDragControlNode.OutputPos;
            LogicTreeUIUtility.DrawBezierBy2Points(start, Event.current.mousePosition);
            Repaint();
        }
    }

    void DrawSingleNode(LogicNode node)
    {
        Rect frameRect = node.frameRect;
        //注意这里不能用Button函数， 因为会导致Event.current检测不到事件，应该是因为这个函数判断鼠标在Button上面的时候会调用Event.use()
        Color guiColor = GUI.color;
        GUI.color = node == curSelectedNode ? Color.yellow : guiColor;
        GUI.Box(node.frameRect,"", GUI.skin.FindStyle("button"));
        GUI.color = guiColor;
        Rect typeRect=new Rect(frameRect.x,frameRect.y+10,frameRect.width,30);
        GUI.Label(typeRect,node.GetType().Name);
        int width=20;
        
        GUI.Box(node.inputRect,LogicTreeUIUtility.DotIcon);
        GUI.Box(node.outPutRect, LogicTreeUIUtility.DotIcon);
        if (node.linkNodes.Count > 0)
        {
            Vector2 startPos = node.OutputPos;
            for (int i = 0; i < node.linkNodes.Count; i++)
            {
                LogicNode linkNode = node.linkNodes[i];
                Vector2 endPos = linkNode.InputPos;
                LogicTreeUIUtility.DrawBezierBy2Points(startPos,endPos);
            }
        }



    }

    /// <summary>
    /// 在旁边绘制可以选择创建的节点，如果当前有选中的话绘制该节点的具体属性
    /// </summary>
    void DrawNodesOnTheSide()
    {
        GUILayout.BeginArea(new Rect(position.width-200,0,200,position.width));
        DrawNodesToCreate();

        if (curSelectedNode)
        {
            EditorGUILayout.Separator();
            
            curSelectedNode.DrawProperty();
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// 这里可以考虑用反射来获得所有可用的节点列表，但是控制显示顺序这样更方便 暂时不管
    /// </summary>
    void DrawNodesToCreate()
    {
        DrawNodeButton<LogicNode>("基本节点");
        DrawNodeButton<AreaTriggerNode>("区域触发器");
    }


    void DrawNodeButton<T>(string text) where T : LogicNode
    {
        if (GUILayout.Button(text))
        {
            CurLogicTree.AddChildNode<T>(new Vector2(position.width*0.5f,position.height*0.5f));
        }
    }

}
