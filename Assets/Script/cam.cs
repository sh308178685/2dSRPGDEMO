using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Tilemaps;

public class cam : MonoBehaviour {

	

    // Update is called once per frame
    Vector3 screenPosition;//将物体从世界坐标转换为屏幕坐标
    Vector3 mousePositionOnScreen;//获取到点击屏幕的屏幕坐标
    Vector3 mousePositionInWorld;//将点击屏幕的屏幕坐标转换为世界坐标



    Camera camera;
    public Vector2 Margin, Smoothing;
    public Collider2D Bounds;
    private Vector3
        _min,
        _max;
    private Vector2 first = Vector2.zero;//鼠标第一次落下点
    private Vector2 second = Vector2.zero;//鼠标第二次位置（拖拽位置）
    private Vector3 vecPos = Vector3.zero;
    private bool IsNeedMove = false;//是否需要移动

    // Use this for initialization
    void Start()
    {
        _min = Bounds.bounds.min;//包围盒
        _max = Bounds.bounds.max;
        first.x = transform.position.x;//初始化
        first.y = transform.position.y;
        
	}

    void Update()
    {
        // MouseFollow();

        if (IsNeedMove == false)
        {
            return;
        }

        var x = transform.position.x;
        var y = transform.position.y;
        x = x - vecPos.x;//向量偏移
        y = y + vecPos.y;

        var cameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);
        //保证不会移除包围盒
        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

        

        transform.position = new Vector3(x, y, transform.position.z);


    }

    public void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            //记录鼠标按下的位置 　　
            first = Event.current.mousePosition;
        }
        if (Event.current.type == EventType.MouseDrag)
        {
            //记录鼠标拖动的位置 　　
            second = Event.current.mousePosition;
            Vector3 fir = Camera.main.ScreenToWorldPoint(new Vector3(first.x, first.y, 0));//转换至世界坐标
            Vector3 sec = Camera.main.ScreenToWorldPoint(new Vector3(second.x, second.y, 0));
            vecPos = sec - fir;//需要移动的 向量
            first = second;
            IsNeedMove = true;

        }
        else
        {
            IsNeedMove = false;
        }

    }


    void MouseFollow()
    {
        //获取鼠标在相机中（世界中）的位置，转换为屏幕坐标；
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //获取鼠标在场景中坐标
        mousePositionOnScreen = Input.mousePosition;
        //让场景中的Z=鼠标坐标的Z
        mousePositionOnScreen.z = screenPosition.z;
        //将相机中的坐标转化为世界坐标
        mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
        //物体跟随鼠标移动
        transform.position = mousePositionInWorld;
        //物体跟随鼠标X轴移动
        //transform.position = new Vector3(mousePositionInWorld.x, transform.position.y, transform.position.z);
    }
}
