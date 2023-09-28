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
using Microsoft.Scripting.Hosting.Shell;
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
            string currentDir = Directory.GetCurrentDirectory();
            string commandFolder = @currentDir + "/PowerSupplyCommands.txt";
            string[] commandFiles = File.ReadAllLines(commandFolder);
            foreach (string file in commandFiles)
            {
                contextMenuStrip.Items.Add(file);
               
            }

            contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(itemClick);
            contextMenuStrip.Show(button1, new Point(0, button1.Height));

        }

       

        private void itemClick(object sender, ToolStripItemClickedEventArgs e) 
        {

            string fileName = e.ClickedItem.Text;
            
            ContextMenuStrip commandList = new ContextMenuStrip();
            string currentDir = Directory.GetCurrentDirectory();
            string commandFile =  currentDir + "/" + fileName + ".txt";
            try
            {
                File.Exists(commandFile);
                string[] fileLines = File.ReadAllLines(commandFile);
                string[,] commands = new string[fileLines.Length, 2];

                for (int line = 0; line < (fileLines.Length); line++)
                {
                    string[] work = fileLines[line].Split(';');
                    Console.WriteLine(string.Format(work[0], work[1]));
                    commands[line, 0] = work[0];
                    commands[line, 1] = work[1];
                    commandList.Items.Add(work[1]);
                    
                }
            commandList.ItemClicked += commandChosen;
            commandList.Show(button1, new Point(0, button1.Height));
            }
            catch { }


        }

        private void commandChosen(object sender, EventArgs e)
        {
            Console.WriteLine("DKASJDKLAJDKJDSAKLDJ");
        }

        
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void gdgsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
