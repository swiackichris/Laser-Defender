﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] float health = 100;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;

    [SerializeField] float projectileVelocity = 10f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;

    [SerializeField] AudioClip enemyDeathAudio;
    [SerializeField] AudioClip enemyShootingAudio;

    [SerializeField] [Range(0,1)] float deathSoundVolume = 0.5f;
    [SerializeField] [Range(0,1)] float shootSoundVolume = 0.1f;

    // Use this for initialization
    void Start () {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
	}
	
	// Update is called once per frame
	void Update () {
        CountDownAndShoot();
	}

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(
            projectilePrefab, 
            transform.position, 
            Quaternion.identity) as GameObject;
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileVelocity);
        AudioSource.PlayClipAtPoint(enemyShootingAudio, Camera.main.transform.position, shootSoundVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
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
        // Destroys SpaceShip
        Destroy(gameObject);

        GameObject explosion = Instantiate(
            explosionPrefab,
            transform.position,
            transform.rotation);
        Destroy(explosion, 1f);

        AudioSource.PlayClipAtPoint(enemyDeathAudio, Camera.main.transform.position, deathSoundVolume);
    }

}
