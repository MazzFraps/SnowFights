using UnityEngine;
using System.Collections;

public class TimeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.I)) {
			Time.timeScale -= 0.1f;
			Debug.Log(Time.timeScale);
		} else if(Input.GetKeyDown(KeyCode.O)) {
			Time.timeScale += 0.1f;
			Debug.Log(Time.timeScale);
		}
	}
}
