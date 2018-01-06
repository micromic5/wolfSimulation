using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class group : MonoBehaviour {

    public GameObject[] currentGroup;

    // Use this for initialization
    void Start () {
        //color of the wolfs of this territory
        Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.1f, 1f);
        foreach (GameObject colorWolf in currentGroup)
        {
            colorWolf.GetComponentInChildren<Renderer>().material.color = color;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
