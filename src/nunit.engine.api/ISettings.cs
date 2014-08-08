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

namespace NUnit.Engine
{
   /// <summary>
   /// Event handler for settings changes
   /// </summary>
   /// <param name="sender">The sender.</param>
   /// <param name="args">The <see cref="SettingsEventArgs"/> instance containing the event data.</param>
    public delegate void SettingsEventHandler( object sender, SettingsEventArgs args );

    /// <summary>
    /// Event argument for settings changes
    /// </summary>
    public class SettingsEventArgs : EventArgs
    {
       /// <summary>
       /// Initializes a new instance of the <see cref="SettingsEventArgs"/> class.
       /// </summary>
       /// <param name="settingName">Name of the setting that has changed.</param>
       public SettingsEventArgs( string settingName )
        {
            SettingName = settingName;
        }

       /// <summary>
       /// Gets the name of the setting that has changed
       /// </summary>
       public string SettingName { get; private set; }
    }

    /// <summary>
    /// The ISettings interface is used to access all user
    /// settings and options.
    /// </summary>
    public interface ISettings
    {
       /// <summary>
       /// Occurs when the settings are changed.
       /// </summary>
        event SettingsEventHandler Changed;

        /// <summary>
        /// Load a setting from the storage.
        /// </summary>
        /// <param name="settingName">Name of the setting to load</param>
        /// <returns>Value of the setting or null</returns>
        object GetSetting( string settingName );

        /// <summary>
        /// Load a setting from the storage or return a default value
        /// </summary>
        /// <param name="settingName">Name of the setting to load</param>
        /// <param name="defaultValue">Value to return if the setting is missing</param>
        /// <returns>Value of the setting or the default value</returns>
        T GetSetting<T>(string settingName, T defaultValue);

        /// <summary>
        /// Remove a setting from the storage
        /// </summary>
        /// <param name="settingName">Name of the setting to remove</param>
        void RemoveSetting( string settingName );

        /// <summary>
        /// Remove an entire group of settings from the storage
        /// </summary>
        /// <param name="groupName">Name of the group to remove</param>
        void RemoveGroup( string groupName );

        /// <summary>
        /// Save a setting in the storage
        /// </summary>
        /// <param name="settingName">Name of the setting to save</param>
        /// <param name="settingValue">Value to be saved</param>
        void SaveSetting( string settingName, object settingValue );
    }
}
