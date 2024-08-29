using System;
using Management;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ANode.MapObjects
{
    public class CharacterMapObject : AStarAgent,IMapObject
    {
        public bool CanJump => true;
        public bool CanMove => true;
        public bool CanDestroy => true;
        public bool IsMoving { get; }
        
        public virtual int ID => rn;
        public IMapObject.ObjectType Type => IMapObject.ObjectType.Character;
        public TileObjectInfo TileObjectInfo { get; } // 객체 정보
        public int rn = 0;
        public float Angle => 1f;
        public virtual Vector3 Position => gameObject.transform.position;
        
        public override void Start()
        {
            rn = Random.Range(1, 1000);
            ObjectInfo = this;
            base.Start();
        }

        public void OnEnable()
        {
            MapGrid.AddIMapObject(this);
        }

        public void OnDisable()
        {
            MapGrid.RemoveIMapObject(this);
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyUp(KeyCode.A))
            {
                EditorMove();
            }
        }

        public void EditorMove()
        {
            SetDestination(testObject.transform.position);
        }        
    }
}