using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{
    private  static Transform anchor;
    public List<Transform> anchors;
    private float maxDistance = 100;
    private LineRenderer lineRenderer;
    public PointingTaskFunction pointingtaskfunction;
    private bool RayValidBool = true;// Rayの有効/無効状態
    private float touchRadius = 0.05f; // タッチ検出用の半径
    private static int isRightHand = 1;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        anchor=anchors[isRightHand];
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.RawButton.Y)&&OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)){
            Debug.LogWarning("---dbg2---reloaaaaaaaad");
            pointingtaskfunction.NextMethodFunction();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X)){
            pointingtaskfunction.PracticeFinishFunction();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y)){
            pointingtaskfunction.CenterDefineFunction();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            if(pointingtaskfunction.GetisIdselected())return;
            pointingtaskfunction.ChangeRightLeftFunction();
            isRightHand = (isRightHand + 1) % 2;
            anchor = anchors[isRightHand];
            Debug.Log("---dbg2---isRightHand: " + isRightHand);
        }
        RayValidBool = pointingtaskfunction.GetRayValidBoolFunction();
        if (OVRInput.Get(OVRInput.RawButton.B))
        {
            Debug.LogWarning("RAYACTIVE");
            RayValidBool = true;
        }
        bool isInput = false;
        if (isRightHand == 1 && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            isInput = true;
            Debug.Log("---dbg2---isInput: " + isInput);
        }
        if (isRightHand == 0 && OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            isInput = true;

            Debug.Log("---dbg2---isInput: " + isInput);
        }

        if (RayValidBool)
        {
            HandleRay(isInput); // Rayによる操作処理
        }
        else
        {
            HandleTouch(isInput); // タッチによる操作処理
        }
    }
    // Rayによる操作処理
    private void HandleRay(bool isInput)
    {
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);

        // LineRendererの始点を設定
        lineRenderer.SetPosition(0, ray.origin);

        // Raycastでヒット判定
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point); // ヒット位置を終点に設定

            GameObject target = hit.collider.gameObject;
            pointingtaskfunction.ObjectHoverringFunction(target);

            // トリガーボタンで選択
            if (isInput)
            {
                pointingtaskfunction.ObjectSelected(target);
            }
            // Debug.Log("---everyray---object: "+target);
        }
        else
        {
            pointingtaskfunction.ObjectNotHoverringFunction();
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * maxDistance)); // 最大距離まで描画

            // トリガーボタンが押された場合
            if (isInput)
            {
                pointingtaskfunction.NoneSelected();
            }
        }
    }

    // タッチによる操作処理
    private void HandleTouch(bool isInput)
    {
        // LineRendererをリセット（描画を無効化）
        lineRenderer.SetPosition(0, anchor.position);
        lineRenderer.SetPosition(1, anchor.position);
        bool isSelected = false;

        // OverlapSphereでタッチ検出
        Collider[] hitColliders = Physics.OverlapSphere(anchor.position, touchRadius);
        // Debug.Log("buttainokazu:"+hitColliders.Length);

        foreach (var hitCollider in hitColliders)
        {
            GameObject target = hitCollider.gameObject;
            isSelected = true;
            pointingtaskfunction.ObjectHoverringFunction(target);

            // RINDEXボタンで選択
            if (isInput)
            {
                pointingtaskfunction.ObjectSelected(target);
                // Debug.Log($"Touched object: {target.tag}");
            }
        }
        if (!isSelected)
        {
            pointingtaskfunction.ObjectNotHoverringFunction();
            if (isInput)
            {
                pointingtaskfunction.NoneSelected();
                // Debug.Log($"Touched object: {target.tag}");
            }

        }
    }


}
