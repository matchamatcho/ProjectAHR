// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR;

// public class HeadRedirection2 : MonoBehaviour
// {
//     // public Transform realHead;  // 現実世界での頭のTransform (Rrh)
//     public Transform virtualHead;  // 仮想世界での頭のTransform (Rvh)
//     public float alpha = 1.0f;  // リマッピングパラメータ α

//     private Vector3 prevRealHeadRotation;  // 前フレームの現実の頭の回転 (R'rh)
//     private Vector3 prevVirtualHeadRotation;  // 前フレームの仮想頭の回転 (R'vh)
//     //HMDの回転座標格納用（クォータニオン）
//     private Quaternion HMDRotationQ;
//     //HMDの回転座標格納用（オイラー角）
//     private Vector3 HMDRotation;
    

//     void Start()
//     {
//         // 初期化: 現実と仮想の頭の回転を初期化
//         // prevRealHeadRotation = realHead.rotation.eulerAngles;
//         prevVirtualHeadRotation = virtualHead.rotation.eulerAngles;
//         //回転座標をクォータニオンで値を受け取る
//         HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);
//         //取得した値をクォータニオン → オイラー角に変換
//         HMDRotation = HMDRotationQ.eulerAngles;
//         prevRealHeadRotation = HMDRotation;
//     }

//     void Update()
//     {
//         Quaternion currentHMDRotationQ =InputTracking.GetLocalRotation(XRNode.Head);
//         // 現実の頭の現在の回転 (Euler角) を取得
//         Vector3 currentRealHeadRotation = currentHMDRotationQ.eulerAngles;

//         // 現実の頭のY軸回転の変化量を計算 (Mathf.DeltaAngleで補正)
//         float deltaYRotation = Mathf.DeltaAngle(prevRealHeadRotation.y, currentRealHeadRotation.y);

//         // リダイレクション: Y軸回転の変化量にαを掛ける
//         float redirectedYRotation = virtualHead.rotation.eulerAngles.y + (deltaYRotation * alpha);

//         // 仮想頭の新しい回転を計算 (X, Zは変更せず、Yのみリダイレクト)
//         Vector3 newVirtualHeadRotation = new Vector3(
//             virtualHead.rotation.eulerAngles.x,
//             redirectedYRotation,
//             virtualHead.rotation.eulerAngles.z
//         );

//         // 仮想頭の回転を更新
//         virtualHead.rotation = Quaternion.Euler(newVirtualHeadRotation);

//         // デバッグログで角度情報を表示
//         Debug.Log("現実の頭のY軸回転 (currentRealHeadRotation.y): " + currentRealHeadRotation.y);
//         Debug.Log("Y軸回転の変化量 (deltaYRotation): " + deltaYRotation);
//         Debug.Log("リダイレクトされた仮想頭のY軸回転 (redirectedYRotation): " + redirectedYRotation);

//         // 次のフレーム用に前回の回転を保存
//         prevRealHeadRotation = currentRealHeadRotation;
//     }

// }
