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
        //this.transform.position = new Vector3(Random.Range(-10, 9), -1.5f, 1f);
        this.transform.position = new Vector3(Mathf.Sin(Time.time * 10) * 10, 0, 0);
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
