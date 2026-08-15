using System.Collections.Generic;
using UnityEngine;

public class RobotGarbageController : MonoBehaviour
{
    [SerializeField] private List<GameObject> garbageObjects;
    [SerializeField] private Transform garbageSpawnPoint;

    private GarbageObject _garbageObject;

    public void SpawnGarbage()
    {
        if (_garbageObject != null)
            return;

        GameObject garbagePrefab = garbageObjects[Random.Range(0, garbageObjects.Count)];

        GameObject garbage = Instantiate(
                garbagePrefab,
                garbageSpawnPoint.position,
                garbageSpawnPoint.rotation
            );

        _garbageObject = garbage.GetComponent<GarbageObject>();
        _garbageObject.SetPhysical(false);
    }

    public void DropGarbage()
    {
        if (_garbageObject == null) return;

        _garbageObject.SetPhysical(true);
        _garbageObject = null;
    }

    private void LateUpdate()
    {
        if (_garbageObject == null) return;

        _garbageObject.transform.SetPositionAndRotation(
            garbageSpawnPoint.position,
            garbageSpawnPoint.rotation
        );
    }
}