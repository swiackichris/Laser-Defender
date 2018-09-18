using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    // config params
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 5f;
    float bottomPadding = 0.45f;
    [SerializeField] public float health = 200f;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [SerializeField] AudioClip playerDeathAudio;

    [Header("Projectile")]
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileFiringPeriod = 0.2f;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.1f;
    [SerializeField] AudioClip playerShootingAudio;

    Coroutine firingCoroutine;
    
    float xMin;
    float xMax;

    float yMin;
    float yMax;

    // Use this for initialization
    void Start () {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    public void Update () {
        Move();
        Fire();
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
            laserPrefab,
            transform.position,
            Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

            AudioSource.PlayClipAtPoint(playerShootingAudio, Camera.main.transform.position, shootSoundVolume);

            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + bottomPadding; // + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - bottomPadding; // - padding;
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DeathExplosion();
        AudioSource.PlayClipAtPoint(playerDeathAudio, Camera.main.transform.position, deathSoundVolume);
        StartCoroutine(GameOver());
    }

    private void DeathExplosion ()
    {
        GameObject explosion = Instantiate(
            explosionPrefab,
            transform.position,
            transform.rotation);
        Destroy(explosion, 2f);
    }

    IEnumerator GameOver()
    {
        print("Corutine Started");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameOver");
        print("Corutine Ended");
        Destroy(gameObject);
    }

    // Coroutine doesn't work because the object with the Script attached is Destroyed,
    // so script is no longer working
}
