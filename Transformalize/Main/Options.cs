#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Main {

    public class Options {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private LogLevel _logLevel = LogLevel.Info;
        private string _mode = Common.DefaultValue;
        private List<string> _problems = new List<string>();

        public int Top { get; set; }

        public List<string> Problems { get { return _problems; } set { _problems = value; } }
        public bool Force { get; set; }

        public LogLevel LogLevel {
            get { return _logLevel; }
            set {
                _logLevel = value;
                SetLogLevel(value);
            }
        }

        public string Mode {
            get { return _mode; }
            set {
                _mode = value;
            }
        }

        public Options(string settings = "") {

            if (!string.IsNullOrEmpty(settings)) {
                try {
                    if (settings.Contains("'")) {
                        settings = settings.Replace('\'', '"');
                    }
                    var options = JSON.Instance.ToObject<Dictionary<string, object>>(settings);

                    foreach (var option in options) {
                        var key = option.Key.ToLower();
                        var value = option.Value.ToString().ToLower();
                        switch (key) {

                            case "mode":
                                Mode = value;
                                break;

                            case "loglevel":
                                LogLevel = LogLevel.FromString(value);
                                break;

                            case "force":
                                bool input;
                                if (bool.TryParse(value, out input)) {
                                    Force = input;
                                } else {
                                    RecordBadValue(option, typeof(bool));
                                }
                                break;

                            case "top":
                                int top;
                                if (int.TryParse(value, out top)) {
                                    Top = top;
                                } else {
                                    RecordBadValue(option, typeof(int));
                                }
                                break;

                            default:
                                RecordBadProperty(option);
                                break;
                        }
                    }
                } catch (Exception e) {
                    var message = string.Format("Couldn't parse options: {0}.", settings);
                    _log.DebugException(message + " " + e.Message, e);
                    Problems.Add(message);
                }
            }

        }

        private static void SetLogLevel(LogLevel logLevel) {

            if (logLevel == LogLevel.Info)
                return;

            foreach (var rule in LogManager.Configuration.LoggingRules) {
                if (rule.Targets.All(t => t.Name != "console"))
                    continue;

                foreach (var level in rule.Levels) {
                    if (level.Ordinal < logLevel.Ordinal)
                        rule.DisableLoggingForLevel(level);
                    else
                        rule.EnableLoggingForLevel(logLevel);
                }
            }
            LogManager.ReconfigExistingLoggers();
        }

        public bool Valid() {
            return !Problems.Any();
        }

        private void RecordBadValue(KeyValuePair<string, object> option, Type type) {
            Problems.Add(string.Format("The {0} option value of {1} must evaluate to a {2}.", option.Key, option.Value, type));
        }

        private void RecordBadProperty(KeyValuePair<string, object> option) {
            Problems.Add(string.Format("The {0} property is invalid.", option.Key));
        }

    }
}