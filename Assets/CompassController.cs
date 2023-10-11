using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{

    [SerializeField] GameObject markerPrefab;
    [SerializeField] Transform markerParent;
    [SerializeField] float maxWidth, yPos, detatchRadius;
    [SerializeField] bool drawGizmos;

    private void Update()
    {
        foreach (var m in Directory.mapMan.GetMapObjects()) if (m && m.isActiveAndEnabled) TrackOnCompass(m);
    }

    void TrackOnCompass(MapObject obj)
    {
        if (!obj.importantMarker) {
            if (obj.compassMarker != null) obj.compassMarker.gameObject.SetActive(false);
            return;
        }

        if (obj.compassMarker == null) CreateCompassMarker(obj);
        UpdatePosition(obj);
    }

    void UpdatePosition(MapObject obj)
    {
        if (Directory.gMan.currentLocation == obj.GetComponent<LocationController>().data.name) {
            obj.compassMarker.gameObject.SetActive(false);
            return;
        }

        bool detatch = Vector3.Distance(Camera.main.transform.position, obj.transform.position) < detatchRadius;
        var newPos = Camera.main.WorldToScreenPoint(obj.transform.position);
        if (!detatch) newPos.y = yPos;
        newPos.z = 0;
        var oldPos = obj.compassMarker.GetComponent<RectTransform>().position;
        obj.compassMarker.GetComponent<RectTransform>().position = Vector3.Lerp(oldPos, newPos, 0.5f);
        obj.compassMarker.GetComponent<Image>().color = obj.color;

        float xPos = obj.compassMarker.GetComponent<RectTransform>().anchoredPosition.x;
        bool inRange = xPos < (maxWidth / 2) && xPos > -(maxWidth / 2);
        if (!detatch) obj.compassMarker.gameObject.SetActive(inRange && GetAngleToObject(obj.transform) > 0);
        else obj.compassMarker.gameObject.SetActive(GetAngleToObject(obj.transform) > 0);
    }


    float GetAngleToObject(Transform obj)
    {
        var camForward = Camera.main.transform.forward;
        var objDir = obj.position - Camera.main.transform.position;
        camForward.y = 0;
        objDir.y = 0;
        float angle = Vector3.Dot(camForward, objDir);
        
        return angle / 180;
    }

    void CreateCompassMarker(MapObject obj)
    {
        var newMarker = Instantiate(markerPrefab, markerParent).GetComponent<RectTransform>();
        newMarker.anchoredPosition = new Vector2(0, yPos);
        obj.compassMarker = newMarker.transform;
        newMarker.GetComponentInChildren<TextMeshProUGUI>().text = obj.displayName;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos) Gizmos.DrawWireSphere(Camera.main.transform.position, detatchRadius);
    }

}
