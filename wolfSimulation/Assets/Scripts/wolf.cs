using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf : MonoBehaviour {
    [SerializeField]
    private int hp;
    [SerializeField]
    private bool isMale;
    [SerializeField]
    private int age;
    [SerializeField]
    private int hunger;
    [SerializeField]
    private int strengh;
    [SerializeField]
    private int aggression;
    [SerializeField]
    //The order of the Group members is the hirarchie
    private GameObject[] group;

    private bool pregnant = false;
    private GameObject partner;
    private GameObject territory;
    private GameObject[] children;
    private GameObject[] parents;
    private GameObject[] oldRelations;

    public wolf()
    {
    }
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
