/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#if USING_XR_MANAGEMENT && (USING_XR_SDK_OCULUS || USING_XR_SDK_OPENXR)
#define USING_XR_SDK
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Node = UnityEngine.XR.XRNode;

/// <summary>
/// This class is the core component of the <see cref="OVRCameraRig"/> prefab, which provides the transform object to
/// represent the Oculus tracking space and the trackable objects. It contains  a tracking space
/// game object to fine-tune the relationship between the head tracking reference frame and your world.
/// It also contains a center eye anchor, which is the main Unity camera, two anchor game objects for each eye,
/// and left and right hand anchors for controllers.
///
/// For more information, see https://developer.oculus.com/documentation/unity/unity-add-camera-rig/.
/// </summary>
[ExecuteInEditMode]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-add-camera-rig/")]
public class CameraRig2 : MonoBehaviour
{
    /// <summary>
    /// The left eye camera.
    /// </summary>
    public Camera leftEyeCamera
    {
        get { return (usePerEyeCameras) ? _leftEyeCamera : _centerEyeCamera; }
    }

    /// <summary>
    /// The right eye camera.
    /// </summary>
    public Camera rightEyeCamera
    {
        get { return (usePerEyeCameras) ? _rightEyeCamera : _centerEyeCamera; }
    }

    /// <summary>
    /// Provides a root transform for all anchors in tracking space.
    /// </summary>
    public Transform trackingSpace { get; private set; }

    /// <summary>
    /// Always coincides with the pose of the left eye.
    /// </summary>
    public Transform leftEyeAnchor { get; private set; }

    /// <summary>
    /// Always coincides with average of the left and right eye poses.
    /// </summary>
    public Transform centerEyeAnchor { get; private set; }

    /// <summary>
    /// Always coincides with the pose of the right eye.
    /// </summary>
    public Transform rightEyeAnchor { get; private set; }

    /// <summary>
    /// Always coincides with the pose of the left active input device.
    /// </summary>
    public Transform leftHandAnchor { get; private set; }

    /// <summary>
    /// Always coincides with the pose of the right active input device.
    /// </summary>
    public Transform rightHandAnchor { get; private set; }

    /// <summary>
    /// Coincides with the pose of the left input device if it is detached.
    /// </summary>
    public Transform leftHandAnchorDetached { get; private set; }

    /// <summary>
    /// Coincides with the pose of the left input device if it is detached.
    /// </summary>
    public Transform rightHandAnchorDetached { get; private set; }

    /// <summary>
    /// Coincides with the pose of the left controller when it is in a hand
    /// </summary>
    public Transform leftControllerInHandAnchor { get; private set; }

    /// <summary>
    /// Coincides with the pose of the left hand when it is on a controller
    /// </summary>
    public Transform leftHandOnControllerAnchor { get; private set; }

    /// <summary>
    /// Coincides with the pose of the right controller when it is in a hand
    /// </summary>
    public Transform rightControllerInHandAnchor { get; private set; }

    /// <summary>
    /// Coincides with the pose of the right hand when it is on a controller
    /// </summary>
    public Transform rightHandOnControllerAnchor { get; private set; }

    /// <summary>
    /// Anchors controller pose to fix offset issues for the left hand.
    /// </summary>
    public Transform leftControllerAnchor { get; private set; }

    /// <summary>
    /// Anchors controller pose to fix offset issues for the right hand.
    /// </summary>
    public Transform rightControllerAnchor { get; private set; }

    /// <summary>
    /// Always coincides with the pose of the sensor.
    /// </summary>
    public Transform trackerAnchor { get; private set; }

    /// <summary>
    /// Occurs when the eye pose anchors have been set.
    /// </summary>
    public event System.Action<CameraRig2> UpdatedAnchors;

    /// <summary>
    /// Occurs when the <see cref="trackingSpace"/>'s transform changes.
    /// </summary>
    /// <remarks>
    /// This event provides a single argument: the `Transform` of the tracking space. This is the same as the
    /// <see cref="trackingSpace"/> property.
    /// </remarks>
    public event Action<Transform> TrackingSpaceChanged;

    /// <summary>
    /// If true, separate cameras will be used for the left and right eyes.
    /// </summary>
    public bool usePerEyeCameras = false;

