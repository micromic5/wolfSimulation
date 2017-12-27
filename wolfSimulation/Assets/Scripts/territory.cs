using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class territory : MonoBehaviour {
    //1 is the value one wolf needs per day
    public int food;
    //1 means it regenerates 1 food value per day
    public int regenerationRate;
    // Use this for initialization
    private List<GameObject> wolfsInterritory;
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

    public void setWolfsInTerritory(List<GameObject> wolfs)
    {
        this.wolfsInterritory = wolfs;
        Debug.Log(wolfsInterritory.Count);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
