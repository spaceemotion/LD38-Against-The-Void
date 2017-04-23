using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanShootThings : MonoBehaviour {
    public Gun Weapon;

    private float rechargeTimer;


	protected void Update () {
        rechargeTimer -= Time.deltaTime;

        // Kill objects that are out of reach
        if (transform.position.y < -100) {
            Destroy (gameObject);
        }
    }

    public void DoShootLogic(Rigidbody body = null, Vector3 offset = default(Vector3)) {
        if (rechargeTimer > 0) {
            return;
        }

        var go = Instantiate (Weapon.Bullet,
            transform.position + (transform.rotation * ((Vector3.forward * .2f) + offset)), 
            transform.rotation);

        // Make the player immune to their own bullets
        PlayerController.Instance.IgnoreColliders (go.GetComponents <Collider> ());

        // Play the SFX
        var sfx = go.AddComponent<AudioSource> ();
        sfx.clip = Weapon.Sound;
        sfx.pitch = Random.Range (0.8f, 1.2f);
        sfx.volume = Random.Range (0.8f, 1f);
        sfx.spatialBlend = .75f;
        sfx.Play ();

        // Add Recoil
        if (body != null) {
            body.AddRelativeForce (Vector3.back * Weapon.Recoil);
        }

        CameraShake.Shake(Weapon.Shake);

        rechargeTimer = Weapon.Recharge;
    }

}
