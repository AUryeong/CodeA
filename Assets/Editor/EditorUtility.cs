using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace EditorUtil
{
    [System.Serializable]
    public class XMLTalks
    {
        [XmlElement("Title")] public string cgTitle;

        [XmlElement("Talk")] public List<Talk> talks = new List<Talk>();
    }


    public class EditorUtility
    {
        private const string REMAINDER_REGEX = "(.*?((?=})|(/|$)))";
        private const string TIP_REGEX_STRING = "{(?<Tip>" + REMAINDER_REGEX + ")}";
        private static readonly Regex pauseRegex = new Regex(TIP_REGEX_STRING);

        [MenuItem("Assets/Convert Xml To ScriptableObject/Talk")]
        public static void CreateTalkScriptableObject()
        {
            Debug.Log("난카이데모");
            const string csvPath = "/Editor/Xmls/Talk";
            var dir = new DirectoryInfo(Application.dataPath + csvPath);
            ConvertXmlTalk(dir);

            AssetDatabase.SaveAssets();
        }

        private static void ConvertXmlTalk(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length > 0)
            {
                var directories = dir.GetDirectories();
                foreach (var directory in directories)
                {
                    ConvertXmlTalk(directory);
                }
            }
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                if (fileInfo.FullName.EndsWith(".meta")) continue;

                Debug.Log(Path.GetFileNameWithoutExtension(fileInfo.FullName));
                string str = File.ReadAllText(fileInfo.FullName);

                XMLTalks talks;
                using (var stringReader = new StringReader(str))
                {
                    talks = (XMLTalks)new XmlSerializer(typeof(XMLTalks)).Deserialize(stringReader);
                }

                var newTalks = ScriptableObject.CreateInstance<Talks>();

                newTalks.talks = talks.talks;
                string background = string.Empty;
                float backgroundScale = 1;
                string bgm = string.Empty;

                var characterDictionary = new Dictionary<string, string>();
                var faceDictionary = new Dictionary<string, string>();
                foreach (var talk in newTalks.talks)
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
                        if (talk.dialogue.tipEvent != null)
                        {
                            var matches = pauseRegex.Matches(talk.dialogue.text);
                            int indexAdd = 0;
                            for (var i = 0; i < matches.Count; i++)
                            {
                                var match = matches[i];
                                string command = match.Groups["Tip"].Value;
                                string eventName = talk.dialogue.tipEvent[i].eventName;

                                string s = "<#FFEE00><bounce><link=" + eventName + ">" + command + "</color></bounce></link>";
                                talk.dialogue.text = talk.dialogue.text.Insert(match.Index + indexAdd, s);
                                indexAdd += s.Length ;
                            }

                            Debug.Log(talk.dialogue.text );
                            talk.dialogue.text = Regex.Replace(talk.dialogue.text, TIP_REGEX_STRING, "");
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
                            if (characterDictionary.ContainsKey(character.name))
                                characterDictionary[character.name] = character.clothes;
                            else
                                characterDictionary.Add(character.name, character.clothes);
                        }

                        if (string.IsNullOrEmpty(character.face))
                        {
                            if (!faceDictionary.ContainsKey(character.name))
                            {
                                faceDictionary.Add(character.name, "Default");
                            }

                            character.face = faceDictionary[character.name];
                        }
                        else
                        {
                            if (faceDictionary.ContainsKey(character.name))
                                faceDictionary[character.name] = character.face;
                            else
                                faceDictionary.Add(character.name, character.face);
                        }
                    }
                }

                newTalks.cgTitle = talks.cgTitle;

                AssetDatabase.CreateAsset(newTalks, $"Assets/ScriptableObjects/Talks/{Path.GetFileNameWithoutExtension(fileInfo.FullName)}.asset");
            }
        }


        [MenuItem("Assets/Convert Tsv To ScriptableObject/Tip")]
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
}