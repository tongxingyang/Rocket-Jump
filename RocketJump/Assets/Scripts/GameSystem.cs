using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameSystem : MonoBehaviour {
    public Transform playerT;

    private new CameraSystem camera;
    public BackgroundSystem bgSystem;
    public PlayerSkinSystem playerSkinSystem;
    public UISystem ui;

    private int currentLevel, lastLevel;

    public GameObject[] secretRoom;
    public GameObject[] wiseMovement;

    public SaveLoadSystem saveLoadSystem;

    public TrailRenderer[] trails;

    private void Start() {
        camera = Camera.main.GetComponent<CameraSystem>();

        LoadGame();
    }

    private void FixedUpdate() {
        currentLevel = (int)playerT.position.y / 11;

        if(currentLevel != lastLevel) {
            if(currentLevel > lastLevel && currentLevel % 14 == 0) {
                bgSystem.ChangeBackground(currentLevel / 14);
            }
            if(currentLevel < lastLevel && lastLevel % 14 == 0) {
                bgSystem.ChangeBackground(currentLevel / 14);
            }

            if(currentLevel < lastLevel) {
                SteamAchievements.FallAmount += 1;
                SteamAchievements.FallAmountCurrentGame += 1;
            }

            ui.ChangeLevelNr(currentLevel + 1);
            if(currentLevel >= 14) {
                SteamAchievements.UnlockAchievement("NEW_ACHIEVEMENT_1_0");
            }
            lastLevel = currentLevel;
        }
    }

    public void LoadGame() {
        saveLoadSystem.LoadGame();
        currentLevel = (int)playerT.position.y / 11;

        lastLevel = currentLevel;

        ui.ChangeLevelNr(currentLevel + 1);

        camera.LoadCamera();

        bgSystem.ChangeBackground(currentLevel / 14);
        playerSkinSystem.LoadSkinSystem();

        TurnOnTrail();
        ui.ShowCircle(false);

        for(int i = 0; i < secretRoom.Length; i++) {
            if(int.Parse(FileManager.LoadData("currentGame/secretRoom" + i)) == 0) {
                secretRoom[i].SetActive(false);
            }
        }
        for(int i = 0; i < wiseMovement.Length; i++) {
            if(int.Parse(FileManager.LoadData("currentGame/wiseMovement" + i)) == 0) {
                wiseMovement[i].SetActive(false);
            }
        }
    }

    public void SaveGame() {
        saveLoadSystem.SaveGame();

        playerSkinSystem.SaveSkinSystem();
    }

    public void TurnOnTrail() {
        for(int i = 0; i < trails.Length; i++) {
            trails[i].enabled = true;
        }
    }

    private void OnApplicationQuit() {
        SaveGame();
    }
}
