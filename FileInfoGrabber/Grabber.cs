using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FileInfoGrabber.Common;

namespace FileInfoGrabber
{
    public class Grabber
    {
        public static bool IsLogEnabled = false;

        string _input = AppDomain.CurrentDomain.BaseDirectory;
        string _output = AppDomain.CurrentDomain.BaseDirectory;
        List<string> _extensions = new List<string>();
        bool _isCustomExtension = false;

        public bool Grab(string[] args)
        {
            ParseArgs(args);
            if (!CheckArgs())
            {
                return false;
            }

            string text = "Directory:\t" + _input + "\n";
            text += "Extensions:\t";
            if (_isCustomExtension)
            {
                text += String.Join(" | ", _extensions) + "\n";
            }
            else
            {
                text += "all";
            }

            if (IsLogEnabled)
            {
                Console.WriteLine(text);
            }
            
            text += "\n\n";

            foreach (string filePath in Directory.GetFiles(_input))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (_isCustomExtension)
                    {
                        if (!_extensions.Exists(item => item == fileInfo.Extension))
                        {
                            continue;
                        }
                    }

                    string tempText = "";

                    tempText += fileInfo.Name + "\n";
                    tempText += "\tVersion:\t\t" + FileVersionInfo.GetVersionInfo(filePath).FileVersion + "\n";
                    tempText += "\tCreationTime:\t" + fileInfo.CreationTime.ToString("dd/MM/yyyy HH:mm:ss");

                    if (IsLogEnabled)
                    {
                        Console.WriteLine(tempText);
                    }

                    text += tempText + "\n\n";
                }
                catch (Exception ex)
                {
                    if (IsLogEnabled)
                    {
                        Console.WriteLine("Сouldn`t get file information! File ignored: " + filePath);
                    }
                    continue;
                }
            }

            if (IsLogEnabled)
            {
                Console.WriteLine();
            }

            try
            {
                File.WriteAllText(_output, text);
                if (IsLogEnabled)
                {
                    Console.WriteLine("Saved to file: " + _output);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (IsLogEnabled)
                {
                    Console.WriteLine("Error saving file: " + _output + "\n");
                    Console.WriteLine(ex.Message);
                }
                return false;
            }
        }

        private void ParseArgs(string[] args)
        {
            if (args?.Any() != true)
            {
                return;
            }

            bool isInput = false;
            bool isOutput = false;
            bool isExt = false;

            foreach (string arg in args)
            {
                if (arg == InputFlag)
                {
                    isInput = true;
                    isOutput = false;
                    isExt = false;

                    _input = "";
                    continue;
                }

                if (arg == OutputFlag)
                {
                    isInput = false;
                    isOutput = true;
                    isExt = false;

                    _output = "";
                    continue;
                }

                if (arg == ExtensionsFlag)
                {
                    isInput = false;
                    isOutput = false;
                    isExt = true;

                    _isCustomExtension = true;
                    _extensions.Clear();
                    continue;
                }

                if (isInput)
                {
                    _input += _input == "" ? arg : " " + arg;
                }

                if (isOutput)
                {
                    _output += _output == "" ? arg : " " + arg;
                }

                if (isExt)
                {
                    foreach (string ext in arg.Split(' '))
                    {
                        _extensions.Add(ext);
                    }
                }
            }
        }

        private bool CheckArgs()
        {
            if (!Directory.Exists(_input))
            {
                if (IsLogEnabled)
                {
                    Console.WriteLine("Cannot find input directory: " + _input);
                }
                return false;
            }

            try
            {
                FileAttributes attr = File.GetAttributes(_output);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    _output = Path.Combine(_output, DefaultFileName);
                }
            }
            catch
            {

            }

            if (_isCustomExtension && _extensions?.Any() != true)
            {
                if (IsLogEnabled)
                {
                    Console.WriteLine("Empty extensions list!" + _output);
                }
                return false;
            }

            return true;
        }
    }
}
