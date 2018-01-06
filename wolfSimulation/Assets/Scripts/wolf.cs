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

    public bool pregnant = false;
    public GameObject partner;
    public GameObject territory;
    public group group;
    public GameObject[] children;
    public GameObject[] parents;
    public GameObject[] oldRelations;

    private enum States {idl, changeTerritory, fight, procreate, snuffling, dead};
    private States state = States.idl;
    private bool updateWolf = false;
    private int lastYear = 0;
    private bool yearPassed = false;
    public wolf()
    {
    }
	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update()
    {
        if (timeDisplay.dayChanged)
        {
            updateWolf = true;
        }
    }
    private void FixedUpdate()
    {
        if (updateWolf)
        {            
            //get older
            if (Mathf.Floor(timeDisplay.time) % 365 == 0 && (int)Mathf.Floor(timeDisplay.time) != lastYear)
            {
                lastYear = (int)Mathf.Floor(timeDisplay.time);
                yearPassed = true;
            }
            switch (state)
            {
                case States.idl:
                    //if not hungry and max hp wolf gets strogner
                    //if hp not ful recover hp
                    if (territory.GetComponent<territory>().food > 0)
                    {
                        if (hp < 100)
                        {
                            hp++;
                        }
                    }
                    else
                    {
                        
                        if(hunger > 15)
                        {
                            hp--;
                            if(hp <= 0)
                            {
                                state = States.dead;
                            }
                        }
                        else
                        {
                            hunger++;
                        }
                    }
                    //decide if territory should be changed
                    if (age >= 2 && Mathf.Floor(timeDisplay.time) % 365 > 100 && partner == null)
                    {
                        //between 0 and 10 1% chance to change territory every day, 87.8% chance after 20 days
                        if (hunger > 10 && Random.Range(0, 10) == 1)
                        {
                            state = States.changeTerritory;
                        }
                        //between 0 and 49 2% chance to change territory every day 86.7% chance every 100 days
                        else if (Random.Range(0, 50) == 1)
                        {
                            state = States.changeTerritory;
                        }
                    }
                    else if (age <= 2 && hunger > 10 || partner != null && hunger > 10)
                    {
                        if(Random.Range(0,100) == 99)
                        {
                            state = States.changeTerritory;
                        }
                    }
                    //on year change check if possible partner is in group which is not a brother or sister or parent
                    if (yearPassed)
                    {
                        if (partner == null)
                        {
                            foreach (GameObject w in group.currentGroup)
                            {
                                wolf wolfScript = w.GetComponent<wolf>();
                                //check if same sex
                                if (wolfScript.isMale != isMale) {
                                    //check on wolf himself and its parents
                                    if (w != gameObject)
                                    {
                                        bool skip = false;
                                        //check for parents
                                        foreach (GameObject p in parents)
                                        {
                                            if (w == p)
                                            {
                                                skip = true;
                                            }
                                        }
                                        //check for own childs
                                        foreach (GameObject c in children)
                                        {
                                            if (w == c)
                                            {
                                                skip = true;
                                            }
                                        }
                                        //check for brothers and sisters
                                        foreach (GameObject c in parents[0].GetComponent<wolf>().children)
                                        {
                                            if (w == c)
                                            {
                                                skip = true;
                                            }
                                        }
                                        if (!skip)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                        Debug.Log(gameObject.name + " " + age);
                        age++;
                    }
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
            yearPassed = false;
        }
    }
}
