using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour {

    public Sprite[] effect;
    int animIndex;

	// Use this for initialization
	void Start () {
        animIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
        animIndex++;
        if (animIndex >= effect.Length)
        {
            //animIndex = 0;
            Destroy(this);
        }
        GetComponent<SpriteRenderer>().sprite = effect[animIndex];
	}
}
