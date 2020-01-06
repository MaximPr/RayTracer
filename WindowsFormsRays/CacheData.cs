using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays
{
    public class CacheData
    {
        private SceneData scene;
        public CacheData(SceneData scene)
        {
            this.scene = scene;
        }

        float min(float l, float r) { return l < r ? l : r; }

        Vector[,,] dataNormal;
        float[,,] dataDist;
        float[,,] dataDistPPP;
        float[,,] dataDistPPM;
        float[,,] dataDistPMP;
        float[,,] dataDistPMM;
        float[,,] dataDistMPP;
        float[,,] dataDistMPM;
        float[,,] dataDistMMP;
        float[,,] dataDistMMM;
        IMaterial[,,] dataType;

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
            dataType = new IMaterial[cx + 1, cy + 1, cz + 1];
            dataNormal = new Vector[cx + 1, cy + 1, cz + 1];

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

                        Vector hitPos = new Vector(worldX, worldY, worldZ);
                        float d = scene.QueryDatabase(hitPos, out var hitObj);

                        dataDist[i, j, k] = d;
                        dataType[i, j, k] = hitObj.Material;
                        dataNormal[i, j, k] = scene.QueryDatabaseNorm(hitPos, d, hitObj);

                        if (float.IsNaN(dataNormal[i, j, k].x))
                            dataNormal[i, j, k] = new Vector(0, 0, 1);
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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
                        Vector normal = dataNormal[x, y, z];
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

        public IMaterial FastFastQueryDatabaseHitType(Vector position)
        {
            float ceilX = ((position.x - minX) * cx / (maxX - minX - 1));
            float ceilY = ((position.y - minY) * cy / (maxY - minY - 1));
            float ceilZ = ((position.z - minZ) * cz / (maxZ - minZ - 1));
            int i = (int)ceilX;
            int j = (int)ceilY;
            int k = (int)ceilZ;
            return dataType[(int)Math.Round(ceilX), (int)Math.Round(ceilY), (int)Math.Round(ceilZ)];
        }

        public Vector FastFastQueryDatabaseNorm(Vector position)
        {
            float ceilX = ((position.x - minX) * cx / (maxX - minX - 1));
            float ceilY = ((position.y - minY) * cy / (maxY - minY - 1));
            float ceilZ = ((position.z - minZ) * cz / (maxZ - minZ - 1));
            int i = (int)ceilX;
            int j = (int)ceilY;
            int k = (int)ceilZ;

            Vector v5 = dataNormal[i, j, k];
            Vector v1 = dataNormal[i + 1, j, k];
            Vector v2 = dataNormal[i + 1, j + 1, k];
            Vector v3 = dataNormal[i + 1, j + 1, k + 1];
            Vector v4 = dataNormal[i + 1, j, k + 1];

            Vector v6 = dataNormal[i, j + 1, k];
            Vector v7 = dataNormal[i, j + 1, k + 1];
            Vector v8 = dataNormal[i, j, k + 1];

            float l = ceilX - i;
            float s = ceilY - j;
            float m = ceilZ - k;

            Vector a = (v1 - v5) * l + v5;
            Vector b = (v2 - v6) * l + v6;

            Vector c = (v4 - v8) * l + v8;
            Vector d = (v3 - v7) * l + v7;

            Vector e = (b - a) * s + a;
            Vector f = (d - c) * s + c;

            Vector res = (f - e) * m + e;
            return res;
        }

        public ulong steps = 0;

        public float FastFastQueryDatabase(Vector position, float[,,] data)//, out int hitType)
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

        public float[,,] GetDataDist(Vector direction)
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
    }
}
