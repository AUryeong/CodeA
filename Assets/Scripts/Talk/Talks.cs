﻿using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Talks : ScriptableObject
{ 
    public string cgTitle;
    public List<Talk> talks = new List<Talk>();
}

[System.Serializable]
public class Talk
{
    [XmlElement("Bgm")] public string bgm = "";

    [XmlElement("BG")] public Background background = new Background();

    [XmlElement("Dialogue")] public Dialogue dialogue;

    [XmlArray("Characters")] [XmlArrayItem("Character")]
    public List<Character> characters = new List<Character>();

    [XmlArray("Animations")] [XmlArrayItem("Animation")]
    public List<Animation> animations = new List<Animation>();
}

[System.Serializable]
public class Dialogue
{
    [XmlElement("Talker")] public string talker;

    [XmlElement("Text")] public string text;

    [XmlElement("Event")] public Event tipEvent = null;

    [XmlAttribute("Active")] public bool active = true;

    [XmlAttribute("Owner")] public string owner;

}

[System.Serializable]
public class Event
{
    [XmlAttribute("Name")] public string eventName;
    [XmlAttribute("Talk")] public string talkName;
}

[System.Serializable]
public class Background
{
    [XmlAttribute("Name")] public string name;

    [XmlAttribute("Effect")] public BackgroundEffect effect = BackgroundEffect.NONE;
    [XmlAttribute("Scale")] public float scale = 1;
}

[System.Serializable]
public class Animation
{
    [XmlAttribute("Type")] public AnimationType type = AnimationType.UTIL;

    [XmlAttribute("Effect")] public string effect;

    [XmlAttribute("Name")] public string name;

    [XmlAttribute("Parameter")] public string parameter;

    [XmlAttribute("Duration")] public float duration = -1;
}

[System.Serializable]
public class Character
{
    [XmlAttribute("Name")] public string name;
    
    [XmlAttribute("Clothes")] public string clothes;

    [XmlAttribute("Face")] public string face;

    [XmlAttribute("Pos")] public CharacterPos pos;

    [XmlAttribute("Size")] public CharacterSize size = CharacterSize.M;

    [XmlAttribute("Dark")] public bool dark = true;
}

public enum BackgroundEffect
{
    NONE,
    TRANS,
    FADE
}

public enum AnimationType
{
    CHAR,
    UTIL,
    DIAL,
    CAM
}

public enum CharacterPos
{
    R3,
    R2,
    R1,
    REND,
    C,
    L1,
    L2,
    L3,
    LEND
}

public enum CharacterSize
{
    S,
    M,
    L,
    XL
}