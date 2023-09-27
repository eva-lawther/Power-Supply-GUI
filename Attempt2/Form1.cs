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
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void configuration(int command, int channel, int value)
        {
            List<JsonDataFormat> config = new List<JsonDataFormat>()
            {   
                new JsonDataFormat("configuration.py", 3, new double[] {command, channel, value} ), //amps , ch1 , value  
            };

            ExportJSON(".data.json", config, true);

            string[] arguments = { "PythonCaller.py" };
            string output = RunPowerShellScript("RunPython.ps1", arguments);
        }  // Call python class

        private string sweeping(int channel, string type, string way, int start, int end, int increment, int constant)
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
                Console.WriteLine(fileName);
                Console.WriteLine("00000000000000000000000");
                Console.WriteLine(scriptsDir + fileName);
                Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaa");
                Console.WriteLine(workingDir + fileName);
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

        private void OnOffSwitch(object sender, EventArgs e) //coded
        {
            Control source = (Control)sender;
            CheckBox check = (CheckBox)sender;
            try
            {
                string output;
                string[] tag = ((string)source.Tag).Split(';');
                int channel = int.Parse(tag[0]);

                if (check.Checked) { configuration(1, channel, 0); }
                else { configuration(2, channel, 0); }

            }
            catch
            {Console.WriteLine("Problem with button!");}

        } // OnOff Switch

        private Control findTextFromTag(Control parentControl, string tagString)
        {
            foreach (Control text in parentControl.Controls)
            {
                string controlTag = (string)text.Tag;
                Console.WriteLine(controlTag);
                if (controlTag != null && controlTag == tagString)// Contains(tagString))
                {
                    return text;
                }
            }
            return null;
        }

        private void button(object sender, EventArgs e)
        {
            Control source = (Control)sender;
            try
            {
                string[] tag = ((string)source.Tag).Split(';');
                int channel = int.Parse(tag[0]);
                int element = int.Parse(tag[1]);
                string command = tag[2];
                //Control parent = Controls.Find(source.Parent, true).FirstOrDefault() as TextBox;

                Control parent = source.Parent;

                if (element == 3) { startButton(channel, element, command, parent); }

                else { setValue(channel, element, command, parent); }
            }
            catch { Console.WriteLine("Problem with button!"); }
        }

        private void setValue(int channel, int element, string command, Control parent)
        {
            string tag = channel + ";" + element;
            TextBox textBox = (TextBox)findTextFromTag(parent, tag);

            int newCommand = 0;

            if (command == "setVoltage")
            {
                newCommand = 3;
            }
            if ((command =="setAmplitude"))
            {
                newCommand = 4;
            }

            if (int.TryParse(textBox.Text, out _))
            {
                int value = int.Parse(textBox.Text);
                configuration(newCommand, channel, value);
            }
            else { MessageBox.Show("Voltage needs to be set to a number"); }
        }

        private void startButton(int channel, int element, string command, Control parent)
        {   
            if (command == "stop")
            {
                // use supply.write.end()
            }
            try
            {
                string elementTag = channel + ";"  + element;
                TextBox textBox;

                // get change value
                string variable = variableChoice(parent, elementTag); 

                // get up down
                string upDown = directionChoice(parent,elementTag);

                int start = 0, end = 0, increment = 0, constant = 0;


                // get start
                textBox = (TextBox)findTextFromTag(parent, elementTag + ";" + 1);
                try {
                    int.TryParse(textBox.Text, out _);
                    start = int.Parse(textBox.Text); //which channel  cause this is messy
                }
                catch { MessageBox.Show("Start needs to be set to a number"); }

                // get end
                textBox = (TextBox)findTextFromTag(parent, elementTag + ";" + 2);
                try { 
                    int.TryParse(textBox.Text, out _);
                    end = int.Parse(textBox.Text); //which channel  cause this is messy
                }
                catch { MessageBox.Show("End needs to be set to a number"); }

                // get increment
                textBox = (TextBox)findTextFromTag(parent, elementTag + ";" + 3);
                try {
                    int.TryParse(textBox.Text, out _);
                    increment = int.Parse(textBox.Text); //which channel  cause this is messy
                }
                catch { MessageBox.Show("Increment needs to be set to a number"); }

                // get constant
                textBox = (TextBox)findTextFromTag(parent, elementTag + ";" + 4);
                try{
                    int.TryParse(textBox.Text, out _);
                    constant = int.Parse(textBox.Text); //which channel  cause this is messy
                }
                catch { MessageBox.Show("Constant needs to be set to a number"); }

                // send to code
                sweeping(channel, variable, upDown, start, end, increment, constant);
            }
            catch { Console.WriteLine("Error with start button"); }
        }

        private string variableChoice(Control parent, string tag) // whys it taking more than one???
        {
            string choice = "";
            CheckedListBox checkedListBox = (CheckedListBox)findTextFromTag(parent, tag + ";" + "chooseChange");  
            if (checkedListBox.CheckedItems.Count != 0) //needs to take channel
            {
                choice = checkedListBox.CheckedItems[0].ToString().Split(' ').Last();
                
            }
            return choice;
        }

       
        private string directionChoice(Control parent, string tag) // whys it taking more than one???
        {
            string choice = "";
            CheckedListBox checkedListBox = (CheckedListBox)findTextFromTag(parent, tag + ";" + "chooseDirection");
            if (checkedListBox.CheckedItems.Count != 0) //needs to take channel
            {
                choice = checkedListBox.CheckedItems[0].ToString();
            }
            return choice;
        }

        private void increaseDecreaseButton(object sender, EventArgs e)
        {

        }
        private void chooseVariableButton(object sender, EventArgs e)
        {
            CheckedListBox source = (CheckedListBox)sender;
            int index = source.SelectedIndex;
            int count = source.Items.Count;
            for(int i = 0; i < count; i++)
            {
                if(index != i)
                {
                    source.SetItemChecked(i, false);
                }
            }
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

        private void checkedListBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        /*static void IronPython(string action, string channel, string input)
        {
            var engine = Python.CreateEngine();
            var script = @"C:\Users\EvaLawther\WP3\Power Supply GUI\Attempt2\PythonCaller.py";
            //var script = @"C:\Users\EvaLawther\WP3\Power Supply GUI\module1.py";

            //var searchPaths = engine.GetSearchPaths();
           // searchPaths.Add(@"C:\myProject\packages\DynamicLanguageRuntime.1.1.2");
           // searchPaths.Add(@"C:\myProject\packages\IronPython.2.7.7\lib");
           // searchPaths.Add(@"C:\myProject");
           // searchPaths.Add(@"C:\myProject\"where myfile.py exists");
    
            //engine.SetSearchPaths(searchPaths);

            var source = engine.CreateScriptSourceFromFile(script);

            var argv = new List<string>();
            argv.Add("");
            argv.Add(action); // command
            argv.Add(channel); //channel
            argv.Add(input); //value

            engine.GetSysModule().SetVariable("argv", argv);
            

            // output redirect
            var eIO = engine.Runtime.IO;
            var errors = new MemoryStream();
            eIO.SetErrorOutput(errors, Encoding.Default);

            var results = new MemoryStream();
            eIO.SetOutput(results, Encoding.Default);

            // execute sript
            var scope = engine.CreateScope();
            source.Execute(scope);

            // display output
            string str(byte[] x) => Encoding.Default.GetString(x);

            Console.WriteLine("Errors:");
            Console.WriteLine(str(errors.ToArray()));
            Console.WriteLine();
            Console.WriteLine("Results");
            Console.WriteLine(str(results.ToArray()));


        }*/

    }
}


