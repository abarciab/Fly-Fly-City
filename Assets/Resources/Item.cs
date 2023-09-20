using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemNameOLD { none, clearWater }

[CreateAssetMenu(fileName = "New Item")]
public class Item : ScriptableObject
{
    [SerializeField] ItemNameOLD itemName;

    private void OnValidate() {
        
    }
}
