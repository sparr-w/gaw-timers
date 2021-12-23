using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameGoal : MonoBehaviour {
    public gameController Game;

    void OnTriggerEnter2D(Collider2D obj) {
        if (obj.name.ToLower() == "player") {
            Game.GameVictory();
        }
    }
}
