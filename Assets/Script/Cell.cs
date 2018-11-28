using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {
    public bool IsMine;
    public GameObject text;
    public GameObject Explosion;
    public GameObject Flag;
    public Material[] materials;

	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsMine == true)
        {
            if(Random.Range(0,7)==0){
                GameObject obj = Instantiate(Explosion, transform.position, Quaternion.identity);
                Destroy(obj, 2);
                Destroy(this);
            }

        }
    }


}
