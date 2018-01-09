using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class territory : MonoBehaviour {
    //1 is the value one wolf needs per day
    public int food = 20;
    public int maximumFood = 150;
    //1 means it regenerates 1 food value per day
    public int regenerationRate = 0;
    // Use this for initialization
    public List<GameObject> wolfsInterritory = new List<GameObject>();
    public group group;
    private bool updateTerritory = false;
    private float timeStart;
    private bool startTime = true;
	void Start () {
        timeStart = timeDisplay.time;
	}

    private void OnTriggerEnter(Collider other)
    {
        foreach (Transform comp in other.GetComponentsInParent<Transform>())
        {
            if (comp.tag == "Wolf" && comp.GetComponent<wolf>().state != wolf.States.outOfGame)
            {
                if(startTime && timeDisplay.time - timeStart > 10)
                {
                    startTime = false;
                }
                if (!startTime)
                {
                   // comp.GetComponent<wolf>().territory = gameObject;
                  /*  if (comp.GetComponent<wolf>().age > 0)
                    {*/
                        comp.GetComponent<wolf>().newGroup = group;
                  //  }
                    comp.GetComponent<navigation>().target = GetComponentInChildren<Transform>();
                }
                comp.GetComponent<wolf>().territory = gameObject;                
                wolfsInterritory.Add(comp.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Transform comp in other.GetComponentsInParent<Transform>())
        {
            if (comp.tag == "Wolf")
            {

                wolfsInterritory.Remove(comp.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (timeDisplay.dayChanged)
        {
            updateTerritory = true;
        }
	}

    private void FixedUpdate()
    {
        if (updateTerritory)
        {
            food -= food>0?wolfsInterritory.Count:0;
            food += food<maximumFood?regenerationRate:0;
            updateTerritory = false;
            if (food < wolfsInterritory.Count && regenerationRate < wolfsInterritory.Count)
            {
                for (int i = 0; i < regenerationRate; i++)
                {
                    int luckyWolf = Random.Range(0, wolfsInterritory.Count);
                    wolfsInterritory[luckyWolf].GetComponent<wolf>().inceraseHP();
                    int hunger = wolfsInterritory[luckyWolf].GetComponent<wolf>().hunger;
                    wolfsInterritory[luckyWolf].GetComponent<wolf>().hunger -= (hunger > 0) ? 1 : 0;
                }
            }            
        }
    }
}
