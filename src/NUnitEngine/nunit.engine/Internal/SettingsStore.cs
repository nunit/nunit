// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// SettingsStore extends SettingsGroup to provide for
    /// loading and saving the settings from an XML file.
    /// </summary>
    public class SettingsStore : SettingsGroup
    {
        private readonly string _settingsFile;
        private readonly bool _writeable;

        #region Constructors

        /// <summary>
        /// Construct a SettingsStore without a backing file - used for testing.
        /// </summary>
        public SettingsStore() { }

        /// <summary>
        /// Construct a SettingsStore with a file name and indicate whether it is writeable
        /// </summary>
        /// <param name="settingsFile"></param>
        /// <param name="writeable"></param>
        public SettingsStore(string settingsFile, bool writeable)
        {
            _settingsFile = Path.GetFullPath(settingsFile);
            _writeable = writeable;
        }

        #endregion

        #region Public Methods

        public void LoadSettings()
        {
            var info = new FileInfo(_settingsFile);
            if (!info.Exists || info.Length == 0)
                return;

            try
            {
                var doc = new XmlDocument();
                doc.Load(_settingsFile);

                foreach (XmlElement element in doc.DocumentElement["Settings"].ChildNodes)
                {
                    if (element.Name != "Setting")
                        throw new ApplicationException("Unknown element in settings file: " + element.Name);

                    if (!element.HasAttribute("name"))
                        throw new ApplicationException("Setting must have 'name' attribute");

                    if (!element.HasAttribute("value"))
                        throw new ApplicationException("Setting must have 'value' attribute");

                    SaveSetting(element.GetAttribute("name"), element.GetAttribute("value"));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error loading settings file", ex);
            }
        }

        public void SaveSettings()
        {
            if (_writeable)
            {
                var dirPath = Path.GetDirectoryName(_settingsFile);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var writer = new XmlTextWriter(_settingsFile, System.Text.Encoding.UTF8) {
                    Formatting = Formatting.Indented
                };

                writer.WriteProcessingInstruction("xml", "version=\"1.0\"");
                writer.WriteStartElement("NUnitSettings");
                writer.WriteStartElement("Settings");

                var keys = new List<string>(_settings.Keys);
                keys.Sort();

                foreach (var name in keys)
                {
                    object val = GetSetting(name);
                    if (val != null)
                    {
                        writer.WriteStartElement("Setting");
                        writer.WriteAttributeString("name", name);
                        writer.WriteAttributeString("value", TypeDescriptor.GetConverter(val).ConvertToInvariantString(val));
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();
            }
        }

        #endregion
    }
}
