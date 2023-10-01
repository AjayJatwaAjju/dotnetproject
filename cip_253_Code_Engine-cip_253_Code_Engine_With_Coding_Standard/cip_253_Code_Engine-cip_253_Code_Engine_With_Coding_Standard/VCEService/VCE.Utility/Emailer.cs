using System.Net.Mail;

namespace VCE.Utility
{
    public class Emailer
    {
        /// <summary>
        /// Replacement param for all the variables
        /// </summary>
        /// <param name="replacementParam"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string ArrayReplaceMents(string[,] replacementParam, string message)
        {
            string content = message;
            if (replacementParam != null)
            {
                for (int currentRow = 0; currentRow <= replacementParam.GetUpperBound(0); currentRow++)
                {
                    content = ReplaceParameter(content, replacementParam[currentRow, 0].ToString(), replacementParam[currentRow, 1].ToString());
                }
            }
            return content;
        }

        /// <summary>
        /// Replacing content with key value.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="Key"></param>
        /// <param name="ReplaceValue"></param>
        /// <returns></returns>
        public string ReplaceParameter(string content, string Key, string replaceValue)
        {
            return content.Replace(Key, replaceValue);
        }

        /// <summary>
        ///  This is for sending email
        /// </summary>
        /// <param name="replaceMents"></param>
        /// <param name="emailTemplateKey"></param>
        /// <param name="toEmail"></param>
        /// <param name="filepath"></param>
        /// <param name="fromUserId"></param>
        /// <param name="ToUserId"></param>
        /// <param name="fkUserMessageTypeId"></param>
        /// <returns></returns>
        public bool SendEmail(string[,] replaceMents, string emailTemplateKey, string toEmail, string filepath)
        {
            bool status = false;
            string[] toUserList = toEmail.Split(',');
            MailMessage msg;
            try
            {
                var oEmail = new EmailInfo();//GetEmailBodyByEmailTemplateKey(emailTemplateKey, replaceMents);
                var from = new MailAddress(oEmail.FromEmail, oEmail.FromName);
                if (toEmail != "" && toUserList.Length == 1)
                {
                    var to = new MailAddress(toEmail);
                    msg = new MailMessage(from, to);
                }
                else
                {
                    var to = new MailAddress(toUserList[0]);
                    msg = new MailMessage(from, to);
                }
                foreach (var t in toUserList)
                {
                    if (t.Contains("@"))
                    {
                        msg.To.Add(t);
                    }
                }
                if (oEmail.Cc != string.Empty)
                {
                    //admin email id
                    msg.CC.Add(oEmail.Cc);
                }
                if (oEmail.Bcc != string.Empty)
                {
                    //admin email id
                    msg.Bcc.Add(oEmail.Bcc);
                }
                if (filepath != "")
                {
                    var atcFile = new Attachment(filepath);
                    msg.Attachments.Add(atcFile);
                }

                string messageContent = "";// EZE.Shared.Utility.GetEmailWrapper();
                messageContent = messageContent.Replace("@Content", oEmail.Message);
                msg.Body = messageContent;
                msg.Subject = oEmail.Subject;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                var client = new SmtpClient("{Config.Host}", 1);//1:{Config.port}
                client.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
                client.UseDefaultCredentials = false; // [3] Changed this
                //client.UseDefaultCredentials = false;
                // client.EnableSsl = true;
                var theCredential = new System.Net.NetworkCredential("{Config.UserName}", "{Config.Password}");
                client.Credentials = theCredential;
                // client.EnableSsl = true;
                client.Send(msg);//Sending the mail..*/
                status = true;
            }
            catch
            {
                status = false;
            }
            return status;
        }
    }

    public class EmailInfo
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}