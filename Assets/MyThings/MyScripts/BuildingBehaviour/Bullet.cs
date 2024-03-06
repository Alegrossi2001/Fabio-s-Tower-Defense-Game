using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    private int speed = 20;
    private GameObject bulletPrefab;
    private Vector3 aim;
    private Vector3 startingPosition;

    public Bullet(Vector3 startingPosition, Vector3 aim)
    {
        this.startingPosition = startingPosition;
        this.aim = aim;
        bulletPrefab = Resources.Load("Bullet") as GameObject;
        if(bulletPrefab == null)
        {
            Debug.LogError("Bullet:Constructor: Warning, bullet is null!");
        }
        GenerateBullet();
    }

    private void GenerateBullet()
    {
        Quaternion rotation = Quaternion.LookRotation(aim, Vector3.up);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, startingPosition, rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.LogError("Bullet/GenerateBullet(): Rigidbody on bullet is null");
            bullet.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        rb.AddForce(aim, ForceMode.Impulse);
        

    }
    
}
