using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile tile;

    public int health = 1;
    public int maxHealth = 1;

    public int mana = 5; 
    public int maxMana = 5;

    public bool wasPushed = false;

    public List<Action> actions;
    public List<Action> pickups;
}
