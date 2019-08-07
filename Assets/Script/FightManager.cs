using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    private Dictionary<Vector3Int, Enamy> enamylist;
    private Dictionary<Vector3Int, PlayerControl> playerlist;
    public Dictionary<Vector3Int, PlayerControl> getPlayerlist()
    {
        return playerlist;
    }

    public Dictionary<Vector3Int, Enamy> getEnamylist()
    {
        return enamylist;
    }
    int CurrentTurn = 0;

    int NowActionOrder = 0;

    public int MyCurrentTurn
    {
        set
        {
            CurrentTurn = value;
        }

        get
        {
            return CurrentTurn;
        }
    }

    public int MyNowActionOrder
    {
        set
        {
            NowActionOrder = value;
        }

        get
        {
            return NowActionOrder;
        }
    }

    private void Awake()
    {
        Instance = this;
        enamylist = new Dictionary<Vector3Int, Enamy>();
        playerlist = new Dictionary<Vector3Int, PlayerControl>();
    }
// Start is called before the first frame update
    void Start()
    {

        
        var grid = TilesContro.Instance.GetGrid();
        
        var list = TilesContro.Instance.getEnamyPositions();
        var list2 = TilesContro.Instance.getPlayerPositions();
        int count = 0;
        foreach(var pos in list)
        {
            GameObject enamy = (GameObject)Instantiate(Resources.Load("Prefab/Enamy"));
            enamy.GetComponent<Enamy>().pos = grid.WorldToCell(pos);

            enamy.GetComponent<Enamy>().actionOrder = count;
            enamylist.Add(grid.WorldToCell(pos), enamy.GetComponent<Enamy>());
            count++;
        }

        foreach (var pos in list2)
        {
            GameObject player = (GameObject)Instantiate(Resources.Load("Prefab/Player"));
            player.GetComponent<PlayerControl>().pos = grid.WorldToCell(pos);
            playerlist.Add(grid.WorldToCell(pos), player.GetComponent<PlayerControl>());
            
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerPos(PlayerControl role,Vector3Int posold, Vector3Int pos)
    {
        playerlist.Remove(posold);
        playerlist.Add(pos, role);
    }

    public void  UpdateEnamyPos(Enamy role, Vector3Int posold, Vector3Int pos)
    {
        enamylist.Remove(posold);
        enamylist.Add(pos, role);
    }

    public void CheckFightTurn()
    {
        bool playerFinish = true;
        bool EnamyFinish = true;

        bool PlayerWin = true;
        bool EnamyWin = true;
        
        foreach (var node in playerlist)
        {
            if (node.Value.GetComponent<PlayerControl>().isFinish == false)
            {
                playerFinish = false;
            }
            if (node.Value.GetComponent<PlayerControl>().MyHP > 0)
            {
                EnamyWin = false;
            }
            node.Value.GetComponent<SpriteRenderer>().sortingOrder = UnityEngine.Screen.height - node.Key.y;

            node.Value.gameObject.transform.Find("Canvas").GetComponent<Canvas>().sortingOrder = UnityEngine.Screen.height - node.Key.y + 100;
        }
        foreach (var node in enamylist)
        {
            if (node.Value.GetComponent<Enamy>().isFinish == false  )
            {
                EnamyFinish = false;
            }
            else if(node.Value.GetComponent<Enamy>().isFinish == true)
            {
                //if (node.Value.GetComponent<Enamy>().actionOrder == MyNowActionOrder)
                //{
                //    MyNowActionOrder = node.Value.GetComponent<Enamy>().actionOrder + 1;
                //}
            }

            if (node.Value.GetComponent<Enamy>().MyHP > 0)
            {
                PlayerWin = false;
            }

            node.Value.GetComponent<SpriteRenderer>().sortingOrder = UnityEngine.Screen.height - node.Key.y;
            node.Value.gameObject.transform.Find("Canvas").GetComponent<Canvas>().sortingOrder = UnityEngine.Screen.height - node.Key.y + 100;
        }


        if(PlayerWin)
        {
            Debug.Log("胜利！！！");
        }

        if (EnamyWin)
        {
            Debug.Log("失败！！！");
        }




        if (playerFinish && CurrentTurn == 0)
        {
            Debug.Log("敌人回合");
            CurrentTurn = 1;
            //MyNowActionOrder = 0;
            foreach (var node in enamylist)
            {
                node.Value.GetComponent<Enamy>().isFinish = false;
            }
            EnamyFinish = false;
        }
        if(EnamyFinish && CurrentTurn == 1)
        {

            Debug.Log("玩家回合");
            foreach (var node in playerlist)
            {
                node.Value.GetComponent<PlayerControl>().isFinish = false;

            }
            playerFinish = false;
            CurrentTurn = 0;
        }


        foreach (var node in enamylist)
        {
            if (node.Value.GetComponent<Enamy>().isFinish == false)
            { 
                MyNowActionOrder = node.Value.GetComponent<Enamy>().actionOrder;
                Debug.Log("set actionorder = " + MyNowActionOrder);
            }
            
        }
    }

    

    internal bool hasEnamy(String tag, Vector3Int vector3Int)
    {
        if (tag == "Player")
        {
            return enamylist.ContainsKey(vector3Int);
        }
        else 
        {
            return playerlist.ContainsKey(vector3Int);
        }
    }

    internal void PerformAttack(Vector3Int attacker,Vector3Int Victim)
    {
        if(playerlist.ContainsKey(attacker))
        {
            Random reum = new Random();
            int randomdata = reum.Next(playerlist[attacker].MyATK); //产生1-AKT的随机数

            if(enamylist.ContainsKey(Victim))
            {
                enamylist[Victim].MyHP -= randomdata;
            }
        }
        else if(enamylist.ContainsKey(attacker))
        {
            Random reum = new Random();
            int randomdata = reum.Next(enamylist[attacker].MyATK); //产生1-AKT的随机数

            if (playerlist.ContainsKey(Victim))
            {
                playerlist[Victim].MyHP -= randomdata;
            }
        }
       
    }
}
