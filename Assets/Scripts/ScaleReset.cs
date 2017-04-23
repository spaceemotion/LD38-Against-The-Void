using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleReset : MonoBehaviour {
    public float Speed;


	// Update is called once per frame
	void Update () {
        transform.localScale = Vector3.Lerp (transform.localScale, Vector3.one, Time.deltaTime * Speed);
	}

}
