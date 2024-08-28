using System;
using UnityEngine;

namespace Management
{
    public class StageMapManager : MonoBehaviour
    {
        private MapGrid map;

        private void Start()
        {
            map = new MapGrid();
            map.Load("stage.bytes");
        }
    }
}