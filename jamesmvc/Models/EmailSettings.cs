﻿namespace jamesmvc.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }

        public string SmtpUsername { get; set; }
    }
}
