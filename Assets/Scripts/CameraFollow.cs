using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    Vector2 velocity;

    public float smoothTimeX;
    public float smoothTimeY;

    public GameObject player;

    void FixedUpdate () {
        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
