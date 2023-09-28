using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Attempt2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        
        private void newCommmandButton(object sender, EventArgs e)
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            string workingDir = Directory.GetCurrentDirectory() + "/";
            string commandFile = @workingDir + "PowerSupplyCommands.txt";
            try{
                File.Exists(commandFile);
                string[] fileLines = File.ReadAllLines(commandFile);
                
                string[,] commands = new string [fileLines.Length,2];
                Console.WriteLine(fileLines.Length);
                
                for (int line = 0; line < (fileLines.Length); line++)
                {
                    string[] work = fileLines[line].Split(';');
                    commands[line,0] = work[0];
                    commands[line,1] = work[1];
                    contextMenuStrip.Items.Add(work[1]);
                }
                
                contextMenuStrip.Show(button1, new Point(0, button1.Height));

            }
            catch { }


            }
        }
}
