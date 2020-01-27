using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon
{
    
    public string weaponName;//the weapons name
    public Vector2 spot;//the row and column position of where the icon will be 
    public string weaponURL;//Website URL 
    public int tier = 0;//how far into the spot is the item
    public int rarity = 0;//the rarity of the item
    public bool forge = false;//if it can be created without having to go through the entire tree
    public int previous = -1;
    public List<string> forgeItem = new List<string>();//items needed to create weapon from start
    public List<int> forgeNum;//number of each items needed
    public List<string> item = new List<string>();//items needed for the weapon to be made
    public List<int> num = new List<int>();//number of each items needed

    public bool display = false;
    public bool displayForge = false;
    public bool displayUpgrade = false;

    public Weapon(string n, string u, int t, int r, bool c)
    {
        weaponName = n;
        weaponURL = u;
        tier = t;
        rarity = r;
        forge = c;

        if (forge)
        {
            forgeItem = new List<string>();
            forgeNum = new List<int>();
        }
    }

}
