using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour {
    public float Speed;

    [System.NonSerialized]
    public Transform Target;
	
	
    // Update is called once per frame
	void Update () {
        var speed = Time.deltaTime * Speed;

        transform.LookAt (Target.position);
        transform.position = new Vector3(
            Mathf.MoveTowards (transform.position.x, Target.position.x, speed),
            transform.position.y,
            Mathf.MoveTowards (transform.position.z, Target.position.z, speed));
	}

}
