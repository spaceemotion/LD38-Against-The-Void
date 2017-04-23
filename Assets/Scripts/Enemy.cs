using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float Damage;


    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.CompareTag ("PlayerBase")) {
            PlayerController.Instance.HurtBase (Damage);
            Destroy (gameObject);
        }
    }

}
