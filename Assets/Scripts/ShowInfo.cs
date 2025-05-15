using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShowInfo : MonoBehaviour
{
    public TMP_Text tmpText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tmpText.text = "";
        tmpText.text += "///SETTINGS///\nmizuiro:debug\nkuro:scenereload\n///INFOMATION///\n";
        tmpText.text += "Scene:" + SceneManager.GetActiveScene().name + "\n";

    }







}
