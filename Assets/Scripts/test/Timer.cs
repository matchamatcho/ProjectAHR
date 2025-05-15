using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float startTime; // タイマー開始時の時間
    private bool isTimerRunning = false; // タイマーが実行中かどうかを判定

    // タイマーを開始する関数
    public void StartTimer()
    {
        if (!isTimerRunning) // タイマーが実行中でない場合のみ開始
        {
            startTime = Time.time; // 現在の時間を記録
            isTimerRunning = true;
            Debug.Log("タイマーを開始しました。");
        }
        else
        {
            Debug.Log("タイマーはすでに実行中です。");
        }
    }

    // タイマーを停止し、経過時間をデバッグ出力する関数
    public void StopTimer()
    {
        if (isTimerRunning) // タイマーが実行中の場合のみ停止
        {
            float elapsedTime = Time.time - startTime; // 経過時間を計算
            isTimerRunning = false;
            Debug.Log($"タイマーを停止しました。経過時間: {elapsedTime:F2} 秒");
        }
        else
        {
            Debug.Log("タイマーは実行されていません。");
        }
    }
}
