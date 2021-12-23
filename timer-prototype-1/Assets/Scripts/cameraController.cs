using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {
    public Transform Player;
    public float Y_offset;

    private float yOffset;

    private void SmoothOffset() {
        Rigidbody2D playerRB2D = Player.GetComponent<Rigidbody2D>();
        yOffset = Mathf.Lerp(yOffset, Y_offset * (playerRB2D.velocity.y / 7.5f), Time.deltaTime);
    }

    void Update() {
        SmoothOffset();
        transform.position = new Vector3(Player.position.x, Player.position.y + yOffset, transform.position.z);
    }
}
