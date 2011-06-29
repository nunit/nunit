// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// SettingsGroup is the base class representing a group
    /// of user or system settings. All storge of settings
    /// is delegated to a SettingsStorage.
    /// </summary>
    public class SettingsGroup : ISettings, IDisposable
    {
#if CLR_2_0 || CLR_4_0
        private Dictionary<string, object> storage = new Dictionary<string, object>();
#else
        private Hashtable storage = new Hashtable();
#endif

        public event SettingsEventHandler Changed;

        #region ISettings Members

        /// <summary>
        /// Load the value of one of the group's settings
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <returns>Value of the setting or null</returns>
        public object GetSetting(string settingName)
        {
            return storage.ContainsKey(settingName)
                ? storage[settingName]
                : null;
        }

        /// <summary>
        /// Load the value of one of the group's settings or return a default value
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultValue">Value to return if the seeting is not present</param>
        /// <returns>Value of the setting or the default</returns>
        public object GetSetting(string settingName, object defaultValue)
        {
            object result = GetSetting(settingName);

            if (result == null)
                result = defaultValue;

            return result;
        }

        /// <summary>
        /// Load the value of one of the group's integer settings
        /// in a type-safe manner or return a default value
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultValue">Value to return if the seeting is not present</param>
        /// <returns>Value of the setting or the default</returns>
        public int GetSetting(string settingName, int defaultValue)
        {
            object result = GetSetting(settingName);

            if (result == null)
                return defaultValue;

            if (result is int)
                return (int)result;

            try
            {
                return Int32.Parse(result.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Load the value of one of the group's float settings
        /// in a type-safe manner or return a default value
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultValue">Value to return if the setting is not present</param>
        /// <returns>Value of the setting or the default</returns>
        public float GetSetting(string settingName, float defaultValue)
        {
            object result = GetSetting(settingName);

            if (result == null)
                return defaultValue;

            if (result is float)
                return (float)result;

            try
            {
                return float.Parse(result.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Load the value of one of the group's boolean settings
        /// in a type-safe manner.
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultValue">Value of the setting or the default</param>
        /// <returns>Value of the setting</returns>
        public bool GetSetting(string settingName, bool defaultValue)
        {
            object result = GetSetting(settingName);

            if (result == null)
                return defaultValue;

            // Handle legacy formats
            //			if ( result is int )
            //				return (int)result == 1;
            //
            //			if ( result is string )
            //			{
            //				if ( (string)result == "1" ) return true;
            //				if ( (string)result == "0" ) return false;
            //			}

            if (result is bool)
                return (bool)result;

            try
            {
                return Boolean.Parse(result.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Load the value of one of the group's string settings
        /// in a type-safe manner or return a default value
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultValue">Value to return if the setting is not present</param>
        /// <returns>Value of the setting or the default</returns>
        public string GetSetting(string settingName, string defaultValue)
        {
            object result = GetSetting(settingName);

            if (result == null)
                return defaultValue;

            if (result is string)
                return (string)result;
            else
                return result.ToString();
        }

        /// <summary>
        /// Load the value of one of the group's enum settings
        /// in a type-safe manner or return a default value
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultValue">Value to return if the setting is not present</param>
        /// <returns>Value of the setting or the default</returns>
        public System.Enum GetSetting(string settingName, System.Enum defaultValue)
        {
            object result = GetSetting(settingName);

            if (result == null)
                return defaultValue;

            if (result is System.Enum)
                return (System.Enum)result;

            try
            {
                return (System.Enum)System.Enum.Parse(defaultValue.GetType(), result.ToString(), true);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Load the value of one of the group's Font settings
        /// in a type-safe manner or return a default value
        /// </summary>
        /// <param name="settingName">Name of setting to load</param>
        /// <param name="defaultFont">Value to return if the setting is not present</param>
        /// <returns>Value of the setting or the default</returns>
        public Font GetSetting(string settingName, Font defaultFont)
        {
            object result = GetSetting(settingName);

            if (result == null)
                return defaultFont;

            if (result is Font)
                return (Font)result;

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                return (Font)converter.ConvertFrom(null, CultureInfo.InvariantCulture, result.ToString());
            }
            catch
            {
                return defaultFont;
            }
        }

        /// <summary>
        /// Remove a setting from the group
        /// </summary>
        /// <param name="settingName">Name of the setting to remove</param>
        public void RemoveSetting(string settingName)
        {
            storage.Remove(settingName);

            if (Changed != null)
                Changed(this, new SettingsEventArgs(settingName));
        }

        /// <summary>
        /// Remove a group of settings
        /// </summary>
        /// <param name="GroupName"></param>
        public void RemoveGroup(string groupName)
        {
#if CLR_2_0 || CLR_4_0
            List<string> keysToRemove = new List<string>();
#else
            ArrayList keysToRemove = new ArrayList();
#endif

            string prefix = groupName;
            if (!prefix.EndsWith("."))
                prefix = prefix + ".";

            foreach (string key in storage.Keys)
                if (key.StartsWith(prefix))
                    keysToRemove.Add(key);

            foreach (string key in keysToRemove)
                storage.Remove(key);
        }

        /// <summary>
        /// Save the value of one of the group's settings
        /// </summary>
        /// <param name="settingName">Name of the setting to save</param>
        /// <param name="settingValue">Value to be saved</param>
        public void SaveSetting(string settingName, object settingValue)
        {
            object oldValue = GetSetting(settingName);

            // Avoid signaling "changes" when there is not really a change
            if (oldValue != null)
            {
                if (oldValue is string && settingValue is string && (string)oldValue == (string)settingValue ||
                    oldValue is int && settingValue is int && (int)oldValue == (int)settingValue ||
                    oldValue is bool && settingValue is bool && (bool)oldValue == (bool)settingValue ||
                    oldValue is Enum && settingValue is Enum && oldValue.Equals(settingValue))
                    return;
            }

            storage[settingName] = settingValue;

            if (Changed != null)
                Changed(this, new SettingsEventArgs(settingName));
        }

        #endregion

        #region Other Public Methods

        public void LoadSettings(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (!info.Exists || info.Length == 0)
                return;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

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

        public void SaveSettings(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            XmlTextWriter writer = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            writer.WriteProcessingInstruction("xml", "version=\"1.0\"");
            writer.WriteStartElement("NUnitSettings");
            writer.WriteStartElement("Settings");

#if CLR_2_0 || CLR_4_0
            List<string> keys = new List<string>();
#else
            ArrayList keys = new ArrayList(storage.Keys);
#endif
            keys.Sort();

            foreach (string name in keys)
            {
                object val = GetSetting(name);
                if (val != null)
                {
                    writer.WriteStartElement("Setting");
                    writer.WriteAttributeString("name", name);
                    writer.WriteAttributeString("value", val.ToString());
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose of this group by disposing of it's storage implementation
        /// </summary>
        public void Dispose()
        {
            if (storage != null)
            {
                //storage.Dispose();
                storage = null;
            }
        }
        #endregion
    }
}
