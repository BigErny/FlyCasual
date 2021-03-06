﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUI : MonoBehaviour {

    public GameObject PlaymatSelector;

    public void OnClickPlaymatChange(GameObject playmatImage)
    {
        PlayerPrefs.SetString("PlaymatName", playmatImage.name);
        PlayerPrefs.Save();

        Options.Playmat = playmatImage.name;

        PlaymatSelector.transform.position = playmatImage.transform.position;
    }

}
