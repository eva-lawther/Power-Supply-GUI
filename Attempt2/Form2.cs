using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.PowerShell.Commands;
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
            //string commandFile =  currentDir + "/" + fileName + ".txt";
            string [] fileLines = commandDoc(fileName);
            string[,] commands = new string[fileLines.Length, 2];
            commandList.Items.Add("Back");
            for (int i = 0; i < fileLines.GetLength(0); i++)
            {
                //string[] work = fileLines[i].Split(';');
                //commands[i, 0] = work[0];
                //commands[i, 1] = work[1];
                //commandList.Items.Add(work[1]);
                commandList.Items.Add(fileLines[i].Split(';')[1]);
            }
            string placeHolder = currentDir + "/fileName.txt";
            File.WriteAllText(placeHolder, fileName);

            commandList.ItemClicked += commandChosen;
            commandList.Show(button1, new Point(0, button1.Height));
        }

        private string[] commandDoc(string fileName)
        {
            string currentDir = Directory.GetCurrentDirectory();
            string commandFile = currentDir + "/" + fileName + ".txt";
            try
            {
                File.Exists(commandFile);
                string[] fileLines = File.ReadAllLines(commandFile);
               // string[,] commands = new string[fileLines.Length, 2];
                
                return fileLines;
            }
            catch
            {
                return null;
            }
            
        }

        private void commandChosen(object sender, ToolStripItemClickedEventArgs e)
        {
            string command = e.ClickedItem.Text;
            if (command == "Back")
            {
                Console.WriteLine("ahaha");//newCommmandButton;
            }
            else
            {
                string location = Directory.GetCurrentDirectory() + "/fileName.txt" ;
                string fileName = File.ReadAllLines(location)[0];
                string[] fileLines = commandDoc(fileName);
                string commandString = "";
                int i = 0;
                while (i< fileLines.Length && commandString == "")
                {
                    if (fileLines[i].Contains(commandString))
                    {
                        commandString = fileLines[i];
                    }
                    i++;
                }
                string work = commandString.Split(';')[0];
                Console.WriteLine(work); // Needs to go into a text file with correct values


            }
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
