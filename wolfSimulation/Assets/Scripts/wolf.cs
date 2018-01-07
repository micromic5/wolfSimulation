using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf : MonoBehaviour {
    [SerializeField]
    private int hp;
    [SerializeField]
    private int maxHp = 20;
    [SerializeField]
    private bool isMale;
    [SerializeField]
    private int age;
    [SerializeField]
    public int hunger;
    [SerializeField]
    private int strengh;
    [SerializeField]
    private int aggression;

    public bool pregnant = false;
    public GameObject partner;
    public GameObject territory;
    public group group;
    public group newGroup;
    public GameObject[] children;
    public List<GameObject> parents;
    public GameObject[] oldRelations;
    public bool withParents = true;
    
    private enum States {idl, changeTerritory, fight, procreate, snuffling, dead};
    private States state = States.idl;
    private bool updateWolf = false;
    private int lastYear = 0;
    private bool yearPassed = false;
    private wolf enemy;

    public wolf()
    {
    }

    public void inceraseHP()
    {
        if (hp < maxHp)
        {
            hp++;
        }
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
            updateWolf = false;      
            //get older
            if (Mathf.Floor(timeDisplay.time) % 365 == 0 && (int)Mathf.Floor(timeDisplay.time) != lastYear)
            {
                lastYear = (int)Mathf.Floor(timeDisplay.time);
                yearPassed = true;
                age++;
            }
            switch (state)
            {
                case States.idl:                   
                    feed();
                    changeTerritoryDecision();
                    //on year change check if possible partner is in group which is not a brother or sister or parent
                    if (yearPassed)
                    {
                        findPartner();
                    }
                    break;
                case States.changeTerritory:
                    feed();
                    GameObject[] possibleNewTerritory = GameObject.FindGameObjectsWithTag("territory");
                    bool foundNewTerritory = false;
                    while (!foundNewTerritory)
                    {
                        GameObject newTerritory = possibleNewTerritory[Random.Range(0, possibleNewTerritory.Length)];
                        if (newTerritory != territory)
                        {
                            foundNewTerritory = true;
                            gameObject.GetComponent<navigation>().target = newTerritory.GetComponentInChildren<Transform>();
                        }
                    }
                    state = States.snuffling;
                    //change the group of this wolf and the other wolfs
                    //navigation mesh go to new group
                    break;
                case States.fight:
                    enemy = null;
                    fight();
                    break;
                case States.procreate:
                    //create new Object after intervall
                    break;
                case States.snuffling:
                    feed();
                    //wait until wolf is in new territory
                    if(newGroup != null)
                    {
                        makeContact();
                    }
                    break;
                case States.dead:
                    Debug.Log(gameObject.name + " dieded");
                    //destroy object
                    break;
            }
            yearPassed = false;
        }
    }
    
    //fight
    private void fight()
    {
        //find strongest enemy             
        foreach (GameObject newGroupMember in newGroup.currentGroup)
        {
            if (enemy == null)
            {
                enemy = newGroupMember.GetComponent<wolf>();
            }
            else
            {
                enemy = (enemy.strengh + enemy.hp) <
                    newGroupMember.GetComponent<wolf>().strengh +
                    newGroupMember.GetComponent<wolf>().hp ?
                    newGroupMember.GetComponent<wolf>() :
                    enemy;
            }
        }
        int enemyFightPower = Random.Range(0, 10) + enemy.strengh + enemy.hp;
        int currentWolfFightPower = Random.Range(0, 10) + strengh + hp;
        if (currentWolfFightPower >= enemyFightPower)
        {   //implement chance survival with lower hp
            enemy.state = States.dead;
            //lower the winners hp
            int dmg = Random.Range(0, enemy.strengh + enemy.hp);
            hp = hp < dmg ? 2 : hp - dmg;
            //reset hunger, maybe eat the dead wolf don't know if that happens
            hunger = 0;
            changeGroup();
            state = States.idl;
        }//implement chance survival with lower hp and go back to the old group
        else
        {
            state = States.dead;
            //lower the winners hp
            int dmg = Random.Range(0, strengh + hp);
            enemy.hp = enemy.hp < dmg ? 2 : enemy.hp - dmg;
            //reset hunger, maybe eat the dead wolf don't know if that happens
            enemy.hunger = 0;
        }
    }

    //decide if territory should be changed
    private void changeTerritoryDecision()
    {        
        if (age >= 2 && partner == null && withParents == true)
        {
            //between 0 and 10 10% chance to change territory every day, 87.8% chance after 20 days
            if (hunger > 10 && Random.Range(0, 10) == 1)
            {
                //Debug.Log("hunger and no partner");
                state = States.changeTerritory;
            }
            //between 0 and 49 2% chance to change territory every day,33% 20days, 86.7% chance every 100 days
            else if (Random.Range(0, 50) == 1)
            {
                //Debug.Log("no hunger no partner, just change to search");
                state = States.changeTerritory;
            }
        }
        else if (age <= 2 && age > 0 && hunger > 10 || partner != null && hunger > 10)
        {
            //maybe lower chance
            //between 0 and 33 3% chance to change territory every day,46% 20days, 90% chance every  75 days
            if (Random.Range(0, 34) == 1)
            {
                //Debug.Log("partner or to young but still hunger");
                state = States.changeTerritory;
            }
        }
        else if (withParents == false && partner == null)
        {
            //between 0 and 769 0.13% chance to change territory every day,2.5% 20days,
            //37 % chance every  year, 76% every 3 years
            if (Random.Range(0, 770) == 1)
            {
                //Debug.Log("searching for Partner from new Family");
                state = States.changeTerritory;
            }
        }
    }

    //succesful integrated into a new group
    private void changeGroup()
    {
        newGroup.currentGroup.Add(gameObject);
        //on Territory change forget about the former partner
        if (partner != null)
        {
            partner.GetComponent<wolf>().partner = null;
            partner = null;
        }
        group.currentGroup.Remove(gameObject);
        group = newGroup;
        GetComponentInChildren<Renderer>().material.color = newGroup.color;
        state = States.idl;
        newGroup = null;
        withParents = false;
        //search for new Partner in the new Group
        findPartner();
    }

    //determin if acceptet in a new group or a fight starts
    private void makeContact()
    {
        //needs some tuning
        int ageIndicator = age <= 3 ? -1 : 1;
        //check how high possibility for a fight is
        float acceptingParameter = Random.Range(0,
            100 - territory.GetComponent<territory>().food +
            newGroup.currentGroup.Count +
            ageIndicator);
        if (true)
        {
            changeGroup();
        }
        else
        {
            state = States.fight;
        }
        /*else
        {
            state = States.fight;
        }*/
        //parameter die ganze zeit viel zu hoch
        //Debug.Log(acceptingParameter);
    }

    //search for a partner inside the group
    private void findPartner()
    {
        if (partner == null)
        {
            foreach (GameObject w in group.currentGroup)
            {
                if (partner == null)
                {
                    wolf wolfScript = w.GetComponent<wolf>();
                    //check if same sex and older than 2 and has no partner
                    if (wolfScript.isMale != isMale &&
                        wolfScript.age >= 2 &&
                        wolfScript.partner == null)
                    {
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
                            if (parents.Count > 0)
                            {
                                foreach (GameObject c in parents[0].GetComponent<wolf>().children)
                                {
                                    if (w == c)
                                    {
                                        skip = true;
                                    }
                                }
                            }
                            if (!skip)
                            {
                                if (Random.Range(0, 2) == 1)
                                {
                                    wolfScript.partner = gameObject;
                                    gameObject.GetComponent<wolf>().partner = w;
                                   // Debug.Log(gameObject.name + "  possiblePartner: " + w.name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //if hp not ful recover hp
    //checks if the territory still has food and if its enough for all the wolfs in the territory
    private void feed()
    {
        if ((territory.GetComponent<territory>().food > 0 &&
                        territory.GetComponent<territory>().food > territory.GetComponent<territory>().wolfsInterritory.Count)
                        || territory.GetComponent<territory>().regenerationRate >= territory.GetComponent<territory>().wolfsInterritory.Count
                        )
        {
            if (hp < maxHp)
            {
                hp++;
            }
            hunger -= (hunger > 0 )? 1 : 0;
        }
        else
        {
            if (hunger > 14)
            {
                hp--;
                if (hp <= 0)
                {
                    state = States.dead;
                }
            }
            else
            {
                hunger++;
            }
        }
    }
}