    /// <summary>
    /// If true, all tracked anchors are updated in FixedUpdate instead of Update to favor physics fidelity.
    /// \note: This will cause visible judder unless you tick exactly once per frame using a custom physics
    /// update, because you'll be sampling the position at different times into each frame.
    /// </summary>
    public bool useFixedUpdateForTracking = false;

    /// <summary>
    /// If true, the cameras on the eyeAnchors will be disabled.
    /// \note: The main camera of the game will be used to provide VR rendering. And the tracking space anchors will still be updated to provide reference poses.
    /// </summary>
    public bool disableEyeAnchorCameras = false;


    protected bool _skipUpdate = false;
    protected readonly string trackingSpaceName = "TrackingSpace";
    protected readonly string trackerAnchorName = "TrackerAnchor";
    protected readonly string leftEyeAnchorName = "LeftEyeAnchor";
    protected readonly string centerEyeAnchorName = "CenterEyeAnchor";
    protected readonly string rightEyeAnchorName = "RightEyeAnchor";
    protected readonly string leftHandAnchorName = "LeftHandAnchor";
    protected readonly string rightHandAnchorName = "RightHandAnchor";
    protected readonly string leftControllerAnchorName = "LeftControllerAnchor";
    protected readonly string rightControllerAnchorName = "RightControllerAnchor";
    protected readonly string leftHandAnchorDetachedName = "LeftHandAnchorDetached";
    protected readonly string rightHandAnchorDetachedName = "RightHandAnchorDetached";
    protected readonly string leftControllerInHandAnchorName = "LeftControllerInHandAnchor";
    protected readonly string leftHandOnControllerAnchorName = "LeftHandOnControllerAnchor";
    protected readonly string rightControllerInHandAnchorName = "RightControllerInHandAnchor";
    protected readonly string rightHandOnControllerAnchorName = "RightHandOnControllerAnchor";
    protected Camera _centerEyeCamera;
    protected Camera _leftEyeCamera;
    protected Camera _rightEyeCamera;

    protected Matrix4x4 _previousTrackingSpaceTransform;

    #region Unity Messages

    protected virtual void Awake()
    {
        _skipUpdate = true;
        EnsureGameObjectIntegrity();
    }

    protected virtual void Start()
    {
        UpdateAnchors(true, true);
        Application.onBeforeRender += OnBeforeRenderCallback;
    }

    protected virtual void FixedUpdate()
    {
        if (useFixedUpdateForTracking)
            UpdateAnchors(true, true);
    }

    protected virtual void Update()
    {
        _skipUpdate = false;

        if (!useFixedUpdateForTracking)
            UpdateAnchors(true, true);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        CheckForAnchorsInParent();
#endif
    }

    protected virtual void OnDestroy()
    {
        Application.onBeforeRender -= OnBeforeRenderCallback;
    }

    #endregion

    //////////////////////////
    public float alpha = 1.0f;  // リマッピングパラメータ α

    private Vector3 prevRealHeadRotation = new Vector3(-1f, -1f, -1f);  // 前フレームの現実の頭の回転 (R'rh)
    private Vector3 prevVertualHeadRotation = new Vector3(-1f, -1f, -1f);  // 前フレームの現実の頭の回転 (R'rh)
    private bool InitialRotation = true;  // 前フレームの現実の頭の回転 
    private float RealDistance = 0;
    private float VertualDistance = 0;
    private int frameCounter = 0;  // フレームカウンター
    //////////////////////////////

