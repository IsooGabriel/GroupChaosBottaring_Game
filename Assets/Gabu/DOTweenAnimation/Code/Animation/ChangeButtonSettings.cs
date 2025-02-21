﻿using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.UI.Button;
using UnityEngine.Serialization;

public class ChangeButtonSettings : MonoBehaviour
{
    public List<ImageAnimationManager> targets;
    public ImageAnimationManager[] settings;

    // Event delegates triggered on click.
    [FormerlySerializedAs("onSet")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    public void SetSettings(int index)
    {
        if (index < 0 || index >= settings.Length)
        {
            Debug.LogWarning("Index out of range: " + index);
            return;
        }
        m_OnClick.Invoke();
        for (int i = 0; i < targets.Count; i++)
        {
            for(int j = 0; j < targets[i].uiSystems.Length; j++)
            {
                if(targets[i].uiSystems[j] == null)
                {
                    Debug.LogWarning("uiSystemsが空です:" + targets[i].name);
                    continue;
                }
                targets[i].uiSystems[j].UpdateSettings(settings[index].uiSystems[j]);
            }
        }
    }
}