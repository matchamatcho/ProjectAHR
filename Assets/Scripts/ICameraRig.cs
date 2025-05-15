// ICameraRig.cs
using System.Collections.Generic;
using UnityEngine;
public interface ICameraRig
{
    List<float> GetCameraHandsRotation(); 
    List<Vector3> GetEveryFrameData(); 
    float GetAlphaFunction();
    Vector3 GetRealHeadPosition();
    // 共通で利用したいメソッドをここに定義
    // 他にも必要なメソッドがあれば追加
}
