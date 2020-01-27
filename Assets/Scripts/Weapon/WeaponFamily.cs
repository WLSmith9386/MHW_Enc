using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponFamily
{
    [SerializeField]
    public string familyName;//weapon family name/type
    [SerializeField]
    public List<Weapon> weapons;//all the weapons
    public List<int> branches;
    public Sprite[] weaponImage;

    [HideInInspector]
    public bool display = false;
    [HideInInspector]
    public bool displayWeapons = false;
    [HideInInspector]
    public bool displayBranches = false;
    public WeaponFamily(string n)
    {
        familyName = n;
        weapons = new List<Weapon>();
        branches = new List<int>();
    }

}
