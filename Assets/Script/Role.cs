using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Role : MonoBehaviour
{
    protected Grid Grid;
    public int AttackRange;
    public int MoveRange;
    public Vector3Int pos = new Vector3Int(-9, -1, 0);
    protected Vector3 start;
    protected Vector3 end;
    protected bool canWalk = false;
    protected bool enamyIsMoving = false;
    protected Animator animator;
    [SerializeField]
    protected LayerMask layermask;
    [SerializeField]
    protected Image hpbar;

    [SerializeField]
    protected int MAXHP;

    [SerializeField]
    private int HP;
    [SerializeField]
    private int ATK;
    public int MyHP
    {
        get
        {
            return HP;
        }
        set
        {
            HP = value;
            hpbar.fillAmount = ((float)HP / (float)MAXHP);
        }
    }

    public int MyATK
    {
        get
        {
            return ATK;
        }
        set
        {
            ATK = value;
        }
    }

    public bool isFinish = false;
    // Start is called before the first frame update
   public virtual void  Start()
    {
        MyHP = MAXHP;
        animator = GetComponent<Animator>();
        Grid = TilesContro.Instance.GetGrid();
        start = Grid.CellToWorld(pos);
        transform.position = start;
        end = Grid.CellToWorld(new Vector3Int(pos.x + 1, pos.y, pos.z));
    }

    // Update is called once per frame
    public virtual void  Update()
    {
        if(HP <=0) //播放死亡
        {
            animator.SetInteger("state", 2);
        }
    }
    public virtual void onTouched()
    {
        Debug.Log("onTouch parent");
    }

    void AttackOver()
    {
        Debug.Log("AttackOver");
        animator.SetInteger("state", 0);
    }

    void Dead()
    {
        Debug.Log("Dead");
        if (tag == "Enamy")
        {
            FightManager.Instance.getEnamylist().Remove(Grid.WorldToCell( start));
        }
        else if(tag == "Player")
        {
            FightManager.Instance.getPlayerlist().Remove(Grid.WorldToCell(start));
        }
        Destroy(gameObject);
    }

    protected void RoleAction( Vector3 target)
    {
        if (canWalk)
        {
            canWalk = false;
            transform.DOKill();
            transform.position = start;
            TilesContro.Instance.AstarOver();
            Vector3Int cellpos = Grid.WorldToCell(target);
            List<Vector3Int> res = TilesContro.Instance.Astar(Grid.WorldToCell(start), cellpos);
            
            Transform canvas = transform.Find("Canvas");
            Transform bt1 = canvas.Find("Button1");
            Transform bt2 = canvas.Find("Button2");
            if (bt1 && bt2)
            {
                bt1.gameObject.SetActive(false);
                bt2.gameObject.SetActive(false);
            }
            if (res.Count > 0)
            {
                int arraylength = 0;
                int count = 0;
                
                //检测目标处是否有敌人
                if (FightManager.Instance.hasEnamy(tag,  res[res.Count - 1]))
                {
                    arraylength = res.Count - 1;
                }
                else
                {
                    arraylength = res.Count;
                }
                if (arraylength > 0)
                {
                    Vector3[] points = new Vector3[arraylength];

                    foreach (var point in res)
                    {
                        if (count == arraylength)
                        {
                            break;
                        }
                        points[count] = Grid.CellToWorld(point);
                        count++;
                    }
                    //bool isRevert = false;
                    //if(res[0].x - res[res.Count-1].x > 0 )
                    //{
                    //    var hehe = transform.localScale.x;
                    //    transform.localScale = new Vector3(-hehe, transform.localScale.y, transform.localScale.z);
                    //    isRevert = true;
                    //}

                    animator.SetInteger("state", 3);
                    transform.DOPath(points, 0.2f * res.Count, PathType.CatmullRom).OnComplete(() =>
                    {

                        //检测目标处是否有敌人
                        if (FightManager.Instance.hasEnamy(tag,res[res.Count - 1]))
                        {

                            if (tag == "Player")
                            {
                                FightManager.Instance.UpdatePlayerPos(this as PlayerControl, Grid.WorldToCell(start), res[res.Count - 2]);
                            }
                            else if(tag == "Enamy")
                            {
                                FightManager.Instance.UpdateEnamyPos(this as Enamy, Grid.WorldToCell(start), res[res.Count - 2]);
                            }

                            FightManager.Instance.PerformAttack(res[res.Count - 2], res[res.Count - 1]);
                            start = Grid.CellToWorld(res[res.Count - 2]);
                            animator.SetInteger("state", 1);
                        }
                        else
                        {

                            if (tag == "Player")
                            {
                                FightManager.Instance.UpdatePlayerPos(this as PlayerControl, Grid.WorldToCell(start), res[res.Count - 1]);
                            }
                            else if (tag == "Enamy")
                            {
                                FightManager.Instance.UpdateEnamyPos(this as Enamy, Grid.WorldToCell(start), res[res.Count - 1]);
                            }

                            
                            start = Grid.CellToWorld(res[res.Count - 1]);
                            animator.SetInteger("state", 0);
                            TilesContro.Instance.AttackArea(Grid.WorldToCell(start), AttackRange);
                        }

                        
                        TilesContro.Instance.AstarOver();
                        TilesContro.Instance.MoveAreaOver();
                       
                        enamyIsMoving = false;
                        Debug.Log("77777777777777");
                        isFinish = true;
                        FightManager.Instance.CheckFightTurn();

                    });
                }
                else
                {
                    FightManager.Instance.PerformAttack(Grid.WorldToCell(start), res[res.Count - 1]);
                    animator.SetInteger("state", 1);
                    Debug.Log("666666666666666");
                    enamyIsMoving = false;
                    isFinish = true;
                    FightManager.Instance.CheckFightTurn();
                }
                
                
            }

        }
    }
}
