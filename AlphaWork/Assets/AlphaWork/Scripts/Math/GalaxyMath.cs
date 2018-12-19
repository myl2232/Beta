using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class Vec2
    {
        public Vec2(){}
        public Vec2(float fx, float fy)
        {
            x = fx;
            y = fy;
        }
         public Vec2(Vec2 rh)
        {
            x = rh.x;
            y = rh.y;
        }
        public float x { get; set; }
        public float y { get; set; }
       
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            Vec2 res = new Vec2();
            res.x = a.x - b.x;
            res.y = a.y - b.y;
            return res;
        }
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            Vec2 res = new Vec2();
            res.x = a.x + b.x;
            res.y = a.y + b.y;
            return res;
        }
        public static bool operator ==(Vec2 a, Vec2 b)
        {
            if(a.x != b.x)
                return false;
            if(a.y != b.y)
                return false;
            return true;
        }
         public static bool operator !=(Vec2 a, Vec2 b)
        {
            if(a.x == b.x && a.y == b.y)
                return false;
            return true;
        }

    }
    public class Vec3
    {
        public Vec3()
        {
            x = y = z = 0.0f;
        }
        public Vec3(float fx,float fy,float fz)
        {
            x = fx;
            y = fy;
            z = fz;
        }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public static Vec3 Zero = new Vec3(0, 0, 0);
    }
    public class Vec4
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }
    }
    public interface IGalaxyMath
    {
        void Normal(Vec2 a);
        void Normal(Vec3 a);
        Vec2 Add(Vec2 a, Vec2 b);
        Vec2 Sub(Vec2 a, Vec2 b);
        float Dot(Vec2 a, Vec2 b);
        Vec2 Cross(Vec2 a, Vec2 b);
        Vec3 Add(Vec3 a, Vec3 b);
        Vec3 Sub(Vec3 a, Vec3 b);
        float Dot(Vec3 a, Vec3 b);
        Vec3 Cross(Vec3 a, Vec3 b);
    }

    public class Point2SegmentDis
    {
        /*
        public static float GetDis(Vector3 point, Vector3 p1, Vector3 p2)
        {
            float x = p2.x - p1.x;
            float y = p2.z - p1.z;
            float z = p2.y - p1.y;

            float dx = point.x - p1.x;
            float dy = point.z - p1.z;
            float dz = point.y - p1.y;

            float d = x * x + y * y + z * z;
            float t = x * dx + y * dy + z * dz;
            if (d > 0)
            {
                t /= d;
            }
            if (t < 0)
            {
                t = 0;
            }
            else if(t > 1)
            {
                t = 1;
            }
            dx = p1.x + t * x - point.x;
            dy = p1.z + t * y - point.z;
            dz = p1.y + t * x - point.y;
            return dx * dx + dy * dy + dz * dz;
        }
        */
        // http://geomalgorithms.com/a02-_lines.html
        // 返回欧式距离  最坏情况，3次点积加上1次除法（据说1次除法能顶上3次乘法=-=）
        // TODO: 函数调用的最可能的情形反倒是最坏情况，因此本函数可能使用其他实现方式
        public static float GetDis(Vector3 point, Vector3 p1, Vector3 p2)
        {
            Vector3 v = p2 - p1;
            Vector3 w = point - p1;
            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
                return w.magnitude;//(point - p1).magnitude; // 点在p1方向的外侧

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
                return (point - p2).magnitude; // 点在p2方向的外侧

            float b = c1 / c2;
            Vector3 Pb = p1 + new Vector3(v.x * b , v.y * b , v.z * b); // 点到垂足的距离
            // 最后的距离计算的证明（hanpeicong自己想的）
            // c1 = |w||v|cos α（α为夹角）
            // c2 = |v|
            // b = |w|cos α
            // v * b = p1到垂足的距离
            // pb为垂足坐标
            return (point - Pb).magnitude;
        }
    }

    public class GalaxyMath
    {
        static public bool FLOAT_EQUAL(float a, float b) { return System.Math.Abs((float)((a) - (b))) < 0.0001f; }
    }
}
