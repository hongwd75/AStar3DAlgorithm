using System;
using ANode;
using UnityEngine;

namespace DefaultNamespace
{
    public class AStarNodeMaker : MonoBehaviour
    {

        private Vector3 minBound;
        private Vector3 maxBound;
        private StageMapData MapData = new StageMapData();
 
        void Start()
        {
            FindBounds();
            Debug.Log(Application.dataPath);
        }

        public void OnCreate()
        {
            if (maxBound == Vector3.zero)
            {
                FindBounds();
            }

            MapData = new StageMapData();
            CreateNodes();            
        }

        public void OnLoad()
        {
            if (maxBound == Vector3.zero)
            {
                FindBounds();
            }            
            MapData.LoadFrom("stage.bytes");
        }

        public void OnSave()
        {
            if (maxBound == Vector3.zero)
            {
                FindBounds();
            }            
            MapData.SaveTo("stage.bytes");
        }

        void FindBounds()
        {
            GameObject[] loadObjects = GameObject.FindGameObjectsWithTag("Ground");
            if (loadObjects.Length == 0)
            {
                Debug.LogError("Load 태그의 오브젝트를 찾을 수 없습니다.");
                return;
            }

            minBound = loadObjects[0].transform.position;
            maxBound = loadObjects[0].transform.position;

            foreach (GameObject obj in loadObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    minBound = Vector3.Min(minBound, renderer.bounds.min);
                    maxBound = Vector3.Max(maxBound, renderer.bounds.max);
                }
            }
        }

        void CreateNodes()
        {
            int gridX = 0;
            int gridZ = 0;
            int GroundLayer = LayerMask.GetMask("Load");
            
            float gap = MapData.nodeSize / 2f;
            int MaxWidth = (int)((maxBound.x - minBound.x) / MapData.nodeSize) + 2;
            int MaxHeight = (int)((maxBound.z - minBound.z) / MapData.nodeSize) + 2;

            MapData.tileNodes = new InfoNode[MaxWidth, MaxHeight];
            MapData.MapNodeSize = new SerializableVector2Int(MaxWidth, MaxHeight);
            MapData.Mapstartposition = minBound;
            MapData.Mapstartposition.x -= MapData.nodeSize;
            MapData.Mapstartposition.z -= MapData.nodeSize;
                
            // 버퍼 채우기
            for (float x = minBound.x - MapData.nodeSize + gap; x <= maxBound.x + MapData.nodeSize - gap; x += MapData.nodeSize)
            {
                gridZ = 0;
                for (float z = minBound.z - MapData.nodeSize + gap; z <= maxBound.z + MapData.nodeSize - gap; z += MapData.nodeSize)
                {
                    Vector3 rayStart = new Vector3(x, maxBound.y + 3, z);
                    RaycastHit hit;

                    if (Physics.Raycast(rayStart, Vector3.down, out hit, 10, GroundLayer))
                    {
                        CreateNode(hit.point, new SerializableVector2Int(gridX, gridZ), hit.normal);
                    }
                    else
                    {
                        CreateEmpty(new SerializableVector2Int(gridX, gridZ));
                    }
                    gridZ++;
                }
                gridX++;
            }
        }

        void CreateEmpty(SerializableVector2Int grid)
        {
            MapData.tileNodes[grid.x, grid.y] = new InfoNode()
            {
                Cost = -1f,
                grid = grid,
                position = Vector3.zero,
                slope = 0f,
                type = InfoNode.NodeType.None
            };
        }
        void CreateNode(Vector3 position, SerializableVector2Int grid, Vector3 normal)
        {
            InfoNode node = new InfoNode();
            node.position = position;
            node.grid = grid;

            float slope = Vector3.Angle(normal, Vector3.up);
            node.slope = slope;

            if (slope > MapData.slopeTreshold)
            {
                node.Cost = 2;
            }
            else
            {
                node.Cost = 1;
            }

            // 여기서 장애물 체크 로직을 추가할 수 있습니다.
            // 예: if (IsObstacle(position)) node.type = InfoNode.NodeType.Obstacle;

            MapData.tileNodes[grid.x,grid.y] = node;

        }

        void OnDrawGizmos()
        {
            if (MapData == null || MapData.tileNodes == null || MapData.tileNodes.Length == 0) return;

            for (int x = 0; x < MapData.MapNodeSize.x; x++)
            {
                for (int z = 0; z < MapData.MapNodeSize.y; z++)
                {
                    InfoNode node = MapData.tileNodes[x, z];
                    if (node == null || node.type == InfoNode.NodeType.None)
                    {
                        Gizmos.color = Color.magenta;
                        Vector3 pos = new Vector3(MapData.nodeSize * x + MapData.Mapstartposition.x + (MapData.nodeSize*0.5f),
                            MapData.Mapstartposition.y, MapData.nodeSize * z + MapData.Mapstartposition.z + (MapData.nodeSize*0.5f));
                        Gizmos.DrawCube(pos, new Vector3(0.1f, 0.1f, 0.1f));
                    }
                    else
                    {
                        // 노드 타입에 따라 색상 설정
                        if (node.type == InfoNode.NodeType.Obstacle)
                        {
                            Gizmos.color = Color.red;
                        }
                        else if (node.Cost > 1)
                        {
                            Gizmos.color = new Color(1f, 0.5f, 0f); // 주황색
                        }
                        else
                        {
                            Gizmos.color = Color.blue;
                        }
                        // 노드 위치에 큐브 그리기
                        Gizmos.DrawCube(node.position, new Vector3(0.1f, 0.1f, 0.1f));
                    }                    
                }
            }


            // minBound와 maxBound 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((minBound + maxBound) / 2, maxBound - minBound);            
        }
    }
}