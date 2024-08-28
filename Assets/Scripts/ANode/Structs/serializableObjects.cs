using UnityEngine;

namespace ANode
{
    [System.Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
        
        // Vector3 -> SerializableVector3 암시적 변환
        public static implicit operator SerializableVector3(Vector3 v)
        {
            return new SerializableVector3(v);
        }

        // SerializableVector3 -> Vector3 암시적 변환
        public static implicit operator Vector3(SerializableVector3 sv)
        {
            return new Vector3(sv.x, sv.y, sv.z);
        }        
    }
    
    [System.Serializable]
    public struct SerializableVector2Int
    {
        public int x;
        public int y;

        public SerializableVector2Int(int rX, int rY)
        {
            x = rX;
            y = rY;
        }

        public SerializableVector2Int(Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }

        #region 비교 구분 
        public override bool Equals(object obj)
        {
            if (obj is SerializableVector2Int sobj)
            {
                return sobj.x == x && sobj.y == y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SerializableVector2Int p1, SerializableVector2Int p2)
        {
            if (ReferenceEquals(p1, null))
                return ReferenceEquals(p2, null);

            return p1.Equals(p2);
        }

        public static bool operator !=(SerializableVector2Int p1, SerializableVector2Int p2)
        {
            return !(p1 == p2);
        }        
        #endregion
        
        public Vector2Int Vector2Int()
        {
            return new Vector2Int(x, y);
        }
    }    
}