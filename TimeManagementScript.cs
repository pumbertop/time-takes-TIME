using UnityEngine;
using System;

public class TimeManagementScript : MonoBehaviour {

	public TimeSpan currentTime;
	public float currentSeconds;

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}

	void Update () {
		currentTime = DateTime.Now.TimeOfDay;
		currentSeconds = (float)currentTime.TotalSeconds;
	}
}
