using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Container : MonoBehaviour {
    public Item[] items;
    public float Health;


    private float _damage;


    public void Damage(float damage) {
        _damage += damage;

        if (_damage >= Health) {
            Destroy (gameObject);

            foreach (var item in items) {
                Instantiate (item.Prefab, transform.position + (Vector3.up * .5f), Quaternion.identity)
                .GetComponent<Rigidbody> ()
                    .AddForce (
                        Vector3.up + new Vector3(Random.Range (-.5f, .5f), Random.value, Random.Range (-.5f, .5f)),
                        ForceMode.Impulse);
            }
        }
    }



}
