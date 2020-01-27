using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is to be used as a data holder and only ran inside of the unity editor.
/// </summary>
[Serializable]
public class Items : MonoBehaviour
{
    public string generalURL = "https://mhworld.kiranico.com";//website where data is being pulled from
    public string[] weaponTreeURL;//extensions for each weapon URL
    public string[] weaponName;//what extensions will actually be called
    public Sprite[] weaponTreeImage;//what image will be used in showing the weapons
    public Color[] rarityColors;
    [SerializeField]
    public List<WeaponFamily> weapons = new List<WeaponFamily>(); //weapon types with all weapons

    void Start()
    {
        //StartCoroutine(StartLoad());
    }

    public void PopulateData()//done this way to be able to use as a button if creating custom inspector
    {
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad()//used to start compiling all the needed data
    {
        print("Starting to populate Weapon Families ");
        for (int i = 0; i < weaponName.Length; i++)
        {
            yield return StartCoroutine(PopulateWeaponFamilies(i));//wait untill these functions are finished before starting the next section
        }
        print("<color=red>Finished populating weapon Families</color>");

        print("<color=green>populating weapon details</color>");
        yield return StartCoroutine(StartGetWeaponDetails());//start getting the weapon details
        print("Finished populating weapon details");
    }
    
    IEnumerator PopulateWeaponFamilies(int w)//used to populate each weapon family
    {
        string weaponURL = generalURL + "/" + weaponTreeURL[w];//get the correct URL for each weapon family

        WeaponFamily wf = new WeaponFamily(weaponName[w]);
        weapons.Add(wf);//add family to compete list

        List<string> htmlWeaponClass = new List<string>();//create a list of individual weapon classes base of HTML format
      
        using (WWW www = new WWW(weaponURL))
        {
            yield return www;//wait for page to load
            
            string page = www.text.ToString();//turn page into readable text
            string[] substring = new String[] { "<tr>" };//what to seperate weapons by
            string[] classes = page.Split(substring, StringSplitOptions.RemoveEmptyEntries);//actually seperate out classes
            
            for (int i = 1; i < classes.Length; i++)//starting at 1 as to skip over non-weapon info
            {
                if (classes[i].Contains(generalURL))
                {
                    htmlWeaponClass.Add(classes[i]);
                }
            }

            for (int j = 0; j < htmlWeaponClass.Count; j++)
            {
                CreateWeaponClass(htmlWeaponClass[j], w);
            }
        }
    }

    void CreateWeaponClass(string wclass, int familyIndex)//create the weapons
    {
        WeaponFamily wf = weapons[familyIndex];
        string[] lines = wclass.Split('\n');//split the page into lines
        string itemURL = string.Empty;
        string itemName = string.Empty;
        int rarity = 0;
        int tier = 0;
        bool c = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("&#9495;"))//indicates a break symbol, if a line contains one it is further in the tier
            {
                tier++;
            }

            if (lines[i].Contains("a href"))//the line with the weapon URL and name in it
            {
                string[] weaponUrlName = lines[i].Split('>');
                itemURL = GetWeaponURL(weaponUrlName);//refer to function
                itemName = GetWeaponName(weaponUrlName);//refer to function
            }

            if (lines[i].Contains("RARE"))//line with rarity in it
            {
                rarity = int.Parse(new string(lines[i].Where(Char.IsDigit).ToArray()));
            }

            if (lines[i].Contains("creatable"))//if this shows up the weapon is creatable with out the previous entry in the tree
            {
                c = true;
            }
        }

        Weapon w = new Weapon(itemName, itemURL, tier, rarity, c);
        wf.weapons.Add(w);

        int wIndex = wf.weapons.IndexOf(w);
        int previousTier = 0;

        if (wIndex > 0)
        {
            previousTier = wf.weapons[wIndex - 1].tier; 
        }

