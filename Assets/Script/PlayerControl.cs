using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Role
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
       
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0) && FightManager.Instance.MyCurrentTurn == 0)
        { //检测鼠标左键是否点
            
            RoleAction(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    

    internal void onMove()
    {
        //throw new NotImplementedException();
        
        Debug.Log("移动");
        TilesContro.Instance.MoveArea( Grid.WorldToCell(start),MoveRange);
        canWalk = true;
    }

    internal void onAttack()
    {
        //throw new NotImplementedException();
        Debug.Log("攻击");
    }

    public override void onTouched()
    {
        // 显示攻击 移动按钮
        //Debug.Log("children touched");
       Transform canvas =  transform.Find("Canvas");
        Transform bt1 = canvas.Find("Button1");
        Transform bt2 = canvas.Find("Button2");
        if (canvas)
        {
            if(canvas.gameObject.active)
            {
                canvas.gameObject.active = false;
            }
            else
            {
                canvas.gameObject.active = true;
                if(FightManager.Instance.MyCurrentTurn == 0 && isFinish == false)
                {
                    bt1.gameObject.SetActive(true);
                    bt2.gameObject.SetActive(true);
                }
                else
                {
                    bt1.gameObject.SetActive(false);
                    bt2.gameObject.SetActive(false);
                }
            }
        }
    }

}
