using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;

    [Header ("Global")]
    public float SpeedMultiplier = 0.1f;
    public float FootDistance = .1f;
    public float HealthLerpSpeed = 4;

    public int Money;
    public int PlatformCost; // TODO


    [Header ("The Tower")]
    public Transform PlayerBase;
    public AudioClip BaseHurtSound;
    public AudioClip GameOverSound;
    public Collider BaseCollider;
    public float Health;

    [Header ("Building")]
    public Material BuildMaterial;
    public LayerMask BuildLayer;

    public GameObject PlatformPrefab;
    public LayerMask PlatformBuildLayer;
    public LayerMask FloorLayer;
    
    public RectTransform BuildingPanel;
    public GameObject BuildingInfo;
    public Building CurrentBuilding;
    public BuildInformation CurrentInfo;

    [Header ("Item pickup")]
    public GameObject Arm;

    [Header ("UI")]
    public RectTransform HealthBar;
    public GameObject EscMenu;
    public GameObject GameOverMenu;
    public TextMeshProUGUI MoneyLabel;
    public GameObject Visualizer;
    public LayerMask InfoLayer;
    public Text Highscore;


    private int buildMatTintColorId;

    private AudioSource audioSource;
    private Camera _cam;
    private Collider _collider;
    private Rigidbody body;
    private Vector3 force;

    private int platformsBuilt;
    private int buildingsBuilt;

    [System.NonSerialized]
    public int DamageDealt;

    private bool doBuild;
    private bool doShowInfo;
    private bool inMenu;

    private float stepTimer;
    private float realHealth;
    private float lerpHealth;

    private BuildBase lastHighlight;
    private GameObject lastPlatformHighlight;
    private RaycastHit hit;
    private Material defaultPlatformMaterial;
    private MeshRenderer platformRenderer;
    private GameObject platformBuildBase;

    private GameObject _visualizer;

    private Pickup equipped;


    void Awake () {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    // Use this for initialization
    void Start () {
        Instance = this;

        buildMatTintColorId = Shader.PropertyToID ("_TintColor");

        audioSource = GetComponent<AudioSource> ();
        body = GetComponent<Rigidbody> ();

        _cam = Camera.main;
        _collider = GetComponent<Collider> ();

        BuildingInfo.SetActive (false);
        EscMenu.SetActive (false);
        GameOverMenu.SetActive (false);

        realHealth = Health;

        _visualizer = Instantiate (Visualizer);
        _visualizer.SetActive (false);

        MoneyLabel.text = "";
    }


    // Update is called once per frame
    void Update () {
        if (realHealth < 0) {
            return;
        }

        doBuild |= Input.GetButtonDown ("Fire1");
        doShowInfo |= Input.GetButtonDown ("Fire2");

        if (Input.GetButtonDown ("Cancel")) {
            inMenu = !inMenu;

            EscMenu.SetActive (inMenu);
            Time.timeScale = inMenu ? 0 : 1;
            AudioListener.pause = inMenu;
        }

        force.x = Input.GetAxisRaw ("Horizontal");
        force.z = Input.GetAxisRaw ("Vertical");

        stepTimer -= Time.deltaTime;

        var posOnScreen = _cam.WorldToViewportPoint (transform.position);
        var mousePos = _cam.ScreenToViewportPoint (Input.mousePosition);

        transform.rotation = Quaternion.FromToRotation (new Vector3(
            mousePos.y - posOnScreen.y, 0, mousePos.x - posOnScreen.x
        ), Vector3.right);

        lerpHealth = Mathf.Lerp (lerpHealth, realHealth, Time.deltaTime * HealthLerpSpeed);
        HealthBar.localScale = new Vector3((lerpHealth / Health) + 0.001f, 1, 1);
    }

    void FixedUpdate () {
        if (inMenu) {
            return;
        }

        // Disable raycasts if we're inside our tool panel
        if (!RectTransformUtility.RectangleContainsScreenPoint (BuildingPanel, new Vector2(
            Input.mousePosition.x, Input.mousePosition.y), _cam)) {
            DoBuildLogic ();
        
        } else {
            doBuild = false;

            if (lastPlatformHighlight != null) {
                lastPlatformHighlight.SetActive (false);
            }   

            if (lastHighlight != null) {
                lastHighlight.Hide ();
            }
        }

        // Update UI
        MoneyLabel.text = "$ " + Money;

        // Change color of the highlighted platform we want to build
        // to whether or not we have enough money
        if (lastPlatformHighlight != null) {
            platformRenderer.material.color = Money >= PlatformCost ? Color.green : Color.red;
        }
    }

    void DoBuildLogic () {
        // Check if we got a building an show its information
        if (doShowInfo) {
            if (Physics.Raycast (_cam.ScreenPointToRay (Input.mousePosition), out hit, 100, InfoLayer)) {
                var info = hit.collider.gameObject.GetComponent<BuildInformation> ();
                
                if (info != null) {
                    if (CurrentInfo == info) {
                        HideBuildInfo ();
                    } else {
                        ShowInfo (info);
                    }
                }
            } else {
                HideBuildInfo ();
            }
        }

        // Let the build material flicker
        if (CurrentBuilding != null) {
            var canBuild = Money >= CurrentBuilding.Money;

            BuildMaterial.SetColor (buildMatTintColorId, new Color(
                canBuild ? 0 : 1,
                canBuild ? 1 : 0,
                0, 
                (Mathf.Sin (Time.realtimeSinceStartup * 8f) / 8) + .25f));


            // General building raycast
            if (Physics.Raycast (_cam.ScreenPointToRay (Input.mousePosition), out hit, 100, BuildLayer)) {
                // Show new highlight
                if (lastHighlight != hit.collider.gameObject) {
                    if (lastHighlight != null) {
                        lastHighlight.Hide();
                        lastHighlight = null;
                    }

                    lastHighlight = hit.collider.gameObject.GetComponent<BuildBase> ();
                    lastHighlight.Show ();
                }

                if (doBuild && canBuild) {
                    lastHighlight.Hide ();

                    var go = Instantiate (CurrentBuilding.Prefab, lastHighlight.transform.parent);
                    go.GetComponent<BuildInformation> ().Building = CurrentBuilding;

                    lastHighlight.gameObject.SetActive (false);

                    doBuild = false;
                    Money -= CurrentBuilding.Money;

                    buildingsBuilt++;

                    // TODO ca-tching
                }
            } else if (lastHighlight != null) {
                lastHighlight.Hide ();
                lastHighlight = null;
            }
        }


        // Platform building raycast
        if (Physics.Raycast (_cam.ScreenPointToRay (Input.mousePosition), out hit, 100, PlatformBuildLayer)) {
            // See if we hit a floor instead (layers are bitmasks, so no easier check here)
            if (hit.collider.gameObject.HasLayer(FloorLayer)) {
                if (lastPlatformHighlight != null) {
                    if (hit.collider.gameObject != lastPlatformHighlight) {
                        lastPlatformHighlight.SetActive (false);

                    } else if (doBuild && Money >= PlatformCost) {
                        // We got a platform we want to build
                        Money -= PlatformCost;

                        // Reset material
                        var renderer = lastPlatformHighlight.GetComponent<MeshRenderer> ();
                        renderer.sharedMaterial = defaultPlatformMaterial;

                        // Reactivate build base so we can build stuff again
                        platformBuildBase.SetActive (true);

                        // reset for next platform
                        lastPlatformHighlight = null;
                        platformRenderer = null;

                        platformsBuilt++;
                    }
                }
            } else {
                var snappedToGrid = new Vector3 (
                    Mathf.Round((hit.point.x) / .5f) * .5f,
                    0,
                    Mathf.Round((hit.point.z) / .5f) * .5f);

                if (lastPlatformHighlight != null) {
                    // Only allow platforms near others, so we do a simple four-directional raycast
                    lastPlatformHighlight.transform.position = snappedToGrid;
                    lastPlatformHighlight.SetActive (
                        Physics.Raycast (snappedToGrid, Vector3.forward, out hit, .5f, FloorLayer) ||
                        Physics.Raycast (snappedToGrid, Vector3.right, out hit, .5f, FloorLayer) ||
                        Physics.Raycast (snappedToGrid, Vector3.back, out hit, .5f, FloorLayer) ||
                        Physics.Raycast (snappedToGrid, Vector3.left, out hit, .5f, FloorLayer));

                } else {
                    lastPlatformHighlight = Instantiate (PlatformPrefab, snappedToGrid, Quaternion.identity, PlayerBase);

                    platformRenderer = lastPlatformHighlight.GetComponent<MeshRenderer> ();
                    defaultPlatformMaterial = defaultPlatformMaterial ?? platformRenderer.sharedMaterial;

                    platformBuildBase = FindChildWithBuildLayer (lastPlatformHighlight);
                    platformBuildBase.SetActive (false);
                }
            }
        }

        doBuild = false;
        doShowInfo = false;
    }

    GameObject FindChildWithBuildLayer(GameObject parent) {
        foreach (Transform child in parent.transform) {
            if (child.gameObject.HasLayer(BuildLayer)) {
                return child.gameObject;
            }
        }

        return null;
    }

    void LateUpdate () {
        // Footstep sound logic
        if (stepTimer <= 0 && !audioSource.isPlaying && force != Vector3.zero) {
            stepTimer = FootDistance;

            audioSource.pitch = Random.Range (0.85f, 1.15f);
            audioSource.volume = Random.Range (0.5f, 1f);
            audioSource.Play ();
        }

        // Movement
        body.AddForce (force.normalized * SpeedMultiplier * Time.deltaTime, ForceMode.Impulse);

        // Bullet shooting logic
        if (equipped != null && Input.GetButton ("Jump")) {
            equipped.DoShootLogic (body);
        }
    }

    public void Equip (Pickup item) {
        if (item.Weapon != null) {
            if (equipped != null) {
                Destroy (equipped.gameObject);
            }
            
            item.transform.SetParent (Arm.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            
            equipped = item;
            
            var rotator = item.gameObject.GetComponent<Rotator> ();
            if (rotator != null) {
                Destroy (rotator);
            }
            
            var rb = item.gameObject.GetComponent<Rigidbody> ();
            if (rb != null) {
                Destroy (rb);
            }

            var collider = item.gameObject.GetComponent<Collider> ();
            if (collider != null) {
                Destroy (collider);
            }
        } else {
            Destroy (item.gameObject);
        }

        if (item.Sound != null) {
            AudioSource.PlayClipAtPoint (item.Sound, transform.position);
        }

        AddMoney (item.Money);
    }

    public void AddMoney(int amount) {
        Money += amount;

        // Increase Label for maxxximum powar!
        MoneyLabel.transform.localScale = Vector3.Min (Vector3.one * 5, MoneyLabel.transform.localScale * 1.1f);
    }

    public void IgnoreColliders(Collider[] colliders) {
        foreach (var c in colliders) {
            Physics.IgnoreCollision (_collider, c, true);
            Physics.IgnoreCollision (BaseCollider, c, true);
        }
    }

    public void SetBuilding(Building building) {
        this.CurrentBuilding = CurrentBuilding != building ? building : null;
    }

    public void ShowInfo(BuildInformation info) {
        BuildingInfo.SetActive (info != null);
        CurrentInfo = info;

        _visualizer.transform.localScale = Vector3.one * (info.Radius.radius * 2);
        _visualizer.transform.position = info.transform.position + info.Radius.center;
        _visualizer.SetActive (true);
    }

    public void DeleteBuilding() {
        if (CurrentInfo != null) {
            foreach (Transform child in CurrentInfo.transform.parent) {
                if (child.CompareTag ("BuildBase")) {
                    child.gameObject.SetActive (true);
                }
            }

            AddMoney (Mathf.RoundToInt (CurrentInfo.Building.Money / 2f));
            Destroy (CurrentInfo.gameObject);

            HideBuildInfo ();
        }
    }

    public void HideBuildInfo() {
        CurrentInfo = null;

        BuildingInfo.SetActive (false);
        _visualizer.SetActive (false);
    }

    public void HurtBase (float amount) {
        realHealth -= amount;
        AudioSource.PlayClipAtPoint (BaseHurtSound, Vector3.zero, Random.Range (0.5f, 0.8f));

        if (realHealth <= 0) {
            GameOverMenu.SetActive (true);

            inMenu = true;
            Time.timeScale = 0;

            AudioSource.PlayClipAtPoint (GameOverSound, transform.position);
            Highscore.text = "" + ((Money + DamageDealt + buildingsBuilt) - platformsBuilt + EnemySpawner.WaveNum);
        }
    }

}
