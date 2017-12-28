using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class territory : MonoBehaviour {
    //1 is the value one wolf needs per day
    public int food = 100;
    //1 means it regenerates 1 food value per day
    public int regenerationRate = 0;
    // Use this for initialization
    public List<GameObject> wolfsInterritory = new List<GameObject>();
    private bool updateTerritory = false;
	void Start () {
       /* RaycastHit[] wolfs = Physics.BoxCastAll(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(transform.localScale.x/2, transform.localScale.y/2, transform.localScale.z/2),
            new Vector3(transform.position.x, transform.position.y, transform.position.z)
        );
        Debug.Log(wolfs.Length);
        foreach(RaycastHit wolf in wolfs)
        {
            Debug.Log(wolf.transform.position);
            if (wolf.transform.parent != null)
            {
                Debug.Log(wolf.transform.parent.name);
            }
        }*/
	}

   /* public void setWolfsInTerritory(List<GameObject> wolfs)
    {
       // this.wolfsInterritory = wolfs;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        foreach (Transform comp in other.GetComponentsInParent<Transform>())
        {
            if (comp.tag == "Wolf")
            {
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
            food -= wolfsInterritory.Count;
            food += regenerationRate;
            updateTerritory = false;
        }
    }
}
