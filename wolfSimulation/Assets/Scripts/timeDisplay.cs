using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class timeDisplay : MonoBehaviour {
    public static float time = 360;
    private float oldtime = 0;
    public static bool dayChanged = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        time+=Time.deltaTime;
        if(Math.Floor(time) != Math.Floor(oldtime))
        {
            dayChanged = true;
            oldtime = time;
        }else
        {
            dayChanged = false;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(Math.Floor(time).ToString());
    }
}
