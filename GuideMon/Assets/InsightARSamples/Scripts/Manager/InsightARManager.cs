/// <summary>
/// ARController.cs
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Xml;
using UnityEngine.SceneManagement;

using InsightAR;
using InsightAR.Internal;


public class InsightARManager  : MonoBehaviour
{
    #region VARIABLE

    [SerializeField]
    private bool QuitOnEscOrBack = false;
    [SerializeField]
    private bool autoStartAR = false;
    [SerializeField]
    private GameObject _axis;
    [SerializeField]
    private GameObject _indicator;
    [SerializeField]
    private GameObject _cube;
    [SerializeField]
    private Material _redColorMaterial0 = null;
    [SerializeField]
    private Camera m_ARCamera;
    [SerializeField]
    private GameObject _planePrefab;

    protected InsightARInterface m_ARInterface;

    private bool isRunningAR = false;

    private bool _enableShowIndicator = true;

    private string _algPath = "";
    private int _videoWidth0 = 0;
    private int _videoHeight0 = 0;


    private float hitDist = 0.5f;

    private Text labelStatus;
    private Text labelScene;

    private List<GameObject> m_AllPlanes = new List<GameObject>();

    #endregion

    #region MONO_LIFECYCLE

    void OnEnable()
    {

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #if UNITY_ANDROID
		QuitOnEscOrBack = true;
        #endif


        if (m_ARCamera == null)
            m_ARCamera = GetComponent<Camera>();

        // Fallback to main camera
        if (m_ARCamera == null)
            m_ARCamera = Camera.main;

        if (m_ARInterface == null)
            m_ARInterface = InsightARInterface.GetInterface();

        if (autoStartAR)
            DoStartAR();

    }

    void OnDisable()
    {
        if (isRunningAR)
        {
            m_ARInterface.StopAR();
            _enableShowIndicator = false;
            InsightARInterface.planeAddedAction -= OnPlaneAdded;
            InsightARInterface.planeUpdatedAction -= OnPlaneUpdated;
            InsightARInterface.planeRemovedAction -= OnPlaneRemoved;
        }
    }