    protected virtual void UpdateAnchors(bool updateEyeAnchors, bool updateHandAnchors)
    {
        if (!OVRManager.OVRManagerinitialized)
            return;

        EnsureGameObjectIntegrity();

        if (!Application.isPlaying)
            return;

        if (_skipUpdate)
        {
            centerEyeAnchor.FromOVRPose(OVRPose.identity, true);
            leftEyeAnchor.FromOVRPose(OVRPose.identity, true);
            rightEyeAnchor.FromOVRPose(OVRPose.identity, true);

            return;
        }

        bool monoscopic = OVRManager.instance.monoscopic;
        bool hmdPresent = OVRNodeStateProperties.IsHmdPresent();

        OVRPose tracker = OVRManager.tracker.GetPose();

        trackerAnchor.localRotation = tracker.orientation;

        Quaternion emulatedRotation = Quaternion.Euler(-OVRManager.instance.headPoseRelativeOffsetRotation.x,
            -OVRManager.instance.headPoseRelativeOffsetRotation.y,
            OVRManager.instance.headPoseRelativeOffsetRotation.z);

        //Note: in the below code, when using UnityEngine's API, we only update anchor transforms if we have a new, fresh value this frame.
        //If we don't, it could mean that tracking is lost, etc. so the pose should not change in the virtual world.
        //This can be thought of as similar to calling InputTracking GetLocalPosition and Rotation, but only for doing so when the pose is valid.
        //If false is returned for any of these calls, then a new pose is not valid and thus should not be updated.
        if (updateEyeAnchors)
        {
            if (hmdPresent)
            {
                Vector3 centerEyePosition = Vector3.zero;
                Quaternion centerEyeRotation = Quaternion.identity;

                if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.CenterEye, NodeStatePropertyType.Position,
                        OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out centerEyePosition))
                    centerEyeAnchor.localPosition = centerEyePosition;
                if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.CenterEye,
                        NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render,
                        out centerEyeRotation))
                {
                    //////////////////////////////
                    // centerEyeAnchor.localRotation = centerEyeRotation;
                    // ヨー軸回転の増幅処理
                    if (InitialRotation)
                    {
                        centerEyeAnchor.localRotation = centerEyeRotation;
                        prevRealHeadRotation = centerEyeRotation.eulerAngles;
                        prevVertualHeadRotation = prevRealHeadRotation;
                        InitialRotation = false;
                    }
                    else
                    {
                        // 現実の頭の回転を更新
                        Vector3 RealHeadRotation = centerEyeRotation.eulerAngles;

                        // 現実の頭のY軸回転の変化量を計算
                        float deltaYRotation = Mathf.DeltaAngle(prevRealHeadRotation.y, RealHeadRotation.y);
                        // 回転変化量にαを掛ける
                        float adjustedYRotation = deltaYRotation * alpha;
                        RealDistance += deltaYRotation;
                        VertualDistance += adjustedYRotation;
                        // オブジェクトの回転を更新 (Y軸のみ)
                        centerEyeAnchor.localRotation = Quaternion.Euler(RealHeadRotation.x, prevVertualHeadRotation.y + adjustedYRotation, RealHeadRotation.z);
                        // フレームカウンターをインクリメント
                        frameCounter++;
                        if (frameCounter >= 60)
                        {
                            // Debug.Log("prev現実の頭のY軸回転 : "+ prevRealHeadRotation);
                            // Debug.Log("prev仮想の頭のY軸回転 : " +prevVertualHeadRotation);
                            // Debug.Log("Y軸回転の変化量 (deltaYRotation): " + deltaYRotation);
                            Debug.Log("現実の頭のY軸回転 : " + RealHeadRotation.y);
                            Debug.Log("仮想の頭のY軸回転 : " + centerEyeAnchor.localRotation.eulerAngles.y);
                            Debug.Log("ほんとの倍率 : " + alpha);
                            Debug.Log("累積で計算された倍率 : " + VertualDistance / RealDistance);
                            Debug.Log("今計算された倍率 : " + centerEyeAnchor.localRotation.eulerAngles.y / RealHeadRotation.y);
                            frameCounter = 0;
                        }
                        prevRealHeadRotation = RealHeadRotation;
                        prevVertualHeadRotation = centerEyeAnchor.localRotation.eulerAngles;
                    }

                    ///////////////////////////////////
                }
            }
            else
            {
                centerEyeAnchor.localRotation = emulatedRotation;
                centerEyeAnchor.localPosition = OVRManager.instance.headPoseRelativeOffsetTranslation;
            }

            if (!hmdPresent || monoscopic)
            {
                leftEyeAnchor.localPosition = centerEyeAnchor.localPosition;
                rightEyeAnchor.localPosition = centerEyeAnchor.localPosition;
                leftEyeAnchor.localRotation = centerEyeAnchor.localRotation;
                rightEyeAnchor.localRotation = centerEyeAnchor.localRotation;
            }
            else
            {
                Vector3 leftEyePosition = Vector3.zero;
                Vector3 rightEyePosition = Vector3.zero;
                Quaternion leftEyeRotation = Quaternion.identity;
                Quaternion rightEyeRotation = Quaternion.identity;

                if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftEye, NodeStatePropertyType.Position,
                        OVRPlugin.Node.EyeLeft, OVRPlugin.Step.Render, out leftEyePosition))
                    leftEyeAnchor.localPosition = leftEyePosition;
                if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightEye, NodeStatePropertyType.Position,
                        OVRPlugin.Node.EyeRight, OVRPlugin.Step.Render, out rightEyePosition))
                    rightEyeAnchor.localPosition = rightEyePosition;
                if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.LeftEye,
                        NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeLeft, OVRPlugin.Step.Render,
                        out leftEyeRotation))
                    leftEyeAnchor.localRotation = leftEyeRotation;
                if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.RightEye,
                        NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeRight, OVRPlugin.Step.Render,
                        out rightEyeRotation))
                    rightEyeAnchor.localRotation = rightEyeRotation;
            }
        }

        if (updateHandAnchors)
        {
            //Need this for controller offset because if we're on OpenVR, we want to set the local poses as specified by Unity, but if we're not, OVRInput local position is the right anchor
            if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
            {
                Vector3 leftPos = Vector3.zero;
                Vector3 rightPos = Vector3.zero;
                Quaternion leftQuat = Quaternion.identity;
                Quaternion rightQuat = Quaternion.identity;

                if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftHand, NodeStatePropertyType.Position,
                        OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render, out leftPos))
                    leftHandAnchor.localPosition = leftPos;
                if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightHand, NodeStatePropertyType.Position,
                        OVRPlugin.Node.HandRight, OVRPlugin.Step.Render, out rightPos))
                    rightHandAnchor.localPosition = rightPos;
                if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.LeftHand,
                        NodeStatePropertyType.Orientation, OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render,
                        out leftQuat))
                    leftHandAnchor.localRotation = leftQuat;
                if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.RightHand,
                        NodeStatePropertyType.Orientation, OVRPlugin.Node.HandRight, OVRPlugin.Step.Render,
                        out rightQuat))
                    rightHandAnchor.localRotation = rightQuat;
            }
            else
            {
                OVRInput.Controller leftActiveController =
                    OVRInput.GetActiveControllerForHand(OVRInput.Handedness.LeftHanded);
                OVRInput.Controller rightActiveController =
                    OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);

                // it's possible for niether hands or controllers to be active, in that case
                // manually calculate which one we care about giving hands priority.
                if (leftActiveController == OVRInput.Controller.None)
                {
                    if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LHand))
                    {
                        leftActiveController = OVRInput.Controller.LHand;
                    }
                    else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch))
                    {
                        leftActiveController = OVRInput.Controller.LTouch;
                    }
                }
                if (rightActiveController == OVRInput.Controller.None)
                {
                    if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RHand))
                    {
                        rightActiveController = OVRInput.Controller.RHand;
                    }
                    else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch))
                    {
                        rightActiveController = OVRInput.Controller.RTouch;
                    }
                }

                leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(leftActiveController);
                rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(rightActiveController);
                leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(leftActiveController);
                rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(rightActiveController);

                // left
                OVRInput.ControllerInHandState controllerInHandState =
                    OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandLeft);
                if (controllerInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
                {
                    leftHandAnchorDetached.localPosition =
                        OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
                    leftHandAnchorDetached.localRotation =
                        OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
                    leftHandOnControllerAnchor.localPosition = Vector3.zero;
                    leftHandOnControllerAnchor.localRotation = Quaternion.identity;
                }
                else if (controllerInHandState == OVRInput.ControllerInHandState.ControllerInHand)
                {
                    Vector3 leftRelativeHandPosition =
                        trackingSpace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand));
                    leftHandOnControllerAnchor.localPosition =
                        leftHandAnchor.InverseTransformPoint(leftRelativeHandPosition);
                    leftHandOnControllerAnchor.localRotation = Quaternion.Inverse(leftHandAnchor.localRotation) *
                                                               OVRInput.GetLocalControllerRotation(OVRInput.Controller
                                                                   .LHand);
                    leftHandAnchorDetached.localPosition = Vector3.zero;
                    leftHandAnchorDetached.localRotation = Quaternion.identity;
                }
                else
                {
                    leftHandAnchorDetached.localPosition = Vector3.zero;
                    leftHandAnchorDetached.localRotation = Quaternion.identity;
                    leftHandOnControllerAnchor.localPosition = Vector3.zero;
                    leftHandOnControllerAnchor.localRotation = Quaternion.identity;
                }

                // right
                controllerInHandState = OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandRight);
                if (controllerInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
                {
                    rightHandAnchorDetached.localPosition =
                        OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                    rightHandAnchorDetached.localRotation =
                        OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                    rightHandOnControllerAnchor.localPosition = Vector3.zero;
                    rightHandOnControllerAnchor.localRotation = Quaternion.identity;
                }
                else if (controllerInHandState == OVRInput.ControllerInHandState.ControllerInHand)
                {
                    Vector3 rightRelativeHandPosition =
                        trackingSpace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand));
                    rightHandOnControllerAnchor.localPosition =
                        rightHandAnchor.InverseTransformPoint(rightRelativeHandPosition);
                    rightHandOnControllerAnchor.localRotation = Quaternion.Inverse(rightHandAnchor.localRotation) *
                                                                OVRInput.GetLocalControllerRotation(OVRInput.Controller
                                                                    .RHand);
                    rightHandAnchorDetached.localPosition = Vector3.zero;
                    rightHandAnchorDetached.localRotation = Quaternion.identity;
                }
                else
                {
                    rightHandAnchorDetached.localPosition = Vector3.zero;
                    rightHandAnchorDetached.localRotation = Quaternion.identity;
                    rightHandOnControllerAnchor.localPosition = Vector3.zero;
                    rightHandOnControllerAnchor.localRotation = Quaternion.identity;
                }
            }

            trackerAnchor.localPosition = tracker.position;

            OVRPose leftOffsetPose = OVRPose.identity;
            OVRPose rightOffsetPose = OVRPose.identity;
            if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
            {
                leftOffsetPose = OVRManager.GetOpenVRControllerOffset(Node.LeftHand);
                rightOffsetPose = OVRManager.GetOpenVRControllerOffset(Node.RightHand);

                //Sets poses of left and right nodes, local to the tracking space.
                OVRManager.SetOpenVRLocalPose(trackingSpace.InverseTransformPoint(leftControllerAnchor.position),
                    trackingSpace.InverseTransformPoint(rightControllerAnchor.position),
                    Quaternion.Inverse(trackingSpace.rotation) * leftControllerAnchor.rotation,
                    Quaternion.Inverse(trackingSpace.rotation) * rightControllerAnchor.rotation);
            }

            rightControllerAnchor.localPosition = rightOffsetPose.position;
            rightControllerAnchor.localRotation = rightOffsetPose.orientation;
            leftControllerAnchor.localPosition = leftOffsetPose.position;
            leftControllerAnchor.localRotation = leftOffsetPose.orientation;
        }

