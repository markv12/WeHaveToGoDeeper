using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueUtility {
    // grab the remainder of the text until ">" or end of string
    private const string REMAINDER_REGEX = "(.*?((?=>)|(/|$)))";
    private const string PAUSE_REGEX_STRING = "<p:(?<pause>" + REMAINDER_REGEX + ")>";
    private static readonly Regex pauseRegex = new Regex(PAUSE_REGEX_STRING);
    private const string SPEED_REGEX_STRING = "<sp:(?<speed>" + REMAINDER_REGEX + ")>";
    private static readonly Regex speedRegex = new Regex(SPEED_REGEX_STRING);
    private const string ANIM_START_REGEX_STRING = "<anim:(?<anim>" + REMAINDER_REGEX + ")>";
    private static readonly Regex animStartRegex = new Regex(ANIM_START_REGEX_STRING);
    private const string ANIM_END_REGEX_STRING = "</anim>";
    private static readonly Regex animEndRegex = new Regex(ANIM_END_REGEX_STRING);
    private const string SOUND_REGEX_STRING = "<sound:(?<sound>" + REMAINDER_REGEX + ")>";
    private static readonly Regex soundRegex = new Regex(SOUND_REGEX_STRING);
    private const string PORTRAIT_REGEX_STRING = "<portrait:(?<portrait>" + REMAINDER_REGEX + ")>";
    private static readonly Regex portraitRegex = new Regex(PORTRAIT_REGEX_STRING);
    private const int ICON_SIZE_MULT = 150;

    private static readonly Dictionary<string, float> pauseDictionary = new Dictionary<string, float> {
            { "tiny", .1f },
            { "short", .25f },
            { "normal", 0.666f },
            { "long", 1f },
            { "read", 2f },
        };

    public static List<DialogueCommand> ProcessInputString(string message, out string processedMessage) {
        List<DialogueCommand> result = new List<DialogueCommand>();
        processedMessage = message;

        processedMessage = HandlePauseTags(processedMessage, result);

        processedMessage = HandleSpeedTags(processedMessage, result);

        processedMessage = HandleAnimStartTags(processedMessage, result);
        processedMessage = HandleAnimEndTags(processedMessage, result);

        processedMessage = HandleSoundTags(processedMessage, result);

        processedMessage = HandlePortraitTags(processedMessage, result);

        return result;
    }

    private static string HandlePortraitTags(string processedMessage, List<DialogueCommand> result) {
        MatchCollection portraitMatches = portraitRegex.Matches(processedMessage);
        foreach (Match match in portraitMatches) {
            string stringVal = match.Groups["portrait"].Value;
            result.Add(new DialogueCommand {
                position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                type = DialogueCommandType.Portrait,
                stringValue = stringVal
            });
        }
        processedMessage = Regex.Replace(processedMessage, PORTRAIT_REGEX_STRING, "");
        return processedMessage;
    }

    private static string HandleSoundTags(string processedMessage, List<DialogueCommand> result) {
        MatchCollection soundMatches = soundRegex.Matches(processedMessage);
        foreach (Match match in soundMatches) {
            string stringVal = match.Groups["sound"].Value;
            result.Add(new DialogueCommand {
                position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                type = DialogueCommandType.Sound,
                stringValue = stringVal
            });
        }
        processedMessage = Regex.Replace(processedMessage, SOUND_REGEX_STRING, "");
        return processedMessage;
    }

    private static string HandleAnimEndTags(string processedMessage, List<DialogueCommand> result) {
        MatchCollection animEndMatches = animEndRegex.Matches(processedMessage);
        foreach (Match match in animEndMatches) {
            result.Add(new DialogueCommand {
                position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                type = DialogueCommandType.AnimEnd,
            });
        }
        processedMessage = Regex.Replace(processedMessage, ANIM_END_REGEX_STRING, "");
        return processedMessage;
    }

    private static string HandleAnimStartTags(string processedMessage, List<DialogueCommand> result) {
        MatchCollection animStartMatches = animStartRegex.Matches(processedMessage);
        foreach (Match match in animStartMatches) {
            string stringVal = match.Groups["anim"].Value;
            result.Add(new DialogueCommand {
                position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                type = DialogueCommandType.AnimStart,
                textAnimValue = GetTextAnimationType(stringVal)
            });
        }
        processedMessage = Regex.Replace(processedMessage, ANIM_START_REGEX_STRING, "");
        return processedMessage;
    }

    private static string HandleSpeedTags(string processedMessage, List<DialogueCommand> result) {
        MatchCollection speedMatches = speedRegex.Matches(processedMessage);
        foreach (Match match in speedMatches) {
            string stringVal = match.Groups["speed"].Value;
            if (!float.TryParse(stringVal, out float val)) {
                val = 150f;
            }
            result.Add(new DialogueCommand {
                position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                type = DialogueCommandType.TextSpeedChange,
                floatValue = val
            });
        }
        processedMessage = Regex.Replace(processedMessage, SPEED_REGEX_STRING, "");
        return processedMessage;
    }

    private static string HandlePauseTags(string message, List<DialogueCommand> result) {
        MatchCollection pauseMatches = pauseRegex.Matches(message);
        foreach (Match match in pauseMatches) {
            string val = match.Groups["pause"].Value;
            string pauseName = val;
            Debug.Assert(pauseDictionary.ContainsKey(pauseName), $"no pause registered for '{pauseName}'");
            result.Add(new DialogueCommand {
                position = VisibleCharactersUpToIndex(message, match.Index),
                type = DialogueCommandType.Pause,
                floatValue = pauseDictionary[pauseName]
            });
        }
        message = Regex.Replace(message, PAUSE_REGEX_STRING, "");
        return message;
    }

    private static TextAnimationType GetTextAnimationType(string stringVal) {
        TextAnimationType result;
        try {
            result = (TextAnimationType)Enum.Parse(typeof(TextAnimationType), stringVal, true);
        } catch (ArgumentException) {
            Debug.LogError($"Invalid Text Animation Type: {stringVal}");
            result = TextAnimationType.none;
        }
        return result;
    }

    private static int VisibleCharactersUpToIndex(string message, int index) {
        int result = 0;
        bool insideBrackets = false;
        for (int i = 0; i < index; i++) {
            if (message[i] == '<') {
                insideBrackets = true;
            } else if (message[i] == '>') {
                insideBrackets = false;
                result--;
            }
            if (!insideBrackets) {
                result++;
            } else if (i + 6 < index && message.Substring(i, 6) == "sprite") {
                result++;
            }
        }
        return result;
    }

    public static string GenerateSpriteTag(int spriteIndex) {
        float vOffset = -0.2f * (((float)ICON_SIZE_MULT - 100) / 100f);
        return $"<voffset={vOffset}em><size={ICON_SIZE_MULT}%><sprite index=\"{spriteIndex}\"></size></voffset>";
    }
}
