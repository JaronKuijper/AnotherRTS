﻿using System.Collections.Generic;
using UnityEngine;

namespace AnotherRTS.Management.RemappableInput
{
    public class KeyBindingDatabase
    {
        private ModifierKeyRegister m_ModifierRegister;
        private Dictionary<string, int> m_NameIDPairs;
        private Dictionary<int, KeyGroup> m_GroupDict;
        private KeyGroup[] m_Groups;
        private string[] m_KeyNames;

        public string[] KeyNames { get { return m_KeyNames; } }

        public KeyBindingDatabase(KeyGroup[] groups, Dictionary<string,int> nameId, ModifierKeyRegister register)
        {
            m_Groups = groups;
            m_NameIDPairs = nameId;
            SetControlGroups(groups);
            m_ModifierRegister = register;
        }

        private void SetControlGroups(KeyGroup[] groups)
        {
            m_GroupDict = new Dictionary<int, KeyGroup>();
            // Link all key ID's to their respective control groups
            // So we can find them back quickly later.
            for (int i = 0; i < groups.Length; i++)
            {
                int[] IDs = groups[i].GetAllKeyIDs();
                for (int j = 0; j < IDs.Length; j++)
                {
                    m_GroupDict.Add(IDs[j],groups[i]);
                }
            }
        }

        private KeyGroup FindContainingGroup(int id)
        {
            KeyGroup group;

            if (!m_GroupDict.TryGetValue(id, out group))
                throw new System.Exception("ControlGroup with id: " + id + "not found.");

            return group;
        }

        public void KeyUp(KeyCode keycode)
        {
            m_ModifierRegister.KeyUp(keycode);
                
            for (int i = 0; i < m_Groups.Length; i++)
            {
                m_Groups[i].KeyUp(keycode);
            }
        }

        public void KeyDown(KeyCode keycode)
        {
            m_ModifierRegister.KeyDown(keycode);

            for (int i = 0; i < m_Groups.Length; i++)
            {
                m_Groups[i].KeyDown(keycode);
            }
        }

        public bool GetKeyUp(int id)
        {
            return FindContainingGroup(id).GetKey(id).IsReleased;
        }

        public bool GetKey(int id)
        {
            return FindContainingGroup(id).GetKey(id).IsHeld;
        }

        public bool GetKeyDown(int id)
        {
            return FindContainingGroup(id).GetKey(id).IsPressed;
        }

        public int GetKeyID(string name)
        {
            int id;
            if (!m_NameIDPairs.TryGetValue(name, out id))
                throw new System.Exception("Keybinding \"" + name + "\" doesn't exist!");
            return id;
        }

        public void Start()
        {
            m_KeyNames = GetAllKeyNames();
            for (int i = 0; i < m_Groups.Length; i++)
            {
                m_Groups[i].Start();
            }
        }

        private string[] GetAllKeyNames()
        {
            List<string> keynames = new List<string>();
            for (int i = 0; i < m_Groups.Length; i++)
            {
                for (int j = 0; j < m_Groups[i].Keys.Length; j++)
                {
                    keynames.Add(m_Groups[i].Keys[j].Name);
                }
            }
            return keynames.ToArray();
        }
    }
}
