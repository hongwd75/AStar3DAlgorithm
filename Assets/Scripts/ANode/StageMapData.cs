using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ANode
{
    [Serializable]
    public class StageMapData
    {
        public float nodeSize = 0.5f;      // 노드 크기
        public float slopeTreshold = 0.1f; // 경사로 판단할 기준값
        
        public List<SerializableVector3> spawnPosition = new List<SerializableVector3>();

        public SerializableVector3 Mapstartposition;
        public SerializableVector2Int MapNodeSize;
        
        [SerializeField]
        private List<InfoNode> serializedTileNodes;
        
        [NonSerialized]
        public InfoNode[,] tileNodes = null;

        // 몹 데이터 필요
        // 장애물 데이터 필요
        // 배경으로 사용될 프리팹 필요


        // 데이터 저장/로드
        #region Data load / save functions
#if UNITY_EDITOR        
        public void SaveTo(string filename)
        {
            serializedTileNodes = new List<InfoNode>();
            for (int i = 0; i < MapNodeSize.x; i++)
            {
                for (int j = 0; j < MapNodeSize.y; j++)
                {
                    serializedTileNodes.Add(tileNodes[i, j]);
                }
            }
            
            string filePath = Path.Combine(Application.dataPath,"Resources/data/stages", filename);
            
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Create(filePath))
            {
                // 현재 객체를 바이너리 형식으로 직렬화
                bf.Serialize(file, this);
            }

            Debug.Log($"Data saved to {filePath} in binary format");            
        }
#endif
        public void LoadFrom(string filename)
        {
            string _fileName = $"data/stages/{Path.GetFileNameWithoutExtension(filename)}"; // 확장자 제외

            TextAsset textAsset = Resources.Load<TextAsset>(_fileName);

            if (textAsset != null)
            {
                byte[] binaryData = textAsset.bytes;

                // 바이너리 데이터를 사용하여 원하는 작업 수행
                using (MemoryStream stream = new MemoryStream(binaryData))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    stream.Position = 0; // 스트림의 시작 위치를 0으로 설정
                    StageMapData loadedData  = (StageMapData)formatter.Deserialize(stream);
                    this.nodeSize = loadedData.nodeSize;
                    this.slopeTreshold = loadedData.slopeTreshold;
                    this.spawnPosition = loadedData.spawnPosition;
                    this.MapNodeSize = loadedData.MapNodeSize;
                    this.Mapstartposition  = loadedData.Mapstartposition;
                    this.serializedTileNodes = loadedData.serializedTileNodes;
                }
            }
            else
            {
                Debug.LogError("파일을 찾을 수 없습니다: " + _fileName);
                return;
            }
            
            tileNodes = new InfoNode[MapNodeSize.x, MapNodeSize.y];
            int index = 0;
            for (int i = 0; i < MapNodeSize.x; i++)
            {
                for (int j = 0; j < MapNodeSize.y; j++)
                {
                    tileNodes[i, j] = serializedTileNodes[index++];
                }
            }
            serializedTileNodes.Clear();

        }
        #endregion
    }
}