using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : Unit
{
    public Pickup pickup;

    public int jumpMin = 2;
    public int jumpMax = 2;

    public int ThrowMin = 2;
    public int ThrowMax = 2;

    public void UpdateHP(int addHP, int increaseMaxHP)
    {
        health = Mathf.Clamp(health + addHP, 0, maxHealth + increaseMaxHP);
    }

    public void UpdateMana(int addMana, int increaseMaxMana)
    {
        mana = Mathf.Clamp(mana + addMana, 0, maxHealth + increaseMaxMana);
    }
}
