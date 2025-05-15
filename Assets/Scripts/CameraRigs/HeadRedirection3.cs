using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadRedirection3 : MonoBehaviour
{
    public Transform virtualHead;
    public Transform objectToRotate;  // 回転を加えるオブジェクト
    public float alpha = 1.0f;  // リマッピングパラメータ α

    private Vector3 prevRealHeadRotation;  // 前フレームの現実の頭の回転 (R'rh)
    private List<XRNodeState> nodeStates = new List<XRNodeState>();

    private int frameCounter = 0;  // フレームカウンター

    void Start()
    {
        // 初期化: 現実の頭の回転を取得
        prevRealHeadRotation = UpdateRealHeadRotation();
    }

    void Update()
    {
        // 現実の頭の回転を更新
        Vector3 currentRealHeadRotation = UpdateRealHeadRotation();

        // 現実の頭のY軸回転の変化量を計算
        float deltaYRotation = Mathf.DeltaAngle(prevRealHeadRotation.y, currentRealHeadRotation.y);

        // 回転変化量にαを掛ける
        float adjustedYRotation = deltaYRotation * alpha;

        // オブジェクトの回転を更新 (Y軸のみ)
        objectToRotate.rotation = Quaternion.Euler(0, adjustedYRotation, 0) * objectToRotate.rotation;

        // フレームカウンターをインクリメント
        frameCounter++;

        // 60フレームに1回デバッグログを出力
        if (frameCounter >= 60)
        {
            Debug.Log("現実の頭のY軸回転 (currentRealHeadRotation.y): " + currentRealHeadRotation.y);
            Debug.Log("Y軸回転の変化量 (deltaYRotation): " + deltaYRotation);
            Debug.Log("調整されたY軸回転 (adjustedYRotation): " + virtualHead.rotation.eulerAngles.y);
            Debug.Log("物体の回転: "+objectToRotate.rotation.eulerAngles.y);

            frameCounter = 0;  // カウンターをリセット
        }

        // 次のフレーム用に前回の回転を保存
        prevRealHeadRotation = currentRealHeadRotation;
    }

    // 現実の頭の回転を更新するメソッド
    private Vector3 UpdateRealHeadRotation()
    {
        // HMDの回転を取得
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
        return Vector3.zero;  // 頭の回転が取得できなかった場合は0を返す
    }
}
