using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsTest : MonoBehaviour {
    public int startLevel, endLevel;

    public float size;
    public float startX, endX;
    public float startY;

    private void OnDrawGizmos() {
        for(int i = startLevel; i < endLevel; i++) {
            Debug.DrawLine(new Vector2(startX, startY + i * size), new Vector2(endX, startY + i * size), Color.red);
            Debug.DrawLine(new Vector2(startX, startY + i * size), new Vector2(startX, startY + i * size + size), Color.red);
            Debug.DrawLine(new Vector2(endX, startY + i * size), new Vector2(endX, startY + i * size + size), Color.red);
        }
    }
}
