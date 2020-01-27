using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Items))]
[CanEditMultipleObjects]
[Serializable]
public class ItemsEditor : Editor
{
    Items ITEM;
    bool showColor = false;

    public override void OnInspectorGUI()
    {
        ITEM = (Items)target;

        showColor = EditorGUILayout.Foldout(showColor, "Rarity Colors");

        if (showColor)
        {
            EditorGUI.indentLevel++;
            for (int c = 0; c < ITEM.rarityColors.Length; c++)
            {
                GUILayout.BeginHorizontal();
                ITEM.rarityColors[c] = EditorGUILayout.ColorField("Rarity " + (c + 1), ITEM.rarityColors[c]);

                if(GUILayout.Button("-"))
                {
                    ArrayUtility.RemoveAt(ref ITEM.rarityColors, c);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Color"))
            {
                ArrayUtility.Add(ref ITEM.rarityColors, Color.white);
            }
            EditorGUI.indentLevel--;
        }

        

        GUILayout.Space(10);
        for (int i = 0; i < ITEM.weapons.Count; i++)
        {
            WeaponFamily wf = ITEM.weapons[i];//create reference to specific family
            FamilyDisplay(i, wf);
        }
    }

    void FamilyDisplay(int i, WeaponFamily wf)
    {
        wf.display = EditorGUILayout.Foldout(wf.display, wf.familyName);//fold out for family
        if (wf.display)//if foldout is open
        {
            EditorGUI.indentLevel++;
            Branches(wf);//call on branches display

            wf.displayWeapons = EditorGUILayout.Foldout(wf.displayWeapons, "Show Weapons");
            if (wf.displayWeapons)
            {
                EditorGUI.indentLevel++;
                for (int k = 0; k < wf.weapons.Count; k++)//for each weapon in the family
                {
                    Weapon w = wf.weapons[k];

                    w.display = EditorGUILayout.Foldout(w.display, w.weaponName);
                    if (w.display)
                    {
                        EditorGUI.indentLevel++;
                        if (w.forge)
                        {

                            MaterialsDisplay(ref w.forgeItem, ref w.forgeNum, "Crafting Items", ref w.displayForge);
                            GUILayout.Space(5);
                        }
                        
                        if (w.item.Count > 0)
                        {
                            MaterialsDisplay(ref w.item, ref w.num, "Upgrade Items", ref w.displayUpgrade);
                            GUILayout.Space(5);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
    }

    void MaterialsDisplay(ref List<string> mats, ref List<int> num, string display, ref bool show)
    {
        show = EditorGUILayout.Foldout(show, display);
        if (show)
        {
            EditorGUI.indentLevel++;
            
            for (int f = 0; f < mats.Count; f++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                mats[f] = GUILayout.TextField(mats[f]);
                
                GUILayout.Label(" x ");
                num[f] = EditorGUILayout.IntField(num[f]);
                GUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }

    void Branches(WeaponFamily wf)
    {
        wf.displayBranches = EditorGUILayout.Foldout(wf.displayBranches, "Show Branches");
        if (wf.displayBranches)
        {
            for (int j = 0; j < wf.branches.Count; j++)
            {
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(wf.weapons[wf.branches[j]].weaponName);
                
                wf.branches[j] = EditorGUILayout.IntField(wf.branches[j]);

                if (GUILayout.Button("+"))
                {
                    wf.branches.Insert(j, 0);
                }

                if (GUILayout.Button("-"))
                {
                    wf.branches.RemoveAt(j);
                }

                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

            }            
        }
    }
}
