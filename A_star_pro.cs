using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_star_pro : MonoBehaviour
{
    public Transform begin;
    public Transform end;
    public Vector3[] result;
    Vector3 startpoint, endpoint;
    private void Start()
    {
        //startpoint = begin.position;
        //endpoint = end.position;
        //result=AStarFindWay(startpoint, endpoint);



    }
    private map mymap = new map();
    private List<point> open_List = new List<point>();
    private List<point> close_List = new List<point>();
    //定义一个路径数组
    private ArrayList way = new ArrayList();
    //地图类，用于检查点是否越界
    public class map {
        public int start_x;
        public int start_y;
        public int start_z;
        public int end_x;
        public int end_y;
        public int end_z;
        public map(int a=100,int b=100,int c=100) {
            start_x = -a;
            start_y = -b;
            start_z = -c;
            end_x = a;
            end_y = b;
            end_z = c;
        }
    }
    //点类
    public class point {
        public int x;
        public int y;
        public int z;
        public point father_point;
        public int g;//从起点到现在的开销
        public int h;//从当前点到终点的开销
        //启发函数f=g+h
        public int getf() { return g + h; }
        public point(int _x, int _y, int _z, int _g, int _h, point fp) {
            x = _x;y = _y;z = _z;father_point = fp;
        }
    }
    //判断某个点是否在开启列表当中
    private bool IsInOpenList(int x, int y,int z)

    {
        foreach (var v in open_List)

        {
            if (v.x == x && v.z == z && v.y==y)

                return true;

        }

        return false;

    }

    //判断某点是否在关闭列表中

    private bool IsInCloseList(int x, int y,int z)

    {
        foreach (var v in close_List)

        {
            if (v.x == x && v.y==y&& v.z == z)

                return true;

        }

        return false;

    }

    //从开启列表中找到那个F值最小的格子
    private point FindMinFInOpenList()

    {
        point minPoint = null;
        foreach (var v in open_List)

        {
            if (minPoint == null || minPoint.getf() > v.getf())

                minPoint = v;

        }

        return minPoint;

    }

    //从开启列表中找到格子
    private point FindInOpenList(int x, int y,int z)

    {
        foreach (var v in open_List)
        {
            if (v.x == x && v.z == z &&v.y==y)
                return v;
        }
        return null;
    }

    //判断地图上某个坐标点是不是障碍点
    private bool IsBar(int x, int y,int z)//之前遇到了一个坐标对应问题，导致了寻路失败。
    {
        Vector3 p = new Vector3(x, y, z);
        //检测该点周边是否有障碍物
        //障碍物层级为8
        //static function OverlapSphere (position : Vector3, radius : float, layerMask : int = kAllLayers) : Collider[] 
        Collider[] colliders = Physics.OverlapSphere(p, 2, 1 << 8);
        //Debug.Log(colliders.Length);
        if (colliders.Length > 0)
            return true;//有障碍物，说明该点不可通过，是障碍物点
        return false;

    }

    //计算某方块的G值
    //如果不是跨对角线移动则每次对应付出代价多一点
    public int GetG(point p)
    {
        if (p.father_point == null)
            return 0;
        if (p.x == p.father_point.x || p.z == p.father_point.z||p.y==p.father_point.y)
            return p.father_point.g + 10;//非对角线移动
        else
            return p.father_point.g + 14;//对角线移动
    }

    //计算某方块的H值
    public int GetH(point p, point targetPoint)

    {
        return (Mathf.Abs(targetPoint.x - p.x) + Mathf.Abs(targetPoint.z - p.z)+ Mathf.Abs(targetPoint.y - p.y)) * 10;

    }

    //检查某点周边的格子
    private void CheckPerPointWithMap(point _point, point targetPoint)

    {
        for (int i = _point.x - 1; i <= _point.x + 1; ++i)

        {
            for (int j = _point.y - 1; j <= _point.y + 1; ++j)
            {
                for (int k = _point.z - 1; k <= _point.z + 1; ++k)

                {
                    //剔除超过地图的点
                    if (i < mymap.start_x || i > mymap.end_x || j < mymap.start_y || j > mymap.end_y||k<mymap.start_z||k>mymap.end_z)continue;

                    //剔除该点是障碍点：即周围有墙的点
                    if (IsBar(i,j,k))
                        continue;

                    //剔除已经存在关闭列表或者本身点
                    if (IsInCloseList(i,j,k) || (i == _point.x && j == _point.y&& k==_point.z))
                        continue;

                    //剩下的就是没有判断过的点了

                    if (IsInOpenList(i,j,k))
                    {
                        //如果该点在开启列表中
                        //找到该点
                        //Debug.Log(new Vector3(i,j,k));
                        point point = FindInOpenList(i,j,k);
                        int G = 0;
                        //计算出该点新的移动代价
                        if (point.x == _point.x || point.z == _point.z||point.y==_point.y)
                            G = point.g + 10;
                        else
                            G = point.g + 14;

                        //如果找到的点的g值（到起点的消耗）比经过上次的点作为中转后的新的消耗小的话，更新g值，同是将找到的点的父节点更新
                        if (G < point.g)
                        {
                            //更新新的G点
                            point.g = G;
                            point.father_point = _point;
                        }

                    }

                    else
                    {
                        //如果该点不在开启列表
                        //初始化该点，并将该点添加到开启列表中
                        point newPoint = new point(i,j,k,0,0,_point);

                        //计算该点的G值和H值并赋值
                        newPoint.g = GetG(newPoint);
                        newPoint.h = GetH(newPoint, targetPoint);

                        //将初始化完毕的格子添加到开启列表中                    
                        open_List.Add(newPoint);
                    }
                }
            }
        }
    }

    //A*算法找路线函数
    public Vector3[] AStarFindWay(Vector3 starPoint, Vector3 targetPoint)

    {
        //清空容器        
        way.Clear();

        open_List.Clear();

        close_List.Clear();

        //初始化起点格子
        point starMapPoint = new point(0,0,0,0,0,null);

        starMapPoint.x = (int)starPoint.x;
        starMapPoint.y = (int)starPoint.y;
        starMapPoint.z = (int)starPoint.z;

        //初始化终点格子
        point targetMapPoint = new point(0, 0, 0, 0, 0, null);
        targetMapPoint.x = (int)targetPoint.x;
        targetMapPoint.y = (int)targetPoint.y;
        targetMapPoint.z = (int)targetPoint.z;

        //将起点格子添加到开启列表中        
        open_List.Add(starMapPoint);

        //寻找最佳路径
        //当目标点不在打开路径中时或者打开列表为空时循环执行
        while (!IsInOpenList(targetMapPoint.x, targetMapPoint.y, targetMapPoint.z) || open_List.Count == 0)

        {
            //从开启列表中找到那个F值最小的格子
           point minPoint = FindMinFInOpenList();

            if (minPoint == null)
                return null;
            //将该点从开启列表中删除，同时添加到关闭列表中  
            open_List.Remove(minPoint);
            close_List.Add(minPoint);

            //检查改点周边的格子  
            CheckPerPointWithMap(minPoint,targetMapPoint);    
        }
        //在开启列表中找到终点
        point endPoint = FindInOpenList(targetMapPoint.x, targetMapPoint.y, targetMapPoint.z);
        Vector3 everyWay = new Vector3(endPoint.x, endPoint.y,endPoint.z);//保存单个路径点

        way.Add(everyWay);//添加到路径数组中

        //遍历终点，找到每一个父节点：即寻到的路
        while (endPoint.father_point!= null)
        {
            everyWay.x = endPoint.father_point.x;
            everyWay.y = endPoint.father_point.y;
            everyWay.z = endPoint.father_point.z;
            way.Add(everyWay);
            endPoint = endPoint.father_point;

        }
        //将路径数组从倒序变成正序并返回
        Vector3[] ways = new Vector3[way.Count];

        for (int i = way.Count - 1; i >= 0; --i)
        {
            ways[way.Count - i - 1] = (Vector3)way[i];  
        }
        //清空容器        
        way.Clear();
        open_List.Clear();
        close_List.Clear();
        //返回正序的路径数组
        return ways;
    }
}
