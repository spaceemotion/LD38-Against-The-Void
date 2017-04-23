using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerTo : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag ("Player")) {
            other.gameObject.transform.position = Vector3.up * .5f;
        }
    }

}
