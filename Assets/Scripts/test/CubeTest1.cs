using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest1 : MonoBehaviour
{
    public GameObject cube; // 対象のキューブ
    private bool isVisible = true; // 現在の表示状態

    void Start()
    {
        // 1秒ごとにToggleVisibilityメソッドを繰り返し呼び出す
        InvokeRepeating("ToggleVisibility", 0f, 1f);
    }

    void ToggleVisibility()
    {
        isVisible = !isVisible; // 表示状態を反転
        cube.SetActive(isVisible); // 表示・非表示を切り替え
    }
}
