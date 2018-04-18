﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Generator
{
    public class GeneratedTextTransformation
    {
        private Generator _generator;

        public void Init()
        {
            _generator = new Generator(this);

            WriteLine("// ------------------------------------------------------------------------------------------------");
            WriteLine("// This code was generated by EntityFramework Reverse POCO Generator (http://www.reversepoco.com/).");
            WriteLine("// Created by Simon Hughes (https://about.me/simon.hughes).");
            WriteLine("//");
            WriteLine("// Do not make changes directly to this file - edit the template instead.");
            WriteLine("//");

            if (Settings.IncludeConnectionSettingComments)
            {
                WriteLine("// The following connection settings were used to generate this file:");
                if (!string.IsNullOrEmpty(Settings.ConnectionStringName) && !string.IsNullOrEmpty(Settings.ConfigFilePath))
                {
                    WriteLine("//     Configuration file:     \"{0}\"", Settings.ConfigFilePath);
                    WriteLine("//     Connection String Name: \"{0}\"", Settings.ConnectionStringName);
                }
                WriteLine("//     Connection String:      \"{0}\"", ZapPassword());
                WriteLine("// ------------------------------------------------------------------------------------------------");
            }

            if (string.IsNullOrEmpty(Settings.ProviderName))
            {
                Warning("Failed to find providerName in the connection string");
                WriteLine(string.Empty);
                WriteLine("// ------------------------------------------------------------------------------------------------");
                WriteLine("//  Failed to find providerName in the connection string");
                WriteLine("// ------------------------------------------------------------------------------------------------");
                WriteLine(string.Empty);
                return;
            }

            try
            {
                _generator.Init();
            }
            catch (Exception x)
            {
                var error = FormatError(x);
                Warning(string.Format("Failed to load provider \"{0}\" - {1}", Settings.ProviderName, error));
                WriteLine(string.Empty);
                WriteLine("// ------------------------------------------------------------------------------------------------");
                WriteLine("// Failed to load provider \"{0}\" - {1}", Settings.ProviderName, error);
                WriteLine("// ------------------------------------------------------------------------------------------------");
                WriteLine(string.Empty);
            }
        }

        public void ReadSchema()
        {
            try
            {
                Settings.Tables = _generator.LoadTables();
                Settings.StoredProcs = _generator.LoadStoredProcs();
            }
            catch (Exception x)
            {
                var error = FormatError(x);
                Warning(string.Format("Failed to read the schema information. Error: {0}", error));
                WriteLine(string.Empty);
            }
        }

        private static string FormatError(Exception ex)
        {
            return ex.Message.Replace("\r\n", "\n").Replace("\n", " ");
        }

        private string ZapPassword()
        {
            var rx = new Regex("password=[^\";]*", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return rx.Replace(Settings.ConnectionString, "password=**zapped**;");
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public void WriteLine(string message)
        {
            LogToOutput(message);
        }

        public void Warning(string message)
        {
            LogToOutput(string.Format(CultureInfo.CurrentCulture, "Warning: {0}", message));
        }

        public void Error(string message)
        {
            LogToOutput(string.Format(CultureInfo.CurrentCulture, "Error: {0}", message));
        }

        private void LogToOutput(string message)
        {
            Trace.WriteLine(message);
        }
    }
}