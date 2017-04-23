using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour {
    public Building Building;

    private Text text;
    private Button button;

	
	void Start () {
        text = GetComponentInChildren<Text> ();
        button = GetComponent<Button> ();

        text.text = Building.name + "\n$ " + Building.Money;
	}
	
	void FixedUpdate () {
        if (PlayerController.Instance != null) {
            var block = button.colors;
            block.colorMultiplier = PlayerController.Instance.CurrentBuilding == Building ? 1.35f : 1;
            button.colors = block;
        }
	}

}
