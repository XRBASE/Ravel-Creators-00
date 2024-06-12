using System;
using Base.Ravel.CustomAttributes;


[Serializable]
public class QuestData
{
    [ReadOnly] public string guid;
    [ReadOnly] public string environmentId;
    public string title;
    public string description;
}