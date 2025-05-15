using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OVRCameraRigTransformInfo : MonoBehaviour
{
    [SerializeField]
    private Text text;  //情報を乗せるText

    [SerializeField]
    private GameObject[] go;    //知りたいObjectをアタッチ

    
    private void Update()
    {
        text.text = "";    
        foreach(var obj in go)
        {
            GetGameObjectPositon(obj);
        }

    }

    private void GetGameObjectPositon(GameObject obj)
    {
        /*
        float worldPosX = obj.transform.position.x;
        float worldPosY = obj.transform.position.y;
        float worldPosZ = obj.transform.position.z;
        */

        float localPosX = obj.transform.localPosition.x;
        float localPosY = obj.transform.localPosition.y;
        float localPosZ = obj.transform.localPosition.z;

        /*
        float worldRotX = obj.transform.rotation.x;
        float worldRotY = obj.transform.rotation.y;
        float worldRotZ = obj.transform.rotation.z;
        */

        float localRotX = obj.transform.localRotation.x;
        float localRotY = obj.transform.localRotation.y;
        float localRotZ = obj.transform.localRotation.z;
 
        text.text += obj.name + " Pos " + " X: " + localPosX.ToString("0.000") + " Y: " + localPosY.ToString("0.000") + " Z: " + localPosZ.ToString("0.000")
                        + " Rotation " + " X: " + localRotX.ToString("0.000") + " Y: " + localRotY.ToString("0.000") + " Z: " + localRotZ.ToString("0.000") + "\n";
        Debug.Log("---zahyo---: "+obj.name + " Pos " + " X: " + localPosX.ToString("0.000") + " Y: " + localPosY.ToString("0.000") + " Z: " + localPosZ.ToString("0.000")+ " Rotation " + " X: " + localRotX.ToString("0.000") + " Y: " + localRotY.ToString("0.000") + " Z: " + localRotZ.ToString("0.000") + "\n");
    }
}

