using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour {
    public float Speed;
    public float Divide;
    public float Add;

    public float ScrollX;
    public float ScrollY;

    private MeshRenderer _renderer;
    private int tintColorId;


	void Start () {
        _renderer = GetComponent <MeshRenderer> ();
        tintColorId = Shader.PropertyToID ("_TintColor");
	}
	
	void Update () {
        _renderer.material.SetColor (tintColorId, new Color (0,.666f, 1,
            (Mathf.Sin (Time.realtimeSinceStartup * Speed) / Divide) + Add));
        
        _renderer.material.mainTextureOffset = new Vector2(
            (Time.timeSinceLevelLoad * ScrollX) % 1,
            (Time.timeSinceLevelLoad * ScrollY) % 1);
	}

}
