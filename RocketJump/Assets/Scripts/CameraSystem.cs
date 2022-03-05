using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    public Transform player;

    public BackgroundSystem bgSystem;
    public GameSystem gameSystem;

    float yPos;

    int stageNr, lastStageNr;
    private float _maxXPos = 18f, _minXPos = -1f;

    private void Update() {
        float x = player.position.x;
        if(x < _minXPos) {
            x = _minXPos;
        } else if(x > _maxXPos) {
            x = _maxXPos;
        }
        yPos = player.transform.position.y % 154;

        if(yPos % 154 > 148.5f) {
            yPos = 148.5f;
        }
        if(yPos < 5.5f) {
            yPos = 5.5f;
        }

        stageNr = (int)player.position.y / 154;
        if(stageNr != lastStageNr) {
            transform.position = new Vector3(x, yPos + (stageNr * 154), -10);
        }
        lastStageNr = stageNr;


        transform.position = Vector3.Lerp(transform.position, new Vector3(x, yPos + (stageNr * 154), -10), 5f * Time.deltaTime);
    }

    //Load main camera after game starts
    public void LoadCamera() {
        stageNr = (int)player.position.y / 154;
        float x = player.position.x;
        if(x < _minXPos) {
            x = _minXPos;
        } else if(x > _maxXPos) {
            x = _maxXPos;
        }
        float y = player.transform.position.y % 154;

        if(yPos % 154 > 148.5f) {
            yPos = 148.5f;
        }
        if(y < 5.5f) {
            y = 5.5f;
        }

        transform.position = new Vector3(x, y + (stageNr * 154), -10);
        bgSystem.LoadCamera(x, y + (stageNr * 154));
    }

    public float MaxXPos {
        set {
            _maxXPos = value;
        }
    }
    public float MinXPos {
        set {
            _minXPos = value;
        }
    }
}
