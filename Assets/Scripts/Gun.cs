using UnityEngine;

[CreateAssetMenu]
public class Gun : Item {

    public GameObject Bullet;

    public float Recoil;
    public float Recharge;
    public float Shake;
    public int AmmoCount;

    public AudioClip Sound;
    public AudioClip HitSound;

}
