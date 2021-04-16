using UnityEngine;
using System.Collections.Generic;

public class FindPath : MonoBehaviour
{
    public MapGrid mapGrid;
    // 起点
    public Transform startPos;
    // 目标
    public Transform destPos;

    public bool isGenerate = false;

    public List<MapGrid.NodeItem> Path;

    private GameObject PathRange;
    private List<GameObject> pathObj;

    // Use this for initialization
    void Start()
    {
        //mapGrid = GetComponent<MapGrid>();
        Path = new List<MapGrid.NodeItem>();
        PathRange = new GameObject("PathRange");
        pathObj = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        FindingPath(startPos.position, destPos.position);
    }

    // A*寻路
    void FindingPath(Vector3 start, Vector3 end)
    {
        MapGrid.NodeItem startNode = mapGrid.GetItem(start);
        MapGrid.NodeItem endNode = mapGrid.GetItem(end);

        List<MapGrid.NodeItem> openSet = new List<MapGrid.NodeItem>();
        HashSet<MapGrid.NodeItem> closeSet = new HashSet<MapGrid.NodeItem>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // 找到open中代价最小的节点n
            MapGrid.NodeItem curNode = openSet[0];
            for (int i = 0, max = openSet.Count; i < max; i++)
            {
                /*if (openSet[i].fCost <= curNode.fCost &&
                    openSet[i].hCost < curNode.hCost)
                {
                    curNode = openSet[i];
                }*/
                if (openSet[i].fCost < curNode.fCost)
                {
                    curNode = openSet[i];
                }
            }

            // 将代价最小的节点从open移到close
            openSet.Remove(curNode);
            closeSet.Add(curNode);

            // n是终点，算法结束，生成路径
            if (curNode == endNode)
            {
                GeneratePath(startNode, endNode);
                return;
            }

            // n不是终点
            // 判断周围节点
            foreach (var item in mapGrid.GetNeibourhood(curNode))
            {
                // 如果是墙或者已经在close中，跳过
                if (item.isWall || closeSet.Contains(item))
                {
                    continue;
                }
                // 计算当前相邻节点的新起点代价
                float new_gCost = curNode.gCost + GetDistanceNodes(curNode, item);
                // 如果距离更小，或者原来不在开始列表中
                if (new_gCost < item.gCost || !openSet.Contains(item))
                {
                    // 更新与开始节点的距离
                    item.gCost = new_gCost;
                    // 更新与终点的距离
                    item.hCost = GetDistanceNodes(item, endNode);
                    // 更新父节点为当前选定的节点
                    item.parent = curNode;
                    // 如果节点是新加入的，将它加入打开列表中
                    if (!openSet.Contains(item))
                    {
                        openSet.Add(item);
                    }
                }
            }
        }

        // 没找到路径
        GeneratePath(startNode, null);
    }

    // 生成路径
    private void GeneratePath(MapGrid.NodeItem startNode, MapGrid.NodeItem endNode)
    {
        Path.Clear();
        if (endNode != null)
        {
            MapGrid.NodeItem temp = endNode;
            while (temp != startNode)
            {
                Path.Add(temp);
                temp = temp.parent;
            }
            // 反转路径
            Path.Reverse();
        }
        // 画出路径
        if (isGenerate)
        {
            UpdatePath(Path);
        }
        
    }

    // 获取两个节点之间的代价(对角距离)
    private float GetDistanceNodes(MapGrid.NodeItem a, MapGrid.NodeItem b)
    {
        float cntX = Mathf.Abs(a.x - b.x);
        float cntZ = Mathf.Abs(a.z - b.z);
        
        if (cntX > cntZ)
        {
            return 1.4f * cntZ + (cntX - cntZ);
        }
        else
        {
            return 1.4f * cntX + (cntZ - cntX);
        }
    }

    // Debug
    // 显示路径
    private void UpdatePath(List<MapGrid.NodeItem> Path)
    {
        int curListSize = pathObj.Count;
        //Debug.Log("path.Count" + path.Count);
        for (int i = 0, max = Path.Count; i < max; i++)
        {
            if (i < curListSize)
            {
                pathObj[i].transform.position = Path[i].pos;
                pathObj[i].SetActive(true);
            }
            else
            {
                /*GameObject obj = GameObject.Instantiate(Node, path[i].pos, Quaternion.identity) as GameObject;
                obj.transform.localScale = new Vector3(NodeWidth, 0.1f, NodeWidth);
                obj.transform.SetParent(PathRange.transform);*/
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(obj.GetComponent<BoxCollider>());
                obj.transform.position = Path[i].pos;
                obj.transform.localScale = new Vector3(mapGrid.NodeWidth / 3, 0.1f, mapGrid.NodeWidth / 3);
                obj.GetComponent<MeshRenderer>().material.color = Color.red;
                obj.transform.SetParent(PathRange.transform);
                pathObj.Add(obj);
            }
        }
        for (int i = Path.Count; i < curListSize; i++)
        {
            pathObj[i].SetActive(false);
        }
    }

}