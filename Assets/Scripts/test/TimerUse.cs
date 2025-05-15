using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerUse : MonoBehaviour
{
    public Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKey(KeyCode.A)){
            timer.StartTimer();
            Debug.Log("aaaaaaa");
            // SceneManager.LoadScene (SceneManager.GetActiveScene().name);
        }
        if(Input.GetKey(KeyCode.B)){
            timer.StopTimer();
        }
        
    }
}
