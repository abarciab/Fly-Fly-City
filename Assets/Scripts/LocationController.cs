using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public class LocationData
{
    public string name;
    public int district;
}

public class personSlot
{
    public Vector3 spawnPos;
    public GameObject person;
}

public class LocationController : MonoBehaviour
{
    [SerializeField] bool forceReload;
    [SerializeField] TextAsset textFile;

    [SerializeField] LocationData _data;

    MapObject mapObj;

    public string District {  get { return Directory.current.GetDistrictName(data.district);  } }
    public LocationData data { get { return _data; } }

    [Header("People")]
    [SerializeField] Transform peopleParent;
    [SerializeField] List<Vector3> spawnPositions = new List<Vector3>();
    [SerializeField] List<personSlot> usedSlots = new List<personSlot>();

    bool visited;

    private void OnValidate()
    {
        if (!forceReload || textFile == null) return;
        forceReload = false;

        ProcessTextFile();
    }

    public void Enter()
    {
        visited = true;
        mapObj.displayName = data.name;
    }

    private void Start()
    {
        ProcessTextFile();
        Directory.current.AddLocation(this);
        mapObj = GetComponent<MapObject>();
    }

    public void SpawnDeliveryClient()
    {
        SpawnPerson(Directory.gMan.GetDeliveryClient());
    }

    public void SpawnDeliveryRecipient()
    {
        SpawnPerson(Directory.gMan.GetDeliveryRecipient());
    }

    void SpawnPerson(GameObject prefab)
    {
        if (spawnPositions.Count <= 0) KillPerson();

        var pos = spawnPositions[Random.Range(0, spawnPositions.Count)];
        spawnPositions.Remove(pos);
        var data = new personSlot();
        data.spawnPos = pos;

        Physics.Raycast(transform.TransformPoint(pos), Vector3.down, out var hit);
        pos = hit.point;
        var newPerson = Instantiate(prefab, peopleParent);
        newPerson.transform.position = pos;
        data.person = newPerson;
        usedSlots.Add(data);
    }

    void KillPerson()
    {
        if (usedSlots.Count <= 0) return;

        var chosen = usedSlots[Random.Range(0, usedSlots.Count)];
        usedSlots.Remove(chosen);

        Destroy(chosen.person);
        spawnPositions.Add(chosen.spawnPos);
    }


    void ProcessTextFile()
    {
        ReadFile();
        gameObject.name = data.name;
    }

    void ReadFile()
    {
        string text = textFile.text;
        _data = JsonUtility.FromJson<LocationData>(text);
    }

    public void ActivateMarkerAsPickup()
    {
        if (!visited) mapObj.displayName = "Pickup Location";
        mapObj.color = Directory.current.colors.pickup;
        mapObj.importantMarker = true;
    }

    public void MarkerRegular()
    {
        mapObj.color = Directory.current.colors.normal;
        mapObj.importantMarker = false;
    }

    public void ActivateMarkerAsDropoff()
    {
        if (!visited) mapObj.displayName = "DropOff Location";
        mapObj.color = Directory.current.colors.dropoff;
        mapObj.importantMarker = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var pos in spawnPositions) Gizmos.DrawWireSphere(transform.TransformPoint(pos), 0.5f);
        Gizmos.color = Color.red;
        foreach (var pos in usedSlots) Gizmos.DrawWireSphere(transform.TransformPoint(pos.spawnPos), 0.5f);
    }
}
