using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static List<T> GetRandomListOfObjects<T>(List<T> randomList, int quantityOfThings, bool shouldRepeatObject) where T: class
    {
        //Get Random Enemy positions
        System.Random random = new System.Random();
        List<T> objectsToReturn = new List<T>();
        for (int i = 0; i < quantityOfThings; i++)
        {
            try
            {
                int RandomIndex = random.Next(0, randomList.Count);
                T randomObject = randomList[RandomIndex];
                objectsToReturn.Add(randomObject);
                if (shouldRepeatObject == true)
                {
                    randomList.RemoveAt(RandomIndex);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                break;
            }
        }
        return objectsToReturn;
    }

    public static Transform GetClosestObject(List<Transform> targets, Vector3 currentPosition, float range = Mathf.Infinity)
    {
        Transform closestObject = null;
        float distanceFromCurrentClosestTarget = Mathf.Infinity;
        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(currentPosition, target.position);
            if (distance < distanceFromCurrentClosestTarget && distance <= range)
            {
                closestObject = target;
                distanceFromCurrentClosestTarget = distance;
            }
        }

        return closestObject;
    }

    public static Vector3 GetClosestObject(List<Vector3> targets, Vector3 currentPosition)
    {
        Vector3 closestObject = new Vector3(0,0,0);
        float distanceFromCurrentClosestTarget = Mathf.Infinity;
        foreach (Vector3 target in targets)
        {
            float distanceFromThisTarget = Vector3.Distance(currentPosition, target);
            if (distanceFromThisTarget < distanceFromCurrentClosestTarget)
            {
                distanceFromCurrentClosestTarget = distanceFromThisTarget;
                closestObject = target;
            }
        }
        return closestObject;
    }

    public static Enemy GetClosestObject(List<Enemy> targets, Vector3 currentPosition)
    {
        Enemy closestObject = null;
        float distanceFromCurrentClosestTarget = Mathf.Infinity;
        foreach (Enemy target in targets)
        {
            Vector3 targetDistance = target.GetCurrentPosition();
            float distanceFromThisTarget = Vector3.Distance(currentPosition, targetDistance);
            if (distanceFromThisTarget < distanceFromCurrentClosestTarget)
            {
                distanceFromCurrentClosestTarget = distanceFromThisTarget;
                closestObject = target;
            }
        }

        return closestObject;
    }
}
