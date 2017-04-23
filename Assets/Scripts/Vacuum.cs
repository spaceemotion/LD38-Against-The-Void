using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour {
    public string Tag;
    public float Strength;

    private HashSet<Rigidbody> InReach;

    // Collect all coins that get in contact with this building
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag (Tag)) {
            PlayerController.Instance.Equip(other.gameObject.GetComponent<Pickup> ());
        }
    }

    // Refresh the coins we want to "suck in" every frame
    // TODO maybe make this more performant?
    void OnTriggerStay(Collider other) {
        if (!other.gameObject.CompareTag (Tag)) {
            return;
        }

        if (InReach == null) {
            InReach = new HashSet<Rigidbody> ();
        }

        InReach.Add (other.attachedRigidbody);
    }

    void LateUpdate () {
        if (InReach == null) {
            return;
        }

        foreach (var body in InReach) {
            if (body != null) {
                body.AddExplosionForce (-Strength + (Random.value * 0.1f), transform.position, 0, 0);
            }
        }

        InReach.Clear ();
    }

}
