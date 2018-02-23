using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace V_sem___GK___projekt2
{
    public static class LambertLightModel
    {

        public static System.Drawing.Color GetColor(System.Drawing.Color objectColor, System.Drawing.Color lightColor, Vector3D lightVector, int x, int y, Color[,] bumpMap, double offset = 0.0)
        {
            Vector3D Nobj = new Vector3D(0, 0, 1);
            Vector3D D = BlinnNormalVectorModification(x, y, bumpMap, offset);
            if (D != new Vector3D(0, 0, 0))
                D.Normalize();
            Vector3D N = Nobj + D;
            lightVector.Normalize();
            N.Normalize();

            double cos = N.X * lightVector.X + N.Y * lightVector.Y + N.Z * lightVector.Z;

            int r = (int)(lightColor.R / 255.0 * objectColor.R * cos);
            int g = (int)(lightColor.G / 255.0 * objectColor.G * cos);
            int b = (int)(lightColor.B / 255.0 * objectColor.B * cos);
            r = r > 0 ? r : 0;
            g = g > 0 ? g : 0;
            b = b > 0 ? b : 0;
            //int r = (int)(lightColor.R / 255.0 * objectColor.R * Math.Cos(Vector3D.AngleBetween(N, lightVector)));
            //int g = (int)(lightColor.G / 255.0 * objectColor.G * Math.Cos(Vector3D.AngleBetween(N, lightVector)));
            //int b = (int)(lightColor.B / 255.0 * objectColor.B * Math.Cos(Vector3D.AngleBetween(N, lightVector)));
            //r = r > 0 ? r : 0;
            //g = g > 0 ? g : 0;
            //b = b > 0 ? b : 0;


            return System.Drawing.Color.FromArgb(r, g, b);
        }

        private static Vector3D BlinnNormalVectorModification(int x, int y, Color[,] bumpMap, double offset = 0.0) // xMax and yMax - size of myBitmap
        {
            Vector3D T = new Vector3D(1, 0, 0);
            Vector3D B = new Vector3D(0, 1, 0);

            if (bumpMap == null)
                return new Vector3D(0,0,0);

            x = (x - (int)offset) % bumpMap.GetLength(0);
            //x = x % bumpMap.GetLength(0);
            y = y % bumpMap.GetLength(1);

            int r_x = bumpMap[(x + 1) % bumpMap.GetLength(0), y].R - bumpMap[x, y].R;
            //int g_x = bumpMap[(x + 1) % bumpMap.GetLength(0), y].G - bumpMap[x, y].G;
            //int b_x = bumpMap[(x + 1) % bumpMap.GetLength(0), y].B - bumpMap[x, y].B;

            int r_y = bumpMap[x, (y + 1) % bumpMap.GetLength(1)].R - bumpMap[x, y].R;
            //int g_y = bumpMap[x, (y + 1) % bumpMap.GetLength(1)].G - bumpMap[x, y].G;
            //int b_y = bumpMap[x, (y + 1) % bumpMap.GetLength(1)].B - bumpMap[x, y].B;

            //Vector3D dh_dx = new Vector3D(r_x, g_x, b_x);
            //Vector3D dh_dy = new Vector3D(r_y, g_y, b_y);

            //Vector3D TT = T * r_x + T * g_x + T * b_x;
            //Vector3D BB = B * r_y + B * g_y + B * b_y;
            //return TT + BB;

            //return Vector3D.CrossProduct(T, dh_dx) + Vector3D.CrossProduct(B, dh_dy);

            return T * -r_x + B * -r_y;
        }
    }
}