#if USING_XR_SDK
#if UNITY_2020_3_OR_NEWER
        if (OVRManager.instance.LateLatching)
        {
            XRDisplaySubsystem displaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
            if (displaySubsystem != null)
            {
                displaySubsystem.MarkTransformLateLatched(centerEyeAnchor.transform,
                    XRDisplaySubsystem.LateLatchNode.Head);
                displaySubsystem.MarkTransformLateLatched(leftHandAnchor, XRDisplaySubsystem.LateLatchNode.LeftHand);
                displaySubsystem.MarkTransformLateLatched(rightHandAnchor, XRDisplaySubsystem.LateLatchNode.RightHand);
            }
        }
#endif
#endif
        RaiseUpdatedAnchorsEvent();
        CheckForTrackingSpaceChangesAndRaiseEvent();
    }

    protected virtual void OnBeforeRenderCallback()
    {
        if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus) //Restrict late-update to only Oculus devices
        {
            bool controllersNeedUpdate = OVRManager.instance.LateControllerUpdate;
#if USING_XR_SDK
            //For the XR SDK, we need to late update head pose, not just the controllers, because the functionality
            //is no longer built-in to the Engine. Under legacy, late camera update is done by default. In the XR SDK, you must use
            //Tracked Pose Driver to get this by default, which we do not use. So, we have to manually late update camera poses.
            UpdateAnchors(true, controllersNeedUpdate);
#else
            if (controllersNeedUpdate)
                UpdateAnchors(false, true);
#endif
        }
    }

    /// <summary>
    /// Checks for changes to <see cref="trackingSpace"/>'s transform and, if it has changed, raises the
    /// <see cref="TrackingSpaceChanged"/> event.
    /// </summary>
    /// <remarks>
    /// This method compares the <see cref="trackingSpace"/> transform's `localToWorldMatrix` matrix against its value
    /// during the last invocation of this method.
    ///
    /// If <see cref="trackingSpace"/> is `null`, this method has no effect.
    /// </remarks>
    protected virtual void CheckForTrackingSpaceChangesAndRaiseEvent()
    {
        if (trackingSpace == null) return;

        var currentLocalToWorld = trackingSpace.localToWorldMatrix;
        var shouldRaiseEvent = TrackingSpaceChanged != null &&
                               !_previousTrackingSpaceTransform.Equals(currentLocalToWorld);
        _previousTrackingSpaceTransform = currentLocalToWorld;

        if (shouldRaiseEvent)
        {
            TrackingSpaceChanged(trackingSpace);
        }
    }

    protected virtual void RaiseUpdatedAnchorsEvent()
    {
        if (UpdatedAnchors != null)
        {
            UpdatedAnchors(this);
        }
    }

    public virtual void EnsureGameObjectIntegrity()
    {
        bool monoscopic = OVRManager.instance != null ? OVRManager.instance.monoscopic : false;

        if (trackingSpace == null)
        {
            trackingSpace = ConfigureAnchor(null, trackingSpaceName);
            _previousTrackingSpaceTransform = trackingSpace.localToWorldMatrix;
        }

        if (leftEyeAnchor == null)
            leftEyeAnchor = ConfigureAnchor(trackingSpace, leftEyeAnchorName);

        if (centerEyeAnchor == null)
            centerEyeAnchor = ConfigureAnchor(trackingSpace, centerEyeAnchorName);

        if (rightEyeAnchor == null)
            rightEyeAnchor = ConfigureAnchor(trackingSpace, rightEyeAnchorName);

        if (leftHandAnchor == null)
            leftHandAnchor = ConfigureAnchor(trackingSpace, leftHandAnchorName);

        if (rightHandAnchor == null)
            rightHandAnchor = ConfigureAnchor(trackingSpace, rightHandAnchorName);

        if (leftHandAnchorDetached == null)
            leftHandAnchorDetached = ConfigureAnchor(trackingSpace, leftHandAnchorDetachedName);

        if (rightHandAnchorDetached == null)
            rightHandAnchorDetached = ConfigureAnchor(trackingSpace, rightHandAnchorDetachedName);

        if (leftControllerInHandAnchor == null)
            leftControllerInHandAnchor = ConfigureAnchor(leftHandAnchor, leftControllerInHandAnchorName);

        if (leftHandOnControllerAnchor == null)
            leftHandOnControllerAnchor = ConfigureAnchor(leftControllerInHandAnchor, leftHandOnControllerAnchorName);

        if (rightControllerInHandAnchor == null)
            rightControllerInHandAnchor = ConfigureAnchor(rightHandAnchor, rightControllerInHandAnchorName);

        if (rightHandOnControllerAnchor == null)
            rightHandOnControllerAnchor = ConfigureAnchor(rightControllerInHandAnchor, rightHandOnControllerAnchorName);

        if (trackerAnchor == null)
            trackerAnchor = ConfigureAnchor(trackingSpace, trackerAnchorName);

        if (leftControllerAnchor == null)
            leftControllerAnchor = ConfigureAnchor(leftHandAnchor, leftControllerAnchorName);

        if (rightControllerAnchor == null)
            rightControllerAnchor = ConfigureAnchor(rightHandAnchor, rightControllerAnchorName);

        if (_centerEyeCamera == null || _leftEyeCamera == null || _rightEyeCamera == null)
        {
            _centerEyeCamera = centerEyeAnchor.GetComponent<Camera>();
            _leftEyeCamera = leftEyeAnchor.GetComponent<Camera>();
            _rightEyeCamera = rightEyeAnchor.GetComponent<Camera>();

            if (_centerEyeCamera == null)
            {
                _centerEyeCamera = centerEyeAnchor.gameObject.AddComponent<Camera>();
                _centerEyeCamera.tag = "MainCamera";
            }

            if (_leftEyeCamera == null)
            {
                _leftEyeCamera = leftEyeAnchor.gameObject.AddComponent<Camera>();
                _leftEyeCamera.tag = "MainCamera";
            }

            if (_rightEyeCamera == null)
            {
                _rightEyeCamera = rightEyeAnchor.gameObject.AddComponent<Camera>();
                _rightEyeCamera.tag = "MainCamera";
            }

            _centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            _leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
            _rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
        }

        if (monoscopic && !OVRPlugin.EyeTextureArrayEnabled)
        {
            // Output to left eye only when in monoscopic mode
            if (_centerEyeCamera.stereoTargetEye != StereoTargetEyeMask.Left)
            {
                _centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
            }
        }
        else
        {
            if (_centerEyeCamera.stereoTargetEye != StereoTargetEyeMask.Both)
            {
                _centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            }
        }

        if (disableEyeAnchorCameras)
        {
            _centerEyeCamera.enabled = false;
            _leftEyeCamera.enabled = false;
            _rightEyeCamera.enabled = false;
        }
        else
        {
            // disable the right eye camera when in monoscopic mode
            if (_centerEyeCamera.enabled == usePerEyeCameras ||
                _leftEyeCamera.enabled == !usePerEyeCameras ||
                _rightEyeCamera.enabled == !(usePerEyeCameras && (!monoscopic || OVRPlugin.EyeTextureArrayEnabled)))
            {
                _skipUpdate = true;
            }

            _centerEyeCamera.enabled = !usePerEyeCameras;
            _leftEyeCamera.enabled = usePerEyeCameras;
            _rightEyeCamera.enabled = (usePerEyeCameras && (!monoscopic || OVRPlugin.EyeTextureArrayEnabled));
        }
    }

    protected virtual Transform ConfigureAnchor(Transform root, string name)
    {
        Transform anchor = (root != null) ? root.Find(name) : null;

        if (anchor == null)
        {
            anchor = transform.Find(name);
        }

        if (anchor == null)
        {
            anchor = new GameObject(name).transform;
        }

        anchor.name = name;
        anchor.parent = (root != null) ? root : transform;
        anchor.localScale = Vector3.one;
        anchor.localPosition = Vector3.zero;
        anchor.localRotation = Quaternion.identity;

        return anchor;
    }

    public virtual Matrix4x4 ComputeTrackReferenceMatrix()
    {
        if (centerEyeAnchor == null)
        {
            Debug.LogError("centerEyeAnchor is required");
            return Matrix4x4.identity;
        }

        // The ideal approach would be using UnityEngine.VR.VRNode.TrackingReference, then we would not have to depend on the OVRCameraRig. Unfortunately, it is not available in Unity 5.4.3

        OVRPose headPose = OVRPose.identity;

        Vector3 pos;
        Quaternion rot;
        if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.Head, NodeStatePropertyType.Position,
                OVRPlugin.Node.Head, OVRPlugin.Step.Render, out pos))
            headPose.position = pos;
        if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.Head, NodeStatePropertyType.Orientation,
                OVRPlugin.Node.Head, OVRPlugin.Step.Render, out rot))
            headPose.orientation = rot;

        OVRPose invHeadPose = headPose.Inverse();
        Matrix4x4 invHeadMatrix = Matrix4x4.TRS(invHeadPose.position, invHeadPose.orientation, Vector3.one);

        Matrix4x4 ret = centerEyeAnchor.localToWorldMatrix * invHeadMatrix;

        return ret;
    }

    protected void CheckForAnchorsInParent()
    {
        void Check<T>(Transform node) where T : MonoBehaviour
        {
            var anchor = node.GetComponent<T>();
            if (anchor && anchor.enabled)
            {
                anchor.enabled = false;
                Debug.LogError(
                    $"The {typeof(T).Name} '{anchor.name}' is a parent of the {nameof(CameraRig2)} '{name}', which is not allowed. An {typeof(T).Name} may not be the parent of an {nameof(CameraRig2)} because the {nameof(CameraRig2)} defines the tracking space for the anchor, and its transform is relative to the {nameof(CameraRig2)}.");
            }
        }

        var parent = transform.parent;
        while (parent)
        {
            Check<OVRSpatialAnchor>(parent);
#pragma warning disable CS0618 // Type or member is obsolete
            Check<OVRSceneAnchor>(parent);
#pragma warning restore CS0618 // Type or member is obsolete
            parent = parent.parent;
        }
    }
}
