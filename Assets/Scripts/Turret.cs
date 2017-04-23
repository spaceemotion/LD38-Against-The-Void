using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    public float Speed;
    public Vector3 ShootOffset;

    private Transform target;
    private CanShootThings weapon;


    void Start () {
        weapon = GetComponent<CanShootThings> ();
    }

	// Update is called once per frame
	void Update () {
        if (target == null) {
            return;
        }

        var direction = target.position - transform.position;
        transform.parent.localRotation = Quaternion.LookRotation (Vector3.RotateTowards (
            transform.parent.forward, direction, Time.deltaTime * Speed, 0));


        // start shooting when we're in range
        float angle = Vector3.Angle(direction, transform.parent.forward);

        if (Mathf.Abs (angle) < 5.0f) {
            weapon.DoShootLogic (null, ShootOffset);
        }
    }

    void OnTriggerEnter(Collider enemy) {
        if (enemy.CompareTag ("Enemy")) {
            target = enemy.transform;
        }
    }

}
