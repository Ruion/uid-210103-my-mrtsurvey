using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToCloseScript : MonoBehaviour {
	private Button restartButton;

	void Awake(){
		restartButton = GetComponent<Button> ();    
		if (restartButton) {
            restartButton.onClick.AddListener (Hide);
        }
	}

	void Hide(){
		gameObject.SetActive(false);
	}
}
