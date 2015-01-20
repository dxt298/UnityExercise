using UnityEngine;
using System.Collections;

public class Object : MonoBehaviour {

    public bool winner;

	// Use this for initialization
	void Start () {
        winner = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (winner)
        {
            this.renderer.material.color = Color.red;
        }
        else
        {
            this.renderer.material.color = Color.white;
        }
	}
}
