﻿using System;

namespace RPS.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
    }
}
