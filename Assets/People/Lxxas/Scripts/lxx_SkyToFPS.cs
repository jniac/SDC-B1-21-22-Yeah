using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class lxx_SkyToFPS : MonoBehaviour {
    [SerializeField] private CinemachineVirtualCamera cineCam;
    [SerializeField] private CinemachineVirtualCamera cineSkyCam;

    [SerializeField] private bool skyMode; //par default setté à False

    void Start () {
        cineCam.enabled = !skyMode; //true
        cineSkyCam.enabled = skyMode; //false
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.M)) {
            skyMode = !skyMode;
            Debug.Log ("switch cam Mode " + skyMode);
            cineCam.enabled = !skyMode;
            cineSkyCam.enabled = skyMode;
        }
    }
}