using System;
using ANode.MapObjects;
using Management;
using UnityEngine;

namespace ANode
{
    public class AStarAgent : MonoBehaviour
    {
        public float speed = 0.5f;
        public float rotatespeed = 0.5f;
        public IMapObject ObjectInfo;

        public GameObject testObject;
        
        private bool _isMove = false;
        private AStarPathfindAgent _agent;

        public virtual void Start()
        {
            _agent = new AStarPathfindAgent(MapGrid.worldGrid, ObjectInfo);
        }

        public bool SetDestination(Vector3 dist)
        {
            _isMove = _agent.SetDestination(dist);
            return _isMove;
        }

        public virtual void Update()
        {
            if (_isMove == true)
            {
                _isMove = _agent.UpdatePosition(this, rotatespeed, speed);
            }
        }
    }
}