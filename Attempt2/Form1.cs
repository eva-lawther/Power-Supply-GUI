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

        private string powerSupply(int command, int channel, int value)
        {
            List<JsonDataFormat> var = new List<JsonDataFormat>()
            {
                new JsonDataFormat("1", 3, new double[] {command, channel, value} ), //amps , ch1 , value
                
            };
            ExportJSON(".data.json", var);

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
                File.Copy(scriptsDir + fileName, workingDir + fileName, true);
            }
        }


        public bool ExportJSON(string jsonFile, List<JsonDataFormat> jsonDataOut)
        {
            try
            {
                // Lock the file
                FileStream fs = new FileStream(jsonFile, FileMode.Open,
                                                FileAccess.ReadWrite, FileShare.Read);
                // Generating a list of JsonDataFormat types
                List<JsonDataFormat> existingData = new List<JsonDataFormat>();
                // Try extracting the existing data into the list.
                try
                {
                    using (StreamReader r = new StreamReader(fs))
                    {
                        string json = r.ReadToEnd();
                        existingData = JsonSerializer.Deserialize<List<JsonDataFormat>>(json);
                    }
                }
                catch
                {
                    // Empty file!;
                }
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
                string jsonString = JsonSerializer.Serialize(existingData, new JsonSerializerOptions() { WriteIndented = true });
                using (StreamWriter outputFile = new StreamWriter(jsonFile))
                {
                    outputFile.WriteLine(jsonString);
                }
                // Unlock the file.
                fs.Close();
                fs.Dispose();
            }
            catch
            {
                // Print("File open in another application!");
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

                if (check.Checked) { output = powerSupply(1, channel, 0); }
                else { output = powerSupply(2, channel, 0); }

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
                    Console.WriteLine("TYAYAYSYSYSYYS");
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
                
                setValue(channel, element, command, parent);
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
                string output = powerSupply(newCommand, channel, value);
            }
            else { MessageBox.Show("Voltage needs to be set to a number"); }
        }








        private void startButton(object sender, EventArgs e)
        {
            Control source = (Control)sender;
            try
            {
                string[] tag = ((string)source.Tag).Split(';');
                int channel = int.Parse(tag[0]);

                // get change value
                string variable = variableChoice(channel);

                // get up down
                string upDown = directionChoice(channel);

                // get start
                if (int.TryParse(textBox9.Text, out _))
                {
                    int start = int.Parse(textBox9.Text); //which channel  cause this is messy
                }
                else { MessageBox.Show("Start needs to be set to a number"); }

                // get end
                if (int.TryParse(textBox10.Text, out _))
                {
                    int end = int.Parse(textBox10.Text); //which channel  cause this is messy
                }
                else { MessageBox.Show("End needs to be set to a number"); }

                // get increment
                if (int.TryParse(textBox11.Text, out _))
                {
                    int increment = int.Parse(textBox11.Text); //which channel  cause this is messy
                }
                else { MessageBox.Show("Increment needs to be set to a number"); }

                // get constant
                if (int.TryParse(textBox12.Text, out _))
                {
                    int constant = int.Parse(textBox12.Text); //which channel  cause this is messy
                }
                else { MessageBox.Show("Constant needs to be set to a number"); }

                // send to code
            }
            catch { }
        }



        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }






        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void chooseVariableButton(object sender, EventArgs e)
        {
        }

        private string variableChoice(int channel) // whys it taking more than one???
        {
            string choice = "";
            if (checkedListBox1.CheckedItems.Count != 0) //needs to take channel
            {
                choice = checkedListBox1.CheckedItems[0].ToString().Split(' ').Last();
                
            }
            return choice;
        }

        private void increaseDecreaseButton(object sender, EventArgs e)
        {

        }

        private string directionChoice(int channel) // whys it taking more than one???
        {
            string choice = "";
            if (checkedListBox2.CheckedItems.Count != 0) //needs to take channel
            {
                choice = checkedListBox2.CheckedItems[0].ToString();
            }
            return choice;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void button10_Click(object sender, EventArgs e)
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


