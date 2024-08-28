using UnityEngine;

namespace ANode.MapObjects
{
    /// <summary>
    /// 맵 겍체 기본 정의
    /// </summary>
    public interface IMapObject
    {
        public enum ObjectType
        {
            Obstacle,
            Character,
        };
        
        bool CanJump { get; } // 점프 가능한가
        bool CanMove { get; } // 이동 가능한가
        bool CanDestroy { get; } // 파괴 가능한가.
        bool IsMoving { get; }
        
        int ID { get; }      // 생성된 객체 ID        
        ObjectType Type { get; }
        TileObjectInfo TileObjectInfo { get; } // 객체 정보
        
        float Angle { get; } // 회전 상태
        Vector3 Position { get; } // 위치값
    }
}