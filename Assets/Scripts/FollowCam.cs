using UnityEngine;

public class FollowCam : MonoBehaviour {
    public GameObject Follow;
    public float Speed = 5;

    private Transform _transform;


    void Start () {
        _transform = transform;
    }

    void LateUpdate () {
        // Smooth follow player
        _transform.position = new Vector3 (
            Mathf.Clamp (
                Mathf.Lerp (_transform.position.x, Follow.transform.position.x, Time.deltaTime * Speed),
                _transform.position.x - .5f, _transform.position.x + .5f),

            _transform.position.y,

            Mathf.Clamp(
                Mathf.Lerp (_transform.position.z, Follow.transform.position.z, Time.deltaTime * Speed),
                _transform.position.z - .5f, _transform.position.z + .5f));
    }

}
