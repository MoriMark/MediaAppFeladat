﻿namespace MediaAppFeladat.Models
{
    public class EmailSettings
    {
        public string SmptServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set;} = string.Empty;
        public string SenderPassword {  get; set; } = string.Empty;
    }
}
