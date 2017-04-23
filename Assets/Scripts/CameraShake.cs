using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    private static float Intensity;

    private Vector3 offsetPos;
    private Vector3 offsetRot;


    public static void Shake(float amount) {
        Intensity = Mathf.Max (Intensity, amount);
    }

	void Start () {
        offsetPos = transform.localPosition;
        offsetRot = transform.localEulerAngles;
	}
	
	void Update () {
        transform.localPosition = new Vector3 ((Random.value - 0.5f) * Intensity / 2, 0, 0) + offsetPos;
        transform.localEulerAngles = new Vector3 ((Random.value - 0.5f) * Intensity * 10, 0, 0) + offsetRot;

        Intensity = Mathf.Max (0, Intensity - Time.deltaTime);
	}

}
