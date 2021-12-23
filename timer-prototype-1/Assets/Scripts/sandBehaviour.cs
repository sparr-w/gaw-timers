using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    #region Old Solution
/* old solution
public class sandBehaviour : MonoBehaviour {
    public GameObject SandParticlePrefab;
    public Transform Sand;

    List<GameObject> SandParticles;

    void Start() {
        SandParticles = new List<GameObject>();
    }

    private void GenerateParticles() {
        // instantiate particle prefab; move to generator location; remove constraints so it falls and fills the bowl
        SandParticles.Add(Instantiate(SandParticlePrefab));
        GameObject lastParticle = SandParticles[SandParticles.Count - 1];
        Transform lastTF = lastParticle.GetComponent<Transform>();
        Rigidbody2D lastRB = lastParticle.GetComponent<Rigidbody2D>();

        lastTF.position = transform.position;
        lastRB.constraints = RigidbodyConstraints2D.None;
    }

    private void MoveSand() {
        // move the solid sand square upward by scaling; remove particles rigidbody to improve performance
        float increment = 0.0005f;

        Sand.localScale += new Vector3(0f, increment, 0f);
        Sand.position += new Vector3(0f, increment / 2, 0f);
    }

    // occurs 50 times a second
    void FixedUpdate() {
        GenerateParticles();
        MoveSand();
    }

    void Update() {
        Debug.Log(SandParticles.Count);
    }
}
*/
    #endregion

public class sandBehaviour : MonoBehaviour {
    public gameController Game;

    public GameObject SandParticlePrefab;
    private Queue<GameObject> sandParticles;
    [SerializeField] private int particleLimit = 250;
    [SerializeField] private float sprayRange = 20f;
    
    public Sand Sand;

    private void Start() {
        // initialize; occurs within first frame
        sandParticles = new Queue<GameObject>();

        for (int i = 0; i < particleLimit; i++) {
            GameObject obj = Instantiate(SandParticlePrefab);
            obj.SetActive(false);
            sandParticles.Enqueue(obj);
        }
    }

    private GameObject SpawnSandParticles() {
        // dequeue and spawn sand; enqueue the sand back into object pool
        GameObject objToSpawn = sandParticles.Dequeue();

        objToSpawn.transform.position = new Vector3(transform.position.x + (float)Random.Range(-100, 100) / 100f, transform.position.y, transform.position.z);
        objToSpawn.SetActive(true);

        objToSpawn.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-sprayRange, sprayRange), 0f));
        objToSpawn.transform.parent = gameObject.transform; // keeps hierarchy clean

        sandParticles.Enqueue(objToSpawn);
        return objToSpawn;
    }

    private void SandRising() {
        // handle the sand (death floor) rising
        Sand.Transform.localScale = new Vector3(Sand.DefaultSize.x, 0.6f * Game.Progress, Sand.DefaultSize.z);
        Sand.Transform.position = new Vector3(Sand.DefaultPosition.x, Sand.DefaultPosition.y + 0.3f * Game.Progress, Sand.DefaultPosition.z);
    }

    // occurs 50 times a second
    private void FixedUpdate() {
        if (Game.GameState == GameStates.Active) {
            SpawnSandParticles();
            SandRising();
        }
    }
}
