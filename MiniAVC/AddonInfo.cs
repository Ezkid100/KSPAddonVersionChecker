﻿// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace MiniAVC
{
    public class AddonInfo
    {
        #region Fields

        private static readonly System.Version actualKspVersion;
        private static readonly System.Version defaultMinVersion = new System.Version();
        private static readonly System.Version defaultMaxVersion = new System.Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
        private readonly string path;

        private System.Version kspVersion;
        private System.Version kspVersionMax;
        private System.Version kspVersionMin;

        #endregion

        #region Contructors

        static AddonInfo()
        {
            actualKspVersion = new System.Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
        }

        public AddonInfo(string path, string json)
        {
            try
            {
                this.path = path;
                this.Parse(json);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.ParseError = true;
            }

            if (this.ParseError)
            {
                Logger.Log("Version file contains errors: " + path);
            }
        }

        #endregion

        #region Properties

        public bool ParseError { get; private set; }

        public string Name { get; private set; }

        public string Url { get; private set; }

        public string Download { get; private set; }

        public System.Version Version { get; private set; }

        public System.Version KspVersion
        {
            get { return (this.kspVersion ?? actualKspVersion); }
        }

        public System.Version KspVersionMin
        {
            get { return (this.kspVersionMin ?? defaultMinVersion); }
        }

        public System.Version KspVersionMax
        {
            get { return (this.kspVersionMax ?? defaultMaxVersion); }
        }

        public bool IsCompatibleKspVersion
        {
            get { return Equals(this.KspVersion, actualKspVersion); }
        }

        public bool IsCompatibleKspVersionMin
        {
            get { return this.KspVersionMin <= actualKspVersion; }
        }

        public bool IsCompatibleKspVersionMax
        {
            get { return this.KspVersionMax >= actualKspVersion; }
        }

        public bool IsCompatible
        {
            get { return this.IsCompatibleKspVersion || ((this.kspVersionMin != null || this.kspVersionMax != null) && this.IsCompatibleKspVersionMin && this.IsCompatibleKspVersionMax); }
        }

        #endregion

        #region Parse Json

        private void Parse(string json)
        {
            var data = Json.Deserialize(json) as Dictionary<string, object>;
            if (data == null)
            {
                this.ParseError = true;
                return;
            }
            foreach (var key in data.Keys)
            {
                switch (key)
                {
                    case "NAME":
                        this.Name = (string)data["NAME"];
                        break;

                    case "URL":
                        this.Url = (string)data["URL"];
                        break;

                    case "DOWNLOAD":
                        this.Download = (string)data["DOWNLOAD"];
                        break;

                    case "VERSION":
                        this.Version = this.GetVersion(data["VERSION"]);
                        break;

                    case "KSP_VERSION":
                        this.kspVersion = this.GetVersion(data["KSP_VERSION"]);
                        break;

                    case "KSP_VERSION_MIN":
                        this.kspVersionMin = this.GetVersion(data["KSP_VERSION_MIN"]);
                        break;

                    case "KSP_VERSION_MAX":
                        this.kspVersionMax = this.GetVersion(data["KSP_VERSION_MAX"]);
                        break;
                }
            }
        }

        private System.Version GetVersion(object data)
        {
            if (data is Dictionary<string, object>)
            {
                var version = data as Dictionary<string, object>;

                switch (version.Count)
                {
                    case 2:
                        return new System.Version((int)(long)version["MAJOR"], (int)(long)version["MINOR"]);

                    case 3:
                        return new System.Version((int)(long)version["MAJOR"], (int)(long)version["MINOR"], (int)(long)version["PATCH"]);

                    case 4:
                        return new System.Version((int)(long)version["MAJOR"], (int)(long)version["MINOR"], (int)(long)version["PATCH"], (int)(long)version["BUILD"]);

                    default:
                        return null;
                }
            }
            return new System.Version((string)data);
        }

        #endregion

        #region Debugging

        public override string ToString()
        {
            return this.path +
                   "\n\tNAME: " + (this.Name ?? "NULL") +
                   "\n\tURL: " + (this.Url ?? "NULL") +
                   "\n\tDOWNLOAD: " + (this.Download ?? "NULL") +
                   "\n\tVERSION: " + (this.Version != null ? this.Version.ToString() : "NULL") +
                   "\n\tKSP_VERSION: " + this.KspVersion +
                   "\n\tKSP_VERSION_MIN: " + this.KspVersionMin +
                   "\n\tKSP_VERSION_MAX: " + this.KspVersionMax +
                   "\n\tCompatibleKspVersion: " + this.IsCompatibleKspVersion +
                   "\n\tCompatibleKspVersionMin: " + this.IsCompatibleKspVersionMin +
                   "\n\tCompatibleKspVersionMax: " + this.IsCompatibleKspVersionMax;
        }

        #endregion
    }
}