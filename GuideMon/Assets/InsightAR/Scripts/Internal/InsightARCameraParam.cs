using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace InsightAR.Internal
{
	
	public enum AR_CAMERA_FORMAT
	{
		ARCameraFormat_unknow = 0,
		ARCameraFormat_BRGA = 1,
		ARCameraFormat_420f = 2,
		ARCameraFormat_420v = 3,
	};

	public enum AR_CAMERA_SESSIONPRESET
	{
		ARCameraSessionPreset640x480 = 0,
		ARCameraSessionPreset1280x720 = 1,
		ARCameraSessionPreset1920x1080 = 2,
	};

	public enum AR_CAMERA_ORIENTATION
	{
		ARCameraOrientationPortrait = 1,
		ARCameraOrientationPortraitUpsideDown = 2,
		ARCameraOrientationLandscapeRight = 3,
		ARCameraOrientationLandscapeLeft = 4,
	} ;


	public enum AR_CAMERA_POSITION
	{
		ARCameraDevicePositionUnspecified = 0,
		ARCameraDevicePositionBack = 1,
		ARCameraDevicePositionFront = 2,
	};

	public  enum AR_CAMERA_FOCUS
	{
		ARCameraFocusModeLocked = 0,
		ARCameraFocusModeAutoFocus = 1,
		ARCameraFocusModeContinuousAutoFocus = 2,
	} ;

	public enum AR_CAMERA_EXPOSURE
	{
		ARCameraExposureModeLocked = 0,
		ARCameraExposureModeAutoExpose = 1,
		ARCameraExposureModeContinuousAutoExposure = 2,
		ARCameraExposureModeCustom = 3,
	};

	public enum AR_CAMERA_WHITEBALANCE
	{
		ARCameraWhiteBalanceModeLocked = 0,
		ARCameraWhiteBalanceModeAutoWhiteBalance = 1,
		ARCameraWhiteBalanceModeContinuousAutoWhiteBalance = 2,
	} ;

	public enum AR_CAMERA_TORCH
	{
		ARCameraTorchModeOff = 0,
		ARCameraTorchModeOn = 1,
		ARCameraTorchModeAuto = 2,
	} ;

	public enum AR_CAMERA_FLASH
	{
		ARCameraFlashModeOff = 0,
		ARCameraFlashModeOn = 1,
		ARCameraFlashModeAuto = 2,
	} ;

    public struct InsightARCameraParam
    {
        public int width;
        public int height;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] fov;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] focalLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] principalPoint;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] distortionCoefficients;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] projectionMatrix;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] transformationIMU2Camera;

        public int captureFps;

        public AR_CAMERA_FORMAT format;

        public AR_CAMERA_SESSIONPRESET sessionPreset;

        public AR_CAMERA_POSITION devicePosition;

        public AR_CAMERA_ORIENTATION orientation;

        public AR_CAMERA_FOCUS focusMode;
        public float lensPosition;
        public float lensAperture;

        public AR_CAMERA_EXPOSURE exposureMode;
        public float exposureDuration;
        public float exposureBias;
        public float iso;

        public AR_CAMERA_WHITEBALANCE whiteBalanceMode;
        public float temperature;
        public float tint;
		public float ambientIntensity;

        public AR_CAMERA_TORCH torchMode;
        public AR_CAMERA_FLASH flashMode;
    }
}
