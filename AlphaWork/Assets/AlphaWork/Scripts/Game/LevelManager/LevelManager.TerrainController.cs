using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework;

namespace AlphaWork
{
    public struct BlockInfo
    {
        int entityId;

        public int EntityId
        {
            get
            {
                return entityId;
            }
            set
            {
                entityId = value;
            }
        }       
    }

    public partial class LevelManager
    {
        private Terrain m_Terrain;
        private int m_blockSize = 4;
        private int m_terrainSize;
        private List<List<int>> m_blocks = new List<List<int>>(); 

        public int BlockSize
        {
            get
            { return m_blockSize; }
        }

        public int TerrainSize
        {
            get
            {
                return (int)m_Terrain.terrainData.size.x;
            }
        }
        protected void GetDefaultTerrain()
        {
            m_Terrain =  ObjectUtility.GetTerrain("TerrainBlock") as Terrain;
        }

        protected void BuildBlocks()
        {
            m_terrainSize = (int)m_Terrain.terrainData.size.x;
            for (int j = 0; j < m_terrainSize/m_blockSize; ++j)
            {
                List<int> blocklist = new List<int>();
                for (int i = 0; i < m_terrainSize / m_blockSize; ++i)
                {
                    //create structures
                    int Id = GameEntry.Entity.GenerateSerialId();
                    GameEntry.Entity.ShowStructure(new StructureData(Id, 90004)
                    {
                        Position = new Vector3((i+0.5f)* m_blockSize, 5, (j+0.5f)* m_blockSize),
                    });

                    blocklist.Add(Id);
                }
                m_blocks.Add(blocklist);
            }
        }

        public int GetBlockWeight(Vector3 pos)
        {
            int blockId = m_blocks[(int)pos.x / m_blockSize][(int)pos.z / m_blockSize];
            UnityGameFramework.Runtime.Entity ent = GameEntry.Entity.GetEntity(blockId);
            Structure st = ent.Logic as Structure;
            return st.Weight;
        }

    }
}
