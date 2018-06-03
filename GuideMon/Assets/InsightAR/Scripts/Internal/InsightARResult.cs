using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InsightAR.Internal
{

    public enum InsightARState
    {
        Uninitialized = 0,
        Initing = 1,
        Init_OK = 2,
        Init_Fail = 3,
        Detecting = 4,
        Detect_OK = 5,
        Detect_Fail = 6,
        Tracking = 7,
        Track_Limited = 8,
        Track_Lost = 9,
        Track_Fail = 10,
        Track_Stop = 11,
    }

    public enum InsightARInitFailReason
    {
        InsightARInitFailReason_ReasonNone = 0,
        InsightARInitFailReason_ConfigFile_Not_Found = 1,
        InsightARInitFailReason_ConfigFile_Error = 2,
        InsightARInitFailReason_Camera_Error = 3,
        InsightARInitFailReason_IMU_Error = 4,
        InsightARInitFailReason_Device_Unsupported = 5,
        InsightARInitFailReason_AppKey_Secret_Error = 6,
        InsightARInitFailReason_AR_RUNGING = 7
    }

    public enum InsightARDetectFailReason
    {
        InsightARDetectFailReason_ReasonNone,
        InsightARDetectFailReason_NO_Feature,
        InsightARDetectFailReason_Track_Bad,
        InsightARDetectFailReason_Camera_Error,
    }

    public enum InsightARTrackFailReason
    {
        InsightARTrackFailReason_ReasonNone,
        InsightARTrackFailReason_LowLight,
        InsightARTrackFailReason_ExcessiveMotion,
        InsightARTrackFailReason_InsufficientFeatures,
    }
}

namespace InsightAR
{
    using InsightAR.Internal;

    public struct InsightARResult
    {
        public int sceneType;

        public int state;

        public int reason;

        public InsightARCameraParam param;

        public InsightARCameraPose camera;

        public double timestamp;
    }
}
