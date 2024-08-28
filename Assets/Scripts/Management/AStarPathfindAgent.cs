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
        private List<SerializableVector2Int> pathlist;
        
        public AStarPathfindAgent(MapGrid g, IMapObject o)
        {
            grid = g;
            targetObject = o;
        }

        public bool SetDestination(SerializableVector2Int dist)
        {
            pathlist.Clear();
            
            var info = grid.GetNearWorldObjectNode(targetObject.Position);
            if (info != null)
            {
                pathlist = FindPath(info.grid,dist);
                return pathlist.Count > 0;
            }                
            return false;
        }

        public bool SetDestination(Vector3 dist)
        {
            var info = grid.GetNearWorldObjectNode(dist);
            if (info != null)
            {
                return SetDestination(info.grid);
            }

            return false;
        }
        

        private List<SerializableVector2Int> FindPath(SerializableVector2Int start, SerializableVector2Int goal)
        {
            SearchNodeData startNode =new SearchNodeData(grid.GetNodeAt(start));
            SearchNodeData goalNode = new SearchNodeData(grid.GetNodeAt(goal));

            List<SearchNodeData> openSet = new List<SearchNodeData>();
            HashSet<SearchNodeData> closedSet = new HashSet<SearchNodeData>();

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
                closedSet.Add(current);

                foreach (SearchNodeData neighbor in GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor)) continue;

                    float newCostToNeighbor = current.GCost + GetMovementCost(current.Node, neighbor.Node);
                    if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor.Node, goalNode.Node);
                        neighbor.Parent = current;

                        if (!openSet.Contains(neighbor))
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

        private List<SerializableVector2Int> RetracePath(SearchNodeData startNode, SearchNodeData endNode)
        {
            List<SerializableVector2Int> path = new List<SerializableVector2Int>();
            SearchNodeData currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.Node.grid);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }        
    }
}