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

    private bool pregnant = false;
    private GameObject partner;
    private GameObject territory;
    private GameObject[] children;
    private GameObject[] parents;
    private GameObject[] oldRelations;

    private enum States {idl, changeTerritory, fight, procreate, snuffling, dead};
    private States state = States.idl;
    public wolf()
    {
    }
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        //get older
        switch (state)
        {
            case States.idl:
                //if not hungry and max hp wolf gets strogner
                //if hp not ful recover hp
                break;
            case States.changeTerritory:
                //change the group of this wolf and the other wolfs
                //navigation mesh go to new group
                break;
            case States.fight:
                //calculate fighting power maybe survive with dmg
                state = States.idl;
                //or
                state = States.dead;
                break;
            case States.procreate:
                //create new Object after intervall
                break;
            case States.snuffling:
                //if not successfull start fight
                state = States.fight;
                //if successfull add new group member
                state = States.idl;
                break;
            case States.dead:
                //destroy object
                break;
        }
	}
}
