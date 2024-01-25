using System;
using System.Collections.Generic;
using Base.Ravel.Creator.Components;
using TMPro;
using UnityEngine;

public partial class WheelOfFortuneComponent : ComponentBase, IUniqueId
{
    public bool SetUniqueID {
        get { return _data.networked; }
    }

    public int ID {
        get { return _data.netIndex;}
        set { _data.netIndex = value; }
    }
    
    public override ComponentData Data {
        get { return _data; }
    }

    [SerializeField] private WheelData _data;
    
    protected override void DisposeData() { }

    public void Spin() { }

    public void AddEntry() { }

    [Serializable]
    public class WheelData : ComponentData
    {
        //wheel
        [Tooltip("The amount of options available on the wheel. This cannot change dynamically")]
        public int wheelSegments = 32;
        [Tooltip("The transform component of the wheel. This component will spin around its Z axis.")]
        public Transform wheelTransform;
        [Tooltip("determines the speed of rotation over time.")]
        public AnimationCurve rotationSpeedCurve;
        [Tooltip("Duration of the spin in seconds.")]
        public float spinDuration = 2.0f;
        [Tooltip("Amount of additional spins the wheel makes before stopping")]
        public int extraLoops = 2;
        
        //canvas
        [Tooltip("Should input and data of this object be networked among player?")]
        public bool networked;
        [HideInInspector] public int netIndex;
        [Tooltip("In this text component the result option will be displayed")]
        public TMP_Text resultField;
        [Tooltip("This is the template that is used to create new entries/options from")]
        public WheelEntryComponent templateEntry;
        [Tooltip("Place holder for new entries before a name has been filled in")] 
        public string namePlaceHolder = "Name";
        [Tooltip("This is the initial list of options available to the player")] 
        public List<string> options;
    }
}
