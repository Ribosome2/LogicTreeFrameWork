using UnityEngine;
using System.Collections;

public class LogicTreeUIUtility : MonoBehaviour
{

    private static Texture2D mDotIcon;//圆点Icon，用来代表输入输出节点

    public static Texture2D DotIcon
    {
        get
        {
            if (mDotIcon == null)
            {
                //todo 用自定义的icon
            }
            return LogicTreeUIUtility.mDotIcon;
        }
        set { LogicTreeUIUtility.mDotIcon = value; }
    }

    public static void DrawBezierBy2Points(Vector2 startPoint, Vector2 endPoint)
    {
        Vector3 startTangent = new Vector2(startPoint.x, (startPoint.y + endPoint.y) * 0.5f);
        Vector3 endTangent = new Vector2(endPoint.x, (startPoint.y + endPoint.y) * 0.5f);
        
        Color color = Color.grey;
        UnityEditor.Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, color, null, 2);

    }
	
}
