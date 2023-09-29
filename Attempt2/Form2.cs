using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.PowerShell.Commands;
using Microsoft.Scripting.Hosting.Shell;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using Button = System.Windows.Forms.Button;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;

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
        // Make command button that lists categories

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
                commandList.Items.Add(fileLines[i].Split(';')[1]);
            }
            string placeHolder = currentDir + "/fileName.txt";
            File.WriteAllText(placeHolder, fileName);
            commandList.Show(button1, new Point(0, button1.Height));
            commandList.ItemClicked += commandChosen;

        } 
        // Makes a sub menu based on category chosen

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
        // reads text file into array

        private void commandChosen(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStrip source = (ToolStrip)sender;
            source.Hide();
            string command = e.ClickedItem.Text;
            if (command == "Back")
            {
                newCommmandButton(null, null);
            }
            else
            {
                string location = Directory.GetCurrentDirectory() + "/fileName.txt" ;
                string fileName = File.ReadAllLines(location)[0];
                string[] fileLines = commandDoc(fileName);
                string commandString = "";
                int i = -1;
                while (i< fileLines.Length && commandString == "")
                {
                    i++;
                    if (fileLines[i].Contains(command))
                    {
                        commandString = fileLines[i];
                    }
                    
                }
                string work = commandString.Split(';')[0];
                string[] commandArray = fillInCommands(work);
                
                //string final = fillInCommands(work);
                //writeToFile("commandList.txt", final);
            }
        }
        // gets name of chosen command withn category and writes machine instruction to file



        private string[] fillInCommands(string command)
        {
            string[] commandString = command.Split('<','>');
            (string[,] inputArray, string[] key) = fillInInputArray();
            int tag = 0;
            string[] commandArray = new string[commandString.Length];
            Button button = new Button();
            button.Text = "okay";
            foreach (string element in commandString)
            {
                string chosen = element;
                Console.WriteLine(chosen);
                if (element != " ")
                {
                    

                    bool found = false;
                    int i = -1;
                    while (!found && i < key.Length - 1)
                    {
                        i++;
                        if (element == key[i])
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        chosen =  makePopUp(inputArray[i, 1], inputArray[i, 2], tag);
                        //chosen = null;//button.Text;
                    }
                }
                commandArray[tag] = chosen;
                tag++;


            }
            //string commandFinal = string.Join("", commandArray);
            //Console.WriteLine(commandFinal);

            return commandArray;
        } 
        // breaks command into what needs user value and makes calls pop up 

        private string makePopUp(string values, string request,int tag) 
        {
            Font font = new Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            Label label = new Label { AutoSize = true, Font = font, Tag = tag  };
            label.Text = request;
            label.Location = new Point(30, 34);
            Button button = new Button { Tag = tag };
            button.Text = "Okay";

            if (values.Substring(0,1) == "[") {checkListPopUp(label, tag, values, button);}
            else if (values == "0-49") {stringEnterPopUp(label,tag, button);}
            else { decimalEnterPopUp() ; }
            
            return "ugh"; // needs to be chosen value from popup
        }

        private void decimalEnterPopUp(Label label, int tag, Button button)
        {
            TextBox textBox = new TextBox { Tag = tag + "1" };
            textBox.Location = new Point(30, 50);
            button.Location = new Point(30, 80);
            button.Click += new System.EventHandler(setValue);

            Form form = new Form { Tag = tag };
            form.Text = "Request!";
            form.Controls.Add(button);
            form.Controls.Add(label);
            form.Controls.Add(textBox);
            //label.Visible = true;
            form.Show();
        }

        private void setValue(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                Control parent = button.Parent;
                string tag = button.Tag.ToString();
                TextBox textbox = (TextBox)findTextFromTag(parent, tag + "1");
                string choice = textbox.Text;
                Form form = (Form)parent;
                form.Hide();
                //if (!int.TryParse(choice, out _)) { throw new Exception(); } //needs to be string not int


            }
            catch { MessageBox.Show("please write a number between 1 and 49"); }
        }

        // makes pop up form



        private void stringEnterPopUp(Label label, int tag, Button button)
        {
            TextBox textBox = new TextBox { Tag = tag + "1" };
            textBox.Location = new Point(30, 50);
            button.Location = new Point(30, 80);
            button.Click += new System.EventHandler(setNumber);

            Form form = new Form { Tag = tag };
            form.Text = "Request!";
            form.Controls.Add(button);
            form.Controls.Add(label);
            form.Controls.Add(textBox);
            //label.Visible = true;
            form.Show();

        }

        private void setNumber(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                Control parent = button.Parent;
                string tag = button.Tag.ToString();
                TextBox textbox = (TextBox)findTextFromTag(parent, tag + "1");
                string choice = textbox.Text;
                Form form = (Form)parent;
                form.Hide();
                if (!int.TryParse(choice, out _)) { throw new Exception(); }


            }
            catch { MessageBox.Show("please write a number between 1 and 49"); }
            
            //button.Text = choice;
            //return choice;
        }

        
        private void checkListPopUp(Label label, int tag, string values, Button button)
        {
            
            values = values.Substring(1, values.Length - 2);
            string[] toReplace = values.Split(',');
            CheckedListBox options = new CheckedListBox { FormattingEnabled = true, Tag = tag + "1" };
            options.SelectedIndexChanged += new System.EventHandler(onlyOneBox);

            button.Location = new Point(30, 150);
            button.Click += new System.EventHandler(okayPressed);
            foreach (string value in toReplace)
            {
                options.Items.Add(value);
            }
            options.Location = new Point(30, 80);
            //using (Form form = new Form())



            Form form = new Form { Tag = tag };
            form.Text = "Request!";
            form.Controls.Add(button);
            form.Controls.Add(label);
            form.Controls.Add(options);
            //label.Visible = true;
            form.Show();
        }

        private Control findTextFromTag(Control parentControl, string tagString)
        {
            foreach (Control text in parentControl.Controls)
            {
                string controlTag = text.Tag.ToString();
                if (controlTag != null && controlTag == tagString)// Contains(tagString))
                {
                    return text;
                }
            }
            return null;
        }

        private void onlyOneBox(object sender, EventArgs e)
        {
            CheckedListBox source = (CheckedListBox)sender;
            int index = source.SelectedIndex;
            int count = source.Items.Count;
            
            for (int i = 0; i < count; i++)
            {
                if (index != i)
                {
                    source.SetItemChecked(i, false);
                }
            }
            //string choice = source.CheckedItems.ToString();
            //Console.WriteLine(choice);
        }
        // Makes sure only one box is chosen in checkListBox

        private void okayPressed(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Control parent = button.Parent;
            string tag = button.Tag.ToString();
            string choice = getChoice(tag, parent);
            //button.Text = choice;
            //return choice;
        }

        private string getChoice(string tag, Control parent) //gets choice and closes form box
        {
            CheckedListBox checkedList = (CheckedListBox)findTextFromTag(parent, tag + "1");
            string choice = checkedList.CheckedItems[0].ToString();
            Console.WriteLine($" Choice: {choice}");
            Console.WriteLine(tag);
            Form form = (Form)parent;
            form.Hide();
            return choice;
        }
           

        private (string[,],string[]) fillInInputArray()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string file = Directory.GetCurrentDirectory() + "/PowerSupplyInputs.txt";
            string[] list = File.ReadAllLines(file);
            string[,] array = new string[list.Length, 3];
            string[] key = new string[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                string[] working = list[i].Split(';');
                array[i, 0] = working[0];
                key[i] = working[0];
                array[i, 1] = working[1];
                array[i, 2] = working[2];
            }
            return (array, key);
        } //fills array of inputs given command input file

        private void writeToFile(string fileName, string command)
        {
            try
            {
                string myfile = Directory.GetCurrentDirectory() + "/"+ fileName;

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(myfile))
                {
                    sw.WriteLine(command);

                }
            }
            catch { }
        }



        // Irrelevant
        
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


    }
}
