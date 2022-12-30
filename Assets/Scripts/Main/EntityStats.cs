using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float health { get; private set; }
    [SerializeField] float xp;
    [SerializeField] float xpToNextLevel;
    public int level { get; private set; }
}
