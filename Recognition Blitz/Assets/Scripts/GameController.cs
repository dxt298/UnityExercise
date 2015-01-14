using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {

    List<GameObject> possibilities;
    int objectsPerRound = 1;
    float delay = 2.0f;

	// Use this for initialization
	void Start () {
        possibilities = GameObject.FindGameObjectsWithTag("Possible").ToList();
        foreach (GameObject go in possibilities)
        {
            go.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(possibilities.Count());
            for (int i = 0; i < objectsPerRound; i++)
            {
                PeekABoo(Pick(possibilities));
            }
        }
	}

    void PeekABoo(GameObject go)
    {
        Instantiate(go, new Vector3(0, 0, 10), Quaternion.identity);
    }

    T Pick<T>(List<T> from)
    {
        if (from.Count == 0) throw new Exception("Cannot pick from an empty list!");
        return (from[Random.Range(0, from.Count)]);
    }
    
}
