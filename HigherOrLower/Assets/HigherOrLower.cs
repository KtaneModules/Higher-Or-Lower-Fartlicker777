using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class HigherOrLower : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable Higher;
    public KMSelectable Lower;

    public TextMesh NumbahFuckah;

    int[][] RGBSequences = {
      new int[] {2, 45, 58, 80, 50, 25, 83},
      new int[] {60, 5, 48, 79, 40, 54, 44},
      new int[] {19, 94, 43, 31, 1, 42, 53},
      new int[] {51, 84, 24, 6, 67, 47, 26},

      new int[] {11, 40, 22, 38, 65, 65, 79},
      new int[] {41, 42, 17, 95, 80, 45, 11},
      new int[] {14, 31, 92, 69, 57, 21, 38},
      new int[] {94, 34, 70, 44, 67, 16, 82},
    };
    int Selector = 0;
    int Iteration = 0;
    int NumberShown = 0;

    string[] ColorName = {"Grey", "Red", "Green", "Blue", "Cyan", "Magenta", "Yellow", "White"};

    bool IsHigher = false;

    Color32[] ColorsForText = new Color32[] {new Color32(127, 127, 127, 255), new Color32(255, 0, 0, 255), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255), new Color32(0, 255, 255, 255), new Color32(255, 0, 255, 255), new Color32(255, 255, 0, 255), new Color32(255, 255, 255, 255)};

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        Higher.OnInteract += delegate () { HigherPress(); return false; };
        Lower.OnInteract += delegate () { LowerPress(); return false; };
    }

    void Start () {
      RowSelector();
      NumberSelector();
    }

    void HigherPress () {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Higher.transform);
      if (moduleSolved)
        return;
      if (IsHigher) {
        Iteration++;
        if (Iteration > 6) {
          GetComponent<KMBombModule>().HandlePass();
          moduleSolved = true;
        }
        else
          NumberSelector();
      }
      else {
        GetComponent<KMBombModule>().HandleStrike();
        Iteration = 0;
      }
    }

    void LowerPress () {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Lower.transform);
      if (moduleSolved)
        return;
      if (!IsHigher) {
        Iteration++;
        if (Iteration > 6) {
          GetComponent<KMBombModule>().HandlePass();
          moduleSolved = true;
        }
        else
          NumberSelector();
      }
      else {
        GetComponent<KMBombModule>().HandleStrike();
        Iteration = 0;
      }
    }

    void RowSelector () {
      Selector = UnityEngine.Random.Range(0,8);
      Debug.LogFormat("[Higher Or Lower #{0}] The chosen color now is {1}.", moduleId, ColorName[Selector]);
      NumbahFuckah.color = ColorsForText[Selector];
    }

    void NumberSelector () {
      RowSelector();
      NumberShown = UnityEngine.Random.Range(1,100);
      while (NumberShown == RGBSequences[Selector][Iteration])
        NumberShown = UnityEngine.Random.Range(1,100);
      if (NumberShown > RGBSequences[Selector][Iteration])
        IsHigher = true;
      else
        IsHigher = false;
      Debug.LogFormat("[Higher Or Lower #{0}] The current number is {1} at step {2}.", moduleId, NumberShown, Iteration + 1);
      if (IsHigher)
        Debug.LogFormat("[Higher Or Lower #{0}] This is higher.", moduleId);
      else
        Debug.LogFormat("[Higher Or Lower #{0}] This is lower.", moduleId);
      NumbahFuckah.text = NumberShown.ToString("00");
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} higher/lower to press that corresponding side.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      if (Command.Trim().ToUpper() == "HIGHER")
        Higher.OnInteract();
      else if (Command.Trim().ToUpper() == "LOWER")
        Lower.OnInteract();
      else
        yield return "sendtochaterror I don't understand!";
    }

    IEnumerator TwitchHandleForcedSolve () {
      while (!moduleSolved) {
        if (IsHigher)
          Higher.OnInteract();
        else
          Lower.OnInteract();
        yield return new WaitForSeconds(.1f);
      }
    }
}
