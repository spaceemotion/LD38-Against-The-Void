using UnityEngine;

public class BuildBase : MonoBehaviour {

    private MeshRenderer _renderer;


    void Awake () {
        _renderer = GetComponent<MeshRenderer> ();
        Hide ();
    }
    
    public void Show() {
        _renderer.enabled = true;
    }

    public void Hide() {
        _renderer.enabled = false;
    }
    

    /*private Color color;

    void Start() {
        color = _render.material.color;
    }

    void FixedUpdate() {
        _render.material.color = new Color (color.r, color.g, color.b,
            (Mathf.Sin ((float)Time.realtimeSinceStartup * 10) / 2) + .5f);
    }*/

}
