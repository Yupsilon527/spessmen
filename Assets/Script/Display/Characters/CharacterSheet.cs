using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

[System.Serializable]
public class CharacterSheet 
{
    public float scale = 1;
    public CharacterBodyPart[] SpriteReplacements;
    public SpriteLibraryAsset spriteLibrary;
    public Material eyesMaterial;
    public Material bodyMaterial;
    public Material shadowMaterial;
    [Serializable]
    public class CharacterBodyPart
    {
        public bool multiple;
        public string bodyPartName;
        public Sprite sprite;
    }
}
