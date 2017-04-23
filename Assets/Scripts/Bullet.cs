using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {
    public float Speed;
    public float Force;
    public float Damage;
    public Gun Weapon;

    private AudioSource _audio;
    private Camera _cam;


    void Start() {
        _cam = Camera.main;
        _audio = GetComponent<AudioSource> ();

        // Auto-shoot bullets when they spawn
        GetComponent<Rigidbody> ().AddRelativeForce (Vector3.forward * Speed, ForceMode.Impulse);
    }

    // Remove bullets when they're out of reach / screen
    void FixedUpdate () {
        if (_audio.isPlaying) {
            return;
        }

        var screenPoint = _cam.WorldToViewportPoint(transform.position);

        if (screenPoint.x < -0.2f || screenPoint.x > 1.2f || screenPoint.y < -0.2f || screenPoint.y > 1.2f) {
            Destroy (gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody != null) {
            var trueAngle = Mathf.Atan2 (
                collision.transform.position.z - transform.position.z,
                collision.transform.position.x - transform.position.x)
                * Mathf.Rad2Deg;
            
            collision.rigidbody.AddForce ((Vector3.forward * trueAngle).normalized * Force);

            var container = collision.gameObject.GetComponent<Container> ();
            if (container != null) {
                container.Damage (Damage);
            }
        }
            
        Destroy (gameObject);

        if (Weapon != null) {
            if (Weapon.HitSound != null) {
                AudioSource.PlayClipAtPoint (Weapon.HitSound, gameObject.transform.position);
            }

            CameraShake.Shake(Weapon.Shake * 2);
        }

        // TODO particle fx?
    }

}
