using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public Color color = Color.white;

    public bool active { get { return gameObject.activeInHierarchy && enabled; } }
    public Vector2 mapPos { get { return new Vector2(transform.position.x, transform.position.z); } }

    [HideInInspector] public Transform mapMarker;
    [HideInInspector] public Transform compassMarker;
    [HideInInspector] public string displayName;
    public bool importantMarker;

    private void Start()
    {
        Directory.mapMan.RegisterNewMapObject(this);
    }

    private void OnDestroy()
    {
        Directory.mapMan.RemoveMapObject(this);
    }
}
