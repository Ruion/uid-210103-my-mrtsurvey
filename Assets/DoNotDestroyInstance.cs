using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyInstance : MonoBehaviour {

	private static DoNotDestroyInstance instance = null;
	public static DoNotDestroyInstance Instance {
		get { return instance; }
	}
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
}
