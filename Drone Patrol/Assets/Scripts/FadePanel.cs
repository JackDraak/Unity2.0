using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadePanel : MonoBehaviour {

	public float fadeInTime= 1f;

	private Image fadePanel;
	private Color currentColor = new Color(0f,0f,0f,1f); // or = Color.black;
	
	void Start() {
		fadePanel = GetComponent<Image>(); if (!fadePanel) Debug.LogError (this + " fadePanel IMAGE failure");
	}
	
	void Update () {
		if (Time.timeSinceLevelLoad < fadeInTime) {
			float alphaChange = Time.deltaTime / fadeInTime;
			currentColor.a -= alphaChange;
			fadePanel.color = currentColor;
		} else {
			gameObject.SetActive(false);
		}
	}
}
