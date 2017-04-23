using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour {
    public bool Endless;
    public AudioClip NextWaveSound;

    public TextMeshProUGUI WaveLabel;
    public int Delay = 5;
    public Wave[] Waves;

    public Transform PlayerBase;


    void Start () {
        WaveLabel.text = "";

        StartCoroutine (Spawn ());
    }

    IEnumerator Spawn () {
        // TODO check player death
        for (int i = 0 ; i < Waves.Length; i = Endless ? (i + 1) % Waves.Length : i + 1) {
            var wave = Waves [i];

            yield return new WaitForSeconds (Delay + (wave.Delay * wave.Amount));

            WaveLabel.text = "wave " + (i + 1);
            WaveLabel.transform.localScale = Vector3.one * 5;

            AudioSource.PlayClipAtPoint (NextWaveSound, PlayerController.Instance.transform.position);

            // Calculate bounds
            var bounds = new Bounds (transform.position, Vector3.one);

            foreach (Transform child in PlayerBase) {
                bounds.Encapsulate (child.GetComponent<MeshRenderer> ().bounds);
            }

            for (int j = 0; j <= wave.Amount; j++) {
                Vector3 origin;

                foreach (var entity in wave.Entities) {
                    switch (Random.Range (0, 4)) {
                    default:
                        // Top
                        origin = new Vector3 (Random.Range (bounds.min.x - 2f, bounds.max.x + 2f), 0, bounds.min.z - 1);
                        break;
                    case 1:
                        // Right
                        origin = new Vector3 (bounds.max.x + 1, 0, Random.Range (bounds.min.z - 2f, bounds.max.z + 2f));
                        break;
                    case 2:
                        // Bottom
                        origin = new Vector3 (Random.Range (bounds.min.x - 2f, bounds.max.x + 2f), 0, bounds.max.z + 1);
                        break;
                    case 3:
                        // Left
                        origin = new Vector3 (bounds.min.x - 1, 0, Random.Range (bounds.min.z - 2f, bounds.max.z + 2f));
                        break;
                    }
                    
                    var homing = Instantiate (entity, origin, Quaternion.identity).GetComponent<Homing> ();
                    if (homing != null) {
                        homing.Target = PlayerBase.root;
                    }
                    
                    yield return new WaitForSeconds (wave.Delay);
                }
            }
        }

        WaveLabel.text = "done!";
    }

}
