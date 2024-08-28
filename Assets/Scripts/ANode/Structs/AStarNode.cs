using System;
using UnityEngine;

namespace ANode
{
    /*
     *  A*용 노드 설정
     */
    [Serializable]
    public class InfoNode
    {
        [Flags]
        public enum NodeType
        {
            None = 0,
            walkable = 1,         // 걷기 가능
            Obstacle = 2          // 장애물 있음
        }

        public NodeType type = NodeType.walkable;
        public SerializableVector3 position;    // 월드 좌표
        public SerializableVector2Int grid;     // 그리드 내 위치
        public float Cost = 1;
        public float slope;                     // 경사도
        
        [NonSerialized]
        public int holdState = 0;               // 이미 차지하고 있는 객체 ID
    }


    /*
     *  길 찾기용
     */
    public class SearchNodeData
    {
        public InfoNode Node;
        public SearchNodeData Parent;
        public float GCost, HCost;        
        public float FCost { get { return GCost + HCost; } }

        public SearchNodeData(InfoNode node)
        {
            Node = node;
        }
    }
}