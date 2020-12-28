using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Record the motor turn info into sqlite file
/// </summary>
public class VendingMachineOperateRecordDBModelEntity : DBModelMaster
{

    /// <summary>
    /// Save PlayerPrefs value into all table column set in inspector
    /// </summary>
    public override void SaveToLocal()
    {
        base.SaveToLocal();

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int i = 0; i < col.Count; i++)
        {
            val[i] = PlayerPrefs.GetString(col[i]);
        }

        AddData(col, val);
    }

}