using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

[Serializable]
public class XMLDialogs
{
    [XmlElement("Title")] public string cgTitle;

    [XmlElement("SkipText")] public string skipText;

    [XmlElement("Dialog")] public List<Talk> talks = new List<Talk>();
}

[Serializable]
public class XMLTalkOwners
{
    [XmlElement("Owner")] public List<Owner> owners = new List<Owner>();
}

[Serializable]
public class Owner
{
    [XmlAttribute("Name")] public string name;
    [XmlAttribute("OwnerName")] public string ownerName;
}


public class EditorUtility
{
    private const string remainderRegex = "(.*?((?=})|(/|$)))";
    private const string tipRegexString = "{(?<Tip>" + remainderRegex + ")}";
    private static readonly Regex tipRegex = new Regex(tipRegexString);

    [MenuItem("Assets/Convert Xml To ScriptableObject Talk")]
    public static void CreateTalkScriptableObject()
    {
        var ownerDictionaries = new Dictionary<string, string>();
        var str = File.ReadAllText(Application.dataPath + "/Editor/Xmls/TalkOwners.xml");
        XMLTalkOwners talkOwners;
        using (var stringReader = new StringReader(str))
        {
            talkOwners = (XMLTalkOwners)new XmlSerializer(typeof(XMLTalkOwners)).Deserialize(stringReader);
        }

        foreach (var owner in talkOwners.owners)
            ownerDictionaries.Add(owner.name, owner.ownerName);

        const string xmlPath = "/Editor/Xmls/Talk";
        var dir = new DirectoryInfo(Application.dataPath + xmlPath);
        ConvertXmlTalk(dir, ownerDictionaries);

        AssetDatabase.SaveAssets();
    }

    private static void ConvertXmlTalk(DirectoryInfo dir, Dictionary<string, string> ownerDictionaries)
    {
        var directories = dir.GetDirectories();
        if (directories.Length > 0)
        {
            foreach (var directory in directories)
            {
                ConvertXmlTalk(directory, ownerDictionaries);
            }
        }

        foreach (var fileInfo in dir.GetFiles())
        {
            if (fileInfo.FullName.EndsWith(".meta")) continue;

            string str = File.ReadAllText(fileInfo.FullName);

            XMLDialogs talks;
            using (var stringReader = new StringReader(str))
            {
                talks = (XMLDialogs)new XmlSerializer(typeof(XMLDialogs)).Deserialize(stringReader);
            }

            CreateNewAsset(Path.GetFileNameWithoutExtension(fileInfo.FullName), talks.talks, talks.cgTitle, talks.skipText, ownerDictionaries);
        }
    }

    private static void CreateNewAsset(string assetName, List<Talk> talkList, string title, string skipText, Dictionary<string, string> ownerDictionaries)
    {
        Debug.Log(assetName);
        var newTalks = ScriptableObject.CreateInstance<Talks>();

        ParsingTalks(assetName, ref talkList, ownerDictionaries);
        newTalks.talks = talkList;
        newTalks.cgTitle = title;
        newTalks.skipText = string.IsNullOrEmpty(skipText) ? null : skipText.Replace("\\n", "\n");

        AssetDatabase.CreateAsset(newTalks, $"Assets/ScriptableObjects/Talks/{assetName}.asset");
    }

