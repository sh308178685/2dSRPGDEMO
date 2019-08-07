using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enamy : Role
{

    float closeDis = 0;
    Vector3Int closePos;
    public int actionOrder = 0;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(FightManager.Instance.MyCurrentTurn == 1 && enamyIsMoving == false && actionOrder == FightManager.Instance.MyNowActionOrder )
        {
            
            //查找最近的玩家
            if (canWalk == false)
            {
                var playerlist = FightManager.Instance.getPlayerlist();
                
                foreach (var player in playerlist)
                {
                    if (closeDis == 0)
                    {
                        closeDis = Mathf.Abs(start.x - player.Key.x) + Mathf.Abs(start.y - player.Key.y);
                        closePos = player.Key;
                    }
                    else
                    {
                        if (Mathf.Abs(start.x - player.Key.x) + Mathf.Abs(start.y - player.Key.y) < closeDis)
                        {
                            closeDis = Mathf.Abs(start.x - player.Key.x) + Mathf.Abs(start.y - player.Key.y);
                            closePos = player.Key;
                        }
                    }
                }
                TilesContro.Instance.MoveArea(Grid.WorldToCell(start), MoveRange);
                canWalk = true;
            }
            else
            {
                
                Debug.Log("现在是" + actionOrder + "号选手行走");
                enamyIsMoving = true;
                RoleAction(Grid.CellToWorld(closePos));
                
            }

        }
    }
}
