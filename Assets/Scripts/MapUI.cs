using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] Transform markerParent, PanParent, scaleParent;
    [SerializeField] Transform playerMarker;
    [SerializeField] GameObject markerPrefab;
    Transform player;

    [SerializeField] List<MapObject> mapObjects = new List<MapObject>();
    List<MapObject> currentMarkers = new List<MapObject>();

    [Space()]
    [SerializeField] float zoomSensitivity;
    [SerializeField] float panSensitivity;
    Vector2 oldMousePos;

    [Header("Params")]
    [SerializeField] float mapScale = 1;
    [SerializeField] Vector2 mapOffset, scaleLimits;
    [SerializeField] Vector2 playerOffset;
    [SerializeField] float markerScale = 0.1f;

    [Header("Important markers")]
    [SerializeField] Vector2 anchoredPosLimits = new Vector2(316, 175);
    [SerializeField] Vector2 mapStartPos, mapStartScale;

    [Header("Icons")]
    [SerializeField] Sprite defaultIcon;
    [SerializeField] Sprite unvisitedIcon;


    bool expanded, panSet;

    public List<MapObject> GetMapObjects()
    {
        return mapObjects;
    }

    public void RegisterNewMapObject(MapObject mapObj)
    {mapObjects.Add(mapObj);}

    public void RemoveMapObject(MapObject mapObj)
    { mapObjects.Remove(mapObj);}

    private void OnEnable()
    {
        if (Directory.gMan == null) gameObject.SetActive(false);
        else DisplayMap();
    }

    void DisplayMap()
    {
        ClearMap();
        
        mapOffset = Vector2.zero;
        panSet = false; 

        foreach (var obj in mapObjects) if (obj && obj.active) DisplayMarker(obj);
    }

    void ClearMap()
    {
        for (int i = markerParent.childCount - 1; i >= 0; i--) {
            Destroy(markerParent.GetChild(i).gameObject);
        }
        currentMarkers.Clear();
    }

    private void Start()
    {
        if (!player) player = Directory.gMan.player.transform;
    }

    private void Update()
    {
        ZoomMap();
        PanMap();

        UpdatePlayerMarker();
        
        foreach (var c in currentMarkers) UpdateMarker(c);
    }


    void ZoomMap()
    {
        var scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0) mapScale += Mathf.Sign(scrollDelta) * zoomSensitivity;
        mapScale = Mathf.Clamp(mapScale, scaleLimits.x, scaleLimits.y);
        scaleParent.localScale = Vector3.one * mapScale;
    }

    void PanMap()
    {
        if (!Input.GetMouseButton(0) || !expanded) return;

        Vector2 newMousePos = Input.mousePosition;
        var newOffset = Vector2.zero;
        if (!Input.GetMouseButtonDown(0)) {
            var delta = newMousePos - oldMousePos;
            newOffset += delta * panSensitivity;
        }
        oldMousePos = newMousePos;
        PanParent.transform.position += (Vector3)newOffset;
    }

    void UpdateMarker(MapObject info)
    {
        var marker = info.mapMarker;
        if (!marker) return;

        marker.GetComponent<Image>().color = info.color;
        if (info.importantMarker) ClampMarkerToScreen(marker.transform);
        else {
            Vector2 pos = new Vector2(info.transform.position.x, info.transform.position.z);
            marker.localPosition = pos;
        }
        marker.transform.parent = info.importantMarker ? PanParent : markerParent;
        marker.transform.localScale = Vector3.one * markerScale / mapScale;

        marker.GetComponent<Image>().sprite = string.IsNullOrEmpty(info.displayName) ? unvisitedIcon : defaultIcon;
    }

    void UpdatePlayerMarker()
    {
        playerMarker.localEulerAngles = new Vector3(0, 0, Directory.cam.inputs.y * -1);

        var playerMapPos = new Vector2(player.position.x, player.position.z);
        playerMarker.transform.localPosition = playerMapPos;
        playerOffset = playerMapPos;
        playerMarker.localScale = Vector3.one * markerScale / mapScale;

        if (!panSet || (!expanded && PanParent.GetComponent<RectTransform>().anchoredPosition != -playerOffset)) {
            PanParent.GetComponent<RectTransform>().anchoredPosition = -playerOffset;
            panSet = true;
        }
    }
    void DisplayMarker(MapObject info)
    {
        var newMarker = Instantiate(markerPrefab, markerParent).transform;

        Vector2 pos = new Vector2(info.transform.position.x, info.transform.position.z);
        newMarker.localPosition = pos;
        newMarker.GetComponentInChildren<TextMeshProUGUI>().text = info.displayName;

        info.mapMarker = newMarker;

        currentMarkers.Add(info);
    }

    public void SetMapActive(bool active, bool overrideExpand)
    {
        if (expanded && !overrideExpand) return;
        SetMapActive(active);
    }

    public void SetMapActive(bool active)
    {
        if (gameObject.activeInHierarchy != active) ToggleMap();
    }

    public void ExpandMap()
    { 
        Directory.cam.enabled = false;
        Directory.gMan.player.SetPaused(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        expanded = true;
    }

    public void ToggleMap()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);

        if (!gameObject.activeInHierarchy) {
            expanded = false;
            Directory.gMan.player.SetPaused(false);
            Directory.cam.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ClampMarkerToScreen(Transform marker)
    {
        var rect = marker.GetComponent<RectTransform>();
        float x = Mathf.Clamp(rect.anchoredPosition.x, anchoredPosLimits.x * -1, anchoredPosLimits.x);
        float y = Mathf.Clamp(rect.anchoredPosition.y, anchoredPosLimits.y * -1, anchoredPosLimits.y);
        rect.anchoredPosition = new Vector2(x, y);
    }
}
