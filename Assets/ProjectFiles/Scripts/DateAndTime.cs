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

        // Format time as hh:mm tt (12-hour clock format with AM/PM)
        string timeString = dateTime.ToString("hh:mm tt");
        timeText.text = timeString;

        // Format date as "DayOfWeek, Month Day, Year"
        string dateString = dateTime.ToString("dddd, MMMM dd, yyyy");
        dateText.text = dateString;
    }
}
