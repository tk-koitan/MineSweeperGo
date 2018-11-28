using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {
    public Slider Slider;
    private Text text;
    private string DefaultValue;

	// Use this for initialization
	void Start () {
        Slider = transform.parent.gameObject.GetComponent<Slider>();
        text = GetComponent<Text>();
        DefaultValue = text.text;
	}
	
	// Update is called once per frame
	void Update () {
        text.text = DefaultValue+Slider.value.ToString();
	}
}
