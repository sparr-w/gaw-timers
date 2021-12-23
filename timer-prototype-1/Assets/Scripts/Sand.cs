using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : MonoBehaviour {
    private Vector3 defaultSize, defaultPosition;
    private BoxCollider2D coll;

    public gameController Game;

    private void Awake() {
        defaultSize = transform.localScale;
        defaultPosition = transform.position;

        coll = GetComponent<BoxCollider2D>();
    }

    void OnTriggerStay2D(Collider2D obj) {
        if (obj.name.ToLower() == "player") {
            // check whether the centre of the player's collider is within the trigger to kill the player
            if (obj.GetComponent<BoxCollider2D>().bounds.center.y <= coll.bounds.center.y + coll.bounds.size.y / 2) {
                Game.GameOver();
            }
        }
    }

    public GameObject GameObject {
        get {return gameObject;}
    }

    public Transform Transform {
        get {return transform;}
    }

    public Vector3 DefaultSize {
        get {return defaultSize;}
    }

    public Vector3 DefaultPosition {
        get {return defaultPosition;}
    }
}