    private static void ParsingTalks(string assetName, ref List<Talk> talkList, Dictionary<string, string> ownerDictionaries)
    {
        string background = string.Empty;
        float backgroundScale = 1;
        string bgm = string.Empty;

        var characterDictionary = new Dictionary<string, string>();
        var faceDictionary = new Dictionary<string, string>();
        var posDictionary = new Dictionary<string, CharacterPos>();
        var sizeDictionary = new Dictionary<string, CharacterSize>();
        foreach (var talk in talkList)
        {
            if (string.IsNullOrEmpty(talk.background.name))
            {
                talk.background.name = background;
                talk.background.scale = backgroundScale;
            }
            else
            {
                background = talk.background.name;
                backgroundScale = talk.background.scale;
            }

            if (string.IsNullOrEmpty(talk.bgm))
                talk.bgm = bgm;
            else
                bgm = talk.bgm;

            if (talk.dialogue != null)
            {
                if (!string.IsNullOrEmpty(talk.dialogue.talker))
                {
                    if (ownerDictionaries.TryGetValue(talk.dialogue.talker, out var dictionary))
                    {
                        if (string.IsNullOrEmpty(talk.dialogue.owner))
                        {
                            talk.dialogue.owner = dictionary;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(talk.dialogue.owner))
                        {
                            ownerDictionaries.Add(talk.dialogue.talker, talk.dialogue.owner);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(talk.dialogue.text))
                {
                    var matches = tipRegex.Matches(talk.dialogue.text);
                    int indexAdd = 0;
                    int indexAnimAdd = 0;
                    for (var i = 0; i < matches.Count; i++)
                    {
                        var match = matches[i];
                        var commands = match.Groups["Tip"].Value.Split(':');

                        if (commands[0] != "Tip")
                        {
                            Debug.Log(match.Index - indexAnimAdd + 1);
                            var talkAnimation = new TalkAnimation
                            {
                                startIndex = match.Index - indexAnimAdd + 1,
                                type = Utility.GetEnum<TalkAnimationType>(commands[1])
                            };
                            if (commands.Length < 3)
                            {
                                switch (talkAnimation.type)
                                {
                                    case TalkAnimationType.WAIT:
                                        talkAnimation.parameter = 0.5f;
                                        break;
                                    case TalkAnimationType.ANIM:
                                        talkAnimation.parameter = 1;
                                        break;
                                }
                            }
                            else
                            {
                                talkAnimation.parameter = float.Parse(commands[2]);
                            }

                            talk.dialogue.talkAnimations.Add(talkAnimation);
                            indexAnimAdd += match.Groups["Tip"].Value.Length + 2;
                            continue;
                        }

                        string name = commands[1];
                        string eventName = (commands.Length >= 3) ? commands[2] : commands[1];

                        bool flag = talk.dialogue.tipEvent == null || talk.dialogue.tipEvent.Count <= 0 ||
                                    !talk.dialogue.tipEvent.Exists((x) => !string.IsNullOrEmpty(x.eventName) && x.eventName == eventName);
                        if (flag)
                        {
                            talk.dialogue.tipEvent.Add(new TipEvent()
                            {
                                eventName = eventName,
                                talkName = (commands.Length >= 4) ? commands[3] : null,
                                eventType = (commands.Length >= 5) ? Utility.GetStringToEventType(commands[4]) : EventType.CHANGE
                            });
                        }
                        else
                        {
                            var tipEvent = talk.dialogue.tipEvent.Find((x) => !string.IsNullOrEmpty(x.eventName) && x.eventName == eventName);
                            if (tipEvent.dialogs != null)
                            {
                                string tipEventDialogName = $"{assetName}_TipEvent_{i}";
                                ParsingTalks(tipEventDialogName, ref tipEvent.dialogs, ownerDictionaries);
                                CreateNewAsset(tipEventDialogName, tipEvent.dialogs, string.Empty, tipEvent.skipText, ownerDictionaries);
                                tipEvent.dialogs = null;
                                tipEvent.talkName = tipEventDialogName;
                            }
                        }

                        string s;
                        if (commands.Length >= 4 || !flag)
                            s = "<#FFAA00><bounce><link=" + eventName + ">" + name + "</color></bounce></link>";
                        else
                            s = "<#FFEE00><link=" + eventName + ">" + name + "</color></link>";
                        talk.dialogue.text = talk.dialogue.text.Insert(match.Index + indexAdd, s);
                        indexAdd += s.Length;
                        indexAnimAdd += match.Groups["Tip"].Value.Length + 2 -name.Length;
                    }

                    talk.dialogue.text = Regex.Replace(talk.dialogue.text, tipRegexString, "");
                }

                if (talk.dialogue.active && talk.characters != null)
                {
                    if (!string.IsNullOrEmpty(talk.dialogue.owner))
                    {
                        var findCharacter = talk.characters.Find((character) => character.dark && talk.dialogue.owner == character.name);
                        if (findCharacter != null)
                            findCharacter.dark = false;
                    }
                }
            }

            if (talk.optionList != null && talk.optionList.Count > 0)
            {
                for (int i = 0; i < talk.optionList.Count; i++)
                {
                    Option option = talk.optionList[i];
                    if (option.dialogs != null && option.dialogs.Count > 0)
                    {
                        string tipEventDialogName = $"{assetName}_Option_{i}";
                        ParsingTalks(tipEventDialogName, ref option.dialogs, ownerDictionaries);
                        CreateNewAsset(tipEventDialogName, option.dialogs, string.Empty, option.skipText, ownerDictionaries);
                        option.dialogs = null;
                        option.dialog = tipEventDialogName;
                    }
                }
            }

            foreach (var character in talk.characters)
            {
                if (string.IsNullOrEmpty(character.clothes))
                {
                    if (!characterDictionary.ContainsKey(character.name))
                    {
                        characterDictionary.Add(character.name, "Default");
                    }

                    character.clothes = characterDictionary[character.name];
                }
                else
                {
                    characterDictionary[character.name] = character.clothes;
                }

                string face = character.face;
                if (string.IsNullOrEmpty(face))
                {
                    if (!faceDictionary.ContainsKey(character.name))
                    {
                        faceDictionary.Add(character.name, "Default");
                    }

                    face = faceDictionary[character.name];
                }

                character.face = face;

                CharacterPos pos = character.pos;
                if (pos == CharacterPos.N)
                {
                    if (!posDictionary.ContainsKey(character.name))
                    {
                        posDictionary.Add(character.name, CharacterPos.C);
                    }

                    pos = posDictionary[character.name];
                }

                character.pos = pos;

                CharacterSize size = character.size;
                if (size == CharacterSize.N)
                {
                    if (!sizeDictionary.ContainsKey(character.name))
                    {
                        sizeDictionary.Add(character.name, CharacterSize.M);
                    }

                    size = sizeDictionary[character.name];
                }

                character.size = size;

                if (talk.animationLists.Count > 0)
                {
                    var firstAnimation = talk.animationLists.Find((AnimationList anim) => anim.index == 0);
                    if (firstAnimation == null) return;
                    
                    foreach (var anim in firstAnimation.animations)
                    {
                        if (anim.type != AnimationType.CHAR) continue;
                        
                        if (anim.name == character.name)
                        {
                            switch (anim.effect)
                            {
                                case "Face":
                                    face = anim.parameter;
                                    break;
                                case "Scale":
                                    size = Utility.GetEnum<CharacterSize>(anim.parameter);
                                    break;
                                case "Move":
                                    pos = Utility.GetEnum<CharacterPos>(anim.parameter);
                                    break;
                            }
                        }
                    }

                    if (talk.dialogue.talkAnimations.Count > 0)
                    {
                        foreach (var talkAnimation in talk.dialogue.talkAnimations)
                        {
                            if (talkAnimation.type != TalkAnimationType.ANIM) continue;
                            
                            var animation = talk.animationLists.Find(animation => animation.index == Mathf.RoundToInt(talkAnimation.parameter));
                            foreach (var anim in animation.animations)
                            {
                                if (anim.type != AnimationType.CHAR) continue;
                                
                                if (anim.name == character.name)
                                {
                                    switch (anim.effect)
                                    {
                                        case "Face":
                                            face = anim.parameter;
                                            break;
                                        case "Scale":
                                            size = Utility.GetEnum<CharacterSize>(anim.parameter);
                                            break;
                                        case "Move":
                                            pos = Utility.GetEnum<CharacterPos>(anim.parameter);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                faceDictionary[character.name] = face;

                posDictionary[character.name] = pos;

                sizeDictionary[character.name] = size;
            }
        }
    }


    [MenuItem("Assets/Convert Tsv To ScriptableObject Tip")]
    public static void CreateTipScriptableObject()
    {
        const string csvPath = "/Editor/Csvs/Tip";
        var dir = new DirectoryInfo(Application.dataPath + csvPath);
        foreach (FileInfo fileInfo in dir.GetFiles())
        {
            if (fileInfo.FullName.EndsWith(".meta")) continue;

            Debug.Log(Path.GetFileNameWithoutExtension(fileInfo.FullName));
            var lines = File.ReadAllLines(fileInfo.FullName);
            var tipList = new List<Tip>();

            for (var index = 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (string.IsNullOrEmpty(line)) continue;

                var column = line.Split('\t');
                var tip = new Tip()
                {
                    id = column[0],
                    lore = column[1]
                };
                tipList.Add(tip);
            }

            var newTips = ScriptableObject.CreateInstance<Tips>();
            newTips.tips = tipList;
            AssetDatabase.CreateAsset(newTips, $"Assets/ScriptableObjects/Tips/{Path.GetFileNameWithoutExtension(fileInfo.FullName)}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}