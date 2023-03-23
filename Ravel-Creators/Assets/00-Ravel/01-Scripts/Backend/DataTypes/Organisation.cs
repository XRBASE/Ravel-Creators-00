using System;
using System.Collections.Generic;
using System.Linq;
using Base.Ravel.BackendData.Data;
using UnityEngine;

[Serializable]
public class Organisation : DataContainer
{
    public override string Key {
        get { return organizationId; }
    }
    
    [SerializeField] public string organizationId;
    [SerializeField] public string organizationName;
    
    public Organisation()
    {
        
    }
    
    public Organisation(string organizationId, string organizationName)
    {
        this.organizationId = organizationId;
        this.organizationName = organizationName;
    }
    
    public override bool Overwrite(DataContainer data)
    {
        if (data.GetType() == typeof(Organisation)) {
            bool hasChanges = false;
            Organisation other = (Organisation) data;
            
            if (!string.IsNullOrEmpty(other.organizationId)) {
                organizationId = other.organizationId;
                hasChanges = true;
            }
            if (!string.IsNullOrEmpty(other.organizationName)) {
                organizationName = other.organizationName;
                hasChanges = true;
            }

            return hasChanges;
        }


        throw GetOverwriteFailedException(data);
    }

    public override string ToString()
    {
        return $"{organizationName}-{Key}";
    }

    public static string[] GetOrganisationNames(IEnumerable<Organisation> organisations)
    {
        var orgs = organisations as Organisation[] ?? organisations.ToArray();
        
        string[] output = new string[orgs.Length];
        for (int i = 0; i < orgs.Length; i++) {
            output[i] = orgs[i].organizationName;
        }

        return output;
    }
}
