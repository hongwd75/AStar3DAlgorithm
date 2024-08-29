using System;
using System.Collections.Generic;
using ANode;
using ANode.MapObjects;
using UnityEngine;

namespace Management
{
    public class MapGrid
    {
        #region 이동 객체 관리
        // 현재 모든 에이전트의 위치를 저장하는 정적 딕셔너리
        private static List<IMapObject> IMapObjects = new List<IMapObject>();

        public static void ClearAllIMapObject()
        {
            IMapObjects.Clear();
        }

        public static void AddIMapObject(IMapObject obj)
        {
            if (IMapObjects.Contains(obj) == false)
            {
                IMapObjects.Add(obj);
            }
        }
        
        public static void RemoveIMapObject(IMapObject obj)
        {
            IMapObjects.Remove(obj);
        }        
        #endregion
        
        public static MapGrid worldGrid;
        protected StageMapData _stageMapData;


        public MapGrid(string stagename)
        {
            //ClearAllIMapObject();
            Load(stagename);
        }
        
        // 데이터 로딩
        public bool Load(string stagename)
        {
            _stageMapData = new StageMapData();
            _stageMapData.LoadFrom(stagename);
            if (_stageMapData.MapNodeSize.x == 0) return false;
            
            // 맵에 객체 배치
            worldGrid = this;
            return true;
        }

        public Vector3 SerializableVector2IntToVector3(SerializableVector2Int v)
        {
            return _stageMapData.tileNodes[v.x, v.y].position;
        }

        public SerializableVector2Int Vector3ToSerializableVector2Int(Vector3 pos)
        {
            int x = (int)Math.Round((pos.x - _stageMapData.Mapstartposition.x) / _stageMapData.nodeSize);
            int y = (int)Math.Round((pos.z - _stageMapData.Mapstartposition.z) / _stageMapData.nodeSize);
            // 타일 인덱스가 그리드 범위를 벗어나지 않도록 제한
            x = Math.Max(0, Math.Min(x, _stageMapData.MapNodeSize.x - 1));
            y = Math.Max(0, Math.Min(y, _stageMapData.MapNodeSize.y - 1));
            return new SerializableVector2Int(x, y);
        }
        
        
        // 월드 좌표에서 가까운 위치값 찾기
        public InfoNode GetNearWorldObjectNode(Vector3 pos,IMapObject obj)
        {
            var v2 = Vector3ToSerializableVector2Int(pos);

            // ToDo. 추가적으로 배열의 상태에 따라서 차선의 배열 index를 찾는 로직도 추가해야 함
            return GetNodeAt(FindNearestAvailablePosition(v2,obj));
        }
        
        public bool IsWithinBounds(SerializableVector2Int pos)
        {
            return pos.x > 0 && pos.x < _stageMapData.MapNodeSize.x-1 && pos.y > 0 && pos.y < _stageMapData.MapNodeSize.y-1;
        }

        public InfoNode GetNodeAt(SerializableVector2Int pos)
        {
            if (IsWithinBounds(pos))
                return _stageMapData.tileNodes[pos.x, pos.y];
            return null;
        }

        public void SetNode(SerializableVector2Int pos, int ObjectID)
        {
            if (IsWithinBounds(pos))
                _stageMapData.tileNodes[pos.x, pos.y].holdState = ObjectID;
        }
        
        // 주변에 비어 있는 타일 찾기
        private SerializableVector2Int FindNearestAvailablePosition(SerializableVector2Int originalGoal,IMapObject obj, int searchRadius = 5)
        {
            for (int r = 1; r <= searchRadius; r++)
            {
                for (int x = -r; x <= r; x++)
                {
                    for (int y = -r; y <= r; y++)
                    {
                        if (Math.Abs(x) == r || Math.Abs(y) == r)
                        {
                            SerializableVector2Int checkPos = new SerializableVector2Int(originalGoal.x + x, originalGoal.y + y);

                            InfoNode node = GetNodeAt(checkPos);
                            if (node != null && node.isEmpty() == true && !IsPositionOccupied(node.grid,obj))
                            {
                                return checkPos;
                            }

                        }
                    }
                }
            }
            return originalGoal; // 사용 가능한 위치를 찾지 못한 경우 원래 위치 반환
        }
        
        // 점유 범위 체크
        public bool IsPositionOccupied(SerializableVector2Int position,IMapObject obj)
        {
            float deltaX, deltaZ;
            float distanceSquared;
            float distance;
            foreach (var occupancy in IMapObjects)
            {
                if (occupancy.ID != obj.ID)
                {
                    if (position == Vector3ToSerializableVector2Int(occupancy.Position))
                    {
                        return true;
                    }
                }
            }
            return false; // 임시 반환값
        }
    }
}