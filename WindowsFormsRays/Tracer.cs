﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsRays
{
    public class Tracer
    {
        private int w;
        private int h;

        public delegate float QueryDatabase(Vec position, out int hitType);

        private QueryDatabase queryDatabase;
        public Tracer(int w, int h, QueryDatabase queryDatabase)
        {
            this.w = w;
            this.h = h;
            this.queryDatabase = queryDatabase;

            sourceBmp = new MyPixel[w, h];
            bmp = new Bitmap(w, h);
        }

        public DateTime startTime;

        volatile private MyPixel[,] sourceBmp;
        public volatile Bitmap bmp;
        void AddPixel(int x, int y, int r, int g, int b)
        {
            lock (bmp)
            {
                sourceBmp[x, y].c++;
                sourceBmp[x, y].r += r;
                sourceBmp[x, y].g += g;
                sourceBmp[x, y].b += b;
                bmp.SetPixel(x, y, sourceBmp[x, y].GetColor());
            }
        }

        public int p;

        public void Run()
        {
            int samplesCount = 2000;

            for (p = 0; p < samplesCount; p++)
            {
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        Vec position = new Vec(-22, 5, 25) + new Vec(randomVal(), randomVal(), randomVal()) * 0.7f;
                        //Vec position = new Vec(-20, 5, 18);

                        Vec goal = (new Vec(-3, 4, 0) - position).Normal();
                        Vec left = -(new Vec(goal.z, 0, -goal.x)).Normal() * (1.0f / w);

                        // Cross-product to get the up vector
                        Vec up = (new Vec(goal.y * left.z - goal.z * left.y,
                               goal.z * left.x - goal.x * left.z,
                               goal.x * left.y - goal.y * left.x));

                        //Vec target = (goal + left * (x - w / 2 + randomVal())*0.1f + up * (y - h / 2 + randomVal())*0.1f ).Normal();
                        Vec target = (goal + left * (x - w / 2 + randomVal()) + up * (y - h / 2 + randomVal())).Normal();
                        Vec color = Trace(position, target);
                        AddPixel(x, y, (int)color.x, (int)color.y, (int)color.z);
                    }

                timeSpan = DateTime.Now - startTime;
            }
        }

        Random r = new Random();

        float min(float l, float r) { return l < r ? l : r; }
        float randomVal() { return (float)r.NextDouble(); }


        enum HitType
        {
            HIT_NONE = 0,
            HIT_LETTER = 1,
            HIT_WALL = 2,
            HIT_SUN = 3,
        }

        Vec[,,] dataNormal;
        float[,,] dataDist;
        float[,,] dataDistPPP;
        float[,,] dataDistPPM;
        float[,,] dataDistPMP;
        float[,,] dataDistPMM;
        float[,,] dataDistMPP;
        float[,,] dataDistMPM;
        float[,,] dataDistMMP;
        float[,,] dataDistMMM;
        int[,,] dataType;

        float minX = -35;
        float maxX = 35;

        float minY = -5.5f;
        float maxY = 25;

        float minZ = -35;
        float maxZ = 35;

        //float minX = -55;
        //float maxX = 55;

        //float minY = -10.5f;
        //float maxY = 25;

        //float minZ = -55;
        //float maxZ = 55;


        int cx = 200;
        int cy = 100;
        int cz = 200;

        float diag;

        float min(float x1, float x2, float x3, float x4, float x5, float x6, float x7, float x8)
        {
            return min(x1, min(x2, min(x3, min(x4, min(x5, min(x6, min(x7, x8)))))));
        }
        public void InitData()
        {

            dataDist = new float[cx + 1, cy + 1, cz + 1];
            dataDistPPP = new float[cx + 1, cy + 1, cz + 1];
            dataDistPPM = new float[cx + 1, cy + 1, cz + 1];
            dataDistPMP = new float[cx + 1, cy + 1, cz + 1];
            dataDistPMM = new float[cx + 1, cy + 1, cz + 1];
            dataDistMPP = new float[cx + 1, cy + 1, cz + 1];
            dataDistMPM = new float[cx + 1, cy + 1, cz + 1];
            dataDistMMP = new float[cx + 1, cy + 1, cz + 1];
            dataDistMMM = new float[cx + 1, cy + 1, cz + 1];
            dataType = new int[cx + 1, cy + 1, cz + 1];
            dataNormal = new Vec[cx + 1, cy + 1, cz + 1];

            float cellSizeX = (maxX - minX - 1) / cx;
            float cellSizeY = (maxY - minY - 1) / cy;
            float cellSizeZ = (maxZ - minZ - 1) / cz;

            diag = (float)Math.Sqrt(cellSizeX * cellSizeX + cellSizeY * cellSizeY + cellSizeZ * cellSizeZ);

            for (int i = 0; i < cx; i++)
            {
                for (int j = 0; j < cy + 1; j++)
                    for (int k = 0; k < cz + 1; k++)
                    {
                        float worldX = minX + i * (maxX - minX - 1) / cx;
                        float worldY = minY + j * (maxY - minY - 1) / cy;
                        float worldZ = minZ + k * (maxZ - minZ - 1) / cz;

                        Vec hitPos = new Vec(worldX, worldY, worldZ);

                        dataDist[i, j, k] = queryDatabase(hitPos, out int hitType);
                        dataType[i, j, k] = hitType;

                        float d = queryDatabase(hitPos, out _);

                        dataNormal[i, j, k] = new Vec(queryDatabase(hitPos + new Vec(.01f, 0), out _) - d,
                           queryDatabase(hitPos + new Vec(0, .01f), out _) - d,
                           queryDatabase(hitPos + new Vec(0, 0, .01f), out _) - d).Normal();

                        if (float.IsNaN(dataNormal[i, j, k].x))
                            dataNormal[i, j, k] = new Vec(0, 0, 1);
                    }
            }


            float diagXYZ = (float)Math.Sqrt(cellSizeX * cellSizeX + cellSizeY * cellSizeY + cellSizeZ * cellSizeZ);
            float diagXY = (float)Math.Sqrt(cellSizeX * cellSizeX + cellSizeY * cellSizeY);
            float diagXZ = (float)Math.Sqrt(cellSizeX * cellSizeX + cellSizeZ * cellSizeZ);
            float diagYZ = (float)Math.Sqrt(cellSizeY * cellSizeY + cellSizeZ * cellSizeZ);

            for (int x = 1; x < cx; x++)
                for (int y = 1; y < cy; y++)
                    for (int z = 1; z < cz; z++)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x <= 0 && normal.y <= 0 && normal.z <= 0))
                            startMin = float.MaxValue;


                        dataDistMMM[x, y, z] = min(startMin,
                            dataDistMMM[x - 1, y, z] + cellSizeX,
                            dataDistMMM[x, y - 1, z] + cellSizeY,
                            dataDistMMM[x, y, z - 1] + cellSizeZ,
                            dataDistMMM[x - 1, y - 1, z] + diagXY,
                            dataDistMMM[x, y - 1, z - 1] + diagYZ,
                            dataDistMMM[x - 1, y, z - 1] + diagXZ,
                            dataDistMMM[x - 1, y - 1, z - 1] + diagXYZ);
                    }

            for (int x = 1; x < cx; x++)
                for (int y = 1; y < cy; y++)
                    for (int z = cz - 1; z >= 0; z--)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x <= 0 && normal.y <= 0 && normal.z >= 0))
                            startMin = float.MaxValue;

                        dataDistMMP[x, y, z] = min(startMin,
                            dataDistMMP[x - 1, y, z] + cellSizeX,
                            dataDistMMP[x, y - 1, z] + cellSizeY,
                            dataDistMMP[x, y, z + 1] + cellSizeZ,
                            dataDistMMP[x - 1, y - 1, z] + diagXY,
                            dataDistMMP[x, y - 1, z + 1] + diagYZ,
                            dataDistMMP[x - 1, y, z + 1] + diagXZ,
                            dataDistMMP[x - 1, y - 1, z + 1] + diagXYZ);
                    }

            for (int x = 1; x < cx; x++)
                for (int y = cy - 1; y >= 0; y--)
                    for (int z = 1; z < cz; z++)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x <= 0 && normal.y >= 0 && normal.z <= 0))
                            startMin = float.MaxValue;

                        dataDistMPM[x, y, z] = min(startMin,
                            dataDistMPM[x - 1, y, z] + cellSizeX,
                            dataDistMPM[x, y + 1, z] + cellSizeY,
                            dataDistMPM[x, y, z - 1] + cellSizeZ,
                            dataDistMPM[x - 1, y + 1, z] + diagXY,
                            dataDistMPM[x, y + 1, z - 1] + diagYZ,
                            dataDistMPM[x - 1, y, z - 1] + diagXZ,
                            dataDistMPM[x - 1, y + 1, z - 1] + diagXYZ);
                    }

            for (int x = 1; x < cx; x++)
                for (int y = cy - 1; y >= 0; y--)
                    for (int z = cz - 1; z >= 0; z--)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x <= 0 && normal.y >= 0 && normal.z >= 0))
                            startMin = float.MaxValue;

                        dataDistMPP[x, y, z] = min(startMin,
                            dataDistMPP[x - 1, y, z] + cellSizeX,
                            dataDistMPP[x, y + 1, z] + cellSizeY,
                            dataDistMPP[x, y, z + 1] + cellSizeZ,
                            dataDistMPP[x - 1, y + 1, z] + diagXY,
                            dataDistMPP[x, y + 1, z + 1] + diagYZ,
                            dataDistMPP[x - 1, y, z + 1] + diagXZ,
                            dataDistMPP[x - 1, y + 1, z + 1] + diagXYZ);
                    }

            for (int x = cx - 1; x >= 0; x--)
                for (int y = 1; y < cy; y++)
                    for (int z = 1; z < cz; z++)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x >= 0 && normal.y <= 0 && normal.z <= 0))
                            startMin = float.MaxValue;

                        dataDistPMM[x, y, z] = min(startMin,
                            dataDistPMM[x + 1, y, z] + cellSizeX,
                            dataDistPMM[x, y - 1, z] + cellSizeY,
                            dataDistPMM[x, y, z - 1] + cellSizeZ,
                            dataDistPMM[x + 1, y - 1, z] + diagXY,
                            dataDistPMM[x, y - 1, z - 1] + diagYZ,
                            dataDistPMM[x + 1, y, z - 1] + diagXZ,
                            dataDistPMM[x + 1, y - 1, z - 1] + diagXYZ);
                    }

            for (int x = cx - 1; x >= 0; x--)
                for (int y = 1; y < cy; y++)
                    for (int z = cz - 1; z >= 0; z--)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x >= 0 && normal.y <= 0 && normal.z >= 0))
                            startMin = float.MaxValue;

                        dataDistPMP[x, y, z] = min(startMin,
                            dataDistPMP[x + 1, y, z] + cellSizeX,
                            dataDistPMP[x, y - 1, z] + cellSizeY,
                            dataDistPMP[x, y, z + 1] + cellSizeZ,
                            dataDistPMP[x + 1, y - 1, z] + diagXY,
                            dataDistPMP[x, y - 1, z + 1] + diagYZ,
                            dataDistPMP[x + 1, y, z + 1] + diagXZ,
                            dataDistPMP[x + 1, y - 1, z + 1] + diagXYZ);
                    }

            for (int x = cx - 1; x >= 0; x--)
                for (int y = cy - 1; y >= 0; y--)
                    for (int z = 1; z < cz; z++)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x >= 0 && normal.y >= 0 && normal.z <= 0))
                            startMin = float.MaxValue;

                        dataDistPPM[x, y, z] = min(startMin,
                            dataDistPPM[x + 1, y, z] + cellSizeX,
                            dataDistPPM[x, y + 1, z] + cellSizeY,
                            dataDistPPM[x, y, z - 1] + cellSizeZ,
                            dataDistPPM[x + 1, y + 1, z] + diagXY,
                            dataDistPPM[x, y + 1, z - 1] + diagYZ,
                            dataDistPPM[x + 1, y, z - 1] + diagXZ,
                            dataDistPPM[x + 1, y + 1, z - 1] + diagXYZ);
                    }

            for (int x = cx - 1; x >= 0; x--)
                for (int y = cy - 1; y >= 0; y--)
                    for (int z = cz - 1; z >= 0; z--)
                    {
                        float startMin = dataDist[x, y, z];
                        Vec normal = dataNormal[x, y, z];
                        if (startMin >= diagXYZ / 2 || (normal.x >= 0 && normal.y >= 0 && normal.z >= 0))
                            startMin = float.MaxValue;

                        dataDistPPP[x, y, z] = min(startMin,
                            dataDistPPP[x + 1, y, z] + cellSizeX,
                            dataDistPPP[x, y + 1, z] + cellSizeY,
                            dataDistPPP[x, y, z + 1] + cellSizeZ,
                            dataDistPPP[x + 1, y + 1, z] + diagXY,
                            dataDistPPP[x, y + 1, z + 1] + diagYZ,
                            dataDistPPP[x + 1, y, z + 1] + diagXZ,
                            dataDistPPP[x + 1, y + 1, z + 1] + diagXYZ);
                    }
        }

        int FastFastQueryDatabaseHitType(Vec position)
        {
            float ceilX = ((position.x - minX) * cx / (maxX - minX - 1));
            float ceilY = ((position.y - minY) * cy / (maxY - minY - 1));
            float ceilZ = ((position.z - minZ) * cz / (maxZ - minZ - 1));
            int i = (int)ceilX;
            int j = (int)ceilY;
            int k = (int)ceilZ;
            int hitType = dataType[(int)Math.Round(ceilX),
                                (int)Math.Round(ceilY),
                                (int)Math.Round(ceilZ)];

            return hitType;
        }

        Vec FastFastQueryDatabaseNorm(Vec position)
        {
            float ceilX = ((position.x - minX) * cx / (maxX - minX - 1));
            float ceilY = ((position.y - minY) * cy / (maxY - minY - 1));
            float ceilZ = ((position.z - minZ) * cz / (maxZ - minZ - 1));
            int i = (int)ceilX;
            int j = (int)ceilY;
            int k = (int)ceilZ;

            Vec v5 = dataNormal[i, j, k];
            Vec v1 = dataNormal[i + 1, j, k];
            Vec v2 = dataNormal[i + 1, j + 1, k];
            Vec v3 = dataNormal[i + 1, j + 1, k + 1];
            Vec v4 = dataNormal[i + 1, j, k + 1];

            Vec v6 = dataNormal[i, j + 1, k];
            Vec v7 = dataNormal[i, j + 1, k + 1];
            Vec v8 = dataNormal[i, j, k + 1];

            float l = ceilX - i;
            float s = ceilY - j;
            float m = ceilZ - k;

            Vec a = (v1 - v5) * l + v5;
            Vec b = (v2 - v6) * l + v6;

            Vec c = (v4 - v8) * l + v8;
            Vec d = (v3 - v7) * l + v7;

            Vec e = (b - a) * s + a;
            Vec f = (d - c) * s + c;

            Vec res = (f - e) * m + e;
            return res;
        }
        public int steps = 0;
        // Sample the world using Signed Distance Fields.
        float FastFastQueryDatabase(Vec position, float[,,] data)//, out int hitType)
        {
            steps++;
            float ceilX = ((position.x - minX) * cx / (maxX - minX - 1));
            float ceilY = ((position.y - minY) * cy / (maxY - minY - 1));
            float ceilZ = ((position.z - minZ) * cz / (maxZ - minZ - 1));
            int i = (int)ceilX;
            int j = (int)ceilY;
            int k = (int)ceilZ;

            float v5 = data[i, j, k];

            if (v5 > 2 * diag)
                return v5 - diag;


            float v1 = data[i + 1, j, k];//
            float v2 = data[i + 1, j + 1, k];//
            float v3 = data[i + 1, j + 1, k + 1];//
            float v4 = data[i + 1, j, k + 1];//

            float v6 = data[i, j + 1, k];//
            float v7 = data[i, j + 1, k + 1];//
            float v8 = data[i, j, k + 1];//

            float l = ceilX - i;
            float s = ceilY - j;
            float m = ceilZ - k;

            float a = (v1 - v5) * l + v5;
            float b = (v2 - v6) * l + v6;

            float c = (v4 - v8) * l + v8;
            float d = (v3 - v7) * l + v7;

            float e = (b - a) * s + a;
            float f = (d - c) * s + c;

            float res = (f - e) * m + e;
            return res;

        }

        float[,,] GetDataDist(Vec direction)
        {
            //float[,,] data = dataDist;
            if (direction.x >= 0)
            {
                if (direction.y >= 0)
                {
                    if (direction.z >= 0)
                        return dataDistPPP;
                    else
                        return dataDistPPM;
                }
                else
                {
                    if (direction.z >= 0)
                        return dataDistPMP;
                    else
                        return dataDistPMM;
                }
            }
            else
            {
                if (direction.y >= 0)
                {
                    if (direction.z >= 0)
                        return dataDistMPP;
                    else
                        return dataDistMPM;
                }
                else
                {
                    if (direction.z >= 0)
                        return dataDistMMP;
                    else
                        return dataDistMMM;
                }
            }
        }

        int RayMarching(Vec origin, Vec direction, float accuracy)
        {
            float[,,] data = GetDataDist(direction);

            Vec hitPos = origin;
            int hitType = (int)HitType.HIT_NONE;
            int noHitCount = 0;
            float d = FastFastQueryDatabase(hitPos, data); // distance from closest object in world.
            float prev_d;
            // Signed distance marching
            for (float total_d = d; total_d < 100; total_d += d)
            {
                //Vec prev = hitPos;
                hitPos = origin + direction * total_d;
                prev_d = d;
                d = FastFastQueryDatabase(hitPos, data);
                //if (prev_d >= accuracy + 0.01 && d < accuracy//тут было 0.01
                if (d < accuracy//тут было 0.01
                            || ++noHitCount > 99 || d < 0)
                {
                    hitType = FastFastQueryDatabaseHitType(hitPos);

                    return hitType; // Weird return statement where a variable is also updated.
                }
            }
            return 0;
        }

        // Perform signed sphere marching
        // Returns hitType 0, 1, 2, or 3 and update hit position/normal
        int RayMarching(Vec origin, Vec direction, float accuracy, out Vec hitPos, out Vec hitNorm)
        {
            float[,,] data = GetDataDist(direction);

            hitPos = origin;
            hitNorm = direction;
            int hitType = (int)HitType.HIT_NONE;
            int noHitCount = 0;
            float d = FastFastQueryDatabase(hitPos, data); // distance from closest object in world.
            float prev_d;
            //bool outside = false;
            // Signed distance marching
            for (float total_d = d; total_d < 100; total_d += d)
            {
                //Vec prev = hitPos;
                hitPos = origin + direction * total_d;
                prev_d = d;
                d = FastFastQueryDatabase(hitPos, data);
                //if (prev_d >= accuracy + 0.01 && d < accuracy//тут было 0.01
                if (d < accuracy//тут было 0.01
                            || ++noHitCount > 99 || d < 0)
                {
                    hitType = FastFastQueryDatabaseHitType(hitPos);

                    if (noHitCount < 99)
                    {
                        if (hitType == (int)HitType.HIT_LETTER)
                        {
                            d = queryDatabase(hitPos, out _);
                            hitNorm = new Vec(queryDatabase(hitPos + new Vec(.01f, 0), out _) - d,
                               queryDatabase(hitPos + new Vec(0, .01f), out _) - d,
                               queryDatabase(hitPos + new Vec(0, 0, .01f), out _) - d).Normal();

                            //hitNorm = FastFastQueryDatabaseNorm(hitPos);
                        }
                        else
                        {
                            hitNorm = FastFastQueryDatabaseNorm(hitPos);
                            //hitNorm = new Vec(FastFastQueryDatabase(hitPos + new Vec(.01f, 0)) - d,
                            //  FastFastQueryDatabase(hitPos + new Vec(0, .01f)) - d,
                            //  FastFastQueryDatabase(hitPos + new Vec(0, 0, .01f)) - d).Normal();
                        }
                    }

                    if (float.IsNaN(hitNorm.x))
                    {
                        //hitNorm = hitNorm;
                        hitNorm = direction;
                    }

                    //hitPos = prev;
                    //hitPos = (direction%hitNorm)
                    return hitType; // Weird return statement where a variable is also updated.
                }
            }
            return 0;
        }

        Vec lightDirection = new Vec(.6f, .6f, 1).Normal();// Directional light
        public TimeSpan timeSpan;

        Vec Trace(Vec origin, Vec direction)
        {
            Vec color = new Vec();
            float attenuation = 1;
            //Vec lightDirection = (new Vec(.6f, .6f, 1)).Normal(); 
            float accuracy = 0.02f;
            for (int bounceCount = 3; bounceCount-- > 0;)
            {
                int hitType = RayMarching(origin, direction, accuracy, out var sampledPosition, out var normal);
                accuracy *= 2;
                if (hitType == (int)HitType.HIT_NONE)
                    break; // No hit. This is over, return color.
                if (hitType == (int)HitType.HIT_LETTER)
                { // Specular bounce on a letter. No color acc.
                    direction = direction + normal * (normal % direction * -2);
                    origin = sampledPosition + direction * 0.1f;
                    attenuation = attenuation * 0.2f; // Attenuation via distance traveled.
                }
                if (hitType == (int)HitType.HIT_WALL)
                { // Wall hit uses color yellow?
                    float incidence = normal % lightDirection;
                    float p = 6.283185f * randomVal();
                    float c = randomVal();
                    float s = (float)Math.Sqrt(1 - c);
                    float g = normal.z < 0 ? -1 : 1;
                    float u = -1 / (g + normal.z);
                    float v = normal.x * normal.y * u;
                    direction = new Vec(v,
                                    g + normal.y * normal.y * u,
                                    -normal.y) * ((float)Math.Cos(p) * s)
                                +
                                new Vec(1 + g * normal.x * normal.x * u,
                                    g * v,
                                    -g * normal.x) * ((float)Math.Sin(p) * s) + normal * (float)Math.Sqrt(c);
                    origin = sampledPosition + direction * .1f;
                    attenuation = attenuation * 0.2f;
                    if (incidence > 0)
                    {
                        var ldir = (lightDirection * 20 + new Vec(randomVal(), randomVal(), randomVal())).Normal();
                        if (RayMarching(sampledPosition + normal * .1f,
                                    ldir, accuracy) == (int)HitType.HIT_SUN)

                            color = color + new Vec(500, 400, 100) * (attenuation * incidence * 0.5f);
                    }
                }
                if (hitType == (int)HitType.HIT_SUN)
                { //
                    color = color + new Vec(50, 80, 100) * attenuation;
                    break; // Sun Color
                }
            }
            return color;
        }
    }
}
