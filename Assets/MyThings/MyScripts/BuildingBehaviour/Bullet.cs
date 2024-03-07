using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    private int speed = 20;
    private GameObject bulletPrefab;
    private Enemy aim;
    private Vector3 startingPosition;
    private BulletMovementManager bulletMovement;

    public Bullet(Vector3 startingPosition, Enemy aim)
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
        Vector3 aimDirection = (aim.GetCurrentPosition() - startingPosition).normalized;
        Debug.Log(aimDirection);
        Quaternion rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, startingPosition, rotation);
        //Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bulletMovement = bullet.GetComponent<BulletMovementManager>();
        bulletMovement.StartCoroutine(bulletMovement.UpdateLoopMoveTowardsTarget(aimDirection));

    }

}
