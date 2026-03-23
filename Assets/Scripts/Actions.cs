using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Actions
{ 

    public static Action<TextAsset> SetStory;
    //public static Action<FallingObject> SetSpeakers;
    public static Action<String> GetSpeakers;
    public static Action<Dialogue> GetHFDIalogue;
    public static Action<List<GameObject>> FallingObjectsOrdered;
}