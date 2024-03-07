using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BulletMovementManager : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int speed;
    private bool isBulletActive = false;

    public IEnumerator UpdateLoopMoveTowardsTarget(Vector3 enemy)
    {
        isBulletActive = true;
        while (true)
        {
            MoveTowardsTarget(enemy);
            if(isBulletActive == false)
            {
                break;
            }
            yield return null;
        }

        
    }

    private void MoveTowardsTarget(Vector3 enemy)
    {
        Vector3 direction = (enemy - rb.position).normalized;

        // Apply force to move the Rigidbody towards the target position
        rb.AddForce(direction * speed, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isBulletActive = false;
        this.enabled= false;
        Destroy(this.gameObject);
    }
}
