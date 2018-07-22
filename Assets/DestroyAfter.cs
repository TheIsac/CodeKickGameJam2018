using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {

    public float TimeInSecounds = 10;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, TimeInSecounds);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
