using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

class Node
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }

    public int Dis { get; set; }

    public Node Parent { get; set; }
    public Vector3Int Postion { get; set; }

    public Node(Vector3Int position)
    {
        this.Postion = position;
        this.Dis = 0;
    }
}

public class TilesContro : MonoBehaviour {

    public static TilesContro Instance;
    private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

    private Node current;
    private HashSet<Node> openlist = new HashSet<Node>();
    private HashSet<Node> closedList = new HashSet<Node>();
    
    private Vector3Int startPos;
    private Vector3Int endPos;
    private Grid grid;

    public Tilemap []tilemaps;

    enum TileType
    {
        BGMAP,
        OBSTACLE,
        PATHFINDING,
        ATTACKAREA,
        PLAYERBORN,
    }

    public Tile roadTile;
    private List<Vector3Int> reds;

    

    private bool startPosOK = false;
    private bool endPosOK = false;
    private bool pathOK = false;
    private Stack<Vector3Int> path;
    private Vector3Int MoveAreastartpos;

    private bool isEnamy = false; // 起点的东西是敌人？

    private bool isPlayer = false; // 起点的东西是敌人？

    private HashSet<Vector3Int> moveArea = new HashSet<Vector3Int>();
    public Grid GetGrid()
    {
        return grid;
    }
    public List<Vector3Int> Astar(Vector3Int startPos,Vector3Int endPos)
    {
        if(FightManager.Instance.getEnamylist().ContainsKey(startPos))
        {
            isEnamy = true;
        }
        else
        {
            isEnamy = false;
        }


        if (FightManager.Instance.getPlayerlist().ContainsKey(startPos))
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }

