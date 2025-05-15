using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
 
   // 生成する位置とサイズのリスト
    [System.Serializable]
    public class SphereData
    {
        public Vector3 position; // 位置
        public float size;       // 大きさ
    }

    public List<SphereData> sphereDataList; // 位置とサイズのリスト
    public GameObject spherePrefab;         // 生成する球のプレハブ

    
}
