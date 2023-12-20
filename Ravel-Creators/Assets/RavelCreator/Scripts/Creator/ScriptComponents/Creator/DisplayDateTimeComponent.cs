using System;
using TMPro;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Displays current date on textmeshprougui
    /// </summary>
    [AddComponentMenu("Ravel/Display Date Time")]
    [HelpURL("https://www.notion.so/thenewbase/Display-Date-Component-e8fbf232ef944cc6b648390a27679e30?pvs=4")]
    public partial class DisplayDateTimeComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        public void ShowDateTime() { }

        public static string ParseDateTime(DisplayDateTimeData.DateTimeFormat format, string customFormat = default)
        {
            switch (format)
            {
                case DisplayDateTimeData.DateTimeFormat.Date:
                    return DateTime.Now.ToShortDateString();
                case DisplayDateTimeData.DateTimeFormat.Time:
                    return $"{DateTime.Now:HH:mm:ss}";
                case DisplayDateTimeData.DateTimeFormat.LongDate:
                    return DateTime.Now.ToLongDateString();
                case DisplayDateTimeData.DateTimeFormat.DateTime:
                    return $"{DateTime.Now.ToShortDateString()}, {DateTime.Now:HH:mm:ss}";
                case DisplayDateTimeData.DateTimeFormat.Custom:
                    return DateTime.Now.ToString(customFormat);
                default:
                    return String.Empty;
            }
        }
        
        [SerializeField] private DisplayDateTimeData _data;
    }

    [Serializable]
    public class DisplayDateTimeData : ComponentData
    {
        public TextMeshProUGUI textMeshProUGUI;
        
        public string prefixText;
        public string postfixText;
        
        public DateTimeFormat dateTimeFormat;
        [Tooltip("Only required for datestrings using the custom format")] 
        public string customDateTimeFormat;

        public enum DateTimeFormat
        {
            Date,
            LongDate,
            DateTime,
            Time,
            Custom
        }
    }
}