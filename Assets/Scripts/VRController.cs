using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using Google.XR.Cardboard;
public class VRController : MonoBehaviour
{
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if(IsVREnabled)
        {
            if (Api.IsCloseButtonPressed)
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }

            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Juego")
        {
            if(!IsVREnabled)
            {
                EnterVR();
                Api.MinTriggerHeldPressedTime = 1.0f;
                if(!Api.HasDeviceParams())
                {
                    Api.ScanDeviceParams();
                }
            }
        }
        else
        {
            StopVR();
        }
    }

    private bool IsVREnabled
    {
        get { return XRGeneralSettings.Instance.Manager.isInitializationComplete; }
    }

    private IEnumerator StartVR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }

    private void EnterVR()
    {
        StartCoroutine(StartVR());
        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
    }
    private void StopVR()
    {
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }
}
