using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleIRCLib;

namespace CFSlaves
{
    public partial class Form1 : Form
    {
        
        #region Variables
        private readonly SimpleIRC irc = new SimpleIRC();
        private const int port = 6969;
        #endregion
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("aaaaaa");
        }

        private void ChatOutputCallback(object source, IrcReceivedEventArgs args)
        {
            Console.WriteLine($@"{args.Channel} | {args.User} : {args.Message}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(@"abc");
            irc.SendRawMessage(textBox1.Text);
        }
    }
}