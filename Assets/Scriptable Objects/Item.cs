using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemName { none, clearWater }

[CreateAssetMenu(fileName = "New Item")]
public class Item : ScriptableObject
{
    [SerializeField] ItemName itemName;

    private void OnValidate() {
        
    }
}
