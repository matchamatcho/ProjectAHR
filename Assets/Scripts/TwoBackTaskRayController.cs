using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoBackTaskRayController : MonoBehaviour
{
    public Transform anchor;
    private float maxDistance = 100;
    private LineRenderer lineRenderer;

    public TwoBackTaskFunction twobacktaskfunction;
    private bool Keyboaddebug = true;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
   
    
    }

    void Update()
    {
        if (Keyboaddebug)
        {
            // if (Input.GetKeyDown(KeyCode.Alpha0))
            // {
            //     twobacktaskfunction.ObjectSelected(twobacktaskfunction.targets[0]);
            // }
            // if (Input.GetKeyDown(KeyCode.Alpha1))
            // {
            //     twobacktaskfunction.ObjectSelected(twobacktaskfunction.targets[1]);
            // }
            // if (Input.GetKeyDown(KeyCode.Alpha2))
            // {
            //     twobacktaskfunction.ObjectSelected(twobacktaskfunction.targets[2]);
            // }
            // if (Input.GetKeyDown(KeyCode.Alpha3))
            // {
            //     twobacktaskfunction.ObjectSelected(twobacktaskfunction.targets[3]);
            // }
            // if (Input.GetKeyDown(KeyCode.Alpha4))
            // {
            //     twobacktaskfunction.ObjectSelected(twobacktaskfunction.targets[4]);
            // }
            // if (Input.GetKeyDown(KeyCode.Alpha5))
            // {
            //     twobacktaskfunction.ObjectSelected(twobacktaskfunction.targets[5]);
            // }
            if (Input.GetKeyDown(KeyCode.D))
            {
                twobacktaskfunction.DebugFunction();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log("clicked:O");
                twobacktaskfunction.ObjectSelected(twobacktaskfunction.Origin);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                twobacktaskfunction.ObjectSelected(twobacktaskfunction.NextTarget);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                twobacktaskfunction.ObjectSelected(twobacktaskfunction.TaskSwicher);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                twobacktaskfunction.ReloadSceneFunction();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                twobacktaskfunction.SettingsOnOffFunction();
            }
            if (Input.GetKey(KeyCode.M)&Input.GetKeyDown(KeyCode.Alpha0))
            {
                twobacktaskfunction.ChangeMethodFunction("0");
            }
            if (Input.GetKey(KeyCode.M)&Input.GetKeyDown(KeyCode.Alpha1))
            {
                twobacktaskfunction.ChangeMethodFunction("1");
            }
            
        
   
        }
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);

        lineRenderer.SetPosition(0, ray.origin);//描画する線の長さを設定
        /////////
        
        if(twobacktaskfunction.timeout){
            twobacktaskfunction.FinishTask();
        }


        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);

            GameObject target = hit.collider.gameObject;

            // 右コントローラのtorrigerを押した場合
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                // pointingtaskfunction.ObjectSelected(target);
                twobacktaskfunction.ObjectSelected(target);

            }
        }
        else
        {
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * maxDistance));//描画する線の長さを設定
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                // pointingtaskfunction.NoneSelected();
            }
        }
    }
}
