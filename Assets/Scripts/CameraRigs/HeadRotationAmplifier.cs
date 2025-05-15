using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadRotationAmplifier : MonoBehaviour
{
    public Transform virtualHead; // 仮想空間内の頭部を表すオブジェクト
    public float amplificationFactor = 1.5f; // 回転増幅倍率

    private Vector3 previousRealHeadRotation;
    private List<XRNodeState> nodeStates = new List<XRNodeState>();

    void Start()
    {
        // 最初に現実の頭の回転を取得して初期化
        previousRealHeadRotation = GetRealHeadRotation();
    }

    void Update()
    {
        // 現実の頭の回転を取得
        Vector3 currentRealHeadRotation = GetRealHeadRotation();

        // 各軸の回転変化量を計算
        float deltaXRotation = Mathf.DeltaAngle(previousRealHeadRotation.x, currentRealHeadRotation.x);
        float deltaYRotation = Mathf.DeltaAngle(previousRealHeadRotation.y, currentRealHeadRotation.y);
        float deltaZRotation = Mathf.DeltaAngle(previousRealHeadRotation.z, currentRealHeadRotation.z);

        // 回転の変化量を増幅
        float amplifiedXRotation = deltaXRotation * amplificationFactor;
        float amplifiedYRotation = deltaYRotation * amplificationFactor;
        float amplifiedZRotation = deltaZRotation * amplificationFactor;

        // 仮想頭部オブジェクトの回転を更新
        virtualHead.Rotate(amplifiedXRotation, amplifiedYRotation, amplifiedZRotation, Space.World);

        // 次のフレーム用に回転を保存
        previousRealHeadRotation = currentRealHeadRotation;
    }

    // HMDの回転を取得するメソッド
    private Vector3 GetRealHeadRotation()
    {
        InputTracking.GetNodeStates(nodeStates);
        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == XRNode.Head)
            {
                Quaternion headRotation;
                if (nodeState.TryGetRotation(out headRotation))
                {
                    return headRotation.eulerAngles;
                }
            }
        }
        return Vector3.zero;
    }
}