        if (tier <= previousTier)
        {
            wf.branches.Add(wIndex);
        }
    }

    string GetWeaponURL(string[] weaponUrlName)//gets the weapon page url
    {
        string itemURL;
        string[] weaponURL = weaponUrlName[0].Split('"');
        itemURL = weaponURL[1];
        return itemURL;
    }

    string GetWeaponName(string[] weaponUrlName)//gets the actual weapon name
    {
        string itemName;
        string[] itemNameArray = weaponUrlName[1].Split('<');
        itemName = itemNameArray[0];

        itemName = SpecialCharCheck(itemName);

        return itemName;
    }

    string SpecialCharCheck(string itemName)//used to check names for special characters such as ' and "
    {
        if (itemName.Contains("&#039;"))//' char
        {
            itemName = itemName.Replace("&#039;", "'");
        }

        if (itemName.Contains("&quot;"))//" Char
        {
            itemName = itemName.Replace("&quot;", "\"");
        }

        return itemName;
    }

    IEnumerator StartGetWeaponDetails()//used to get any missing and needed weapon data
    {
        for (int j = 0; j < weapons.Count; j++)//iterate through each weapon type
        {
            WeaponFamily wf = weapons[j];
            print("<color=red>Starting " + wf.familyName + "</color>");
            for (int k = 0; k < wf.weapons.Count; k++)//iteration through each weapon
            {
                Weapon w = wf.weapons[k];
                print("<color=blue>Starting " + w.weaponName + "</color>");
                yield return StartCoroutine(GetWeaponDetails(w, wf));
            }
        }
    }

    IEnumerator GetWeaponDetails(Weapon weapon, WeaponFamily weaponFamily)//gets the missing weapon details
    {
        using (WWW w = new WWW(weapon.weaponURL))
        {
            yield return w;

            string page = w.text.ToString();

            if (weapon.tier >= 1)//no need to look for previous is there isn't one
            {
                GetPreviousStart(weapon, weaponFamily, page);
            }

            GetMaterialsStart(weapon, page);//Used to get the different materials needed for the 
        }
        print("<color=green>Got details for</color> <color=blue>" + weapon.weaponName + " </color>");
    }

    void GetPreviousStart(Weapon weapon, WeaponFamily weaponFamily, string page)//split page in needed elements to get previous
    {
        string[] upgradePathText = new string[] { "upgrade" };
        string[] howToText = new string[] { "How to" };
        string[] upgradePath = page.Split(upgradePathText, StringSplitOptions.RemoveEmptyEntries);
        upgradePath = upgradePath[1].Split(howToText, StringSplitOptions.RemoveEmptyEntries);

        GetPrevious(weapon, weaponFamily, upgradePath[0]);
    }

    void GetPrevious(Weapon weapon, WeaponFamily weaponFamily, string s)//gets the previous weapon index in the current family
    {
        int pageWeapons = 0;

        string[] lines = s.Split('\n');

        for (int t = 0; t < lines.Length; t++)
        {
            if (lines[t].Contains("https://mhworld.kiranico.com/weapon"))//count how many times this shows up
            {
                pageWeapons++;
                if (pageWeapons == weapon.tier)//when the numbers match its the previous weapon
                {
                    string[] itemName = lines[t].Split('>');

                    itemName = itemName[1].Split('<');
                    string previousWeaponname = itemName[0];

                    previousWeaponname = SpecialCharCheck(previousWeaponname);
                    
                    for (int r = 0; r < weaponFamily.weapons.IndexOf(weapon); r++)
                    {
                        Weapon wea = weaponFamily.weapons[r];
                        if (wea.weaponName == previousWeaponname)
                        {
                            //print(wea.weaponName);
                            weapon.previous = r;
                        }
                    }
                }
            }
        }
    }

    void GetMaterialsStart(Weapon weapon, string page)//splite page in needed elements to get materials
    {
        string[] craftText = new string[] { "Crafting" };//what to split by when looking for the craftable items
        string[] upgradeText = new string[] { "Upgrading to" };//what to split by when looking for the upgrade to items

        if (weapon.forge)//if the weapon can be forged
        {
            string[] craftBlocks = page.Split(craftText, StringSplitOptions.RemoveEmptyEntries);//split the page at craftText
            craftBlocks = craftBlocks[1].Split(upgradeText, StringSplitOptions.RemoveEmptyEntries);//split craft blocks to get right area for text
            GetMaterials(craftBlocks[0], ref weapon.forgeItem, ref weapon.forgeNum);//get the materials
        }

        string[] upgradeBlocks = page.Split(upgradeText, StringSplitOptions.RemoveEmptyEntries);//split page by upgradeText
        GetMaterials(upgradeBlocks[1], ref weapon.item, ref weapon.num);//get the materials
    }

    void GetMaterials(string s, ref List<string> items, ref List<int> num)//gets materials needed and amount
    {
        string[] lines = s.Split('\n');
        int amount = 0;
        string name = string.Empty;

        for (int j = 0; j < lines.Length; j++)
        {
            if (lines[j].Contains("https://mhworld.kiranico.com/item"))
            {
                string[] itemName = lines[j].Split('<');
                itemName = itemName[2].Split('>');
                if (itemName[1].Length > 0)
                {
                    name = itemName[1];

                    name = SpecialCharCheck(name);

                    amount = int.Parse(new string(lines[j + 1].Where(Char.IsDigit).ToArray()));
                    items.Add(name);
                    num.Add(amount);
                }
            }
        }
    }
}