using System;
using System.Collections.Generic;
using ANode;
using ANode.MapObjects;
using UnityEngine;

namespace Management
{
    public class AStarPathfindAgent
    {
        private MapGrid grid;
        private IMapObject targetObject;
        private List<InfoNode> pathlist;
        private Vector3 destination = Vector3.zero;
        private Vector3 _dummyNextPos = Vector3.zero; // new가 계속 발생하는 것을 방지하기 위한 변수
        private int currentPathIndex = 0;
       
        public AStarPathfindAgent(MapGrid g, IMapObject o)
        {
            grid = g;
            targetObject = o;
        }
        
        // 멈춰 있는 상태인가.
        public bool isStopped()
        {
            return pathlist == null || currentPathIndex < 0 || currentPathIndex >= pathlist.Count;
        }
        
        public bool UpdatePosition(AStarAgent agent,float rotateSpeed,float speed)
        {
            if (isStopped() == true)
            {
                return false;
            }
            
            InfoNode targetGridPos = pathlist[currentPathIndex];
            Vector3 direction = (_dummyNextPos - agent.transform.position).normalized;

            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // 이동
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, _dummyNextPos, speed * Time.deltaTime);

            // 다음 지점에 충분히 가까워졌는지 확인
            if (Vector3.Distance(agent.transform.position, _dummyNextPos) < 0.01f)
            {
                currentPathIndex++;
                if (currentPathIndex < pathlist.Count)
                {
                    _dummyNextPos = pathlist[currentPathIndex].position;
                }
            }

            return true;
        }

  
        #region 도착 위치 설정  ----------------------------------------------------------------
        private bool setDestination(SerializableVector2Int dist)
        {
            currentPathIndex = 0;
            if(pathlist != null) pathlist.Clear();
            
            var info = grid.GetNearWorldObjectNode(targetObject.Position);
            if (info != null)
            {
                pathlist = FindPath(info.grid,dist);
                if (pathlist != null && pathlist.Count > 0)
                {
                    _dummyNextPos = pathlist[currentPathIndex].position;
                    return true;
                }
            }                
            return false;
        }

        public bool SetDestination(Vector3 dist)
        {
            // 방어코드
            if (grid == null)
            {
                grid = MapGrid.worldGrid;
            }
            
            var info = grid.GetNearWorldObjectNode(dist);
            if (info != null)
            {
                destination = dist;
                return setDestination(info.grid);
            }

            return false;
        }
        #endregion
        
        #region 내부 길찾기 알고리즘  -----------------------------------------------------------
        private List<InfoNode> FindPath(SerializableVector2Int start, SerializableVector2Int goal)
        {
            SearchNodeData startNode =new SearchNodeData(grid.GetNodeAt(start));
            SearchNodeData goalNode = new SearchNodeData(grid.GetNodeAt(goal));

            List<SearchNodeData> openSet = new List<SearchNodeData>();
            HashSet<InfoNode> closedSet = new HashSet<InfoNode>();

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode.Node, goalNode.Node);
            startNode.Parent = null;

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                SearchNodeData current = GetLowestFCostNode(openSet);
                if (current.Node.grid == goal)
                {
                    return RetracePath(startNode, current);
                }

                openSet.Remove(current);
                closedSet.Add(current.Node);

                foreach (SearchNodeData neighbor in GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor.Node)) continue;

                    float newCostToNeighbor = current.GCost + GetMovementCost(current.Node, neighbor.Node);
                    if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor.Node, goalNode.Node);
                        neighbor.Parent = current;
                        if(openSet.Find(x=>x.Node == neighbor.Node ) == null)
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // Path not found            
        }
        
        private List<SearchNodeData> GetNeighbors(SearchNodeData path)
        {
            List<SearchNodeData> neighbors = new List<SearchNodeData>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    SerializableVector2Int checkPos = new SerializableVector2Int(path.Node.grid.x + x, path.Node.grid.y + y);
                    if (grid.IsWithinBounds(checkPos))
                    {
                        InfoNode neighbor = grid.GetNodeAt(checkPos);
                        if (CanMoveTo(path.Node, neighbor))
                        {
                            neighbors.Add(new SearchNodeData(neighbor));
                        }
                    }
                }
            }
            return neighbors;
        }

        private bool CanMoveTo(InfoNode from, InfoNode to)
        {
            return to.type.HasFlag(InfoNode.NodeType.walkable);
            // ToDo. 장애물이 있는데 넘어가는게 가능한지 체크 하는 로직이 추가로 필요함
            // if ((to.type & InfoNode.NodeType.Obstacle) != 0)
            // {
            //     if (targetObject.CanDestroy)
            //         return true;
            //     if (targetObject.CanJump && Math.Abs(from.position.y - to.position.y) <= targetObject.JumpHeight)
            //         return true;
            //     return false;
            // }
            //
            // return (to.type & InfoNode.NodeType.walkable) != 0;
        }

        private float GetMovementCost(InfoNode from, InfoNode to)
        {
            float baseCost = Vector3.Distance(from.position, to.position);
            float slopeCost = to.slope > 0 ? 2 : 1;
            return baseCost * slopeCost * to.Cost;
        }

        private float GetDistance(InfoNode nodeA, InfoNode nodeB)
        {
            return Vector3.Distance(nodeA.position, nodeB.position);
        }

        private SearchNodeData GetLowestFCostNode(List<SearchNodeData> nodeList)
        {
            SearchNodeData lowestFCostNode = nodeList[0];
            for (int i = 1; i < nodeList.Count; i++)
            {
                if (nodeList[i].FCost < lowestFCostNode.FCost)
                    lowestFCostNode = nodeList[i];
            }
            return lowestFCostNode;
        }

        private List<InfoNode> RetracePath(SearchNodeData startNode, SearchNodeData endNode)
        {
            List<InfoNode> path = new List<InfoNode>();
            SearchNodeData currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.Node);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }
        #endregion
    }
}