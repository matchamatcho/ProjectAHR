using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRedirection : MonoBehaviour
{
    public Transform realHead;  // 現実世界での頭のTransform (Rrh)
    public Transform virtualHead;  // 仮想世界での頭のTransform (Rvh)
    public float alpha = 1.0f;  // リマッピングパラメータ α

    private Vector3 prevRealHeadRotation;  // 前フレームの現実の頭の回転 (R'rh)
    private Vector3 prevVirtualHeadRotation;  // 前フレームの仮想頭の回転 (R'vh)

    void Start()
    {
        // 初期化: 現実と仮想の頭の回転を初期化
        prevRealHeadRotation = realHead.rotation.eulerAngles;
        prevVirtualHeadRotation = virtualHead.rotation.eulerAngles;
    }

    void Update()
    {
        // 現実の頭の現在の回転 (Euler角) を取得
        Vector3 currentRealHeadRotation = realHead.rotation.eulerAngles;

        // 現実の頭のY軸回転の変化量を計算 (Mathf.DeltaAngleで補正)
        float deltaYRotation = Mathf.DeltaAngle(prevRealHeadRotation.y, currentRealHeadRotation.y);

        // リダイレクション: Y軸回転の変化量にαを掛ける
        float redirectedYRotation = prevVirtualHeadRotation.y + (deltaYRotation * alpha);

        // 仮想頭の新しい回転を計算 (X, Zは変更せず、Yのみリダイレクト)
        Vector3 newVirtualHeadRotation = new Vector3(
            prevVirtualHeadRotation.x,
            redirectedYRotation,
            prevVirtualHeadRotation.z
        );

        // 仮想頭の回転を更新
        virtualHead.rotation = Quaternion.Euler(newVirtualHeadRotation);

        // デバッグログで角度情報を表示
        Debug.Log("現実の頭のY軸回転 (currentRealHeadRotation.y): " + currentRealHeadRotation.y);
        Debug.Log("Y軸回転の変化量 (deltaYRotation): " + deltaYRotation);
        Debug.Log("リダイレクトされた仮想頭のY軸回転 (redirectedYRotation): " + redirectedYRotation);


        // 次のフレーム用に前回の回転を保存
        prevRealHeadRotation = currentRealHeadRotation;
        prevVirtualHeadRotation = newVirtualHeadRotation;
    }
}
