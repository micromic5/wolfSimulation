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
    public int age;
    [SerializeField]
    public int hunger;
    [SerializeField]
    private int strengh;
    [SerializeField]
    private int aggression;

    public GameObject prefab;
    public bool pregnant = false;
    private GameObject father = null;
    private int pregnancyTime = 0;
    private int lastTimePregnant = -300;
    public GameObject partner;
    public GameObject territory;
    public group group;
    public group newGroup = null;
    public List<GameObject> children;
    public GameObject[] parents = new GameObject[2];
    public GameObject[] oldRelations;
    public bool withParents = true;
    
    public enum States {idl, changeTerritory, fight, snuffling, dead, outOfGame};
    public States state = States.idl;
    private bool updateWolf = false;
    private int lastYear = 0;
    private bool yearPassed = false;
    private wolf enemy;
    private wolf aggressionWolf;
    private bool newborn = false;
    private static int cloneCounter = 0;
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
        if (state != States.outOfGame)
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
                if (pregnant)
                {
                    pregnancyTime++;
                }
                switch (state)
                {
                    case States.idl:
                        feed();
                        //check if groups are set correct
                        if (newGroup != null)
                        {
                            group = newGroup;
                            if (!newGroup.currentGroup.Contains(gameObject))
                            {
                                group.currentGroup.Remove(gameObject);
                                newGroup.currentGroup.Add(gameObject);
                            }
                            newGroup = null;
                            GetComponentInChildren<Renderer>().material.color = group.color;
                        }
                        //can't change territory if the wolf is pregnant
                        if (!pregnant)
                        {
                            changeTerritoryDecision();
                            //decide if wolf should get pregnant
                            if (isMale == false && partner != null && lastTimePregnant + 300 < (int)Mathf.Floor(timeDisplay.time))
                            {
                                //if enough food
                                if (territory.GetComponent<territory>().food > group.currentGroup.Count * 2)
                                {
                                    //higher chance to get pregnant if only two wolfs are left
                                    if (group.currentGroup.Count == 2)
                                    {
                                        //every day chance of 4% to get pregnant,20days chance of 55%, every year chance of 99.99%
                                        if (Random.Range(0, 25) == 0)
                                        {
                                            pregnant = true;
                                            father = partner;
                                        }
                                    }
                                    else
                                    {
                                        //every day chance of 1% to get pregnant,20days chance of 18%, every year chance of 97.5%
                                        if (Random.Range(0, 100) == 0)
                                        {
                                            pregnant = true;
                                            father = partner;
                                        }
                                    }
                                }//not enough food
                                else
                                {
                                    //every day chance of 0.2% to get pregnant,20days chance of 4%, every year chance of 51.84%
                                    if (Random.Range(0, 500) == 0)
                                    {
                                        pregnant = true;
                                        father = partner;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //giving birth
                            if (pregnancyTime > 65)
                            {
                                if (Random.Range(0, 3) == 0)
                                {
                                    givBirth();
                                }
                            }
                        }
                        //on year change check if possible partner is in group which is not a brother or sister or parent and the wolf is not pregnant
                        if (yearPassed && !pregnant)
                        {
                            findPartner();
                        }
                        //chance to die from sickness higher in the first 2 years
                        if (age <= 2)
                        {
                            //   0.2% every day, 76.81% chance to die in 2 years from sickness,  better chances if strengh and hp are high
                            if (Random.Range(0, 500 + strengh * 100 + hp * 10) == 0)
                            {
                                state = States.dead;
                                Debug.Log(gameObject + " :death through sickness younger than 2");
                            }
                        }
                        else
                        {
                            //   0.01369863% every day, 39.34% chance to die in 10 years from sickness
                            if (Random.Range(0, 7300 + strengh * 20 + hp * 10) == 0)
                            {
                                state = States.dead;
                                Debug.Log(gameObject + " :death through sickness older than 2");
                            }
                        }
                        //chance to die from age
                        if (age > 10)
                        {
                            //  0.2% every day, 51,84% every year, 88,83% every 3 years to die from age
                            if (Random.Range(0, 500) == 0)
                            {
                                state = States.dead;
                                Debug.Log(gameObject + " :death from age in the age of :" + age);
                            }
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
                    case States.snuffling:
                        feed();
                        //wait until wolf is in new territory
                        if (newGroup != null)
                        {
                            makeContact();
                        }
                        break;
                    case States.dead:
                        //Debug.Log(gameObject.name + " dieded");
                        if (partner != null)
                        {
                            partner.GetComponent<wolf>().partner = null;
                            partner = null;
                        }
                        while (group.currentGroup.Contains(gameObject))
                        {
                            group.currentGroup.Remove(gameObject);
                        }
                        Destroy(transform.Find("Cube").gameObject);
                        territory.GetComponent<territory>().wolfsInterritory.Remove(gameObject);
                        state = States.outOfGame;
                        hp = 0;
                        //destroy object
                        break;
                    case States.outOfGame:
                        break;
                }
                yearPassed = false;
            }
        }
    }

    private void givBirth()
    {
        pregnant = false;
        pregnancyTime = 0;
        lastTimePregnant = (int)Mathf.Floor(timeDisplay.time);
        int childCount = Random.Range(1, 7);
        for (int i = 0; i < childCount; i++)
        {
            cloneCounter++;
            GameObject newWolf = Instantiate(prefab, new Vector3(transform.position.x + Random.Range(0, 10), 0,
                transform.position.z + Random.Range(0, 10)), Quaternion.identity);
            newWolf.name = newWolf.name + " " + cloneCounter;
            wolf wolfScript = newWolf.GetComponent<wolf>();
            wolfScript.hp = 10;
            wolfScript.maxHp = 10;
            wolfScript.isMale = Random.Range(0, 2) == 1;
            wolfScript.age = 0;
            wolfScript.hunger = 0;            
            wolfScript.strengh = 0;
            wolfScript.aggression = 0;
            wolfScript.prefab = prefab;
            wolfScript.pregnant = false;
            wolfScript.partner = null;
            wolfScript.group = group;
            group.currentGroup.Add(newWolf);
            wolfScript.territory = territory;
            newWolf.GetComponent<navigation>().target = territory.GetComponent<territory>().GetComponentInChildren<Transform>();
            wolfScript.newGroup = null;
            wolfScript.children = null;
            wolfScript.parents[0] = gameObject;
            wolfScript.parents[1] = father;            
            wolfScript.withParents = true;
            wolfScript.state = States.idl;
            wolfScript.children = new List<GameObject>();
            wolfScript.newborn = true;
            //calculate new strengh
            int strenghRandomness = Random.Range(0, 11) - 5;
            int fatherMulti = father.GetComponent<wolf>().strengh > strengh?2:1;
            int motherMulti = fatherMulti == 2 ? 1 : 2;
            if ((strengh * motherMulti + father.GetComponent<wolf>().strengh * fatherMulti) / 3 + strenghRandomness > 0)
            {
                wolfScript.strengh = (strengh * motherMulti + father.GetComponent<wolf>().strengh * fatherMulti) / 3 + strenghRandomness;
            }
            //Debug.Log("mother:"+strengh+"    father:"+father.GetComponent<wolf>().strengh+"    new:"+((strengh * motherMulti + father.GetComponent<wolf>().strengh * fatherMulti) / 3 + strenghRandomness));
            //calculate new aggression
            int aggressionRandomness = Random.Range(0, 5) - 2;
            if ((aggression + father.GetComponent<wolf>().aggression) / 2 + aggressionRandomness > 0)
            {
                wolfScript.aggression = (aggression + father.GetComponent<wolf>().strengh) / 2 + aggressionRandomness;
            }
            //calculate new hp
            int maxHpRandomness = Random.Range(0, 7) - 3;
            if ((maxHp + father.GetComponent<wolf>().maxHp) / 2 + maxHpRandomness > 10)
            {
                wolfScript.maxHp = (maxHp + father.GetComponent<wolf>().maxHp) / 2 + maxHpRandomness;
                wolfScript.hp = wolfScript.maxHp;
            }
            //instantiate new wolf
            newWolf.GetComponentInChildren<Renderer>().material.color = group.color;
            children.Add(newWolf);
        }
        father = null;
    }

    //fight
    private void fight()
    {
        if (newGroup.currentGroup.Count > 0)
        {
            //find strongest enemy             
            foreach (GameObject newGroupMember in newGroup.currentGroup)
            {
                if (newGroupMember.GetComponent<wolf>().state == States.idl)
                {
                    if (enemy == null)
                    {
                        enemy = newGroupMember.GetComponent<wolf>();
                    }
                    else
                    {
                        enemy = (enemy.strengh + enemy.hp + enemy.aggression * 2) <
                            newGroupMember.GetComponent<wolf>().strengh +
                            newGroupMember.GetComponent<wolf>().hp +
                            newGroupMember.GetComponent<wolf>().aggression * 2 ?
                            newGroupMember.GetComponent<wolf>() :
                            enemy;
                    }
                }
            }
            if (enemy != null)
            {
                int enemyFightPower = Random.Range(0, 10) + enemy.strengh + enemy.hp;
                int currentWolfFightPower = Random.Range(0, 10) + strengh + hp;
                if (currentWolfFightPower >= enemyFightPower)
                {   //implement chance survival with lower hp
                    enemy.state = States.dead;
                    Debug.Log(enemy + " :death through fight i am the defender");
                    //lower the winners hp
                    int dmg = Random.Range(0, enemy.strengh + enemy.hp);
                    hp = hp < dmg ? 2 : hp - dmg;
                    //reset hunger, maybe eat the dead wolf don't know if that happens
                    hunger = 0;
                    changeGroup();
                }//could implement chance survival with lower hp and go back to the old group
                else
                {
                    state = States.dead;
                    Debug.Log(gameObject + " :death through fight, i am the invader");
                    //lower the winners hp
                    int dmg = Random.Range(0, strengh + hp);
                    enemy.hp = enemy.hp < dmg ? 2 : enemy.hp - dmg;
                    //reset hunger, maybe eat the dead wolf don't know if that happens
                    enemy.hunger = 0;
                }
            }else
            {
                changeGroup();
            }
        }
        else
        {
            changeGroup();
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
        newGroup = null;
        withParents = false;
        state = States.idl;
        //search for new Partner in the new Group
        if (age > 0)
        {
            findPartner();
        }
    }

    //determin if acceptet in a new group or a fight starts
    private void makeContact()
    {
        //check if territory is free
        if (newGroup.currentGroup.Count <= 0)
        {
            changeGroup();
        }
        else if (newGroup.currentGroup.Count == 1 && newGroup.currentGroup[0].GetComponent<wolf>().isMale != isMale)
        {
            changeGroup();
        }
        else
        {            
            //determin the aggression of the new group  
            int groupAggressionLevel = 0;
            foreach (GameObject wolfAgressor in newGroup.currentGroup)
            {
                groupAggressionLevel += wolfAgressor.GetComponent<wolf>().aggression;
            }
            //territory regenerates not enough for one more wolf or the territory has more wolfs than food
            //the aggression of the new group is 5 times higher
            if (newGroup.currentGroup.Count < territory.GetComponent<territory>().regenerationRate ||
                territory.GetComponent<territory>().food < newGroup.currentGroup.Count)
            {
                groupAggressionLevel *= 5;
            }
            int foodFactor = 0;
            if(newGroup.currentGroup.Count * 10 < territory.GetComponent<territory>().food)
            {
                foodFactor = 15;
            }
            //when the wolf is young his chances to get a place in the new gorup are higher
            if (age <= 3)
            {
                if (Random.Range(0, 100 + groupAggressionLevel - foodFactor) <= 75)
                {
                    changeGroup();
                }
                else
                {
                    state = States.fight;
                }
            }
            else if (age <= 8)
            {
                if (Random.Range(0, 100 + groupAggressionLevel - foodFactor) <= 50)
                {
                    changeGroup();
                }
                else
                {
                    state = States.fight;
                }
            }
            else
            {
                if (Random.Range(0, 100 + groupAggressionLevel - foodFactor) <= 25)
                {
                    changeGroup();
                }
                else
                {
                    state = States.fight;
                }
            }
        }
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
                            if (parents[0] != null)
                            {
                                foreach (GameObject c in parents[0].GetComponent<wolf>().children)
                                {
                                    if (w == c)
                                    {
                                        skip = true;
                                    }
                                }
                            }
                            if (parents[1] != null)
                            {
                                foreach (GameObject c in parents[1].GetComponent<wolf>().children)
                                {
                                    if (w == c)
                                    {
                                        skip = true;
                                    }
                                }
                            }                            
                            if (!skip)
                            {
                                //if only 2 wolfs are left they form a pair
                                if(group.currentGroup.Count == 2)
                                {
                                    wolfScript.partner = gameObject;
                                    gameObject.GetComponent<wolf>().partner = w;
                                }
                                //100%  chance if the partner is strong
                                else if (!isMale && wolfScript.strengh > strengh)
                                {
                                    wolfScript.partner = gameObject;
                                    gameObject.GetComponent<wolf>().partner = w;
                                }
                                else if (isMale && wolfScript.strengh + 2 >= strengh)
                                {
                                    if (Random.Range(0, 2) == 1)
                                    {
                                        wolfScript.partner = gameObject;
                                        gameObject.GetComponent<wolf>().partner = w;
                                        // Debug.Log(gameObject.name + "  possiblePartner: " + w.name);
                                    }
                                }
                                else if (Random.Range(0, 7) == 1)
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
                    Debug.Log(gameObject + " :death through hunger");
                }
            }
            else
            {
                hunger++;
            }
        }
    }
}
