using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static T GetEnum<T>(string enumName)
    {
        return (T)Enum.Parse(typeof(T), enumName);
    }

    public static string GetTalkerName(string talkerName)
    {
        return string.Format(talkerName, SaveManager.Instance.GameData.name);
    }

    public static Vector3 GetVector3Aver(Vector3 vector, Vector3 vector2)
    {
        return (vector + vector2) / 2;
    }

    public static Color ChangeColorFade(Color color, float fade)
    {
        return new Color(color.r, color.g, color.b, fade);
    }

    public static EventType GetStringToEventType(string eventType)
    {
        if (!string.IsNullOrEmpty(eventType))
        {
            switch (eventType)
            {

                case "Before":
                case "BEFORE":
                    return EventType.BEFORE;
                case "Change":
                case "CHANGE":
                    return EventType.CHANGE;
                case "After":
                case "AFTER":
                    return EventType.AFTER;
            }
        }
        return EventType.CHANGE;
    }

    public static string GetTimeToString(TimeType timeType)
    {
        switch (timeType)
        {
            case TimeType.MORNING:
                return "아침";
            case TimeType.AFTERNOON:
                return "오후";
            case TimeType.NIGHT:
                return "밤";
            case TimeType.DAWN:
                return "새벽";
            default:
                return "아침";
        }
    }

    public static float PosToVector2(string pos)
    {
        switch (pos)
        {
            case "REND":
                return 1580;
            case "R3":
                return 540f;
            case "R2":
                return 360f;
            case "R1":
                return 180;
            case "C":
                return 0;
            case "L1":
                return -180;
            case "L2":
                return -360;
            case "L3":
                return -540;
            case "LEND":
                return -1580;
            default:
                return 0;
        }
    }

    public static float PosToVector2(CharacterPos pos)
    {
        switch (pos)
        {
            case CharacterPos.LEND:
                return -1580;
            case CharacterPos.L3:
                return -540f;
            case CharacterPos.L2:
                return -360f;
            case CharacterPos.L1:
                return -180;
            case CharacterPos.C:
                return 0;
            case CharacterPos.R1:
                return 180;
            case CharacterPos.R2:
                return 360;
            case CharacterPos.R3:
                return 540;
            case CharacterPos.REND:
                return 1580;
            default:
                return 0;
        }
    }

    public static Vector3 SizeToScale(string size)
    {
        switch (size)
        {
            default:
                return new Vector3(0.6f, 0.6f, 0.6f);
            case "M":
                return new Vector3(0.7f, 0.7f, 0.7f);
            case "L":
                return new Vector3(0.8f, 0.8f, 0.8f);
            case "XL":
                return new Vector3(1f, 1f, 1f);
        }
    }

    public static Vector3 SizeToScale(CharacterSize size)
    {
        switch (size)
        {
            default:
            case CharacterSize.S:
                return new Vector3(0.6f, 0.6f, 0.6f);
            case CharacterSize.M:
                return new Vector3(0.7f, 0.7f, 0.7f);
            case CharacterSize.L:
                return new Vector3(0.8f, 0.8f, 0.8f);
            case CharacterSize.XL:
                return new Vector3(1f, 1f, 1f);
        }
    }

    public static T SelectOne<T>(List<T> tList)
    {
        return tList[UnityEngine.Random.Range(0, tList.Count)];
    }

    public static T SelectOne<T>(params T[] tList)
    {
        return tList[UnityEngine.Random.Range(0, tList.Length)];
    }
    
    public static Color GetFadeColor(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static Color fadeWhite = new Color(1, 1, 1, 0);
    public static Color fadeOutBlack = new Color(0.16f, 0.16f, 0.16f, 0);
    public static Color fadeDarkColor = new Color(0.75f, 0.75f, 0.75f, 0);
    public static Color darkColor = new Color(0.75f, 0.75f, 0.75f, 1);
}