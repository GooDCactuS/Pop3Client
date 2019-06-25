using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pop3
{
    public class LetterClass
    {
        private string subject;
        private string from;
        private string to;
        private string date;
        private string size;
        private string text;
        private int id;

        public string Subject { get => subject; set => subject = value; }
        public string From { get => from; set => from = value; }
        public string Date { get => date; set => date = value; }
        public string Size { get => size; set => size = value; }
        public string To { get => to; set => to = value; }
        public string Text { get => text; set => text = value; }
        public int Id { get => id; set => id = value; }

        public LetterClass(string subject, string from, string to, string size, string date, string text, int id)
        {
            Subject = subject;
            From = from;
            To = to;
            Size = size;
            Date = date;
            Text = text;
            Id = id;
        }

        public static string GetSubjectFromText(string text)
        {
            string subject = "";
            int index1 = text.IndexOf("Subject: ");
            if (index1 == -1)
                return subject;
            subject = text.Substring(index1 + 9);
            index1 = subject.IndexOf('\n');
            subject = subject.Substring(0, index1);
            subject = DecodeMimeCode(subject);
            
            return subject;
        }

        public static string GetDateFromText(string text)
        {
            string date = "";
            int index1 = text.IndexOf("Date: ");
            if (index1 == -1)
                return date;
            date = text.Substring(index1 + 6);
            index1 = date.IndexOf('\n');
            date = date.Substring(0, index1);
            return date;
        }

        public static string GetSenderFromText(string text)
        {
            string sender = "";
            int index1 = text.IndexOf("From: ");
            if (index1 == -1)
                return sender;
            sender = text.Substring(index1 + 6);
            index1 = sender.IndexOf('\n');
            sender = sender.Substring(0, index1);
            index1 = sender.IndexOf('<');
            if(index1!=-1)
            {
                string temp = sender.Substring(index1 - 1);
                sender = sender.Substring(0, index1 - 1);
                sender = DecodeMimeCode(sender);
                sender += temp;
            }
            else
            {
                sender = DecodeMimeCode(sender);
            }
           
            return sender;
        }

        public static string GetReceiverFromText(string text)
        {
            string receiver = "";
            int index1 = text.IndexOf("To: ");
            if (index1 == -1)
                return receiver;
            receiver = text.Substring(index1 + 4);
            index1 = receiver.IndexOf('\n');
            receiver = receiver.Substring(0, index1-1);
            return receiver;
        }

        private static string DecodeMimeCode(string s)
        {
            return String.Join("", Regex.Matches(s ?? "", @"(?:=\?)([^\?]+)(?:\?B\?)([^\?]*)(?:\?=)").Cast<Match>().Select(m =>
            {
                string charset = m.Groups[1].Value;
                string data = m.Groups[2].Value;
                byte[] b = Convert.FromBase64String(data);
                return Encoding.GetEncoding(charset).GetString(b);
            }));
        }
    }
}
