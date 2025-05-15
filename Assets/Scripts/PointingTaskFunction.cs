using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using TMPro;
using System.Linq;

public class PointingTaskFunction : MonoBehaviour
{
    private List<GameObject> objects; // 順序を固定するためのオブジェクトリスト
    private int currentIndex = -1; // 現在のアクティブなオブジェクトのインデックス
    private GameObject NextTarget = null;
    private bool istargetselected = false;
    public GameObject messageText; // 表示するテキストオブジェクト
    [SerializeField]
    private Text text;  //情報を乗せるText
    private bool isStarted = false;
    private float elapsedTime = 0f; // 経過時間
    private bool isEnded = false;
    private List<float> trialTimes = new List<float>(); // 各試行の時間を保存するリスト
    private float trialStartTime = 0f; // 各試行の開始時間
    public GameObject onoffswitch;
    // 生成する位置とサイズのリスト
    [System.Serializable]
    public class SphereData
    {
        public Vector3 position; // 位置
        public float size;       // 大きさ
        // コンストラクタ
        public SphereData(Vector3 position, float size)
        {
            this.position = position;
            this.size = size;
        }
    }
    private List<float> SphereTheta = new List<float>();
    private static Vector3 circleCenter = new Vector3(0, 1.5f, 0); // 円の中心
    private List<Tuple<float, float>> radiusTupleList = new List<Tuple<float, float>>()
    {
        // new Tuple<float, float>(0.3f, 0.6f),
        // new Tuple<float, float>(0.3f, 0.6f),
        // new Tuple<float, float>(2.0f, 4.0f),
        // new Tuple<float, float>(2.0f, 4.0f),
        // new Tuple<float, float>(0.3f, 0.6f),
        // new Tuple<float, float>(2.0f, 4.0f)
        new Tuple<float, float>(0.45f, 0.45f),
        new Tuple<float, float>(0.45f, 0.45f),
        new Tuple<float, float>(3.0f, 3.0f),
        new Tuple<float, float>(3.0f, 3.0f),
        new Tuple<float, float>(0.45f, 0.45f),
        new Tuple<float, float>(3.0f, 3.0f)
    };
    private List<float> radiusList = new List<float>()
    {
        0.35f,0.35f,3.0f,3.0f
    };
    private int numberOfPoints = 12; // 円周上の点の数、分割する数
    private int numberOfTargets = 10;
    private int MaxPointingNumInt;
    private List<float> pointSizeList = new List<float>(){
        // 0.05f,0.02f,0.25f,0.1f
        // (0.096f*0.45f),(0.032f*0.45f),(0.096f*3.0f),(0.032f*3.0f),(0.128f*0.45f),(0.128f*3.0f)
            0.128f,0.096f,0.064f,0.032f
    };
    private List<SphereData> sphereDataList = new List<SphereData>(); // 位置とサイズのリスト
    private SphereData originData;
    public GameObject spherePrefab;         // 生成する球のプレハブ
    public GameObject originOrefab;
    private bool isTargetGenerated = false;
    private List<string> whichTargetTextList = new List<string>(){
        "Touch-Practice","Touch-Task","Ray-Practice","Ray-Task"
    };
    private string whichTargetText = "";
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;
    // private List<List<int>> rondomPermutationList = new List<List<int>>(){
    //     new List<int>{7, 5, 1, 3, 2, 8, 4, 6, 9, 0},
    //     new List<int>{6, 7, 3, 4, 2, 8, 0, 9, 1, 5},
    //     new List<int>{3, 2, 4, 7, 0, 8, 1, 9, 5, 6},
    //     new List<int>{7, 2, 4, 3, 1, 5, 6, 8, 9, 0},
    //     new List<int>{8, 9, 0, 6, 3, 4, 7, 1, 2, 5},
    //     new List<int>{1, 0, 7, 2, 4, 9, 8, 5, 3, 6}
    // };
    private List<int> randomPermutation = new List<int>();
    // private float halfradius;
    private float nextScene = 0f;

    // 対象となるGameObjectを変数に保持しているとする
    public GameObject cameraObject;
    // 有効な"Camera"で始まるスクリプトを格納するリスト
    private MonoBehaviour activeCameraScripts = null;
    private List<float> startRotations = new List<float>() { 0, 0, 0, 0, 0 };
    private List<float> endRotations = new List<float>() { 0, 0, 0, 0, 0 };
    public TMP_Text tmpText;
    private bool RayValidBool = true;
    private bool isHoveringOrigin = false;
    private bool isHoveringTarget = false;
    private GameObject OriginObject = null;
    private int errorInputNum = 0;
    private string startTime = "";
    private static int id = -1;
    private List<float> prevrotation = new List<float>();
    private List<float> nowrotation = new List<float>();
    private float tmpheadrotationsum = 0;
    private float tmprighthandrotationsum = 0;
    private float tmprighthandpositionsum = 0;
    private float tmplefthandrotationsum = 0;
    private float tmplefthandpositionsum = 0;
    private string tmpnowdata = "";
    private string tmpprevdata = "";
    private static int nextMethodIndex = 0;
    private List<List<int>> NextMethodList = new List<List<int>>(){
        new List<int>{0, 1, 6, 2, 5, 3, 4},
        new List<int>{1, 2, 0, 3, 6, 4, 5},
        new List<int>{2, 3, 1, 4, 0, 5, 6},
        new List<int>{3, 4, 2, 5, 1, 6, 0},
        new List<int>{4, 5, 3, 6, 2, 0, 1},
        new List<int>{5, 6, 4, 0, 3, 1, 2},
        new List<int>{6, 0, 5, 1, 4, 2, 3}
    };
    private List<int> NextTaskList;
    private static int nextTaskIndex = 0;
    private static int touchray = 0;
    private bool gotoNextMethod = false;
    private static bool isExperimentFinished = false;
    public Transform HeadRotation;
    private Vector3 tmpheadrotationtmp = new Vector3(0, 0, 0);
    private float tmptimetmp = 1.000000f;
    public GameObject idsObject;
    private static bool isIdselected = false;
    private List<int> eachSizeError = new List<int>() { 0, 0, 0 };//large,middle,small
    private List<List<float>> eachSizeTrialTimes = new List<List<float>>(){
        new List<float>(),new List<float>(),new List<float>()
    };
    private List<float> eachSizeTrialTimesSum = new List<float>() { 0, 0, 0 };
    private static int isRightHand = 1;
    private static bool isCenterDefined = false;

