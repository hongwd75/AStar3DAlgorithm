using System;
using ANode;
using UnityEngine;

namespace Management
{
    public class MapGrid
    {
        protected StageMapData _stageMapData;

        // 데이터 로딩
        public bool Load(string stagename)
        {
            _stageMapData = new StageMapData();
            _stageMapData.LoadFrom(stagename);
            if (_stageMapData.MapNodeSize.x == 0) return false;
            
            // 맵에 객체 배치
            return true;
        }


        // 월드 좌표에서 가까운 위치값 찾기
        public InfoNode GetNearWorldObjectNode(Vector3 pos)
        {
            int x = (int)Math.Round((pos.x - _stageMapData.Mapstartposition.x) / _stageMapData.nodeSize);
            int y = (int)Math.Round((pos.y - _stageMapData.Mapstartposition.y) / _stageMapData.nodeSize);
            // 타일 인덱스가 그리드 범위를 벗어나지 않도록 제한
            x = Math.Max(0, Math.Min(x, _stageMapData.MapNodeSize.x - 1));
            y = Math.Max(0, Math.Min(y, _stageMapData.MapNodeSize.y - 1));

            // ToDo. 추가적으로 배열의 상태에 따라서 차선의 배열 index를 찾는 로직도 추가해야 함
            return GetNodeAt(new SerializableVector2Int(x, y));
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
    }
}