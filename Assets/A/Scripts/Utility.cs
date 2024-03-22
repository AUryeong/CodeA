using System;
using System.Collections.Generic;
using InGame;
using UnityEngine;

public static class Utility
{
    public static T GetEnum<T>(string enumName)
    {
        return (T)Enum.Parse(typeof(T), enumName);
    }

    public static string GetDialogName(string talkerName)
    {
        string lastName = GameManager.Instance.saveManager.GameData.lastName;
        string name = GameManager.Instance.saveManager.GameData.name;
        string fullName = lastName + name;
        return string.Format(talkerName, fullName, lastName, name);
    }

    public static Vector3 GetVector3Aver(Vector3 vector, Vector3 vector2)
    {
        return (vector + vector2) / 2;
    }

    public static Color GetFadeColor(this Color color, float fade)
    {
        color.a = fade;
        return color;
    }

    public static DialogEventType GetStringToEventType(string eventType)
    {
        if (!string.IsNullOrEmpty(eventType))
        {
            switch (eventType)
            {

                case "Before":
                case "BEFORE":
                    return DialogEventType.Before;
                case "Change":
                case "CHANGE":
                    return DialogEventType.Change;
                case "After":
                case "AFTER":
                    return DialogEventType.After;
            }
        }
        return DialogEventType.Change;
    }

    public static string GetTimeToString(this InGameTime time)
    {
        return $"{time.year}년 {time.month}월 {time.week}주 {GetTimeToString(time.time)}";;
    }
    public static string GetTimeToString(this TimeType timeType)
    {
        switch (timeType)
        {
            case TimeType.Morning:
                return "아침";
            case TimeType.Afternoon:
                return "오후";
            case TimeType.Night:
                return "밤";
            case TimeType.Dawn:
                return "새벽";
            default:
                return "아침";
        }
    }

    public static float PosToVector2(string pos)
    {
        switch (pos)
        {
            case "REnd":
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
            case "LEnd":
                return -1580;
            default:
                return 0;
        }
    }

    public static float PosToVector2(DialogCharacterPos pos)
    {
        switch (pos)
        {
            case DialogCharacterPos.Lend:
                return -1580;
            case DialogCharacterPos.L3:
                return -540f;
            case DialogCharacterPos.L2:
                return -360f;
            case DialogCharacterPos.L1:
                return -180;
            case DialogCharacterPos.C:
                return 0;
            case DialogCharacterPos.R1:
                return 180;
            case DialogCharacterPos.R2:
                return 360;
            case DialogCharacterPos.R3:
                return 540;
            case DialogCharacterPos.Rend:
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

    public static Vector3 SizeToScale(DialogCharacterSize size)
    {
        switch (size)
        {
            default:
            case DialogCharacterSize.S:
                return new Vector3(0.6f, 0.6f, 0.6f);
            case DialogCharacterSize.M:
                return new Vector3(0.7f, 0.7f, 0.7f);
            case DialogCharacterSize.L:
                return new Vector3(0.8f, 0.8f, 0.8f);
            case DialogCharacterSize.XL:
                return new Vector3(1f, 1f, 1f);
        }
    }

    public static T SelectOne<T>(this List<T> tList)
    {
        return tList[UnityEngine.Random.Range(0, tList.Count)];
    }

    public static T SelectOne<T>(params T[] tList)
    {
        return tList[UnityEngine.Random.Range(0, tList.Length)];
    }
    
    public static readonly Color fadeBlackColor = new Color(0.16f, 0.16f, 0.16f, 0);
    public static readonly Color darkColor = new Color(0.75f, 0.75f, 0.75f, 1);
}