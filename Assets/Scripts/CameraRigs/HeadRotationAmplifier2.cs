using UnityEngine;
using Meta.XR;

public class HeadRotationAmplifier2 : MonoBehaviour
{
    [SerializeField]
    private float amplificationFactor = 2.0f;

    [SerializeField]
    private bool amplifyPitch = true;

    [SerializeField]
    private bool amplifyYaw = true;

    [SerializeField]
    private bool amplifyRoll = false;

    [Header("Debug Settings")]
    [SerializeField]
    private bool enableDebugLogs = true;

    [SerializeField]
    private float logInterval = 0.5f;

    private OVRCameraRig cameraRig;
    private Transform centerEyeAnchor;
    private Transform trackingSpace;
    private Quaternion lastRawRotation;
    private float nextLogTime = 0f;

    private void Start()
    {
        cameraRig = GetComponentInParent<OVRCameraRig>();
        if (cameraRig == null)
        {
            Debug.LogError("OVRCameraRig not found!");
            return;
        }

        centerEyeAnchor = cameraRig.centerEyeAnchor;
        if (centerEyeAnchor == null)
        {
            Debug.LogError("CenterEyeAnchor not found!");
            return;
        }

        trackingSpace = cameraRig.trackingSpace;
        lastRawRotation = centerEyeAnchor.localRotation;
    }

    private void LateUpdate()
    {
        if (centerEyeAnchor == null || trackingSpace == null) return;

        // オリジナルの回転を取得
        Quaternion rawRotation = centerEyeAnchor.localRotation;
        Vector3 rawEuler = NormalizeAngles(rawRotation.eulerAngles);

        // 増幅された回転を計算
        Vector3 amplifiedEuler = new Vector3(
            amplifyPitch ? rawEuler.x * amplificationFactor : rawEuler.x,
            amplifyYaw ? rawEuler.y * amplificationFactor : rawEuler.y,
            amplifyRoll ? rawEuler.z * amplificationFactor : rawEuler.z
        );

        // 回転を適用
        centerEyeAnchor.localRotation = Quaternion.Euler(amplifiedEuler);
        lastRawRotation = rawRotation;

        // デバッグログの出力
        if (enableDebugLogs && Time.time >= nextLogTime)
        {
            LogRotationDebug(rawEuler, amplifiedEuler);
            nextLogTime = Time.time + logInterval;
        }
    }

    private void LogRotationDebug(Vector3 original, Vector3 amplified)
    {
        Debug.Log(
            $"Head Rotation Debug:\n" +
            $"Original (P,Y,R): ({original.x:F1}°, {original.y:F1}°, {original.z:F1}°)\n" +
            $"Amplified (P,Y,R): ({amplified.x:F1}°, {amplified.y:F1}°, {amplified.z:F1}°)\n" +
            $"Factor: {amplificationFactor:F1}x"
        );
    }

    private Vector3 NormalizeAngles(Vector3 angles)
    {
        return new Vector3(
            NormalizeAngle(angles.x),
            NormalizeAngle(angles.y),
            NormalizeAngle(angles.z)
        );
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    public void SetAmplificationFactor(float factor)
    {
        amplificationFactor = factor;
    }

    public void SetAxisAmplification(bool pitch, bool yaw, bool roll)
    {
        amplifyPitch = pitch;
        amplifyYaw = yaw;
        amplifyRoll = roll;
    }

    public void SetDebugLogging(bool enable)
    {
        enableDebugLogs = enable;
    }

    public void SetLogInterval(float interval)
    {
        logInterval = Mathf.Max(0.1f, interval);
    }
}