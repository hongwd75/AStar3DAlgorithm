using ANode.MapObjects;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace ANode
{
    [CustomEditor(typeof(AStarNodeMaker))]
    public class CustomInspectorAStarNodeMaker : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // 기본 인스펙터 그리기
            //DrawDefaultInspector();

            // 현재 타겟 가져오기
            AStarNodeMaker myScript = (AStarNodeMaker)target;

            if (GUILayout.Button("생성"))
            {
                myScript.OnCreate();
            }            
            
            if (GUILayout.Button("저장"))
            {
                myScript.OnSave();
            }
            
            if (GUILayout.Button("로드"))
            {
                myScript.OnLoad();
            }            
            
        }
    }
}