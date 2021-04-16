using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGrid : MonoBehaviour
{
    //public GameObject NodeWall;


    // 过滤地图所在的层
    public LayerMask WallLayer;

    // 网格边长
    public float NodeWidth;
    // 寻路节点
    public class NodeItem
    {
        // 是否是障碍物
        public bool isWall;
        // 位置
        public Vector3 pos;
        // 节点下标
        public int x, z;

        // 与起点的长度
        public float gCost;
        // 与目标点的长度
        public float hCost;

        // 总的路径长度
        public float fCost
        {
            get { return gCost + hCost; }
        }

        // 父节点
        public NodeItem parent;

        public NodeItem(bool isWall, Vector3 pos, int x, int z)
        {
            this.isWall = isWall;
            this.pos = pos;
            this.x = x;
            this.z = z;
        }
    }
    // 二维数组,保存整个地图结点信息
    private NodeItem[,] mapGrid;
    // 节点数量
    private int w, h;

    //private GameObject WallRange;

    private Vector3 PlaneOriginSize;
    private Vector3 MapSize;

    // Debug
    private int walkNum;
    void Awake()
    {
        //LayerMask mask = 1 << 8;
        NodeWidth = 1.0f;
        PlaneOriginSize = new Vector3(10, 0, 10);
        MapSize.x = PlaneOriginSize.x * transform.localScale.x;
        MapSize.y = PlaneOriginSize.y * transform.localScale.y;
        MapSize.z = PlaneOriginSize.z * transform.localScale.z;
        // 初始化格子,根据节点半径分割成网格
        // RoundToInt 四舍五入为整数，.5结尾返回偶数
        // 计算网格总大小
        w = Mathf.RoundToInt(MapSize.x / NodeWidth);
        h = Mathf.RoundToInt(MapSize.z / NodeWidth);

        //*********** Debug ***********//
        Debug.Log("MapSize:" + MapSize);
        Debug.Log("w: " + w);
        Debug.Log("h: " + h);
        walkNum = 0;
        //GameObject WallRange = new GameObject("WallRange");
        //***************************//

        mapGrid = new NodeItem[w, h];

        //WalkRange = new GameObject("WalkRange");


        // 将墙的信息写入格子中
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                // 世界坐标系
                Vector3 pos = new Vector3((i - w / 2) * NodeWidth, 0.2f, (j - h / 2) * NodeWidth);
                // 碰撞盒检测障碍物，只检测Walllayer层
                bool isWall = Physics.CheckBox(pos, new Vector3(NodeWidth / 2, NodeWidth / 2, NodeWidth / 2), Quaternion.identity, WallLayer);
                // 构建一个节点
                mapGrid[i, j] = new NodeItem(isWall, pos, i, j);

                //************* Debug  **********//
                // 如果是墙体，则画出不可行走的区域
                /*if (isWall)
                {
                    GameObject obj = GameObject.Instantiate(, pos, Quaternion.identity) as GameObject;
                    //obj.transform.SetParent(WallRange.transform);
                }*/
                if (!isWall)
                {
                    /*GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //Debug.Log(pos);
                    obj.transform.position = pos;
                    obj.transform.localScale = new Vector3(NodeWidth, 0.1f, NodeWidth);
                    obj.GetComponent<MeshRenderer>().material.color = Color.red;*/
                    //obj.transform.SetParent(WallRange.transform);
                    ++walkNum;
                }
                //*****************************//
            }
        }

        //************  Debug  *************//
        /*GameObject obj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj1.transform.SetParent(transform);
        obj1.transform.localPosition = new Vector3(4.8f, 0.5f, 4.8f);
        obj1.transform.localScale = new Vector3(0.25f, 1f, 0.25f);
        obj1.GetComponent<MeshRenderer>().material.color = Color.red;*/
        Debug.Log(walkNum);
        //***************************//

    }

    // 根据坐标获得一个节点
    public NodeItem GetItem(Vector3 position)
    {
        int i = Mathf.RoundToInt(position.x / NodeWidth + w / 2);
        int j = Mathf.RoundToInt(position.z / NodeWidth + h / 2);
        i = Mathf.Clamp(i, 0, w - 1);
        j = Mathf.Clamp(j, 0, h - 1);
        return mapGrid[i, j];
    }

    // 取得周围的节点
    public List<NodeItem> GetNeibourhood(NodeItem node)
    {
        List<NodeItem> list = new List<NodeItem>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // 如果是自己，则跳过
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int x = node.x + i;
                int z = node.z + j;
                // 判断是否越界，如果没有，加到列表中
                if (x < w && x >= 0 && z < h && z >= 0)
                {
                    list.Add(mapGrid[x, z]);
                }
            }
        }
        return list;
    }
}
