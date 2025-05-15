using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TwoBackTaskFunction : MonoBehaviour
{
    public List<GameObject> targets; // ターゲットリスト
    private Color highlightColor = Color.yellow; // ハイライトの色
    private Color WhenSelectedColor = Color.green;

    private int currentIndex = -2;  // 現在のターゲット（2つ前を意味する）
    private int highlightIndex = 0; // ハイライト中のターゲット
    private int NextTargetIndex = 0;
    private Dictionary<GameObject, Color> defaultColors = new Dictionary<GameObject, Color>(); // デフォルト色辞書
    // private GameObject NextTarget;
    public GameObject NextTarget;
    public GameObject Origin;

    private GameObject LastSelectedTarget = null;
    private bool istargetselected = false;
    // private GameObject LastSelectedObject;
    private bool started = false;
    private bool pointingstarted = false;

    public bool timeout = false;
    private int truenum = 0;
    private int falsenum = 0;


    private bool autoHighlighting = true; // 自動ハイライト中かどうか
    public float timeLimit = 15f;
    private float elapsedTime = 0f; // 経過時間
    public GameObject TaskSwicher;
    public GameObject PointingTaskObject;
    public GameObject TwoBackTaskObject;
    public GameObject onoffswitch;

    public GameObject messageText; // 表示するテキストオブジェクト
    [SerializeField]
    private Text text;  //情報を乗せるText
    private List<float> trialTimes = new List<float>(); // 各試行の時間を保存するリスト
    private List<float> EachTimes;

    void Start()
    {
        if (messageText != null)
        {
            messageText.SetActive(false);
        }
        EachTimes = new List<float>(new float[targets.Count]);

    }
    void Update()
    {
        if (started && !timeout)
        {
            elapsedTime += Time.deltaTime; // 経過時間を更新
            if (elapsedTime >= timeLimit)
            {
                timeout = true;
                // FinishTask(); // タスク終了
            }
        }
        
    }


    //あるオブジェクトが選択されたとき
    public void ObjectSelected(GameObject selectedobject)
    {
        // Debug.Log(selectedobject);
        if (!started & selectedobject == Origin)
        {
            Debug.Log("startedddddddd");
            StartTask();
        }
        else if (pointingstarted & selectedobject.tag == "TwoBackTaskTarget")
        {
            // Debug.Log("Changeeeeeee");
            ChangeObjectColor(selectedobject);
        }
        else if (pointingstarted & selectedobject == Origin & LastSelectedTarget != null)
        {
            UpdateTargets();

        }
        if (selectedobject == TaskSwicher)
        {
            SwitchTaskFunction();
        }
        if (selectedobject.tag == "SceneReloaderObject")
        {
            ReloadSceneFunction();
        }
        if (selectedobject.tag == "DebugObject")
        {
            DebugFunction();
        }
        if (selectedobject.tag == "SettingsObject")
        {
            SettingsOnOffFunction();
        }
        if (selectedobject.tag == "MethodsObject")
        {
            ChangeMethodFunction(selectedobject.name);

        }



    }
    public void ChangeMethodFunction(string methodNameString)
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (int.TryParse(methodNameString, out int result))
        {
            SceneManager.LoadScene(result);
        }

    }
    public void StartTask()
    {
        started = true;
        foreach (var target in targets)
        {
            if (target.TryGetComponent<Renderer>(out Renderer renderer))
            {
                defaultColors[target] = renderer.material.color;
            }
        }
        if (targets.Count > 0)
        {
            StartCoroutine(AutoHighlight()); // 自動ハイライトの開始
        }

    }
    IEnumerator AutoHighlight()
    {
        // 最初の2つを2秒ごとにハイライト
        for (int i = 0; i < 2; i++)
        {
            highlightIndex = i;
            UpdateHighlight();
            yield return new WaitForSeconds(5f); // 2秒待つ
        }

        // 自動ハイライト終了後、次はポインティング待機
        autoHighlighting = false;
        currentIndex = -1; // 2つ前を0番目に合わせる
        highlightIndex = 2; // 3番目のターゲットにハイライト
        NextTarget = targets[0];
        UpdateHighlight();
        pointingstarted = true;

    }
    void UpdateHighlight()
    {
        // 全てのターゲットの色をリセット
        foreach (var target in targets)
        {
            if (defaultColors.TryGetValue(target, out Color defaultColor))
            {
                target.GetComponent<Renderer>().material.color = defaultColor;
            }
        }

        // ハイライト対象を設定
        if (highlightIndex < targets.Count)
        {
            targets[highlightIndex].GetComponent<Renderer>().material.color = highlightColor;
        }
        EachTimes[highlightIndex] = Time.time;
    }
    void UpdateTargets()
    {
        if (NextTarget == LastSelectedTarget)
        {
            Debug.Log("correct!!!!");
            truenum += 1;
        }
        else
        {
            Debug.Log("NOTTTTcollect!!!!");
            falsenum += 1;
        }
        float trialEndTime = Time.time;
        float trialDuration = trialEndTime - EachTimes[NextTargetIndex]; // 試行の所要時間
        if (trialDuration > 0)
        {
            trialTimes.Add(trialDuration);
            // Debug.Log("trialTomesaeeded");
        } // 時間をリストに追加
        NextTargetIndex = (NextTargetIndex + 1) % targets.Count;
        NextTarget = targets[NextTargetIndex];
        highlightIndex = (highlightIndex + 1) % targets.Count;
        LastSelectedTarget = null;

        if (highlightIndex < targets.Count)
        {
            UpdateHighlight();
        }
        else
        {
            Debug.Log("All targets completed!");
        }
    }


    void ChangeObjectColor(GameObject targetobject)
    {
        if (targetobject.TryGetComponent<Renderer>(out Renderer renderer))
        {
            if (LastSelectedTarget != null)
            {
                LastSelectedTarget.GetComponent<Renderer>().material.color = defaultColors[LastSelectedTarget];
                targets[highlightIndex].GetComponent<Renderer>().material.color = highlightColor;

            }
            LastSelectedTarget = targetobject;
            renderer.material.color = WhenSelectedColor; // ハイライトされたので緑に変更
        }
    }
    public void FinishTask()
    {
        text.text = "FINISH";
        if (messageText != null)
        {
            messageText.SetActive(true);
        }

    }
    public void SwitchTaskFunction()
    {
        if (PointingTaskObject == null || TwoBackTaskObject == null)
        {
            Debug.Log("TaskObject==null");
            return;
        }
        bool isActivePointingTask = PointingTaskObject.activeSelf;
        PointingTaskObject.SetActive(!isActivePointingTask);
        TwoBackTaskObject.SetActive(isActivePointingTask);
    }
    public void ReloadSceneFunction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public void SettingsOnOffFunction()
    {
        bool isactive = onoffswitch.activeSelf;
        onoffswitch.SetActive(!isactive);
    }


    //デバッグ用
    public void DebugFunction()
    {
        Debug.Log("DEBUG///////////////////////////////");
        Debug.Log("Scene:" + SceneManager.GetActiveScene().name);
        Debug.Log("truenum" + truenum);
        Debug.Log("false" + falsenum);
        Debug.Log("correct percent" + 100f * (float)truenum / (float)(truenum + falsenum));
        Debug.Log("All trial times: " + string.Join(", ", trialTimes));





        Debug.Log("DEBUG///////////////////////////////");
    }
}
