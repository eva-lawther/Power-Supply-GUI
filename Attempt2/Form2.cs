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
        //private GroupBox groupBox;
        //private Control checkedListParent = Form2;

        public Form2()
        {
            InitializeComponent();
        }

        private void runCommandList()
        {
            List<JsonDataFormat> commandList = new List<JsonDataFormat>()
            {
                new JsonDataFormat("runCommandList.py",0, new double[] {} ), 
            };

            ExportJSON(".data.json", commandList, true);
            string[] arguments = { "PythonCaller.py" };
            string output = RunPowerShellScript("RunPython.ps1", arguments);
        }
        

        

        public string PopulatePowerShellScript(string psScriptName, string scriptExt,
                                           string scriptCommand, string[] exclusions)
        {
            // Begin by finding the Python (.py) scripts in the working directory.
            string[] paths = Directory.GetFiles(Directory.GetCurrentDirectory());
            // Generate a list of scripts to run. 
            List<string> scriptsToRun = new List<string>();
            foreach (string path in paths)
            {
                string file = Path.GetFileName(path);
                if (exclusions != null && exclusions.Contains(file))
                {
                    continue;
                }
                else if (Path.GetExtension(file) == scriptExt)
                {
                    scriptsToRun.Add(file);
                }
                else
                {
                    continue;
                }
            }
            // Generate a list of strings to insert into the PS script.
            List<string> commandsToRun = new List<string>();
            foreach (string script in scriptsToRun)
            {
                string command = "\n\t'" + script + "' {" + scriptCommand + " " + script + "}";
                commandsToRun.Add(command);
            }
            // Open the PS script to be populated.
            string psScriptText;
            try
            {
                // Lock the file
                FileStream fs = new FileStream(psScriptName, FileMode.Open,
                                                FileAccess.ReadWrite, FileShare.Read);
                // Try read-in the existing script.
                try
                {
                    using (StreamReader r = new StreamReader(fs))
                    {
                        psScriptText = r.ReadToEnd();
                        string insertPointRed = "Switch";
                        string insertPointAll = "Switch ($selectscript)\n{   ";
                        int index = psScriptText.IndexOf(insertPointRed) + insertPointAll.Length;
                        foreach (string command in commandsToRun)
                        {
                            int offset = command.Length;
                            psScriptText = psScriptText.Insert(index, command);
                            index += offset;
                        }
                    }
                }
                catch
                {
                    return "File empty or couldn't find insertion point!";
                }
                try
                {
                    using (StreamWriter w = new StreamWriter(psScriptName))
                    {
                        w.Write(psScriptText);
                    }
                }
                catch
                {
                    return "File couldn't be written to";
                }
                fs.Close();
            }
            catch
            {
                return "Couldn't open file!";
            }
            return null;
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


        public void ImportAllScripts(string scriptsDir)
        {
            // Determine the number of slashes.
            if (!scriptsDir.EndsWith("/")) scriptsDir += "/";
            uint numberOfSlashes = (uint)(scriptsDir.Split('/')).Length;
            // Find the scripts directory and copy each file one-by-one
            // to the working directory.
            string workingDir = Directory.GetCurrentDirectory() + "/";
            string[] allScripts = Directory.GetFileSystemEntries(scriptsDir);
            foreach (string script in allScripts)
            {
                // Extract just the file name.
                string fileName = script.Split('/')[numberOfSlashes - 1];
                // Copy it over.

                File.Copy(scriptsDir + fileName, workingDir + fileName, true);
            }
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
            //makeAndSaveCommand();
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            for (int k = 0; k< COMMAND_GLOBAL.Length; k++)
            {
                COMMAND_GLOBAL[k] = null;
            }
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
            //makeAndSaveCommand();
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
                length = fillInCommands(work);

                
                //string final = fillInCommands(work);
                //writeToFile("commandList.txt", final);
            }


        }
        // gets name of chosen command withn category and writes machine instruction to file
        private string makeAndSaveCommand()
        {
            string final = string.Join("", COMMAND_GLOBAL); 
            //Console.WriteLine(final);
            //MessageBox.Show(final);
            return final;

        }



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
               // Console.WriteLine(chosen);
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
                        chosen = null;//button.Text;
                    }
                }
                COMMAND_GLOBAL[tag] = chosen;
                tag++;


            }
            //string commandFinal = string.Join("", commandArray);
            //Console.WriteLine(commandFinal);
            return commandString.Length;
        }

        private void addCommand(object sender, EventArgs e)
        {
            Button source = (Button)sender;
            Control parent = source.Parent;
            Form form = (Form)parent;
           // CheckedListBox checkedListBox = (CheckedListBox)findTextFromTag(Form2, "MAIN");
            string final = makeAndSaveCommand();
            CheckedListBox checkedListBox = Controls.Find("checkedListBox1", true).FirstOrDefault() as CheckedListBox;
            checkedListBox.Items.Add(final, CheckState.Checked);
            form.Hide();

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
            else { decimalEnterPopUp(label,tag,button) ; }
            
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
                COMMAND_GLOBAL[Int32.Parse(tag)] = choice;
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
                COMMAND_GLOBAL[Int32.Parse(tag)] = choice;

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
        /*
        private CheckedListBox findCheckedListBox(string tagString)
        {
            foreach (Control list in Form2)
            {

            }
        }
        */
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
           // Console.WriteLine($" Choice: {choice}");
           // Console.WriteLine(tag);
            Form form = (Form)parent;
            form.Hide();
            COMMAND_GLOBAL[Int32.Parse(tag)]=choice;
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

        
        /*
        public static void Main(string[] args)
        {
            Command command = new Command();
        }*/
       
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

        private void resetButtonClick(object sender, EventArgs e)
        {
            // clear list of checked list box
        }

        private void listOfCommands(object sender, EventArgs e)
        {

        }

        private void runButtonClick(object sender, EventArgs e)
        {
            CheckedListBox checkedListBox = Controls.Find("checkedListBox1", true).FirstOrDefault() as CheckedListBox;

            // read through all checked values in checked list box
            //for all checks in checkedListBox
            if (checkedListBox.CheckedItems.Count != 0)
            {
                // If so, loop through all checked items and print results.  
                //string[] s = new string[checkedListBox1.CheckedItems.Count];
                for (int x = 0; x < checkedListBox.CheckedItems.Count; x++)
                {
                    //s[0] = checkedListBox1.CheckedItems[x].ToString();
                    writeToFile("runningFile.txt", checkedListBox.CheckedItems[x].ToString());
                }
                //write to file
                runCommandList();
                // call python file and send link to command file
            }

            
        }   
    }
}
