using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour {
    private Rigidbody2D playerRigidbody;
    private CircleCollider2D playerCollider;
    private PlayerSkinSystem playerSkinSystem;
    private SpriteRenderer playerSprite;

    private Transform cannonTransform;
    private BoxCollider2D cannonCollider;

    public SoundSystem soundSystem;

    public GameObject bulletPrefab;
    private GameObject currentBullet;

    public LayerMask platformLayerMask, slopeLayerMask;

    private Vector2 lookDirection, lastVelocity, startPosition;
    float angle;

    //Number of players collisions.Used with the death system.
    private float collisionCount;

    private bool _playerIsDead, _playerOnSlope, _playerInSecretRoom;
    private float _jumpPower, power;

    private ParticleSystem particle;
    public Animator secretRoomAnim;
    public UISystem ui;

    public UnityEngine.ParticleSystem[] particles;

    //Used to make a delay between sounds
    private float soundTime;

    private void Start() {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CircleCollider2D>();
        playerSkinSystem = GetComponent<PlayerSkinSystem>();

        cannonTransform = transform.Find("Cannon").transform;
        cannonCollider = cannonTransform.GetComponent<BoxCollider2D>();

        playerSprite = transform.Find("PlayerSkin").GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        particle = GameObject.FindGameObjectWithTag("ParticleSystem").GetComponent<ParticleSystem>();
    }

    private void FixedUpdate() {
        if(transform.position.y < 0) {
            playerRigidbody.velocity = Vector2.zero;
            transform.position = startPosition;
        }

        CheckGround();

        void CheckGround() {
            List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
            playerRigidbody.GetContacts(contactPoints);

            int[] contact = new int[2];
            contact[0] = 0;
            contact[1] = 0;

            foreach(ContactPoint2D contactPoint in contactPoints) {
                LayerMask layer = contactPoint.collider.gameObject.layer;

                if(LayerMask.LayerToName(layer.value) == "Slope") {
                    contact[0] = 1;
                }
                if(LayerMask.LayerToName(layer.value) == "Platform") {
                    contact[1] = 1;
                }
            }

            if(contact[1] == 1) {
                PlayerOnSlope = false;
            } else if(contact[1] == 0 && contact[0] == 1) {
                PlayerOnSlope = true;
            }
        }
    }

    bool freeMovement;
    Vector2 savePosition;

    private void Update() {
        soundTime += Time.deltaTime;

        if(!ui.isPaused) {
            RotatePlayer();
            lastVelocity = playerRigidbody.velocity;

            //TestOnly=========================================================================================
            if(Input.GetKeyDown(KeyCode.Z)) {
                freeMovement = !freeMovement;

                if(freeMovement) {
                    playerRigidbody.gravityScale = 0.001f;
                } else {
                    playerRigidbody.gravityScale = 3f;
                }
            }

            if(Input.GetKey(KeyCode.UpArrow)) {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up, 0.4f);
            }
            if(Input.GetKey(KeyCode.DownArrow)) {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down, 0.4f);
            }
            if(Input.GetKey(KeyCode.LeftArrow)) {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left, 0.4f);
            }
            if(Input.GetKey(KeyCode.RightArrow)) {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right, 0.4f);
            }
            if(Input.GetKeyDown(KeyCode.N)) {
                savePosition = transform.position;
            }
            if(Input.GetKeyDown(KeyCode.M)) {
                transform.position = savePosition;
                playerRigidbody.velocity = Vector2.zero;
            }
            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

            if(Input.GetMouseButtonDown(0)) {
                power = 1;
            }
            //Load jump power according to the length of the mouse button press.
            if(Input.GetMouseButton(0)) {
                power += 0.65f * Time.deltaTime;
                ChangeCannon();
            }
            if(Input.GetMouseButtonUp(0)) {
                if(!PlayerIsDead && !PlayerOnSlope) {
                    if(LookPlatform()) {
                        Jump();
                    } else {
                        ShootBullet();
                    }
                    power = 1;
                    ChangeCannon();
                    soundSystem.PlaySound(0);
                }
            }
        }

        void Jump() {
            JumpPower = power;

            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(-lookDirection * 700f * JumpPower);

            SteamAchievements.JumpAmount += 1;
            SteamAchievements.JumpAmountCurrentGame += 1;

            particle.ShowParticle(0, transform.position + new Vector3(lookDirection.x / 4, lookDirection.y / 4, transform.position.z));
        }
        void ShootBullet() {
            //Destroy current bullet if exist
            if(currentBullet != null) {
                Destroy(currentBullet);
            }

            //Create new bullet
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().Initialize(lookDirection, playerRigidbody, particle, soundSystem);
            currentBullet = bullet;

            //Move player (left/right) after shooting
            if((angle < 50 || angle > 130) && IsGrounded("Platform") && !LookPlatform()) {
                float x = lookDirection.x / Mathf.Abs(lookDirection.x);
                playerRigidbody.AddForce(Vector2.left * 120 * x);
            }

            particle.ShowParticle(0, transform.position + new Vector3(lookDirection.x / 1.5f, lookDirection.y / 1.5f, transform.position.z));
        }
        void RotatePlayer() {
            lookDirection = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            cannonTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        void ChangeCannon() {
            if(power == 1f) {
                playerSkinSystem.ChangeCannon(0);
            } else if(power < 1.2f) {
                playerSkinSystem.ChangeCannon(1);
            } else if(power < 1.4f) {
                playerSkinSystem.ChangeCannon(2);
            } else {
                playerSkinSystem.ChangeCannon(3);
            }
        }
    }

    //Return true if the cannon is looking at the platform.
    private bool LookPlatform() {
        float distance = 0.2f;
        RaycastHit2D raycast = Physics2D.BoxCast(cannonCollider.bounds.center, cannonCollider.bounds.size, 0f, lookDirection, distance, platformLayerMask);
        if(raycast.collider != null) {
            return raycast.collider;
        }

        raycast = Physics2D.BoxCast(cannonCollider.bounds.center, cannonCollider.bounds.size, 0f, lookDirection, distance, slopeLayerMask);
        return raycast.collider != null;
    }

    //Return true if the player is touching the certain ground
    private bool IsGrounded(string layerName = null) {
        RaycastHit2D raycast;

        if(layerName == "Platform") {
            raycast = Physics2D.Raycast(playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y), Vector2.down, (2 * playerCollider.bounds.extents.y) + 0.2f, platformLayerMask);

            return raycast.collider;
        } else if(layerName == "Slope") {
            raycast = Physics2D.Raycast(playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y), Vector2.down, (2 * playerCollider.bounds.extents.y) + 0.4f, slopeLayerMask);

            return raycast.collider;
        } else {
            raycast = Physics2D.Raycast(playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y), Vector2.down, (2 * playerCollider.bounds.extents.y) + 0.2f, platformLayerMask);
            if(raycast.collider == null) {
                raycast = Physics2D.Raycast(playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y), Vector2.down, (2 * playerCollider.bounds.extents.y) + 0.4f, slopeLayerMask);
            }

            return raycast.collider;
        }
    }

    //Return the name of the layer the player collides with.
    private string CheckGround(Collision2D collision) {
        LayerMask layer = collision.collider.gameObject.layer;
        return LayerMask.LayerToName(layer.value);
    }

    public void ChangeCannonVisibility(bool value) {
        cannonTransform.gameObject.SetActive(value);
    }

    public void Dead(bool value) {
        PlayerIsDead = value;

        if(value) {
            SteamAchievements.DeathAmount += 1;
            SteamAchievements.DeathAmountCurrentGame += 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(CheckGround(collision) == "Platform") {

            if(PlayerOnSlope) { PlayerOnSlope = false; }

            if(IsGrounded()) {
                if(soundTime >= 0.09f) {
                    soundSystem.PlaySound(4, playerRigidbody.velocity);
                    soundTime = 0;
                }
                particle.ShowParticle(2, collision.contacts[0].point);
            } else {
                particle.ShowParticle(1, collision.contacts[0].point);
                soundSystem.PlaySound(4, playerRigidbody.velocity);
            }

            BouncePlayer();

            void BouncePlayer() {
                if(lastVelocity.y > 0 && Mathf.Abs(collision.contacts[0].normal.x) > Mathf.Abs(collision.contacts[0].normal.y)) {
                    float speed = lastVelocity.magnitude;
                    Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

                    float x = direction.x * 0.75f;
                    float y = direction.y;

                    direction = new Vector3(x, y);

                    playerRigidbody.velocity = direction.normalized * speed;
                }
                if(Mathf.Abs(collision.contacts[0].normal.x) < Mathf.Abs(collision.contacts[0].normal.y) && collision.contacts[0].normal.y < 0) {
                    float speed = lastVelocity.magnitude * 0.7f;
                    Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

                    float x = direction.x * 4;
                    float y = direction.y + (direction.y / 2);

                    direction = new Vector3(x, y);

                    playerRigidbody.velocity = direction.normalized * speed * 1.2f;
                }
            }
        } else if(CheckGround(collision) == "Slope") {
            particle.ShowParticle(2, collision.contacts[0].point);

            if(IsGrounded("Slope")) {
                PlayerOnSlope = true;
            }

            if(soundTime >= 0.09f) {
                soundSystem.PlaySound(4, playerRigidbody.velocity);
                soundTime = 0;
            }
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !PlayerIsDead) {
            Dead(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Slope") && !IsGrounded()) {
            float speed = lastVelocity.magnitude;
            playerRigidbody.velocity = lastVelocity.normalized * speed * 1.8f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Slope") || collision.gameObject.layer == LayerMask.NameToLayer("Platform")) {
            collisionCount++;
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !PlayerIsDead) {
            Dead(true);
        }

        if(collision.gameObject.tag == "Slime" && !PlayerIsDead) {
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.gravityScale = 0.05f;
        } else if(collision.gameObject.tag == "SecretRoom") {
            _playerInSecretRoom = true;

            if(!PlayerIsDead) {
                secretRoomAnim.SetBool("showSecretRoom", true);
            }
        } else if(collision.gameObject.tag == "GoldenMedal" && !PlayerIsDead) {
            FileManager.SaveData("currentGame/wiseMovement" + collision.gameObject.name, 0);
            FileManager.SaveData("stats/wiseMovement" + collision.gameObject.name, 1);
            Destroy(collision.gameObject, 1);

            collision.gameObject.GetComponent<Animator>().Play("HideGoldenCoin");
            soundSystem.PlaySound(1);
            particle.ShowParticle(3, collision.transform.position);

            SteamAchievements.CheckWiseMovement();

            ui.ShowPInfo(3);
        } else if(collision.gameObject.tag == "Magnifier" && !PlayerIsDead) {
            FileManager.SaveData("currentGame/secretRoom" + collision.gameObject.name, 0);
            FileManager.SaveData("stats/secretRoom" + collision.gameObject.name, 1);
            Destroy(collision.gameObject, 1);

            collision.gameObject.GetComponent<Animator>().Play("HideMagnifier");
            soundSystem.PlaySound(1);
            particle.ShowParticle(4, collision.transform.position);

            SteamAchievements.CheckSecretRoom();

            ui.ShowPInfo(2);
        } else if(collision.gameObject.name == "Crown" && !PlayerIsDead) {
            //Finish Game
            particles[0].Play();
            particles[1].Play();

            FileManager.SaveData("currentGame/levelIsCompleted", 1);

            Time.timeScale = 0.1f;
            ui.gameComplete = true;

            soundSystem.PlaySound(2);

            Destroy(collision.gameObject);

            if(ui.timeValue < 1800) {
                SteamAchievements.UnlockAchievement("NEW_ACHIEVEMENT_1_8");
            }
            if(ui.timeValue < 3600) {
                SteamAchievements.UnlockAchievement("NEW_ACHIEVEMENT_1_7");
            }
            SteamAchievements.UnlockAchievement("NEW_ACHIEVEMENT_1_6");

            if(SteamAchievements.DeathAmountCurrentGame == 0) {
                SteamAchievements.UnlockAchievement("NEW_ACHIEVEMENT_1_13");
            }
            if(SteamAchievements.FallAmountCurrentGame == 0) {
                SteamAchievements.UnlockAchievement("NEW_ACHIEVEMENT_1_12");
            }

            StartCoroutine(wait());
            IEnumerator wait() {
                yield return new WaitForSeconds(0.4f);
                Time.timeScale = 1f;
                ui.ShowGameCompleted();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Slope") || collision.gameObject.layer == LayerMask.NameToLayer("Platform")) {
            collisionCount--;
        }

        if(!IsGrounded() && collisionCount <= 0 && PlayerIsDead && (collision.gameObject.layer == LayerMask.NameToLayer("Slope") || collision.gameObject.layer == LayerMask.NameToLayer("Platform")) && !_playerInSecretRoom) {
            Dead(false);
        }

        if(collision.gameObject.tag == "Slime") {
            playerRigidbody.gravityScale = 3f;
        } else if(collision.gameObject.tag == "SecretRoom") {
            _playerInSecretRoom = false;
            secretRoomAnim.SetBool("showSecretRoom", false);
        }
    }

    private float JumpPower {
        set {
            int power = (int)((value - 1) / 0.05f);
            _jumpPower = 1 + (power * 0.05f);
            if (_jumpPower > 1.4f) {
                _jumpPower = 1.4f;
            }
        }
        get {
            return _jumpPower;
        }
    }
    public bool PlayerIsDead {
        set {
            _playerIsDead = value;
            ChangeCannonVisibility(!value);

            playerCollider.isTrigger = value;

            Color color = playerSprite.color;

            if(value) {
                playerRigidbody.velocity = Vector2.zero;
                color.a = 0.25f;
            } else {
                color.a = 1f;
            }

            playerSprite.color = color;
        }
        get { return _playerIsDead; }
    }
    public bool PlayerOnSlope {
        set {
            if(!value) {
                _playerOnSlope = false;
                playerRigidbody.angularDrag = 100f;
                ChangeCannonVisibility(true);
            } else {
                _playerOnSlope = true;
                playerRigidbody.angularDrag = 0.3f;
                ChangeCannonVisibility(false);
            }
        }
        get { return _playerOnSlope; }
    }

    public Vector2 Velocity {
        get {
            return playerRigidbody.velocity;
        }
        set {
            playerRigidbody.velocity = value;
        }
    }
}
