using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {


	public float maxPoints;
	public float currentPoints;
	public Slider progressBar;

	// Use this for initialization
	void Start () {
	 	currentPoints = 0;
	 	progressBar.maxValue = maxPoints;
	}
	
	// Update is called once per frame
	void Update () {
		progressBar.value = currentPoints;
	}
}