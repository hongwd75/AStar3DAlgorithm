using UnityEngine;

namespace ANode.MapObjects
{
    public class CharacterMapObject : AStarAgent,IMapObject
    {
        public bool CanJump => true;
        public bool CanMove => true;
        public bool CanDestroy => true;
        public bool IsMoving { get; }

        public virtual int ID => 1;
        public IMapObject.ObjectType Type => IMapObject.ObjectType.Character;
        public TileObjectInfo TileObjectInfo { get; } // 객체 정보

        public float Angle => 1f;
        public virtual Vector3 Position => gameObject.transform.position;
        
        public override void Start()
        {
            ObjectInfo = this;
            base.Start();
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