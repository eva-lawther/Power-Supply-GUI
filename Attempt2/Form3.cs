using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attempt2
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void drawGraph()
        {
            List<JsonDataFormat> commandList = new List<JsonDataFormat>()
            {
                new JsonDataFormat("drawGraph.py",0, new double[] {} ),
            };

            ExportJSON(".data.json", commandList, true);
            string[] arguments = { "PythonCaller.py" };
            string output = RunPowerShellScript("RunPython.ps1", arguments);
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



        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //does file exist??
                TextBox textBox = Controls.Find("textBox1",true).FirstOrDefault() as TextBox;
                if (File.Exists(textBox.Text))
                {
                    string placeHolder = Directory.GetCurrentDirectory() + "/fileNameToGraph.txt";
                    File.WriteAllText(placeHolder, textBox.Text);
                    drawGraph();
                }
                else { throw new Exception(); }
                // need to plot it back to screen!!
            }
            catch { Console.WriteLine("File doesnt exist"); }


        }

    


    }
}
