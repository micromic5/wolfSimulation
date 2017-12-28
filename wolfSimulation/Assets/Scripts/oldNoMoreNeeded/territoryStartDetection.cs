using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class territoryStartDetection : MonoBehaviour {
    private List<GameObject> wolfsInterritory = new List<GameObject>();
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(wolfsInterritory.Count > 0)
        {
          //  this.GetComponentInParent<territory>().setWolfsInTerritory(wolfsInterritory);
            Destroy(gameObject);
        }
	}

    private void OnTriggerStay(Collider other)
    {
        foreach(Transform comp in other.GetComponentsInParent<Transform>())
        {
            if(comp.tag == "Wolf")
            {
                wolfsInterritory.Add(comp.gameObject);
            }
        }
    }

}
