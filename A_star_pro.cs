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
    //����һ��·������
    private ArrayList way = new ArrayList();
    //��ͼ�࣬���ڼ����Ƿ�Խ��
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
    //����
    public class point {
        public int x;
        public int y;
        public int z;
        public point father_point;
        public int g;//����㵽���ڵĿ���
        public int h;//�ӵ�ǰ�㵽�յ�Ŀ���
        //��������f=g+h
        public int getf() { return g + h; }
        public point(int _x, int _y, int _z, int _g, int _h, point fp) {
            x = _x;y = _y;z = _z;father_point = fp;
        }
    }
    //�ж�ĳ�����Ƿ��ڿ����б���
    private bool IsInOpenList(int x, int y,int z)

    {
        foreach (var v in open_List)

        {
            if (v.x == x && v.z == z && v.y==y)

                return true;

        }

        return false;

    }

    //�ж�ĳ���Ƿ��ڹر��б���

    private bool IsInCloseList(int x, int y,int z)

    {
        foreach (var v in close_List)

        {
            if (v.x == x && v.y==y&& v.z == z)

                return true;

        }

        return false;

    }

    //�ӿ����б����ҵ��Ǹ�Fֵ��С�ĸ���
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

    //�ӿ����б����ҵ�����
    private point FindInOpenList(int x, int y,int z)

    {
        foreach (var v in open_List)
        {
            if (v.x == x && v.z == z &&v.y==y)
                return v;
        }
        return null;
    }

    //�жϵ�ͼ��ĳ��������ǲ����ϰ���
    private bool IsBar(int x, int y,int z)//֮ǰ������һ�������Ӧ���⣬������Ѱ·ʧ�ܡ�
    {
        Vector3 p = new Vector3(x, y, z);
        //���õ��ܱ��Ƿ����ϰ���
        //�ϰ���㼶Ϊ8
        //static function OverlapSphere (position : Vector3, radius : float, layerMask : int = kAllLayers) : Collider[] 
        Collider[] colliders = Physics.OverlapSphere(p, 2, 1 << 8);
        //Debug.Log(colliders.Length);
        if (colliders.Length > 0)
            return true;//���ϰ��˵���õ㲻��ͨ�������ϰ����
        return false;

    }

    //����ĳ�����Gֵ
    //������ǿ�Խ����ƶ���ÿ�ζ�Ӧ�������۶�һ��
    public int GetG(point p)
    {
        if (p.father_point == null)
            return 0;
        if (p.x == p.father_point.x || p.z == p.father_point.z||p.y==p.father_point.y)
            return p.father_point.g + 10;//�ǶԽ����ƶ�
        else
            return p.father_point.g + 14;//�Խ����ƶ�
    }

    //����ĳ�����Hֵ
    public int GetH(point p, point targetPoint)

    {
        return (Mathf.Abs(targetPoint.x - p.x) + Mathf.Abs(targetPoint.z - p.z)+ Mathf.Abs(targetPoint.y - p.y)) * 10;

    }

    //���ĳ���ܱߵĸ���
    private void CheckPerPointWithMap(point _point, point targetPoint)

    {
        for (int i = _point.x - 1; i <= _point.x + 1; ++i)

        {
            for (int j = _point.y - 1; j <= _point.y + 1; ++j)
            {
                for (int k = _point.z - 1; k <= _point.z + 1; ++k)

                {
                    //�޳�������ͼ�ĵ�
                    if (i < mymap.start_x || i > mymap.end_x || j < mymap.start_y || j > mymap.end_y||k<mymap.start_z||k>mymap.end_z)continue;

                    //�޳��õ����ϰ��㣺����Χ��ǽ�ĵ�
                    if (IsBar(i,j,k))
                        continue;

                    //�޳��Ѿ����ڹر��б���߱����
                    if (IsInCloseList(i,j,k) || (i == _point.x && j == _point.y&& k==_point.z))
                        continue;

                    //ʣ�µľ���û���жϹ��ĵ���

                    if (IsInOpenList(i,j,k))
                    {
                        //����õ��ڿ����б���
                        //�ҵ��õ�
                        //Debug.Log(new Vector3(i,j,k));
                        point point = FindInOpenList(i,j,k);
                        int G = 0;
                        //������õ��µ��ƶ�����
                        if (point.x == _point.x || point.z == _point.z||point.y==_point.y)
                            G = point.g + 10;
                        else
                            G = point.g + 14;

                        //����ҵ��ĵ��gֵ�����������ģ��Ⱦ����ϴεĵ���Ϊ��ת����µ�����С�Ļ�������gֵ��ͬ�ǽ��ҵ��ĵ�ĸ��ڵ����
                        if (G < point.g)
                        {
                            //�����µ�G��
                            point.g = G;
                            point.father_point = _point;
                        }

                    }

                    else
                    {
                        //����õ㲻�ڿ����б�
                        //��ʼ���õ㣬�����õ���ӵ������б���
                        point newPoint = new point(i,j,k,0,0,_point);

                        //����õ��Gֵ��Hֵ����ֵ
                        newPoint.g = GetG(newPoint);
                        newPoint.h = GetH(newPoint, targetPoint);

                        //����ʼ����ϵĸ�����ӵ������б���                    
                        open_List.Add(newPoint);
                    }
                }
            }
        }
    }

    //A*�㷨��·�ߺ���
    public Vector3[] AStarFindWay(Vector3 starPoint, Vector3 targetPoint)

    {
        //�������        
        way.Clear();

        open_List.Clear();

        close_List.Clear();

        //��ʼ��������
        point starMapPoint = new point(0,0,0,0,0,null);

        starMapPoint.x = (int)starPoint.x;
        starMapPoint.y = (int)starPoint.y;
        starMapPoint.z = (int)starPoint.z;

        //��ʼ���յ����
        point targetMapPoint = new point(0, 0, 0, 0, 0, null);
        targetMapPoint.x = (int)targetPoint.x;
        targetMapPoint.y = (int)targetPoint.y;
        targetMapPoint.z = (int)targetPoint.z;

        //����������ӵ������б���        
        open_List.Add(starMapPoint);

        //Ѱ�����·��
        //��Ŀ��㲻�ڴ�·����ʱ���ߴ��б�Ϊ��ʱѭ��ִ��
        while (!IsInOpenList(targetMapPoint.x, targetMapPoint.y, targetMapPoint.z) || open_List.Count == 0)

        {
            //�ӿ����б����ҵ��Ǹ�Fֵ��С�ĸ���
           point minPoint = FindMinFInOpenList();

            if (minPoint == null)
                return null;
            //���õ�ӿ����б���ɾ����ͬʱ��ӵ��ر��б���  
            open_List.Remove(minPoint);
            close_List.Add(minPoint);

            //���ĵ��ܱߵĸ���  
            CheckPerPointWithMap(minPoint,targetMapPoint);    
        }
        //�ڿ����б����ҵ��յ�
        point endPoint = FindInOpenList(targetMapPoint.x, targetMapPoint.y, targetMapPoint.z);
        Vector3 everyWay = new Vector3(endPoint.x, endPoint.y,endPoint.z);//���浥��·����

        way.Add(everyWay);//��ӵ�·��������

        //�����յ㣬�ҵ�ÿһ�����ڵ㣺��Ѱ����·
        while (endPoint.father_point!= null)
        {
            everyWay.x = endPoint.father_point.x;
            everyWay.y = endPoint.father_point.y;
            everyWay.z = endPoint.father_point.z;
            way.Add(everyWay);
            endPoint = endPoint.father_point;

        }
        //��·������ӵ��������򲢷���
        Vector3[] ways = new Vector3[way.Count];

        for (int i = way.Count - 1; i >= 0; --i)
        {
            ways[way.Count - i - 1] = (Vector3)way[i];  
        }
        //�������        
        way.Clear();
        open_List.Clear();
        close_List.Clear();
        //���������·������
        return ways;
    }
}
