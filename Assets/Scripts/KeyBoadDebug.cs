using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;



public class KeyBoadDebug : MonoBehaviour
{

    public PointingTaskFunction pointingtaskfunction;
    private bool Keyboaddebug = true;
    private bool OvrDebug = true;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboaddebug)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // pointingtaskfunction.ReloadSceneFunction();
                pointingtaskfunction.SkipFunction();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                pointingtaskfunction.PracticeFinishFunction();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                pointingtaskfunction.CenterDefineFunction();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                pointingtaskfunction.ReloadSceneFunction();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                pointingtaskfunction.TargetSelectedFunction();
            }
            if (Input.GetKey(KeyCode.U))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    pointingtaskfunction.SetIdFunction(7);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    pointingtaskfunction.SetIdFunction(8);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    pointingtaskfunction.SetIdFunction(9);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    pointingtaskfunction.SetIdFunction(10);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    pointingtaskfunction.SetIdFunction(11);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    pointingtaskfunction.SetIdFunction(12);
                }
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    pointingtaskfunction.SetIdFunction(13);
                }
            }
            if (Input.GetKey(KeyCode.I))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    pointingtaskfunction.SetIdFunction(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    pointingtaskfunction.SetIdFunction(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    pointingtaskfunction.SetIdFunction(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    pointingtaskfunction.SetIdFunction(3);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    pointingtaskfunction.SetIdFunction(4);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    pointingtaskfunction.SetIdFunction(5);
                }
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    pointingtaskfunction.SetIdFunction(6);
                }
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                pointingtaskfunction.OriginSelectedFunction();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                List<int> list = new List<int> { 1, 2, 3, 4, 5 };
                // Listをランダムに並べ替える
                System.Random random = new System.Random();
                List<int> shuffledList = list.OrderBy(x => random.Next()).ToList();
                Debug.Log("Randomlist:" + string.Join(",", shuffledList));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                // pointingtaskfunction.ObjectSelected(pointingtaskfunction.TaskSwicher);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                pointingtaskfunction.DebugFunction();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                pointingtaskfunction.TaskFinish();
            }
            if (Input.GetKey(KeyCode.G))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    pointingtaskfunction.GenerateTargetsFunction(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    pointingtaskfunction.GenerateTargetsFunction(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    pointingtaskfunction.GenerateTargetsFunction(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    pointingtaskfunction.GenerateTargetsFunction(3);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    pointingtaskfunction.GenerateTargetsFunction(4);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    pointingtaskfunction.GenerateTargetsFunction(5);
                }
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                pointingtaskfunction.SettingsOnOffFunction();
            }
            if (Input.GetKey(KeyCode.K))
            {
                pointingtaskfunction.ObjectHoverringFunction(pointingtaskfunction.GetNextTargetFunction());
            }
            if (Input.GetKey(KeyCode.L))
            {
                pointingtaskfunction.ObjectHoverringFunction(pointingtaskfunction.GetOriginObjectFunction());
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                pointingtaskfunction.NextMethodFunction();
            }
            if (Input.GetKey(KeyCode.M))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    pointingtaskfunction.ChangeMethodFunction(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    pointingtaskfunction.ChangeMethodFunction(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    pointingtaskfunction.ChangeMethodFunction(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    pointingtaskfunction.ChangeMethodFunction(3);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    pointingtaskfunction.ChangeMethodFunction(4);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    pointingtaskfunction.ChangeMethodFunction(5);
                }
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    pointingtaskfunction.ChangeMethodFunction(6);
                }
            }
        }
        if (OvrDebug)
        {


        }
    }
    public void SceneSwitchFunction(int i)
    {
        SceneManager.LoadScene(i); // 例: ビルド設定で2番目のシーンをロード
    }
}
