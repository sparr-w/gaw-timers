using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum TitleScenes {
    MainMenu,
    StartGame,
    LevelSelection
}

public class titleController : MonoBehaviour {
    public GameObject SandClock;
    public Sand TopSand;
    public Sand BottomSand;
    [Header("Sand Generator")]
    public GameObject particleParent;
    public GameObject SandParticlePrefab;
    [SerializeField] private int particleLimit = 200;
    [SerializeField] private float sprayRange = 20f;
    public GameObject[] ObjectsToHideOnRotation = new GameObject[1];
    public Image UICover;
    public GameObject[] MenuScreens = new GameObject[1];

    private Queue<GameObject> sandParticles;
    private float timer = 6f;
    private Transform sandClockGlass, sandClockHolder;
    private TitleScenes titleScene;

    private void Start() {
        sandParticles = new Queue<GameObject>();

        for (int i = 0; i < particleLimit; i++) {
            GameObject obj = Instantiate(SandParticlePrefab);
            obj.transform.parent = particleParent.transform;
            obj.SetActive(false);
            sandParticles.Enqueue(obj);
        }

        sandClockGlass = SandClock.transform.Find("glass").transform;
        sandClockHolder = SandClock.transform.Find("glass_holder").transform;
    }

    private GameObject SpawnSandParticles() {
        // dequeue and spawn sand; enqueue the sand back into object pool
        GameObject objToSpawn = sandParticles.Dequeue();

        objToSpawn.transform.position = new Vector3(-37.2f + (float)Random.Range(-250, 250) / 100f, 41.85f, 0);
        objToSpawn.SetActive(true);

        objToSpawn.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-sprayRange, sprayRange), 0f));

        sandParticles.Enqueue(objToSpawn);
        return objToSpawn;
    }

    private void SandRising() {
        // make top sand lower
        TopSand.Transform.localScale = new Vector3(TopSand.DefaultSize.x, TopSand.DefaultSize.y * (timer / 60f), TopSand.DefaultSize.z);
        TopSand.Transform.position = new Vector3(TopSand.DefaultPosition.x, TopSand.DefaultPosition.y - (TopSand.DefaultSize.y * (1 - (timer / 60f))) / 2, TopSand.DefaultPosition.z);
        // make bottom sand raise
        BottomSand.Transform.localScale = new Vector3(BottomSand.DefaultSize.x, BottomSand.DefaultSize.y * (1f - (timer / 60f)), BottomSand.DefaultSize.z);
        BottomSand.Transform.position = new Vector3(BottomSand.DefaultPosition.x, 10.1308f + (BottomSand.DefaultSize.y * (1 - (timer / 60f))) / 2, BottomSand.DefaultPosition.z);
    }

    private void FlipTimeGlass() {
        sandClockGlass.Rotate(Vector3.right * Time.deltaTime * 30f);
        sandClockHolder.Rotate(Vector3.right * Time.deltaTime * 30f);
        
        if (sandClockHolder.localEulerAngles.x >= 180f) {
            sandClockHolder.rotation = new Quaternion(0, sandClockHolder.rotation.y, sandClockHolder.rotation.z, sandClockHolder.rotation.w);
            sandClockGlass.rotation = new Quaternion(0, sandClockGlass.rotation.y, sandClockGlass.rotation.z, sandClockGlass.rotation.w);
            timer = 60f;
        }
    }

    private void ToggleScenery(bool toggle) {
        foreach (GameObject obj in ObjectsToHideOnRotation) {
            obj.SetActive(toggle);
        }
    }

    public void PlayGame() {
        titleScene = TitleScenes.StartGame;
        timer = 2f;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void SelectLevel(int level) {
        levelSelection.Level = level;
        titleScene = TitleScenes.StartGame;
        timer = 2f;
    }

    public void OpenLevelSelection() {
        titleScene = TitleScenes.LevelSelection;
    }

    private void MenuBackground() {
        if (timer > 0f) {
            ToggleScenery(true);

            SpawnSandParticles();
            SandRising();
            timer -= 0.02f;
        }
        else {
            ToggleScenery(false);
            FlipTimeGlass();
        }
    }

    private void FixedUpdate() {
        switch (titleScene) {
            case TitleScenes.MainMenu:
                foreach(GameObject screen in MenuScreens) {
                    screen.SetActive(false);
                }     
                MenuScreens[0].SetActive(true);

                MenuBackground();
                break;
            case TitleScenes.StartGame:
                foreach(GameObject screen in MenuScreens) {
                    screen.SetActive(false);
                }

                Camera cam = GetComponent<Camera>();
                if (timer > 0f) {
                    timer -= 0.02f;
                    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize * 0.3f, Time.deltaTime * 3f);
                    float alpha = (1 - (timer / 2f)) + 0.05f;
                    UICover.color = new Color(UICover.color.r, UICover.color.g, UICover.color.b, alpha);
                } else {
                    SceneManager.LoadScene("SampleScene");
                }

                break;
            case TitleScenes.LevelSelection:
                foreach(GameObject screen in MenuScreens) {
                    screen.SetActive(false);
                }     
                MenuScreens[1].SetActive(true);

                MenuBackground();
                break;
        }
    }

    private void Update() {
        if (titleScene == TitleScenes.LevelSelection) {
            if (Input.GetKeyDown(KeyCode.Escape)) // go back to main menu
                    titleScene = TitleScenes.MainMenu;
        }
    }
}
