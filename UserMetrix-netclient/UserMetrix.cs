/*
 * UserMetrix.cs
 * UserMetrix-netclient
 *
 * Copyright (c) 2011 UserMetrix Pty Ltd. All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.IO;
using System.Diagnostics;

namespace UserMetrix
{
	public class UserMetrix
	{
		const int LOG_VERSION = 1;
		
		private static string logFile = "usermetrix.log";

		private static string idFile = "usermetrix.id";

		private static UserMetrix instance = null;

		private string clientID;

		private string log;

		private StreamWriter logWriter = null;

		private Stopwatch timer;

		private Configuration config;

		private UserMetrix(Configuration configuration) {
			config = configuration;
		}

		public static void Initalise(Configuration config) {
			if (instance == null) { 
				instance = new UserMetrix(config);

				// Determine UUID for this client - ID file exists on disk read UUID from file.
				if (File.Exists(config.GetUmDirectory() + idFile)){
					Console.WriteLine("UUID exists");
					instance.SetUniqueID(File.ReadAllText(config.GetUmDirectory() + idFile));

				// ID file does not exist - generate a new UUID.
				} else {
					Guid id = Guid.NewGuid();
					StreamWriter file = new StreamWriter(config.GetUmDirectory() + idFile);
					file.WriteLine(id.ToString());
					instance.SetUniqueID(idFile.ToString());
					file.Close();
				}
				
				instance.SetLogDestination(config.GetUmDirectory() + logFile);
				instance.StartLog();
			}
		}

		public static Logger GetLogger<T>() {
			if (instance != null) {
				return new Logger(typeof(T), instance);
			}

			return null;
		}

		public static void Shutdown() {
			// Terminate the UserMetrix logging session.
			if (instance != null) {
				instance.FinishLog();
				instance = null;
			}
		}

		private void SetUniqueID(String id) {
			clientID = id;
		}

		private void SetLogDestination(string logDestination) {
			log = logDestination;

			if (File.Exists(log)) {
				SendLog();
			}

			logWriter = new StreamWriter(logDestination);
		}

		private void StartLog() {
			if (logWriter != null) {
				logWriter.Write("---" + Environment.NewLine);
				
				logWriter.Write("v: " + LOG_VERSION + Environment.NewLine);
				logWriter.Write("system:" + Environment.NewLine);
				
				// Write the unique client identifier to the log.
				logWriter.Write("  id: " + clientID);

				// Write the details of the operating system out to the log.
				logWriter.Write("  os: " + Environment.OSVersion.ToString() + Environment.NewLine);

				// Write the details of the application start time out to the log.
				logWriter.Write("  start: " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + Environment.NewLine);
				timer = new Stopwatch();
				timer.Start();

				logWriter.Write("meta: " + Environment.NewLine);
				logWriter.Write ("log: " + Environment.NewLine);
				logWriter.Flush();
			}
		}

		public void View(string tag, Type source) {
			if (logWriter != null) {
				logWriter.Write("  - type: view" + Environment.NewLine);
                WriteMessageDetails(tag, source);
			}
		}

		public void Event(string tag, Type source) {
			if (logWriter != null) {
				logWriter.Write ("  - type: usage" + Environment.NewLine);
				WriteMessageDetails(tag, source);
			}
		}

		public void Error(string message, Type source) {
			if (logWriter != null) {
				logWriter.Write("  - type: error" + Environment.NewLine);
				WriteMessageDetails(message, source);
			}
		}

		public void Error(string message, Exception error, Type source) {
			if (logWriter != null) {
				this.Error(message, source);
				WriteStackDetails(error);
			}
		}

		public void Error(Exception error, Type source) {
			if (logWriter != null) {
				this.Error(error.Message, source);
				WriteStackDetails(error);
			}
		}

		public void Frustration(string tag, Type source) {
			if (logWriter != null) {
				logWriter.Write("  - type: frustration" + Environment.NewLine);
				WriteMessageDetails(tag, source);
			}
		}

		private void WriteMessageDetails(string message, Type source) {
			if (logWriter != null) {
				logWriter.Write("    time: " + instance.GetElapsedMilliseconds() + Environment.NewLine);
				logWriter.Write("    source: " + source.ToString() + Environment.NewLine);
				logWriter.Write("    message: " + message + Environment.NewLine);
				logWriter.Flush();
			}
		}

		private void WriteStackDetails(Exception error) {
			if (logWriter != null) {
				logWriter.Write("    stack:" + Environment.NewLine);
				StackTrace trace = new StackTrace(error);
				Console.WriteLine("Frames: " + trace.FrameCount + Environment.NewLine);
				foreach(StackFrame frame in trace.GetFrames()) {
					if (frame.GetFileName() == null) {
						logWriter.Write("      - class: unavailable" + Environment.NewLine);
					} else {
						logWriter.Write("      - class: " + frame.GetFileName() + Environment.NewLine);
					}

					if (frame.GetFileLineNumber() == 0) {
						logWriter.Write("        line: unavailable" + Environment.NewLine);
					} else {
						logWriter.Write("        line: " + frame.GetFileLineNumber() + Environment.NewLine);
					}
					logWriter.Write("        method: " + frame.GetMethod().ToString() + Environment.NewLine);
				}
			}
		}

		private void FinishLog() {
			if (logWriter != null) {
				logWriter.Write("duration: " + instance.GetElapsedMilliseconds() + Environment.NewLine);
				timer.Stop();
				logWriter.Close();

				if (config.GetProjectID() != 0) {
					SendLog();
				}
			}
		}

		private long GetElapsedMilliseconds() {
			return timer.ElapsedMilliseconds;
		}

		private void SendLog() {
		}
	}
}