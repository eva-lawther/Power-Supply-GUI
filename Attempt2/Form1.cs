using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using Attempt2.Properties; // HEREEEEEEEEE YAYYY
using System.Threading;
using System.Text.Json;
using System.IO.Pipes;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;



using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Net.WebRequestMethods;



using System.Diagnostics; // dont need
using RohdeSchwarz.RsInstrument; // dont need
using IronPython.Hosting; //dont need
using System.Security.Authentication.ExtendedProtection;
using RohdeSchwarz.RsInstrument.Conversions;
using TextBox = System.Windows.Forms.TextBox;
using static IronPython.Modules._ast;
using System.Runtime.Remoting.Channels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Globalization;

namespace Attempt2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ImportAllScripts("../../Scripts/");
            InitialiseJsons(new List<string>() { ".data.json", ".dataReady.json" });
            PopulatePowerShellScript("RunPython.ps1", ".py", "python", null);
            initialise_BackWorkers();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
        private void configuration(int command, int channel, double value)
        {
            List<JsonDataFormat> config = new List<JsonDataFormat>()
            {   
                new JsonDataFormat("configuration.py", 3, new double[] {command, channel, value} ), 
            };

            ExportJSON(".data.json", config, true);

            string[] arguments = { "PythonCaller.py" };
            string output = RunPowerShellScript("RunPython.ps1", arguments);
            ImportJSON(".dataReady.json");
            JsonDataFormat obj = ReturnObjFromJsonBuffer("1");
            string fileLocation = obj.Id;
            MessageBox.Show(fileLocation);

        }  // Call python class
        

        private void initialise_BackWorkers()
        {
            /*
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            */
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted +=  backgroundWorker1_RunWorkerCompleted;
        }

        string dummy_g = "none";
        bool first_flag_g = true;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
          
            if (first_flag_g) {
                string[] arguments = { "PythonCaller.py" };
                dummy_g = RunPowerShellScript("RunPython.ps1", arguments);
                first_flag_g = false;
            }

            //MessageBox.Show("Hey, I'm done woohoo: " + output);
        
            //dummy_g = "not none anymore";
            return;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                MessageBox.Show("Import JSON Problem. " + dummy_g);
            }
            
            
       
        }
        /*
        private void configuration(int command, int channel, double value)
        {
            List<JsonDataFormat> config = new List<JsonDataFormat>()
            {
                new JsonDataFormat("configuration.py", 3, new double[] {command, channel, value} ),
            };

            ExportJSON(".data.json", config, true);
            //backgroundWorker1_RunWorkerCompleted(null, null);
            backgroundWorker1.RunWorkerAsync();
            
        }  // Call python class
        */
        /*
        private string sweeping(int channel, string type, string way, double start, double end, double increment, double constant)
        {
            int variable, direction;
            if (type == "Voltage") { variable = 0; } else { variable = 1; }
            if (way == "up") { direction = 0; } else { direction = 1; }


            List<JsonDataFormat> sweep = new List<JsonDataFormat>()
            {
                new JsonDataFormat("sweeping.py", 7, new double[] { channel, variable, direction, start, end, increment, constant } ),
            };
            ExportJSON(".data.json", sweep, true);

            string[] arguments = { "PythonCaller.py" };
            string output = RunPowerShellScript("RunPython.ps1", arguments);
            return output;
        }  // Call python class
        */
        private void sweeping(int channel, string way, double start, double end, double increment, double constant)
        {
            int variable = 0, direction;
            //if (type == "Voltage") { variable = 0; } else { variable = 1; }
            if (way == "up") { direction = 0; } else { direction = 1; }


            List<JsonDataFormat> sweep = new List<JsonDataFormat>()
            {
                new JsonDataFormat("sweeping.py", 7, new double[] { channel, variable, direction, start, end, increment, constant } ),
            };

            ExportJSON(".data.json", sweep, true);
            //backgroundWorker1_DoWork(null, null);
            backgroundWorker1.RunWorkerAsync();
            /*int j=0;
            while (true)
            {
                j++;
            }*/
        }  // Call python class

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
                    return element;
                    //if (element.Id == id) return element;
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

        private void OnOffSwitch(object sender, EventArgs e) 
        {
            Control source = (Control)sender;
            CheckBox check = (CheckBox)sender;
            try
            {
                //string output;
                string[] tag = ((string)source.Tag).Split(';');
                int channel = int.Parse(tag[0]);

                if (check.Checked) { configuration(1, channel, 0); }
                else { configuration(2, channel, 0); }

            }
            catch
            {Console.WriteLine("Problem with button!");}

        }
        //turn channel on or off 

        private Control findObjectFromTag(Control parentControl, string tagString)
        {
            try
            {
                foreach (Control text in parentControl.Controls)
                {
                    Console.WriteLine(text.Text);
                    string controlTag = text.Tag.ToString();
                    if (controlTag != null && controlTag == tagString)
                    {
                        return text;
                    }
                }
                return null;
            }
            catch
            {
                Console.WriteLine("ERROR in findObjectFromTag");
                return null;
            }
        }
        // return an object when given its parent and tag

        private void button(object sender, EventArgs e)
        {
            Control source = (Control)sender;
            try
            {
                string[] tag = ((string)source.Tag).Split(';');
                int channel = int.Parse(tag[0]);
                int element = int.Parse(tag[1]);
                string command = tag[2];

                Control parent = source.Parent;

                if (element == 3) { startButton(channel, element, command, parent); }

                else { setValue(channel, element, command, parent); }
            }
            catch { Console.WriteLine("Problem with button!"); }
        }
        // when button pressed call relevant function

        private void setValue(int channel, int element, string command, Control parent)
        {
            try {
                string tag = channel + ";" + element;
                TextBox textBox = (TextBox)findObjectFromTag(parent, tag);

                int newCommand = 0;

                if (command == "setVoltage")
                {
                    newCommand = 3;
                }
                if ((command == "setAmplitude"))
                {
                    newCommand = 4;
                }

                if (double.TryParse(textBox.Text, out _))
                {
                    double value = double.Parse(textBox.Text);
                    configuration(newCommand, channel, value);
                }
                else { MessageBox.Show("Voltage needs to be set to a number"); }
            }
            catch
            {
                Console.WriteLine("ERROR in setValue");
            }
            
        }
        // setVoltage or amplitude given user input //DOUBLE!!!

        private void startButton(int channel, int element, string command, Control parent)
        {   
            if (command == "stop")
            {
                for (int i = 1; i<=4; i++)
                {
                    
                    for (int j = 3; j <= 4; j++)
                    {
                        configuration(j, i, 0);
                    }
                    configuration(2, i, 0);
                }
      
            }
            try
            {
                string elementTag = channel + ";"  + element;
                TextBox textBox;

                // get change value
                //string variable = variableChoice(parent, elementTag); 

                // get up down
                string upDown = directionChoice(parent,elementTag);

                double start = 0, end = 0, increment = 0, constant = 0;


                // get start
                textBox = (TextBox)findObjectFromTag(parent, elementTag + ";" + 1);
                try {
                    double.TryParse(textBox.Text, out _);
                    start = double.Parse(textBox.Text); 
                }
                catch { MessageBox.Show("Start needs to be set to a number"); }

                // get end
                textBox = (TextBox)findObjectFromTag(parent, elementTag + ";" + 2);
                try {
                    double.TryParse(textBox.Text, out _);
                    end = double.Parse(textBox.Text); 
                }
                catch { MessageBox.Show("End needs to be set to a number"); }

                // get increment
                textBox = (TextBox)findObjectFromTag(parent, elementTag + ";" + 3);
                try {
                    double.TryParse(textBox.Text, out _);
                    increment = double.Parse(textBox.Text); 
                }
                catch { MessageBox.Show("Increment needs to be set to a number"); }

                // get constant
                textBox = (TextBox)findObjectFromTag(parent, elementTag + ";" + 4);
                try{
                    double.TryParse(textBox.Text, out _);
                    constant = double.Parse(textBox.Text); 
                }
                catch { MessageBox.Show("Constant needs to be set to a number"); }

                // send to code
                sweeping(channel, upDown, start, end, increment, constant);
            }
            catch { Console.WriteLine("Error with start button"); }
        }
        // run sweeping file given inputs

        private string variableChoice(Control parent, string tag) 
        {
            try
            {
                string choice = "";
                CheckedListBox checkedListBox = (CheckedListBox)findObjectFromTag(parent, tag + ";" + "chooseChange");
                choice = checkedListBox.CheckedItems[0].ToString().Split(' ').Last();
                return choice;
            }
            catch 
            { 
                Console.WriteLine("ERROR in variableChoice");
                return null;
            }
            
        }
        //return whether variable is voltage or amplitude

        private string directionChoice(Control parent, string tag) 
        {
            try
            {
                string choice = "";
                CheckedListBox checkedListBox = (CheckedListBox)findObjectFromTag(parent, tag + ";" + "chooseDirection");
                choice = checkedListBox.CheckedItems[0].ToString();
                return choice;
            }
            catch
            {
                Console.WriteLine("ERROR in directionChoice");
                return null;
            }
            
        }
        //return whether sweeping direction is up or down

        private void checkedListBox(object sender, EventArgs e)
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
            catch { Console.WriteLine("ERROR in checkedListBox"); }
            
        }
        // make sure there is only one box ticked in checked list box

        private void createTestListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 diy = new Form2();
                diy.Show();
            }
            catch { Console.WriteLine("ERROR in createTestListToolStripMenuItem_Click "); }
            
        }
        // call form2 when clicked

        private void drawGraphFromDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form3 diy = new Form3();
                diy.Show();
            }
            catch { Console.WriteLine("ERROR in drawGraphFromDataToolStripMenuItem_Click "); }
        }


        // IRRELEVANT

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