    void Start()
    {
        // Debug.Log("---dbgtmp---Scene: " + SceneManager.GetActiveScene().name);
        // ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        // Vector3 realHeadPosition=new Vector3();
        // if (cameraRig != null)
        // {
        //     realHeadPosition=cameraRig.GetRealHeadPosition();
        // }
        // circleCenter.y=realHeadPosition.y;
        // Debug.Log("realHeadPosition.y: "+realHeadPosition.y);



        idsObject.SetActive(!isIdselected);
        if (HeadRotation != null) HeadRotation.localPosition = tmpheadrotationtmp;
        // if (id < 7)
        // {
        if (touchray == 0) NextTaskList = new List<int>() { 0, 1 };
        else NextTaskList = new List<int>() { 2, 3 };//0=touchpractice,1=touchtask,2=raypractice,3=touchpractice
        // }
        // else
        // {
        //     if (touchray == 0) NextTaskList = new List<int>() { 4, 1, 0 };
        //     else NextTaskList = new List<int>() { 5, 3, 2 };
        // }
        MaxPointingNumInt = 2 * numberOfTargets;
        if (messageText != null)
        {
            messageText.SetActive(false);
        }

        objects = new List<GameObject>();
        ///////////////////

        ////////////////////////
        // 子オブジェクトの名前を指定して取得
        Transform child = transform.Find("TargetRay2");
        if (child != null)
        {
            lineRenderer2 = child.GetComponent<LineRenderer>();
            if (lineRenderer2 != null)
            {

                // LineRendererの設定を変更する例
                lineRenderer2.SetPosition(0, new Vector3(5, 0, 1));
                lineRenderer2.SetPosition(1, new Vector3(5, 0, 1));
                // 太さの設定
                lineRenderer2.startWidth = 0.005f;
                lineRenderer2.endWidth = 0.005f;

                // 幅カーブを設定しない（均一な太さ）
                // lineRenderer2.widthCurve = AnimationCurve.Linear(0, 1, 1, 1);

                // 材料と色
                // lineRenderer2.material = new Material(Shader.Find("Unlit/Color"));
                // lineRenderer2.material.color = Color.white;

            }

        }
        Transform child2 = transform.Find("TargetRay3");
        if (child != null)
        {
            lineRenderer3 = child2.GetComponent<LineRenderer>();
            if (lineRenderer3 != null)
            {

                // LineRendererの設定を変更する例
                lineRenderer3.SetPosition(0, new Vector3(5, 0, 1));
                lineRenderer3.SetPosition(1, new Vector3(5, 0, 1));
                // 太さの設定
                lineRenderer3.startWidth = 0.005f;
                lineRenderer3.endWidth = 0.005f;

                // 幅カーブを設定しない（均一な太さ）
                // lineRenderer3.widthCurve = AnimationCurve.Linear(0, 1, 1, 1);

                // 材料と色
                // lineRenderer3.material = new Material(Shader.Find("Unlit/Color"));
                // lineRenderer3.material.color = Color.blue;

            }

        }
        // アタッチされている全てのMonoBehaviourスクリプトを取得
        MonoBehaviour[] allScripts = cameraObject.GetComponents<MonoBehaviour>();

        // 有効なスクリプトを格納するリスト
        List<MonoBehaviour> activeScripts = new List<MonoBehaviour>();
        foreach (var script in allScripts)
        {
            // scriptが有効かつ、GameObject自体もアクティブ
            // かつスクリプト名が"Camera"で始まる場合のみ追加
            if (script != null && script.enabled && script.gameObject.activeInHierarchy)
            {
                if (script.GetType().Name.StartsWith("Camera"))
                {
                    activeScripts.Add(script);
                    activeCameraScripts = script;
                }
            }
        }
        Debug.Log("---gbg2---activeScripts" + string.Join(", ", activeScripts));
        Debug.Log("---dbg2---activeCameraScripts" + activeCameraScripts);
        // 現在のシーンを取得
        Scene currentScene = SceneManager.GetActiveScene();

        // 現在のシーンのビルドインデックスを取得
        int buildIndex = currentScene.buildIndex;
        tmpText.text = "";
        tmpText.text += "///INFORMATION///\n";
        tmpText.text += "Scene:" + buildIndex + "\n";
        tmpText.text += "ID:" + id + "\n";
        startTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        float alp = -1;
        ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        if (cameraRig != null)
        {
            alp = cameraRig.GetAlphaFunction();
            // Debug.Log("notnullllll");
        }
        else
        {
            // Debug.Log("nullllll");
        }
        Debug.Log("---dbg2---alpha: " + alp);
        if (isIdselected) GenerateTargetsFunction(NextTaskList[nextTaskIndex]);
    }
    void Update()
    {
        if (gotoNextMethod)
        {
            text.text = "FINISH";
            if (messageText != null)
            {
                messageText.SetActive(true);
            }
            return;
        }
        tmpText.text = "";
        // tmpText.text += "///INFORMATION///\n";
        tmpText.text += "ID:" + id + "\n";
        tmpText.text += "Method:" + SceneManager.GetActiveScene().name + "\n";
        tmpText.text += "Target:" + whichTargetText + "\n";
        if (isCenterDefined) tmpText.text += "center:OK" + "\n";
        // tmpText.text = "";
        if (isStarted && !isEnded)
        {
            elapsedTime += Time.deltaTime; // 経過時間を更新
        }
        if (isEnded)
        {
            nextScene += Time.deltaTime;
        }
        if (nextScene >= tmptimetmp)
        {
            if (gotoNextMethod) NextMethodFunction();
            else ReloadSceneFunction();
        }
        // if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) && OVRInput.GetDown(OVRInput.RawButton.Y))
        // {
        //     Debug.LogWarning("---dbg2---clicked:Y");
        //     EndCallCameraRigMethod();
        // }
        // if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) && OVRInput.GetDown(OVRInput.RawButton.X))
        // {
        //     Debug.LogWarning("---dbg2---clicked:X");
        //     StartCallCameraRigMethod();
        // }
        // Debug.Log("---tmpisHoveringOrigin"+isHoveringOrigin);
        // Debug.Log("---tmpisHoveringTarget"+isHoveringTarget);
        SetOriginTargetColorFunction();
        EveryFrameCsvFunction();
    }
    public void ChangeRightLeftFunction()
    {
        isRightHand = (isRightHand + 1) % 2;
    }
    public bool GetisIdselected()
    {
        return isIdselected;
    }

