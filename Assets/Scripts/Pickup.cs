using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : CanShootThings {
    public AudioClip Sound;

    public int Money;

    public float DespawnDelay;

    private float ticker;


    protected new void Update() {
        base.Update ();

        if (DespawnDelay > 0) {
            ticker += Time.deltaTime * (Random.value / 2 + 0.5f);
            
            if (ticker >= DespawnDelay) {
                Destroy (gameObject);
            }
        }
    }

    void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag ("Player")) {
            other.gameObject.SendMessage ("Equip", this);
            DespawnDelay = 0;
        }
    }

}
