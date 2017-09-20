namespace EventBus.Alert
{
    public class SMSAlertOptions
    {
        public string[] Contacts { get; set; }

        public long AlertIntervalSecs { get; set; } = 60 * 5;

        public bool Enable { get; set; } = true;
    }
}
