using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateAndTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    private void Update()
    {
        DateTime dateTime = DateTime.Now;

        // Format time as hh:mm:ss tt (12-hour clock format with AM/PM)
        string timeString = dateTime.ToString("hh:mm:ss tt");
        timeText.text = "Time: " + timeString;

        // Format date as "DayOfWeek, Month Day, Year"
        string dateString = dateTime.ToString("dddd, MMMM dd, yyyy");
        dateText.text = "Date: " + dateString;
    }
}
