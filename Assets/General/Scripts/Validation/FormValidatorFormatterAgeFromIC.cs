using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class FormValidatorFormatterAgeFromIC : FormValidatorFormatter
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private string GetDob(string IC_)
    {
        try
        {
            string year = IC_.Substring(0, 2);  // 85
            string month = IC_.Substring(2, 2); // 05
            string date = IC_.Substring(4, 2);   // 10

            // if year > current year
            if (Convert.ToInt32(year) > Convert.ToInt32(DateTime.Today.ToString("yy")))
                year = "19" + year; // birth year is 20th century (1900)
            else
                year = "20" + year; // birth year is 21th century (2000)

            string ICDate = year + '-' + month + '-' + date;
            //Debug.Log(ICDate);
            return ICDate;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return "";
        }
    }

    [Button]
    private string GetAge(string IC_)
    {
        try
        {
            string dob = GetDob(IC_);
            string age = (DateTime.Now.Year - DateTime.Parse(dob).Year).ToString();
            //Debug.Log(age);
            // Calculate the age.
            return age;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return "";
        }
    }

    public override string FormatTextForValidate(string value)
    {
        //return value;
        return GetAge(value);
    }

    public override string FormatTextForSave(string value)
    {
        return GetAge(value);
    }
}