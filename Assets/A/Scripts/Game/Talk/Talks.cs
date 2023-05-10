using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class Talks : ScriptableObject
{
    public string cgTitle;
    [TextArea] public string skipText;
    public List<Talk> talks = new List<Talk>();
}

[System.Serializable]
public class Talk
{
    [XmlElement("Bgm")] 
    public string bgm = "";

    [XmlElement("BG")] 
    public Background background = new Background();

    [XmlElement("Dialogue")] 
    public Dialogue dialogue;

    [XmlElement("Event")] 
    public List<Event> eventList = new List<Event>();

    [XmlElement("Option")] 
    public List<Option> optionList;

    [XmlArray("Characters")]
    [XmlArrayItem("Character")]
    public List<Character> characters = new List<Character>();

    [FormerlySerializedAs("animations")] [XmlElement("Animations")] 
    public List<AnimationList> animationLists = new List<AnimationList>();
}

[System.Serializable]
public class TalkAnimation
{
    [XmlAttribute("Index")]
    public int startIndex = 0;

    [XmlAttribute("Type")] 
    public TalkAnimationType type = TalkAnimationType.ANIM;
    
    [XmlAttribute("Parameter")]
    public float parameter;
}

[System.Serializable]
public class AnimationList
{
    [XmlAttribute("Index")]
    public int index = 0;
    
    [XmlElement("Animation")]
    public List<Animation> animations = new List<Animation>();
}

[System.Serializable]
public class Option
{
    [XmlAttribute("Script")] 
    public string script;

    [XmlElement("SkipText")] 
    public string skipText;
    
    [XmlAttribute("Dialog")] 
    public string dialog;

    [XmlAttribute("DialogType")]
    public EventType eventType;
    
    [XmlAttribute("Special")]
    public bool special;

    [XmlArray("Dialogs")]
    [XmlArrayItem("Dialog")]
    [HideInInspector]
    public List<Talk> dialogs;

    [XmlElement("Event")] 
    public List<Event> eventList = new List<Event>();
}


[System.Serializable]
public class TipEvent
{
    [XmlAttribute("Name")] 
    public string eventName;

    [XmlElement("SkipText")] 
    public string skipText;

    [XmlAttribute("Dialog")] 
    public string talkName;

    [XmlArray("Dialogs")]
    [XmlArrayItem("Dialog")]
    [HideInInspector]
    public List<Talk> dialogs;

    [XmlAttribute("Type")]
    public EventType eventType = EventType.CHANGE;
}

[System.Serializable]
public class Dialogue
{
    [XmlElement("Talker")] 
    public string talker;

    [XmlElement("Text")] 
    public string text;

    [XmlElement("TipEvent")]  
    public List<TipEvent> tipEvent = new List<TipEvent>();

    [XmlElement("TalkAnimation")]  
    public List<TalkAnimation> talkAnimations = new List<TalkAnimation>();

    [XmlAttribute("Active")] 
    public bool active = true;

    [XmlAttribute("Invisible")] 
    public bool invisible;

    [XmlAttribute("Owner")] 
    public string owner;

}

[System.Serializable]
public class Event
{
    [XmlAttribute("Name")] 
    public string name;
    
    [XmlAttribute("Type")] 
    public Type type;
    public enum Type
    {
        ADD_TIP,
    }
}


[System.Serializable]
public class Background
{
    [XmlAttribute("Name")] 
    public string name;
    
    [XmlAttribute("Effect")]
    public BackgroundEffect effect = BackgroundEffect.NONE;

    [XmlAttribute("Title")]
    public string title;

    [XmlAttribute("Description")]
    public string description;
    
    [XmlAttribute("Duration")] 
    public float effectDuration = 1;

    [XmlAttribute("Scale")] 
    public float scale = 1;
}

[System.Serializable]
public class Animation
{
    [XmlAttribute("Type")] 
    public AnimationType type = AnimationType.UTIL;

    [XmlAttribute("Effect")] 
    public string effect;

    [XmlAttribute("Name")] 
    public string name;

    [XmlAttribute("Parameter")] 
    public string parameter;

    [XmlAttribute("Duration")] 
    public float duration = -1;
}

[System.Serializable]
public class Character
{
    [XmlAttribute("Name")] 
    public string name;

    [XmlAttribute("Clothes")] 
    public string clothes;

    [XmlAttribute("Face")] 
    public string face;

    [XmlAttribute("Pos")] 
    public CharacterPos pos = CharacterPos.N;

    [XmlAttribute("Size")] 
    public CharacterSize size = CharacterSize.N;

    [XmlAttribute("Dark")] 
    public bool dark = true;
}

public enum BackgroundEffect
{
    NONE,
    TRANS,
    FADE
}

public enum EventType
{
    BEFORE,
    CHANGE,
    AFTER
}

public enum AnimationType
{
    CHAR,
    UTIL,
    DIAL,
    CAM
}

public enum TalkAnimationType
{
    WAIT,
    ANIM
}
public enum CharacterPos
{
    N,
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
    N,
    S,
    M,
    L,
    XL
}