    void Update()
    {

        #if UNITY_ANDROID
		if (QuitOnEscOrBack && Input.GetKeyDown (KeyCode.Escape)){
			DoStopAR();
			SceneManager.LoadScene ("login");
		}
        #endif

        if (!isRunningAR)
            return;

        m_ARInterface.Update();

        updateStatusLabel();

        InsightARState arState = m_ARInterface.CurrentState;

        if (arState == InsightARState.Tracking || arState == InsightARState.Track_Limited)
        {
            if (_axis != null)
            {
                _axis.SetActive(true);
            }

            if (_enableShowIndicator)
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 hitPos = camPos + Camera.main.transform.forward * hitDist;
                if (_indicator != null)
                {
                    _indicator.SetActive(true);
                    _indicator.transform.position = hitPos;
                    MeshRenderer r = _indicator.GetComponent<MeshRenderer>();
                    if (r != null)
                    {
                        r.sharedMaterial.SetColor("_Color", Color.white);
                    }
                }
            }
            else
            {
                if (_indicator != null)
                {
                    _indicator.SetActive(false);
                }
            }
        }
        else
        {
            if (_axis != null)
            {
                _axis.SetActive(false);
            }
            if (_cube != null)
            {
                _cube.SetActive(false);
            }
            if (_indicator != null)
            {
                _indicator.SetActive(false);
            }
        }

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                InsightARUserHitAnchor userHitAnchor = m_ARInterface.GetHitTestResult(touch);
                if (userHitAnchor.isValid == 1)
                {
                    _enableShowIndicator = false;
                    if (_cube != null)
                    {
                        _cube.SetActive(true);
                        _cube.transform.position = userHitAnchor.position;
                        _cube.transform.rotation = userHitAnchor.rotation;
                    }
                }
            }
        }
    }

    public void OnPreRender()
    {
        if (isRunningAR)
        {
            m_ARInterface.UpdateARBackground();
        }
    }

    void OnPostRender()
    {
        /*
		#if UNITY_ANDROID
		string tSceneName = PlayerPrefs.GetString ("SceneName");
		if(!tSceneName.Equals("scene9")){
			return;
		}
		if( !_running){
			return;
		}
		InsightARState arState = (InsightARState)trackResult.state;
		if(arState < InsightARState.InsightARState_Detecting){
			return;
		}
		string lineStr = InsightARPluginFunctions.get3DLine();

		if(!lineStr.Contains(" ")){
			return;
		}
		string [] coposes = lineStr.Split(' ');
		if(coposes.Length <2){
			return;
		}
		float[] poi = new float[coposes.Length];
		for(int i = 0;i < coposes.Length;i++){
			poi[i] = float.Parse(coposes[i]);
		}

		float alignedHeight = (float)_videoWidth0 * (float)Screen.height / (float)Screen.width ;
		Vector3[] points = new Vector3[poi.Length / 2];
		for(int i = 0;i < points.Length;i++){

			points[i].x = poi[i*2 + 0] / _videoWidth0;
			points[i].y = (alignedHeight - 2f * poi[i*2 + 1] + _videoHeight0) / (alignedHeight * 2f);
			points[i].z = 0.0f;
		}


		GL.PushMatrix();
		_redColorMaterial0.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.LINES);
		for(int i = 0;i < points.Length / 2;i++){
			GL.Vertex(points[i*2 + 0]);
			GL.Vertex(points[i*2 + 1]);
		}
		GL.End();
		GL.PopMatrix();
		#endif
		*/
    }

    #endregion //MONO_LIFECYCLE

    #region CUSTOM_FUNC

    private void DoStartAR()
    {
		#if UNITY_IOS
		_algPath = Application.streamingAssetsPath + "/" + "scene12" + "/config";
		#elif UNITY_ANDROID
		_algPath = "/storage/emulated/0/InsightAR/" + "scene12" + "/config";
		#endif

        InsightARSettings setting = new InsightARSettings()
        {
            configPath = _algPath,
            appKey = "AR-905E-782934EC990A-a-f",
            appSecret = "1AfDaV7ALl"
        };
        m_ARInterface.StartAR(setting);
        m_ARInterface.SetupCamera(m_ARCamera);
        isRunningAR = true;

        InsightARInterface.planeAddedAction += OnPlaneAdded;
        InsightARInterface.planeUpdatedAction += OnPlaneUpdated;
        InsightARInterface.planeRemovedAction += OnPlaneRemoved;

        if (_algPath.Equals(""))
        {
            OnChangeARBtn();
        }

    }

    private void DoResetAR(string path)
    {
        if (m_AllPlanes.Count > 0)
        {
            foreach (GameObject obj in m_AllPlanes)
            {
                GameObject.Destroy(obj);
            }
            m_AllPlanes.Clear();
        }
        m_ARInterface.ResetAR(path);
        _enableShowIndicator = true;
    }


    private void DoStopAR()
    {
        if (!isRunningAR)
        {
            return;
        }
        m_ARInterface.StopAR();

        isRunningAR = false;
    }



    private void updateStatusLabel()
    {
        if (labelStatus == null)
        {
            GameObject obj = GameObject.Find("Canvas/LabelStatus");
            if (obj != null)
            {
                labelStatus = obj.GetComponent<Text>();
                if (labelStatus == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        labelStatus.text = "Status:" + m_ARInterface.CurrentState;
        if (m_ARInterface.CurrentState == InsightARState.Init_Fail)
        {
            labelStatus.text += "\n";
        }
    }

    private void updateSceneLabel(string name)
    {
        if (labelScene == null)
        {
            GameObject obj = GameObject.Find("Canvas/LabelScene");
            if (obj != null)
            {
                labelScene = obj.GetComponent<Text>();
                if (labelScene == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        labelScene.text = "Scene:" + name;
    }

    private void OnPlaneAdded(InsightARPlaneAnchor plane)
    {
		Debug.Log("-ar- OnPlaneAdded:PlaneAnchor("
			+plane.center.ToString("f2")+","+plane.rotation.ToString("f2")
			+plane.identifier+")");
        GameObject go = CreatePlaneInScene(plane);
        m_AllPlanes.Add(go);
    }

    private void OnPlaneUpdated(InsightARPlaneAnchor plane)
    {
        GameObject go = GameObject.Find(plane.identifier);
        UpdatePlaneWithAnchorTransform(go, plane);
    }

    private void OnPlaneRemoved(InsightARPlaneAnchor plane)
    {
        GameObject go = GameObject.Find(plane.identifier);
        m_AllPlanes.Remove(go);
        GameObject.Destroy(go);
    }

    private GameObject CreatePlaneInScene(InsightARPlaneAnchor arPlaneAnchor)
    {
        GameObject plane;
        if (_planePrefab != null)
        {
            plane = GameObject.Instantiate(_planePrefab);
            plane.transform.localScale = Vector3.one;
            plane.transform.localPosition = Vector3.zero;
            plane.transform.localRotation = Quaternion.identity;
        }
        else
        {
            plane = new GameObject(); //put in a blank gameObject to get at least a transform to manipulate
        }

        plane.name = arPlaneAnchor.identifier;

        return UpdatePlaneWithAnchorTransform(plane, arPlaneAnchor);
    }

    private GameObject UpdatePlaneWithAnchorTransform(GameObject plane, InsightARPlaneAnchor arPlaneAnchor)
    {
        if (plane == null)
            return null;
        //do coordinate conversion from ARKit to Unity
        plane.transform.localPosition = arPlaneAnchor.center;
        plane.transform.localRotation = arPlaneAnchor.rotation;
        plane.transform.localScale = arPlaneAnchor.extent;

        return plane;
    }

    #endregion //CUSTOM_FUNC

    #region UI_FUNC

    public void OnBackBtn()
    {
        DoStopAR();
        SceneManager.LoadScene("login");
    }

    public void OnResetBtn()
    {
        DoResetAR("");
    }

    public void OnChangeARBtn()
    {
        string tSceneName = PlayerPrefs.GetString("SceneName");
        int sceneNumber = 0;
        if (tSceneName == null || tSceneName.Equals(""))
        {
            sceneNumber = 1;
        }
        else
        {
            sceneNumber = int.Parse(tSceneName.Substring(5, tSceneName.Length - 5)) + 1;
        }
        if (sceneNumber == 16)
        {
            sceneNumber = 1;
        }
        #if UNITY_IOS
        _algPath = Application.streamingAssetsPath + "/scene" + sceneNumber + "/config";
        #elif UNITY_ANDROID
		_algPath = "/storage/emulated/0/InsightAR/scene" + sceneNumber + "/config";
        #endif
        PlayerPrefs.SetString("SceneName", "scene" + sceneNumber);
        updateSceneLabel("scene" + sceneNumber);
        DoResetAR(_algPath);
    }

    public void OnSwitchCameraBtn()
    {
        m_ARInterface.SwitchCamera();
    }

    public void OnPlaceBtn()
    {

        InsightARUserHitAnchor userHitAnchor = m_ARInterface.GetHitTestResult(0.5f, 0.5f);
        if (userHitAnchor.isValid == 1)
        {
            _enableShowIndicator = false;
            if (_cube != null)
            {
                _cube.SetActive(true);
                _cube.transform.position = userHitAnchor.position;
            }
        }
    }

    #endregion //UI_FUNC



}
