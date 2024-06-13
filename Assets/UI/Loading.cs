using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {
	public Slider slider;
	public string Level;
	private int slidV;
	private float timer;

	
	void Update () {
		slider.value = slidV;
		timer +=6  * Time.deltaTime;
		if (timer >= 2) {
			slidV += 7;
			timer = 0;
		}
		if (slider.value >= 100) {
			SceneManager.LoadScene (Level);
		}
	}
}