        this.startPos = startPos;
        this.endPos = endPos;
        //this.moveArea = moveArea;
        Algrithm();
        List<Vector3Int> res = new List<Vector3Int>();
        foreach (var node in reds)
        {
            if(moveArea.Contains( node))
            {
                res.Add(node);
            }
        }
        return res;
    }

    public void AstarOver() // 清除路径
    {
        path = null;
        openlist.Clear();
        closedList.Clear();
        current = null;
        isEnamy = false;
        isPlayer = false;
        //moveArea.Clear();
        //foreach (var t in reds)
        //{
        //    setColor(t, Color.white,tilemap3);
        //}

        tilemaps[ (int) TileType.PATHFINDING].ClearAllTiles();
        reds.Clear();
    }


    private   void checkMoveArea(Vector3Int parentPos,ref Dictionary<Vector3Int, Node>  neighbors ,int MoveRange) // 计算可移动范围
    {
        var left = new Vector3Int(parentPos.x - 1, parentPos.y, parentPos.z);
        var right = new Vector3Int(parentPos.x + 1, parentPos.y, parentPos.z);
        var up = new Vector3Int(parentPos.x, parentPos.y + 1, parentPos.z);
        var down = new Vector3Int(parentPos.x, parentPos.y - 1, parentPos.z);

        checkMoveable(left,parentPos,MoveRange,ref neighbors);
        checkMoveable(right, parentPos, MoveRange, ref neighbors);
        checkMoveable(up, parentPos,MoveRange, ref neighbors);
        checkMoveable(down, parentPos, MoveRange, ref neighbors);

    }

    private  void checkMoveable(Vector3Int pos,Vector3Int parentPos, int MoveRange, ref Dictionary<Vector3Int, Node> neighbors)
    {
        var parent = GetNode(parentPos);
        if(tilemaps[(int)TileType.BGMAP].GetTile(pos) &&  !tilemaps[(int)TileType.OBSTACLE].GetTile(pos))
        {
            
            var node = GetNode(pos);
           

            if (parent.Dis + 1 <= MoveRange)
            {
                bool isok = true;
                if (parent.Parent != null)
                {
                    if (node.Postion == parent.Parent.Postion)
                    {
                        isok = false;
                    }
                }
                if (isok)
                {
                   

                    if (neighbors.ContainsKey(pos))
                    {
                        if (node.Dis > parent.Dis + 1)
                        {
                            node.Parent = parent;
                            node.Dis = parent.Dis + 1;
                            checkMoveArea(pos, ref neighbors, MoveRange);
                        }
                    }
                    else if(node.Postion != MoveAreastartpos)
                    {
                        node.Dis = parent.Dis + 1;
                        neighbors.Add(pos, node);
                        //neighbors[left] = node;
                        node.Parent = parent;
                        checkMoveArea(pos, ref neighbors, MoveRange);
                    }

                    
                }
            }



        }
        
    }

    public  void MoveArea(Vector3Int pos,int MoveRange) // 计算移动范围
    {
        MoveAreastartpos = pos;
        Dictionary<Vector3Int, Node> neighbors = new Dictionary<Vector3Int, Node>();
        checkMoveArea(pos, ref neighbors, MoveRange);
        foreach (var node in neighbors)
        {
            moveArea.Add(node.Key);
            setColor(node.Key, Color.red, tilemaps[(int)TileType.BGMAP]);
        }
        neighbors.Clear();
    }

    public void MoveAreaOver()
    {

        foreach (var node in moveArea)
        {
            //moveArea.Add(node);
            setColor(node, Color.white, tilemaps[(int)TileType.BGMAP]);
        }
        allNodes.Clear();
        moveArea.Clear();
    }

    public  void AttackArea(Vector3Int pos, int moveRange) // 计算攻击范围
    {
        
    }

    public void AttackAreaOver() 
    {

    }

    private Node GetNode(Vector3Int postion)
    {
        if (allNodes.ContainsKey(postion))
        {
            return allNodes[postion];
        }
        else
        {
            Node node = new Node(postion);
            allNodes.Add(postion, node);
            return node;
        }
    }
    private void init()
    {
        current = GetNode(startPos);
        //openlist = new HashSet<Node>();
       // closedList = new HashSet<Node>();
        //moveArea = new HashSet<Vector3Int>();
        openlist.Add(current);
    }
    void Awake()
    {
        Instance = this;
        grid = GetComponent<Grid>();
        reds = new List<Vector3Int>();
    }
    // Use this for initialization1

    void Start () {

       

    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetMouseButtonDown(0))
        {

           

        //   Vector3Int cellpos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //    if(startPosOK == false)
        //    {
        //        reds.Add(cellpos);
        //        startPos = cellpos;
        //        startPosOK = true;
        //        setColor(cellpos, Color.red);
        //    }
        //    else
        //    {
        //        if(endPosOK == false)
        //        {
        //            reds.Add(cellpos);
        //            endPosOK = true;
        //            endPos = cellpos;
        //            setColor(cellpos, Color.blue);
        //        }
        //        else
        //        {
        //            if (pathOK == false)
        //            {
        //                Algrithm();
        //                pathOK = true;
        //            }
        //            else
        //            {
        //                endPosOK = false;
        //                startPosOK = false;
        //                pathOK = false;
                        
        //                path = null;
        //                openlist.Clear();
        //                closedList.Clear();
        //                current = null;
        //                foreach (var t in reds)
        //                {
        //                    setColor(t, Color.white);
        //                }
        //                reds.Clear();
        //            }
        //        }
        //    }
        }

    }

    private void Algrithm()
    {
        if (current == null)
        {
            init();
            
        }
        while (openlist.Count > 0 && path == null)
        {
            List<Node> neighbors = FindNeighbors(current.Postion);
            ExaminNeighbors(neighbors, current);
            UpdateCurrentTile(ref current);
            path = GeneratePath(current);
        }
        if (path != null)
        {
            foreach (var pos in path)
            {
                if (pos != startPos /*&& pos != endPos*/)
                {
                    reds.Add(pos);
                    tilemaps[(int)TileType.PATHFINDING].SetTile(pos, roadTile);
                }
            }
        }
    }

    private Stack<Vector3Int> GeneratePath(Node current)
    {
        if(current.Postion == endPos)
        {
            Stack<Vector3Int> finalpath = new Stack<Vector3Int>();
            while(current.Postion != startPos)
            {
                finalpath.Push(current.Postion);
                current = current.Parent;
            }
            return finalpath;
        }

        return null;
    }

    private int DeterminG(Vector3Int neighbor ,Vector3Int current)
    {
        //int G = 0;
        return 10;
    }

    

    private void CalcValues(Node parent,Node neighbor,int cost)
    {
        neighbor.Parent = parent;
        neighbor.G = parent.G + cost;
        neighbor.H = (  (Math.Abs(neighbor.Postion.x - endPos.x) + Math.Abs(neighbor.Postion.y - endPos.y)) * 10);
        neighbor.F = neighbor.G + neighbor.H;
    }

    

    private void UpdateCurrentTile(ref Node current)
    {
        openlist.Remove(current);

        closedList.Add(current);

        if(openlist.Count > 0)
        {
            current = openlist.OrderBy(x => x.F).First();
        }
    }

    private void ExaminNeighbors(List<Node> list,Node current)
    {
        foreach(var node in list)
        {
            int g = DeterminG(node.Postion, current.Postion);
            if (openlist.Contains(node))
            {
                if(current.G + g < node.G )
                {
                    CalcValues(current, node, g);
                }
            }
            else if(!closedList.Contains(node))
            {
                CalcValues(current, node, g);
                openlist.Add(node);
            }
            

        }
    }
    private List<Node> FindNeighbors(Vector3Int parentPos)
    {
        List<Node> neighbors = new List<Node>();

        var left = new Vector3Int(parentPos.x - 1, parentPos.y, parentPos.z);
        var right = new Vector3Int(parentPos.x + 1, parentPos.y, parentPos.z);
        var up =  new Vector3Int(parentPos.x, parentPos.y+1, parentPos.z);
        var down = new Vector3Int(parentPos.x, parentPos.y-1, parentPos.z);

        if(checkNeightbor(left))
        {
            neighbors.Add( GetNode(left));
        }

        if (checkNeightbor(right))
        {
            neighbors.Add(GetNode(right));
        }

        if (checkNeightbor(up))
        {
            neighbors.Add(GetNode(up));
        }

        if (checkNeightbor(down))
        {
            neighbors.Add(GetNode(down));
        }


        return neighbors;
    }

    bool checkNeightbor(Vector3Int pos)
    {
        if (pos != startPos && tilemaps[(int)TileType.BGMAP].GetTile(pos) && !tilemaps[(int)TileType.OBSTACLE].GetTile(pos) /*&& moveArea.Contains(pos)*/) 
        {
           if(isEnamy)
            {
                if(FightManager.Instance.getEnamylist().ContainsKey(pos))
                {
                    return false;
                }
            }

           if(isPlayer)
            {
                if (FightManager.Instance.getPlayerlist().ContainsKey(pos))
                {
                    return false;
                }
            }
           
            return true;
        }
        else
        {
            return false;
        }
    }
    void setColor(Vector3Int cellpos, Color color, Tilemap map)
    {
        if (map != null)
        { 
            map.SetTileFlags(cellpos, TileFlags.None);
            map.SetColor(cellpos, color);
        }
        
    }

    public List<Vector3> getEnamyPositions()
    {
       

        var tileWorldLocations = new List<Vector3>();

        foreach (var pos in tilemaps[(int)TileType.PLAYERBORN].cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemaps[(int)TileType.PLAYERBORN].CellToWorld(localPlace);
            TileBase tile = tilemaps[(int)TileType.PLAYERBORN].GetTile(localPlace);
            if (tile != null && tile.name != "TilesetExample_13")
            {

                tileWorldLocations.Add(place);
            }
        }

        return tileWorldLocations;
    }

    public List<Vector3> getPlayerPositions()
    {
        var tileWorldLocations = new List<Vector3>();

        foreach (var pos in tilemaps[(int)TileType.PLAYERBORN].cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemaps[(int)TileType.PLAYERBORN].CellToWorld(localPlace);
            TileBase tile = tilemaps[(int)TileType.PLAYERBORN].GetTile(localPlace);
            if (tile!=null && tile.name == "TilesetExample_13")
            {

                tileWorldLocations.Add(place);
            }
        }

        return tileWorldLocations;
    }
}