    public void EveryFrameCsvFunction()
    {
        // Debug.Log("---tmpevery---:");
        if ((!isStarted) || isEnded) return;
        // Debug.Log("---tmpevery---:");
        if (whichTargetText == "Touch-Practice" || whichTargetText == "Ray-Practice") return;

        // 保存先ディレクトリの基準パス
        string baseDirectoryPath = Application.persistentDataPath;

        // IDを使用してサブフォルダを作成（例: 1234_everyframe）
        string idDirectoryPath = Path.Combine(baseDirectoryPath, $"{id}_EveryFrame");

        // IDフォルダが存在しない場合は作成
        if (!Directory.Exists(idDirectoryPath))
        {
            Directory.CreateDirectory(idDirectoryPath);
        }
        string sceneName = SceneManager.GetActiveScene().name;

        // ファイル名（例: DebugData_20241209.csv）
        string fileName = $"{id}_EveryFrame_{sceneName}_{whichTargetText}_{startTime}.csv";
        string filePath = Path.Combine(idDirectoryPath, fileName);

        // デバッグデータの収集
        List<string> lines = new List<string>();

        // ヘッダー行（初回のみ書き込み）
        if (!File.Exists(filePath))
        {
            lines.Add("ID,FrameCount,currentDate,currentTime,Method,WhichTarget,HeadRotation.x,HeadRotation.y,HeadRotation.z,HeadForward.x,HeadForward.y,HeadForward.z,RightHandRotation.x,RightHandRotation.y,RightHandRotation.z,RightHandForward.x,RightHandForward.y,RightHandForward.z,RightHandPosition.x,RightHandPosition.y,RightHandPosition.z,LeftHandRotation.x,LeftHandRotation.y,LeftHandRotation.z,LeftHandForward.x,LeftHandForward.y,LeftHandForward.z,LeftHandPosition.x,LeftHandPosition.y,LeftHandPosition.z");
        }

        // 現在の日付と時刻
        string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
        string currentTime = DateTime.Now.ToString("HH:mm:ss");

        // データ行
        // string sceneName = SceneManager.GetActiveScene().name;
        List<Vector3> everyFrameDataList = new List<Vector3>();
        ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        if (cameraRig != null)
        {
            everyFrameDataList = cameraRig.GetEveryFrameData();
        }
        nowrotation = new List<float>() { everyFrameDataList[0][1], everyFrameDataList[2][1], everyFrameDataList[4][0], everyFrameDataList[4][1], everyFrameDataList[4][2], everyFrameDataList[5][1], everyFrameDataList[7][0], everyFrameDataList[7][1], everyFrameDataList[7][2] };

        // Debug.Log("---every---:" + string.Join(",", nowrotation));
        // `everyFrameDataList`をカンマ区切りの文字列に変換
        List<string> vector3Strings = new List<string>();
        foreach (var vector in everyFrameDataList)
        {
            vector3Strings.Add($"{vector.x},{vector.y},{vector.z}");
        }

        string everyFrameDataString = string.Join(",", vector3Strings);

        // データ行の作成
        string data = $"{id},{Time.frameCount},{currentDate},{currentTime},{sceneName},{whichTargetText},{everyFrameDataString}";
        // string data2 = "{id},{Time.frameCount},{currentDate},{currentTime},{sceneName},{whichTargetText},{everyFrameDataString}";

        lines.Add(data);

        // Debug.Log("---everyframe---: " + everyFrameDataString);

        // CSVにデータを書き込む（末尾に追記）
        File.AppendAllLines(filePath, lines);

        // Debug.Log($"Debug data saved to CSV at: {filePath}");
        ////////////////////deug
        // ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        List<float> tmplist = new List<float>();
        if (cameraRig != null)
        {
            tmplist = cameraRig.GetCameraHandsRotation();
            // Debug.Log("---everysum---sumHeadRotation,sumLeftHandPosition,,sumRightHandPosition,sumRightHandRotation");
            // Debug.Log("---everysum---endRotations: " + string.Join(", ", endRotations));
            // Debug.Log("---everysum1---sumHeadRotation: " + (tmplist[0] - startRotations[0]));
            // Debug.Log("---everysum1---sumRightHandPosition: " + (tmplist[3] - startRotations[3]));
            // Debug.Log("---everysum1---sumRightHandRotation: " + (tmplist[4] - startRotations[4]));
        }
        if (prevrotation.Count > 0)
        {
            tmpheadrotationsum += Mathf.Abs(nowrotation[0] - prevrotation[0]);
            tmprighthandrotationsum += Mathf.Abs(nowrotation[1] - prevrotation[1]);
            tmprighthandpositionsum += Mathf.Sqrt((nowrotation[2] - prevrotation[2]) * (nowrotation[2] - prevrotation[2]) + (nowrotation[3] - prevrotation[3]) * (nowrotation[3] - prevrotation[3]) + (nowrotation[4] - prevrotation[4]) * (nowrotation[4] - prevrotation[4]));
            tmplefthandrotationsum += Mathf.Abs(nowrotation[5] - prevrotation[5]);
            tmplefthandpositionsum += Mathf.Sqrt((nowrotation[6] - prevrotation[6]) * (nowrotation[6] - prevrotation[6]) + (nowrotation[7] - prevrotation[7]) * (nowrotation[7] - prevrotation[7]) + (nowrotation[8] - prevrotation[8]) * (nowrotation[8] - prevrotation[8]));

            // Debug.Log("---everysum1---sumHeadRotation: " + (tmplist[0] - startRotations[0]));
            // Debug.Log("---everysum2---sumHeadRotation: " + (tmpheadrotationsum));
            // Debug.Log("---everysum1---sumRightHandPosition: " + (tmplist[3] - startRotations[3]));
            // Debug.Log("---everysum2---sumRightHandPosition: " + tmprighthandpositionsum);
            // Debug.Log("---everysum1---sumRightHandRotation: " + (tmplist[4] - startRotations[4]));
            // Debug.Log("---everysum2---sumRightHandRotation: " + tmprighthandrotationsum);
            // Debug.Log($"---everyrp---nowrotation: {Time.frameCount}," + string.Join(",", nowrotation));
            // Debug.Log("---everyrp---tmpprevdata: " + string.Join(",", prevrotation));
        }
        prevrotation = nowrotation;
        //////////////////////
    }
    public void EverySelectCsvFunction(bool iscollect)
    {
        if ((!isStarted) || isEnded) return;
        if (whichTargetText == "Touch-Practice" || whichTargetText == "Ray-Practice") return;

        if (iscollect == false) errorInputNum += 1;

        // 保存先ディレクトリの基準パス
        string baseDirectoryPath = Application.persistentDataPath;

        // IDを使用してサブフォルダを作成（例: 1234_everyframe）
        string idDirectoryPath = Path.Combine(baseDirectoryPath, $"{id}_EverySelect");

        // IDフォルダが存在しない場合は作成
        if (!Directory.Exists(idDirectoryPath))
        {
            Directory.CreateDirectory(idDirectoryPath);
        }

        // ファイル名（例: DebugData_20241209.csv）
        string sceneName = SceneManager.GetActiveScene().name;

        string fileName = $"{id}_EverySelect_{sceneName}_{whichTargetText}_{startTime}.csv";
        string filePath = Path.Combine(idDirectoryPath, fileName);

        // デバッグデータの収集
        List<string> lines = new List<string>();

        // ヘッダー行（初回のみ書き込み）
        if (!File.Exists(filePath))
        {
            lines.Add("ID,FrameCount,currentDate,currentTime,Method,WhichTarget,TargetNumber,randomPermutation[currentIndex],TargetSize,Theta,nowtime-trialStartTime,error");
        }

        // 現在の日付と時刻
        string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
        string currentTime = DateTime.Now.ToString("HH:mm:ss");

        // データ行
        // string sceneName = SceneManager.GetActiveScene().name;
        float nowtime = Time.time;
        int isError = 0;
        if (!iscollect) isError = 1;
        string targetSizeString = "";
        int whichSize = randomPermutation[currentIndex];//0~9Large
        if (!iscollect) eachSizeError[whichSize / 10] += 1;
        eachSizeTrialTimes[whichSize / 10].Add(nowtime - trialStartTime);
        eachSizeTrialTimesSum[whichSize / 10] += nowtime - trialStartTime;
        if (whichSize < 10)
        {
            targetSizeString = "Large";
        }
        else if (whichSize < 20)
        {
            targetSizeString = "Middle";
        }
        else
        {
            targetSizeString = "Small";
        }
        // Debug.Log("---everyistargetselected: " + istargetselected);
        // Debug.Log("---everycurrentindex: " + currentIndex);
        // Debug.Log("---everyiserror: " + isError);

        // データ行の作成
        string data = $"{id},{Time.frameCount},{currentDate},{currentTime},{sceneName},{whichTargetText},{currentIndex + 1},{randomPermutation[currentIndex]},{targetSizeString},{SphereTheta[randomPermutation[currentIndex]]},{nowtime - trialStartTime},{isError}";
        string data2 = "{id},{Time.frameCount},{currentDate},{currentTime},{sceneName},{whichTarget},{currentIndex+1},{randomPermutation[currentIndex]},{targetSizeString},{targetSize},{Theta},{nowtime - trialStartTime},{isError}";

        Debug.Log("---dbg2---select: " + data);
        Debug.Log("---dbg2---selectString: " + data2);

        lines.Add(data);

        // CSVにデータを書き込む（末尾に追記）
        File.AppendAllLines(filePath, lines);
    }
    public void SetOriginTargetColorFunction()
    {
        if (OriginObject != null)
        {
            if (isHoveringOrigin)
            {
                OriginObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
            }
            else
            {
                OriginObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        if (NextTarget != null)
        {
            if (!istargetselected && isHoveringTarget)
            {
                NextTarget.GetComponent<MeshRenderer>().material.color = Color.cyan;
            }
            else if (!istargetselected && !isHoveringTarget)
            {
                NextTarget.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            else
            {
                NextTarget.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }
    public void ObjectSelected(GameObject selectedobject)
    {
        if (selectedobject.tag == "Origin")
        {
            OriginSelectedFunction();
        }
        if (selectedobject.tag == "Target")
        {
            TargetSelectedFunction();
        }
        if (selectedobject.tag == "SceneReloaderObject")
        {
            ///////////////////////
            if (OVRInput.Get(OVRInput.RawButton.B) && OVRInput.Get(OVRInput.RawButton.A))
            {
                Debug.LogWarning("---dbg2---SKIPPPPPPPPPPPP");
                SkipFunction();
            }
            //////////////////////////////////

            return;
            ReloadSceneFunction();
        }
        if (selectedobject.tag == "DebugObject")
        {
            DebugFunction();
        }
        if (selectedobject.tag == "SettingsObject")
        {
            // PracticeFinishFunction();
            SettingsOnOffFunction();
        }
        if (selectedobject.tag == "MethodsObject")
        {
            // ChangeMethodFunction(selectedobject.name);
            if (int.TryParse(selectedobject.name, out int result))
            {
                // SceneManager.LoadScene(result);
                ChangeMethodFunction(result);
            }
        }
        if (selectedobject.tag == "GenerateTargetsObject")
        {
            // GenerateTargetsFunction(selectedobject.name);
            if (int.TryParse(selectedobject.name, out int result))
            {
                // SceneManager.LoadScene(result);
                GenerateTargetsFunction(result);
            }
        }
        if (selectedobject.tag == "NextMethodObject")
        {
            NextMethodFunction();
        }
        if (selectedobject.tag == "IdObject")
        {

            if (int.TryParse(selectedobject.name, out int result))
            {
                SetIdFunction(result);
            }
        }
    }
    public void CenterDefineFunction()
    {

        if (isCenterDefined) return;
        Debug.Log("---dbg2---CenterDefined");
        isCenterDefined = true;
        Vector3 realHeadPosition = new Vector3();
        ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        if (cameraRig != null)
        {
            realHeadPosition = cameraRig.GetRealHeadPosition();
        }
        circleCenter = realHeadPosition;
        // circleCenter.y -= 0.2f;
        circleCenter.y += 3.2f;
        Debug.Log("---dbg2---cerclecenter: " + string.Join(",", circleCenter));
        Debug.Log("---dbg2---realHeadPosition: " + string.Join(",", realHeadPosition));

        if (isIdselected) NextMethodFunction();

    }
    public void PracticeFinishFunction()
    {
        if (!(nextTaskIndex == 0 && isStarted)) return;
        Debug.Log("PractiveFinish");
        TaskFinish();
    }
    public void SkipFunction()
    {
        if (!isEnded) return;
        nextMethodIndex = 0;
        touchray += 1;
        gotoNextMethod = true;
        nextTaskIndex = 0;
    }
    public void SetIdFunction(int idnum)
    {
        if (isIdselected) return;
        isIdselected = true;
        idsObject.SetActive(false);
        id = idnum;
        // Debug.Log("---dbgtmp---id: " + id);
        // Debug.Log("NextTaskList: "+string.Join(",",NextTaskList));
        // Debug.Log("NextTaskIndex: "+nextTaskIndex);
        // GenerateTargetsFunction(NextTaskList[nextTaskIndex]);
        // Vector3 realHeadPosition = new Vector3();
        // ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        // if (cameraRig != null)
        // {
        //     realHeadPosition = cameraRig.GetRealHeadPosition();
        // }
        // circleCenter = realHeadPosition;
        // circleCenter.y -= 0.2f;
        // Debug.Log("---dbg2---cerclecenter: " + string.Join(",", circleCenter));
        // Debug.Log("---dbg2---realHeadPosition: " + string.Join(",", realHeadPosition));
        if (isCenterDefined) NextMethodFunction();
    }
    public void NextMethodFunction()
    {
        if (isExperimentFinished) return;
        if (id != -1) SceneManager.LoadScene(NextMethodList[id % 7][nextMethodIndex]);
        else
        {
            Debug.LogWarning("id==-1");
        }
        // Debug.Log("NextMethodList[id % 7]: " +string.Join( ",",NextMethodList[id % 7]));
        // Debug.Log("nextMethodIndex: " +nextMethodIndex);
        // Debug.Log("---dbgtmp---Scene: " +SceneManager.GetActiveScene().name);
    }
    public void ObjectHoverringFunction(GameObject hoveringObject)
    {
        if (hoveringObject.tag == "Target")
        {
            isHoveringTarget = true;
            isHoveringOrigin = false;
        }
        if (hoveringObject.tag == "Origin")
        {
            isHoveringTarget = false;
            isHoveringOrigin = true;
        }
    }
    public void ObjectNotHoverringFunction()
    {
        isHoveringOrigin = false;
        isHoveringTarget = false;
    }
    public void AddCirclePoints(int targetsI)
    {
        Vector3 center = circleCenter;
        // Tuple<float, float> radiusPair = radiusTupleList[targetsI];
        float radius = radiusList[targetsI];
        // int numPoints = numberOfPoints;
        // float size = pointSizeList[targetsI];
        whichTargetText = whichTargetTextList[targetsI];
        // Debug.Log("---dbgtmp---Target: " + whichTargetText);
        // randomPermutation = rondomPermutationList[targetsI];
        if (targetsI == 0 || targetsI == 2)
        {
            float size = pointSizeList[0] * radius;
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 0, 6, 7, 8, 9 };
            System.Random random = new System.Random();
            randomPermutation = list.OrderBy(x => random.Next()).ToList();
            Debug.Log("---dbg2---randompermutation10: " + string.Join(",", randomPermutation));

            // halfradius = (radiusPair.Item1 + radiusPair.Item2) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (i == 3 || i == 9) continue;
                // 円周上の角度を計算（ラジアン）
                float angle = i * Mathf.PI * 2 / numberOfPoints;

                // 円周上の点の位置を計算
                // float x = center.x + Mathf.Cos(angle) * radiusPair.Item1;
                // float z = center.z + Mathf.Sin(angle) * radiusPair.Item1;
                float x = center.x + Mathf.Cos(angle) * radius;
                float z = center.z + Mathf.Sin(angle) * radius;
                Vector3 position = new Vector3(x, center.y, z);
                // SphereDataを作成してリストに追加
                sphereDataList.Add(new SphereData(position, size));
            }

            // size = size * radiusPair.Item1 / radiusPair.Item2;
            Vector3 originposition = new Vector3(center.x, center.y, center.z + radius);
            originData = new SphereData(originposition, size);
            GenerateSpheres();
        }
        else
        {
            List<int> list = Enumerable.Range(0, 30).ToList();
            System.Random random = new System.Random();
            randomPermutation = list.OrderBy(x => random.Next()).ToList();
            Debug.Log("---dbg2---randompermutation30: " + string.Join(",", randomPermutation));

            // halfradius = (radiusPair.Item1 + radiusPair.Item2) / 2;
            for (int ii = 0; ii < numberOfPoints * 3; ii++)
            {
                float size = pointSizeList[(ii / numberOfPoints) + 1] * radius;
                int i = ii % numberOfPoints;
                if (i == 3 || i == 9) continue;
                // 円周上の角度を計算（ラジアン）
                float angle = i * Mathf.PI * 2 / numberOfPoints;

                // 円周上の点の位置を計算
                float x = center.x + Mathf.Cos(angle) * radius;
                float z = center.z + Mathf.Sin(angle) * radius;
                Vector3 position = new Vector3(x, center.y, z);
                float fi = ((float)(ii / numberOfPoints) + 1) / 10.0f;
                // Vector3 position = new Vector3(x, center.y+fi, z);
                // SphereDataを作成してリストに追加
                sphereDataList.Add(new SphereData(position, size));
                float angle2 = angle * 180.0f / Mathf.PI;
                angle2 = 90.0f - angle2;
                while (angle2 > 180.0f)
                {
                    angle2 -= 360f;
                }
                while (angle2 < -180.0f)
                {
                    angle2 += 360f;
                }
                SphereTheta.Add(angle2);
            }
            Debug.Log("spheretheta: " + string.Join(";", SphereTheta) + "--" + SphereTheta.Count);
            // size = size * radiusPair.Item1 / radiusPair.Item2;
            Vector3 originposition = new Vector3(center.x, center.y, center.z + radius);
            originData = new SphereData(originposition, pointSizeList[1] * radius);
            GenerateSpheres();
        }
    }
    public void GenerateSpheres()
    {
        if (isTargetGenerated) return;
        isTargetGenerated = true;
        // Debug.Log(isTargetGenerated);

        for (int pi = 0; pi < sphereDataList.Count; pi++)
        {
            var sphereData = sphereDataList[randomPermutation[pi]];
            // プレハブを指定位置に生成
            GameObject sphere = Instantiate(spherePrefab, sphereData.position, Quaternion.identity);

            // サイズを設定
            sphere.transform.localScale = Vector3.one * sphereData.size;

            // タグを設定
            sphere.tag = "Target";
            objects.Add(sphere);
        }
        GameObject origin = Instantiate(originOrefab, originData.position, Quaternion.identity);
        origin.transform.localScale = Vector3.one * originData.size;
        origin.tag = "Origin";
        origin.GetComponent<MeshRenderer>().material.color = Color.red;
        OriginObject = origin;
        lineRenderer2.SetPosition(0, originData.position);
        lineRenderer2.SetPosition(1, originData.position);
        lineRenderer3.SetPosition(0, originData.position);
        lineRenderer3.SetPosition(1, originData.position);
        DebugFunction2();
        // onoffswitch.SetActive(false);
    }
    public void DebugFunction2()
    {
        Debug.Log("---dbg---////////////DEBUGSTART2///////////////////////////");
        Debug.Log("---dbg---id: " + id);
        Debug.Log("---dbg---Scene: " + SceneManager.GetActiveScene().name);
        Debug.Log("---dbg---whichTarget: " + whichTargetText);
        Debug.Log("---dbg---nextMethodIndex: " + nextMethodIndex);
        Debug.Log("---dbg---nextTaskIndex: " + nextTaskIndex);
        Debug.Log("---dbg---touchray: " + touchray);
        Debug.Log("---dbg---isExperimentFinished: " + isExperimentFinished);
        Debug.Log("---dbg---////////////DEBUGEND2///////////////////////////");
    }

    //アクティブにするオブジェクトを管理
    public void SetActiveObject(int index)
    {
        // すべてのオブジェクトを非アクティブ化
        foreach (var obj in objects)
        {
            obj.SetActive(false);
        }
        //矢印を消す
        // RightAssist.SetActive(false);
        // LeftAssist.SetActive(false);
        if (nextTaskIndex == 0) index = index % objects.Count;

        // 指定したインデックスをアクティブ化
        if (index >= 0 && index < objects.Count)
        {
            istargetselected = false;
            NextTarget = objects[index];
            NextTarget.GetComponent<MeshRenderer>().material.color = Color.white;

            // if ((NextTarget.transform.position - circleCenter).magnitude < halfradius || true)
            if (true)
            {
                // Debug.Log(NextTarget.transform.position.magnitude);
                // Debug.Log(halfradius);
                lineRenderer2.SetPosition(1, NextTarget.transform.position);
                lineRenderer3.SetPosition(1, originData.position);
            }
            else
            {
                lineRenderer3.SetPosition(1, NextTarget.transform.position);
                lineRenderer2.SetPosition(1, originData.position);
            }
            objects[index].SetActive(true);
            currentIndex = index; // 現在のインデックスを更新
            trialStartTime = Time.time; // 新しい試行の開始時間を記録
        }
        // float directyajirushi = NextTarget.transform.localPosition.x;
        // if (directyajirushi > 0) RightAssist.SetActive(true);
        // else LeftAssist.SetActive(true);
    }
    //あるオブジェクトが選択されたとき

    public void GenerateTargetsFunction(int name)
    {
        if (!isTargetGenerated)
        {
            tmpText.text += "Targets:" + $"{name}" + "\n";
            if (name == 0 || name == 1) RayValidBool = false;
        }
        // if (int.TryParse(name, out int result))
        // {
        //     AddCirclePoints(result);
        // }
        AddCirclePoints(name);
    }
    public bool GetRayValidBoolFunction()
    {
        return RayValidBool;
    }
    public GameObject GetOriginObjectFunction()
    {
        return OriginObject;
    }
    public GameObject GetNextTargetFunction()
    {
        return NextTarget;
    }

    public void OriginSelectedFunction()
    {
        if ((!isStarted) && isTargetGenerated)
        {
            isStarted = true;
            if (objects.Count > 0)
            {
                SetActiveObject(0);
            }
            onoffswitch.SetActive(false);
            StartCallCameraRigMethod();
            return;
        }
        GotoNext("Origin");
    }
    public void GotoNext(string targetstring)
    {
        if (isEnded || (!isStarted)) return;
        if (istargetselected)
        {
            if (targetstring != "Origin")
            {
                return;
            }
            istargetselected = false;
            if (NextTarget != null) NextTarget.GetComponent<MeshRenderer>().material.color = Color.white;
            // 試行終了時の時間を計測
            // float trialEndTime = Time.time;
            // float trialDuration = trialEndTime - trialStartTime; // 試行の所要時間
            // if (!isEnded) trialTimes.Add(trialDuration); // 時間をリストに追加
            // Debug.Log($"Trial {trialTimes.Count}: {trialDuration} seconds");
            ActivateNextObject();
        }
        else
        {
            istargetselected = true;
            if (targetstring == "Target")
            {
                EverySelectCsvFunction(true);
            }
            else
            {
                EverySelectCsvFunction(false);
            }
            /////////////////
            if (NextTarget != null) NextTarget.GetComponent<MeshRenderer>().material.color = Color.green;
            float trialEndTime = Time.time;
            float trialDuration = trialEndTime - trialStartTime; // 試行の所要時間
            if (!isEnded) trialTimes.Add(trialDuration);
            /////////////////
        }
    }
    public void TargetSelectedFunction()
    {
        if (!isStarted) return;
        GotoNext("Target");
    }
    public void ChangeMethodFunction(int methodNameInt)
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // if (int.TryParse(methodNameString, out int result))
        // {
        //     SceneManager.LoadScene(result);
        // }
        SceneManager.LoadScene(methodNameInt);
    }

    // 次のオブジェクトをアクティブ化
    public void ActivateNextObject()
    {
        int nextIndex = (currentIndex + 1);  // 次のインデックスを計算
        SetActiveObject(nextIndex);
        if ((nextIndex >= objects.Count) && nextTaskIndex > 0)
        {
            TaskFinish();
        }
    }
    //何も選択されなかったとき
    public void NoneSelected()
    {
        if (isStarted && !isEnded)
        {
            GotoNext("");
        }
    }
    public void TaskFinish()
    {
        if (!isStarted) return;
        if (isEnded) return;
        text.text = "FINISH";
        if (messageText != null)
        {
            messageText.SetActive(true);
        }
        // RightAssist.SetActive(false);
        // LeftAssist.SetActive(false);
        isEnded = true;
        EndCallCameraRigMethod();
        DebugFunction();
        if (NextTarget != null) NextTarget.transform.position = new Vector3(10, 10, 10);
        onoffswitch.SetActive(true);
        // SaveDebugToCSV();
        nextTaskIndex += 1;
        if (nextTaskIndex == NextTaskList.Count)
        {
            nextTaskIndex = 0;
            gotoNextMethod = true;
            nextMethodIndex += 1;
            if (nextMethodIndex == NextMethodList[id % 7].Count)
            {
                nextMethodIndex = 0;
                touchray += 1;
                gotoNextMethod = true;
                if (touchray > 1) isExperimentFinished = true;
            }
        }
        RayValidBool = true;
    }

    public void ReloadSceneFunction()
    {
        Debug.Log("---dbg--------------Reloaded-------------");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void SettingsOnOffFunction()
    {
        // PracticeFinishFunction();
        bool isactive = onoffswitch.activeSelf;
        onoffswitch.SetActive(!isactive);
    }
    void StartCallCameraRigMethod()
    {

        if (activeCameraScripts == null)
        {
            Debug.LogWarning("---dbg2---No active camera script found.");
            return;
        }

        ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        if (cameraRig != null)
        {
            startRotations = cameraRig.GetCameraHandsRotation();
            Debug.Log("---dbg2---startRotations: " + string.Join(", ", startRotations));
        }
        else
        {
            Debug.LogWarning("---dbg2---activeCameraScriptsはICameraRigを実装していません。");
        }
        Debug.Log("---dbg2---startframe: " + Time.frameCount);
    }
    void EndCallCameraRigMethod()
    {
        if (activeCameraScripts == null)
        {
            Debug.LogWarning("---dbg2---No active camera script found.");
            return;
        }

        ICameraRig cameraRig = activeCameraScripts as ICameraRig;
        if (cameraRig != null)
        {
            endRotations = cameraRig.GetCameraHandsRotation();
            // Debug.Log("---dbg2---sumHeadRotation,sumLeftHandPosition,sumLeftHandRotation,sumRightHandPosition,sumRightHandRotation");
            // Debug.Log("---dbg2---endRotations: " + string.Join(", ", endRotations));
            // Debug.Log("---dbg2---sumHeadRotation: " + (endRotations[0] - startRotations[0]));
            // Debug.Log("---dbg2---sumLeftHandPosition: " + (endRotations[1] - startRotations[1]));
            // Debug.Log("---dbg2---sumLeftHandRotation: " + (endRotations[2] - startRotations[2]));
            // Debug.Log("---dbg2---sumRightHandPosition: " + (endRotations[3] - startRotations[3]));
            // Debug.Log("---dbg2---sumRightHandRotation: " + (endRotations[4] - startRotations[4]));
        }
        else
        {
            Debug.LogWarning("---dbg2---activeCameraScriptsはICameraRigを実装していません。");
        }
        SaveDebugToCSV();
        Debug.Log("---dbg2---endframe: " + Time.frameCount);
    }
    //デバッグ用
    public void DebugFunction()
    {
        float sumtime = 0;
        foreach (var trialtime in trialTimes)
        {
            sumtime += trialtime;
        }
        Debug.Log("---dbg---////////////DEBUGSTART///////////////////////////");
        Debug.Log("---dbg---ID: " + id);
        Debug.Log("---dbg---Method: " + SceneManager.GetActiveScene().name);
        Debug.Log("---dbg---whichTarget: " + whichTargetText);
        Debug.Log("---dbg---startTime: " + startTime);
        Debug.Log("---dbg---RandomPermutation: " + string.Join(";", randomPermutation));
        Debug.Log("---dbg---eachSizeError[0]: " + eachSizeError[0]);
        Debug.Log("---dbg---eachSizeError[1]: " + eachSizeError[1]);
        Debug.Log("---dbg---eachSizeError[2]: " + eachSizeError[2]);
        Debug.Log("---dbg---errorInputNum: " + errorInputNum);
        Debug.Log("---dbg---objects.Count: " + objects.Count);
        Debug.Log("---dbg---elapsedTime: " + elapsedTime);
        Debug.Log("---dbg---istargetselected: " + istargetselected);
        Debug.Log("---dbg---SUM(TrialTimes),: " + sumtime);
        Debug.Log("---dbg---All trial times: " + string.Join(", ", trialTimes));
        Debug.Log("---dbg---trialTimes.Count: " + trialTimes.Count);
        Debug.Log("---dbg---eachSizeTrialTimesSum[0]: " + eachSizeTrialTimesSum[0]);
        Debug.Log("---dbg---eachSizeTrialTimesSum[1]: " + eachSizeTrialTimesSum[1]);
        Debug.Log("---dbg---eachSizeTrialTimesSum[2]: " + eachSizeTrialTimesSum[2]);
        Debug.Log("---dbg---eachSizeTrialTimes[0]: " + string.Join(";", eachSizeTrialTimes[0]));
        Debug.Log("---dbg---eachSizeTrialTimes[1]: " + string.Join(";", eachSizeTrialTimes[1]));
        Debug.Log("---dbg---eachSizeTrialTimes[2]: " + string.Join(";", eachSizeTrialTimes[2]));
        // Debug.Log("---dbg---sumHeadRotation: " + (endRotations[0] - startRotations[0]));
        // Debug.Log("---dbg---sumRightHandPosition: " + (endRotations[3] - startRotations[3]));
        // Debug.Log("---dbg---sumRightHandRotation: " + (endRotations[4] - startRotations[4]));
        Debug.Log("---dbg---kottisumHeadRotation: " + (tmpheadrotationsum));
        Debug.Log("---dbg---sumRightHandPosition: " + (tmprighthandpositionsum));
        Debug.Log("---dbg---sumRightHandRotation: " + (tmprighthandrotationsum));
        Debug.Log("---dbg---sumLeftHandPosition: " + (tmplefthandpositionsum));
        Debug.Log("---dbg---sumLeftHandRotation: " + (tmplefthandrotationsum));
        Debug.Log("---dbg---NextMethodList: " + string.Join(";", NextMethodList[id % 7]));
        Debug.Log("---dbg---isRightHand: " + isRightHand);
        Debug.Log("---dbg---///////////////DEBUGEND/////////////");
        // CSV出力
        // SaveDebugToCSV();

    }
    private void SaveDebugToCSV()
    {
        if (whichTargetText == "Touch-Practice" || whichTargetText == "Ray-Practice") return;

        // 保存先ディレクトリ
        string directoryPath = Application.persistentDataPath;

        // IDを使用してフォルダを作成
        string idDirectoryPath = Path.Combine(directoryPath, $"{id}_Result");
        if (!Directory.Exists(idDirectoryPath))
        {
            Directory.CreateDirectory(idDirectoryPath);
        }

        // 日付を含むファイル名（例: DebugData_20241209.csv）
        string fileName = $"{id}_Result_{DateTime.Now:yyyyMMdd}.csv";
        string filePath = Path.Combine(idDirectoryPath, fileName);

        // デバッグデータの収集
        List<string> lines = new List<string>();

        // ヘッダー行（初回のみ書き込み）
        if (!File.Exists(filePath))
        {
            lines.Add("ID,currentDate,currentTime,isRightHand,Method,WhichTarget,StartTime,randomPermutation,LargeSizeError,MiddleSizeError,SmallSizeError,errorInputNum,ErrorRate,LargeTime,LargeTimeSum,MiddleTime,MiddleTimeSum,SmallTime,SmallTimeSum,TrialTimes,SUM(TrialTimes),sumHeadRotation,sumRightHandPosition,sumRightHandRotation,sumLeftHandPosition,sumLeftHandRotation");
        }

        // 現在の日付と時刻
        string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
        string currentTime = DateTime.Now.ToString("HH:mm:ss");

        // データ行
        string sceneName = SceneManager.GetActiveScene().name;
        string trialTimesString = string.Join(";", trialTimes); // 複数の試行時間をセミコロンで区切る
        string Randompermutationstring = string.Join(";", randomPermutation);
        float eee = errorInputNum;
        float ttt = objects.Count;
        float sumtime = 0;
        foreach (var trialtime in trialTimes)
        {
            sumtime += trialtime;
        }

        // データ行の作成
        string data = $"{id},{currentDate},{currentTime},{isRightHand},{sceneName},{whichTargetText},{startTime},{Randompermutationstring},{eachSizeError[0]},{eachSizeError[1]},{eachSizeError[2]},{errorInputNum},{eee / ttt},{string.Join(";", eachSizeTrialTimes[0])},{eachSizeTrialTimesSum[0]},{string.Join(";", eachSizeTrialTimes[1])},{eachSizeTrialTimesSum[1]},{string.Join(";", eachSizeTrialTimes[2])},{eachSizeTrialTimesSum[2]},{trialTimesString},{sumtime},{tmpheadrotationsum},{tmprighthandpositionsum},{tmprighthandrotationsum},{tmplefthandpositionsum},{tmplefthandrotationsum}";
        string datastring = "{id},{currentDate},{currentTime},{isRightHand},{sceneName},{whichTargetText},{startTime},{Randompermutationstring},{eachSizeError[0]},{eachSizeError[1]},{eachSizeError[2]},{errorInputNum},{eee / ttt},{eachSizeTrialTimes[0]},{eachSizeTrialTimesSum[0]},{eachSizeTrialTimes[1]},{eachSizeTrialTimesSum[1]},{eachSizeTrialTimes[2]},{eachSizeTrialTimesSum[2]},{trialTimesString},{sumtime},{tmpheadrotationsum},{tmprighthandpositionsum},{tmprighthandrotationsum},{tmplefthandpositionsum},{tmplefthandrotationsum}";

        lines.Add(data);
        Debug.Log("---dbg2---result: " + data);
        Debug.Log("---dbg2---resultstring: " + datastring);


        // CSVにデータを書き込む（末尾に追記）
        File.AppendAllLines(filePath, lines);

        // Debug.Log($"---dbg2---Debug data saved to CSV at: {filePath}");
    }


}
