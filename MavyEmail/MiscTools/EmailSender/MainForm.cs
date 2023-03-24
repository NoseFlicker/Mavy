using System;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace QiHe.MiscTools.EmailSender
{
    public partial class MainForm : Form
    {
        
        #region Variables
        private static bool result = true;
        #endregion
        
        public MainForm() => InitializeComponent();

        private void buttonSend_Click(object sender, EventArgs e)
        {
            var attachmentFile = fileBrower1.FilePath;
            if (attachmentFile != string.Empty && !File.Exists(attachmentFile))
            {
                MessageBox.Show(attachmentFile + " not found.");
                return;
            }

            buttonSend.Enabled = false;
            result = false;
            if (attachmentFile != string.Empty)
            {
                var mail = new MailMessage(
                    textBoxFrom.Text,
                    textBoxTo.Text) {Subject = textBoxSubject.Text, Body = textBoxBody.Text};
                mail.Attachments.Add(new Attachment(attachmentFile));
                new Thread(() => result = CodeLib.Net.EmailSender.Send(mail)).Start();
            }
            else
                new Thread(() =>
                {
                    result = CodeLib.Net.EmailSender.Send(
                        textBoxFrom.Text,
                        textBoxTo.Text,
                        textBoxSubject.Text,
                        textBoxBody.Text);
                    MessageBox.Show(result ? "The email was successfully sent." : "The email couldn't be sent.");
                }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e) => buttonSend.Enabled = result;
    }
}