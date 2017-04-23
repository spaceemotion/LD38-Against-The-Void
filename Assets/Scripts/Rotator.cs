using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float Speed;

    public float Vertical;
    public float VerticalSpeed;

    private float localY;

    
    void Start () {
        localY = transform.localPosition.y;
    }
	
	void Update () {
        transform.localEulerAngles = transform.localEulerAngles + Vector3.up * Speed * Time.deltaTime;

        if (Vertical > 0) {
            transform.localPosition = new Vector3 (transform.localPosition.x,
                localY + Mathf.Sin (Time.timeSinceLevelLoad * VerticalSpeed) * Vertical,
                transform.localPosition.z);
        }
	}

}
