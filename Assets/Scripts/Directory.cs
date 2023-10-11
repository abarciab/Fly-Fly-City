using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable] 
public class ColorList
{
    public Color pickup, dropoff, normal = Color.white;
}

public class Directory : MonoBehaviour
{
    public static Directory current;
    private void Awake() 
    {
        current = this;
        gMan = FindObjectOfType<GameManager>(true);
        uiMan = FindObjectOfType<UIManager>(true);
        mapMan = FindObjectOfType<MapUI>(true);
        fMan = FindObjectOfType<FlightSpeedController>(true);
        cam = FindObjectOfType<CameraController>(true);
        player = FindObjectOfType<PlayerStats>(true);
        locations.Clear();
    }

    public static GameManager gMan;
    public static UIManager uiMan;
    public static MapUI mapMan;
    public static FlightSpeedController fMan;
    public static CameraController cam;
    public static PlayerStats player;
    public ColorList colors { get { return _colors; } }
    [SerializeField] ColorList _colors;

    [SerializeField] List<string> districtNames = new List<string>();
    List<LocationController> locations = new List<LocationController>();

    public float GetDistance(string name1, string name2)
    {
        foreach (var location1 in locations) {
            if (location1.data.name == name1) {
                foreach (var location2 in locations) {
                    if (location2.data.name == name2) return Vector3.Distance(location1.transform.position, location2.transform.position);
                }
            }
        }
        return -1;
    }

    public int districtCount { get { return districtNames.Count; } }

    public string GetDistrictName(string locationName)
    {
        foreach (var location in locations) if (string.Equals(location.data.name, locationName)) return GetDistrictName(location.data.district);
        return "";
    }

    public string GetDistrictName(int index = -1)
    {
        if (index == -1) index = Random.Range(0, districtNames.Count);
        return districtNames[index];
    }

    public void ResetData()
    {
        locations.Clear();
    }

    public void AddLocation(LocationController location)
    {
        locations.Add(location);
    }

    public LocationData GetRandomLocation()
    {
        return locations[Random.Range(0, locations.Count)].data;
    }

    public LocationData GetRandomLocationInDistrict(int district)
    {
        var location = GetRandomLocation();
        while (location.district != district) location = GetRandomLocation();

        return location;
    }

    public LocationData GetRandomDifferentLocation(List<string> locationNames, int districtID = -1)
    { 
        var location = districtID == -1 ? GetRandomLocation() : GetRandomLocationInDistrict(districtID);
        while (locationNames.Contains(location.name)) location = districtID == -1 ? GetRandomLocation() : GetRandomLocationInDistrict(districtID);
        return location;
    }

    public LocationData GetRandomDifferentLocation(string locationName)
    {
        var location = GetRandomLocation();
        while (location.name == locationName) location = GetRandomLocation();

        return location;
    }

    public LocationController GetLocationRefernce(string locationName)
    {
        foreach (var l in locations) if (string.Equals(l.data.name, locationName)) return l;
        return null;
    }
}
