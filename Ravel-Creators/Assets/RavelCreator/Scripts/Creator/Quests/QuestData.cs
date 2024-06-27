using System;
using Base.Ravel.CustomAttributes;
using UnityEngine;


[Serializable]
public class QuestData
{
    [ReadOnly] public string guid;
    [Header("Copy the environmentId from \"https://YourDomain.org/dashboard/environment\"")]
    public string environmentId;
    public string title;
    public string description;
}