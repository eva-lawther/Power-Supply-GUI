using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.Json;
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
        
        private string[] COMMAND_GLOBAL = new string[8];
        private string instrumentInitials = "";
        //private GroupBox groupBox;
        //private Control checkedListParent = Form2;

        public Form2()
        {
            InitializeComponent();
            initialise_BackWorkers();
        }

        private void runCommandList()
        {
            List<JsonDataFormat> commandList = new List<JsonDataFormat>()
            {
                new JsonDataFormat("runCommandList.py",0, new double[] {} ), 
            };

            ExportJSON(".data.json", commandList, true);
            backgroundWorker2.RunWorkerAsync();
            //string[] arguments = { "PythonCaller.py" };
            //string output = RunPowerShellScript("RunPython.ps1", arguments);
        }

        private void initialise_BackWorkers()
        {
            /*
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            */
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
        }

        string dummy_g = "none";
        bool first_flag_g = true;
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {

            if (first_flag_g)
            {
                string[] arguments = { "PythonCaller.py" };
                dummy_g = RunPowerShellScript("RunPython.ps1", arguments);
                first_flag_g = false;
            }

            //MessageBox.Show("Hey, I'm done woohoo: " + output);

            //dummy_g = "not none anymore";
            return;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show(dummy_g);
            first_flag_g = true;
            ImportJSON(".dataReady.json");
            try
            {
                JsonDataFormat obj = programInScriptOut_jsonBuffer[0];
                string fileLocation = obj.Id;
                MessageBox.Show(fileLocation);
            }
            catch
            {
                MessageBox.Show(dummy_g);
            }



        }


        public string RunPowerShellScript(string psScript, string[] arguments)
        {
            // Concatenate arguments into a single string.
            //MessageBox.Show("in power shell");
            string argumentCat = " ";
            if (arguments != null)
            {
                foreach (string arg in arguments)
                {
                    argumentCat += arg + " ";
                }
            }
            // create Powershell runspace and open it.
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            // create a pipeline and feed it the script   
            Pipeline pipeline = runspace.CreatePipeline();
            /* This implementation may be problematic as the version of PowerShell used must
             * have 'Set-ExecutionPolicy RemoteSigned'. Note: two verison may exist version x86 and x64.
             * The command above must be ran for the relevant version.
             */
            pipeline.Commands.AddScript(@".\" + psScript + argumentCat);
            Collection<PSObject> results = pipeline.Invoke();
            // close the runspace  
            runspace.Close();
            // convert the script result into a single string  
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }
            // Return the resultant output string.
            return stringBuilder.ToString();
        }


        public bool ExportJSON(string jsonFile, List<JsonDataFormat> jsonDataOut, bool overwrite)
        {
            try
            {
                // Lock the file
                FileStream fs = new FileStream(jsonFile, FileMode.Open,
                        FileAccess.ReadWrite, FileShare.Read);
                string jsonString;
                // Generating a list of JsonDataFormat types
                List<JsonDataFormat> existingData = new List<JsonDataFormat>();
                // Try extracting the existing data into the list.
                try
                {
                    using (StreamReader r = new StreamReader(fs))
                    {
                        string json = r.ReadToEnd();
                        if (!overwrite) existingData = JsonSerializer.Deserialize<List<JsonDataFormat>>(json);
                    }
                }
                catch
                {
                    // Empty file!;
                }
                if (overwrite)
                {
                    // Write out the objects to the JSON file.
                    jsonString = JsonSerializer.Serialize(jsonDataOut, new JsonSerializerOptions() { WriteIndented = true });
                }
                else
                {
                    // Check through existing data and either overwrite or append new elements.
                    foreach (JsonDataFormat objectOut in jsonDataOut)
                    {
                        bool match = false;
                        foreach (JsonDataFormat existingObject in existingData)
                        {
                            if (objectOut.Id == existingObject.Id)
                            {
                                existingObject.Id = objectOut.Id;
                                existingObject.Length = objectOut.Length;
                                existingObject.Values = objectOut.Values;
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                        {
                            existingData.Add(objectOut);
                        }
                    }
                    // Write out the objects to the JSON file.
                    jsonString = JsonSerializer.Serialize(existingData, new JsonSerializerOptions() { WriteIndented = true });
                }

                using (StreamWriter outputFile = new StreamWriter(jsonFile))
                {
                    outputFile.WriteLine(jsonString);
                    Console.WriteLine(jsonString.ToString());
                }
                // Unlock the file.
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }

        public bool ImportJSON(string jsonFile)
        {
            try
            {
                // Lock the file
                FileStream fs = new FileStream(jsonFile, FileMode.Open,
                                                FileAccess.Read, FileShare.ReadWrite);
                // Try extracting the existing data into the list.
                try
                {
                    using (StreamReader r = new StreamReader(fs))
                    {
                        string json = r.ReadToEnd();
                        programInScriptOut_jsonBuffer =
                            JsonSerializer.Deserialize<List<JsonDataFormat>>(json);
                    }
                }
                catch
                {
                    // Empty file!;
                }
                fs.Close();
                fs.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public JsonDataFormat ReturnObjFromJsonBuffer(string id)
        {
            if (programInScriptOut_jsonBuffer != null)
            {
                foreach (JsonDataFormat element in programInScriptOut_jsonBuffer)
                {
                    if (element.Id == id) return element;
                }
            }
            return null;
        }

        public void InitialiseJsons(List<string> initFiles)
        {
            // Generate any files that do not alrady exist.
            foreach (string file in initFiles)
            {
                if (!File.Exists(file))
                {
                    FileStream fs = File.Create(file);
                    fs.Close();
                }
            }
        }

        public class JsonDataFormat
        {
            public JsonDataFormat(string id, int length, double[] values)
            {
                Id = id;
                Length = length;
                Values = values;
            }
            public string Id { get; set; }
            public int Length { get; set; }
            public double[] Values { get; set; }
        }

        public List<JsonDataFormat> programInScriptOut_jsonBuffer; // Python Interface



        private void newCommmandButton(object sender, EventArgs e)
        {
            try
            {
                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                for (int k = 0; k < COMMAND_GLOBAL.Length; k++)
                {
                    COMMAND_GLOBAL[k] = null;
                }
                string currentDir = Directory.GetCurrentDirectory();
                string commandFolder = @currentDir + "/Instruments.txt";//"/PowerSupplyCommands.txt";
                string[] commandFiles = File.ReadAllLines(commandFolder);
                foreach (string file in commandFiles)
                {
                    contextMenuStrip.Items.Add(file);
                }
                contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(listCategories);// itemClick);
                contextMenuStrip.Show(button1, new Point(0, button1.Height));
            }
            catch { Console.WriteLine("Error with new command button: ERROR in newCommandButton"); }
            
        }  
        // Make command button that lists instruments//power supply categories

        private void listCategories(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                string instrument = e.ClickedItem.Text;

                ContextMenuStrip commandList = new ContextMenuStrip();
                string currentDir = Directory.GetCurrentDirectory();
                string commandFolder = "";
                if (instrument == "PowerSupplyCommands")
                {
                    instrumentInitials = "PS";
                    commandFolder = @currentDir + "/PowerSupplyCommands.txt";
                    
                }
                else
                {
                    commandFolder = @currentDir + "/MultiMeterCommands.txt";
                    instrumentInitials = "MM";
                }
                

                string[] commandFiles = File.ReadAllLines(commandFolder);
                foreach (string file in commandFiles)
                {
                    commandList.Items.Add(file);
                }
                commandList.ItemClicked += new ToolStripItemClickedEventHandler(itemClick);// itemClick);
                commandList.Show(button1, new Point(0, button1.Height));
                
            }
            catch { Console.WriteLine("Cannot find category file: ERROR in itemClick"); }
        }

        private void itemClick(object sender, ToolStripItemClickedEventArgs e) 
        {
            try
            {
                string fileName = e.ClickedItem.Text;

                ContextMenuStrip commandList = new ContextMenuStrip();
                //string commandFile =  currentDir + "/" + fileName + ".txt";
                string[] fileLines = textFileToArray(fileName);
                string[,] commands = new string[fileLines.Length, 2];
                commandList.Items.Add("Back");
                for (int i = 0; i < fileLines.GetLength(0); i++)
                {
                    commandList.Items.Add(fileLines[i].Split(';')[1]);
                }
                string placeHolder = Directory.GetCurrentDirectory() + "/fileName.txt";
                File.WriteAllText(placeHolder, fileName);
                commandList.Show(button1, new Point(0, button1.Height));
                commandList.ItemClicked += commandChosen;
            }
            catch { Console.WriteLine("Cannot find category file: ERROR in itemClick"); }
            

        }
        // Makes a sub menu based on power supply category chosen


        private string[] textFileToArray(string fileName)
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
                Console.WriteLine("cannot find command file: ERROR in textFileToArray()");
                return null;
            }

        }
        // Reads text file into array

        private void commandChosen(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                int length = 0;
                ToolStrip source = (ToolStrip)sender;
                source.Hide();
                string command = e.ClickedItem.Text;
                if (command == "Back")
                {
                    newCommmandButton(null, null);
                }
                else
                {
                    string location = Directory.GetCurrentDirectory() + "/fileName.txt";
                    string fileName = File.ReadAllLines(location)[0];
                    string[] fileLines = textFileToArray(fileName);
                    string commandString = "";
                    int i = -1;
                    while (i < fileLines.Length && commandString == "")
                    {
                        i++;
                        if (fileLines[i].Contains(command))
                        {
                            commandString = fileLines[i];
                        }

                    }
                    string work = commandString.Split(';')[0];
                    length = fillInCommands(work);
                }
            }
            catch { Console.WriteLine("ERROR in commandChosen"); }
            
        }
        // given chosen command, gets machine code and calls for user input
       
        private int fillInCommands(string command)
        {
            string[] commandString = command.Split('<','>');
            (string[,] inputArray, string[] key) = fillInInputArray();
            int tag = 0;
            
            Form check = new Form();
            check.Text = "complete";
            Button button = new Button { Text = "set" };
            check.Controls.Add(button);
            button.Click += new System.EventHandler(addCommand);
            check.Show();

            foreach (string element in commandString)
            {
                string chosen = element;
                if (element.ToLower() == "auto range" )
                {
                    chosen = "";
                }
                else if (element != " ")
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
                        makePopUp(inputArray[i, 1], inputArray[i, 2], tag);
                        chosen = null;
                    }
                }
                COMMAND_GLOBAL[tag] = chosen;
                tag++;


            }
            return commandString.Length;
        }
        // calls makePopUp for all variables that require user input in command

        private void addCommand(object sender, EventArgs e)
        {
            try
            {
                Button source = (Button)sender;
                Control parent = source.Parent;
                Form form = (Form)parent;
                string final = string.Join("", COMMAND_GLOBAL);
                CheckedListBox checkedListBox = Controls.Find("checkedListBox1", true).FirstOrDefault() as CheckedListBox;
                checkedListBox.Items.Add(instrumentInitials + ": " + final, CheckState.Checked);
                form.Hide();
            }
            catch { Console.WriteLine("ERROR in addCommand"); }
            
        }
        // adds completed command to checkedListBox

        private void makePopUp(string values, string request,int tag) 
        {
            try
            {
                Font font = new Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
                Label label = new Label { AutoSize = true, Font = font, Tag = tag };
                label.Text = request;
                label.Location = new Point(30, 34);
                Button button = new Button { Tag = tag };
                button.Text = "Okay";

                if (values.Substring(0, 1) == "[") { checkListPopUp(label, tag, values, button); }
                else if (values == "0-49") { integerEnterPopUp(label, tag, button, "preset"); }
                else if (values == "int to 3dp") { decimalEnterPopUp(label, tag, button); }
                else if (values == "x") { integerEnterPopUp(label, tag, button, "wait"); }
            }
            catch { Console.WriteLine("ERROR in makePopUp"); }
        }
        //pass pop up base and call correct pop up type to get user inputs

        private void decimalEnterPopUp(Label label, int tag, Button button)
        {
            try
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
                form.Show();
            }
            catch { Console.WriteLine("ERROR in decimalEnterPopUp"); }
            
        }
        // make popup specifically expecting a double user input - creates setValue

        private void setValue(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                Control parent = button.Parent;
                string tag = button.Tag.ToString();
                TextBox textbox = (TextBox)findObjectFromTag(parent, tag + "1");
                string choice = textbox.Text;
                Form form = (Form)parent;
                
                if (!Double.TryParse(choice, out _)){throw new Exception();}
                form.Hide();
                COMMAND_GLOBAL[Int32.Parse(tag)] = choice;
            }
            catch { MessageBox.Show("please write a number up to 3dp"); }
        }
        // checks if user input is a double and adds to COMMAND_GLOBAL

        private void integerEnterPopUp(Label label, int tag, Button button, string type)
        {
            TextBox textBox = new TextBox { Tag = tag + "1" };
            textBox.Location = new Point(30, 50);
            button.Location = new Point(30, 80);
            
            Form form = new Form { Tag = tag };
            form.Text = "Request!";
            form.Controls.Add(button);
            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Show();

            if (type == "preset")
            {
                button.Click += new System.EventHandler(setNumber);
            }
            else if (type == "wait")
            {
                button.Click += new System.EventHandler(setWaitTime);
            }
        }
        //make popup specifically expecting integer user input - creates setWaitTime and setNumber

        private void setWaitTime(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                Control parent = button.Parent;
                string tag = button.Tag.ToString();
                TextBox textbox = (TextBox)findObjectFromTag(parent, tag + "1");
                string choice = textbox.Text;
                Form form = (Form)parent;
                
                if (!int.TryParse(choice, out _)) { throw new Exception(); }
                COMMAND_GLOBAL[Int32.Parse(tag)] = choice;
                form.Hide();

            }
            catch { MessageBox.Show("please enter an integer number of seconds"); }
        }
        // checks if user input is an integer and adds to COMMAND_GLOBAL

        private void setNumber(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                Control parent = button.Parent;
                string tag = button.Tag.ToString();
                TextBox textbox = (TextBox)findObjectFromTag(parent, tag + "1");
                string choice = textbox.Text;
                Form form = (Form)parent;
                
                if (int.TryParse(choice, out _))
                {
                    int num = Convert.ToInt32(choice);
                    if (num <= 0 || num>= 49)
                    {
                        throw new Exception();
                    }
                }
                else { throw new Exception(); }
                COMMAND_GLOBAL[Int32.Parse(tag)] = choice;
                form.Hide();

            }
            catch { MessageBox.Show("please write a number between 1 and 49"); }
        }
        // checks if user input is an integer between 0 and 49 and adds to COMMAND_GLOBAL

        private void checkListPopUp(Label label, int tag, string values, Button button)
        {
            try
            {
                values = values.Substring(1, values.Length - 2);
                string[] toReplace = values.Split(',');
                CheckedListBox options = new CheckedListBox { FormattingEnabled = true, Tag = tag + "1" };
                options.SelectedIndexChanged += new System.EventHandler(onlyOneBox);

                button.Location = new Point(30, 200);
                button.Click += new System.EventHandler(okayPressed);
                foreach (string value in toReplace)
                {
                    options.Items.Add(value);
                }
                options.Location = new Point(30, 80);


                Form form = new Form { Tag = tag };
                form.Text = "Request!";
                form.Controls.Add(button);
                form.Controls.Add(label);
                form.Controls.Add(options);
                form.Show();
            }
            catch { Console.WriteLine("ERROR in checkListPopUp"); }
            
        }
        // makes a pop up so the user can enter their input value (gives choices) 

        private Control findObjectFromTag(Control parentControl, string tagString)
        {
            try
            {
                foreach (Control text in parentControl.Controls)
                {
                    string controlTag = text.Tag.ToString();
                    if (controlTag != null && controlTag == tagString)
                    {
                        return text;
                    }
                }
                return null;
            }
            catch { 
                Console.WriteLine("ERROR in findObjectFromTag"); 
                return null;
            }
        }
        // return an object when given its parent and tag
        
        private void onlyOneBox(object sender, EventArgs e)
        {
            try
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
            }
            catch { Console.WriteLine("ERROR in onlyOneBox"); }
        }
        //makes sure only one box is checked in checked list box

        private void okayPressed(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                Control parent = button.Parent;
                string tag = button.Tag.ToString();
                string choice = getChoice(tag, parent);
            }
            catch { Console.WriteLine("ERROR in okayPressed"); }
            
        }
        // when button pressed call getChoice

        private string getChoice(string tag, Control parent) 
        {
            try
            {
                CheckedListBox checkedList = (CheckedListBox)findObjectFromTag(parent, tag + "1");
                string choice = checkedList.CheckedItems[0].ToString();
                Form form = (Form)parent;
                form.Hide();
                COMMAND_GLOBAL[Int32.Parse(tag)] = choice;
                return choice;
            }
            catch { 
                Console.WriteLine("ERROR in getChoice");
                return null;
            }
            
        }
        // gets user input and closes form box

        private (string[,],string[]) fillInInputArray()
        {
            try
            {
                string currentDir = Directory.GetCurrentDirectory();
                string file = Directory.GetCurrentDirectory() + "/InstrumentInputs.txt";
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
            }
            catch 
            { 
                Console.WriteLine("ERROR in fillInInputArray");
                return (null, null);
            }
            
        } 
        //fills array of inputs given command input file

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
            catch { Console.WriteLine("ERROR in writeToFile"); }
        }
        //writes given text to a given file name in the current directory

        private void resetButtonClick(object sender, EventArgs e)
        {
            try
            {
                CheckedListBox checkedListBox = Controls.Find("checkedListBox1", true).FirstOrDefault() as CheckedListBox;
                while (checkedListBox.CheckedItems.Count > 0)
                {
                    checkedListBox.Items.RemoveAt(checkedListBox.CheckedIndices[0]);
                }
            }
            catch { Console.WriteLine("ERROR in resetCLickButton"); }            
        }
        //removes all checked boxes in checkedListBox

        private void runButtonClick(object sender, EventArgs e)
        {
            try
            {
                CheckedListBox checkedListBox = Controls.Find("checkedListBox1", true).FirstOrDefault() as CheckedListBox;
                string myfile = Directory.GetCurrentDirectory() + "/runningFile.txt";
                File.WriteAllText(@myfile, "");

                if (checkedListBox.CheckedItems.Count != 0)
                {
                    for (int x = 0; x < checkedListBox.CheckedItems.Count; x++)
                    {
                        writeToFile("runningFile.txt", checkedListBox.CheckedItems[x].ToString());
                    }
                    runCommandList();
                }
            }
            catch { Console.WriteLine("ERROR in runButtonClick"); }
        }
        // writes all checked commands into a text file and calls the correct python file


        // IRRELEVANT
        private void label1_Click_1(object sender, EventArgs e)
        {

        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void listOfCommands(object sender, EventArgs e)
        {

        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }

      

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mainControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form1 diy = new Form1();
                diy.Show();
                this.Hide();
            }
            catch { Console.WriteLine("ERROR in mainControlsToolStripMenuItem_Click "); }
        }

        private void drawGraphFromDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form3 diy = new Form3();
                diy.Show();
                this.Hide();
            }
            catch { Console.WriteLine("ERROR in drawGraphFromDataToolStripMenuItem_Click "); }
        }
    }
}
