using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.ServiceLocator;
using System.Linq;

public class DataCenter : Service
{
    public void PushData(string id, AbstractData data)
    {
        if (datasheet.ContainsKey(id)) datasheet[id] = data;
        else datasheet.Add(id, data);
    }

    public bool ArchiveData(string id)
    {
        if (datasheet.ContainsKey(id)) 
        {
            datasheet.Remove(id);
            return true;
        }
        return false;
    }

    public DataType GetData<DataType>(string id) where DataType : AbstractData<DataType>
    {
        if (datasheet.ContainsKey(id))
        {
            var data = datasheet[id] as DataType;
            return data;
        }
        else 
            return null;
    }

    private Dictionary<string, AbstractData> datasheet = new Dictionary<string, AbstractData>();
}
