using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecycleStatus : MonoBehaviour {
    private Text text;
	

    void Start () {
        text = GetComponent<Text> ();
    }

	void LateUpdate () {
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentBuilding != null) {
            text.text = "Recycle\n$" + PlayerController.Instance.CurrentBuilding.Money / 2;
        }
	}

}
