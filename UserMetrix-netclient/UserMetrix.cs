/*
 * UserMetrix.cs
 * UserMetrix-netclient
 *
 * VERSION: 1.0.0
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

		public static void initalise(Configuration config) {
			if (instance == null) { 
				instance = new UserMetrix(config);

				// Determine UUID for this client - ID file exists on disk read UUID from file.
				if (File.Exists(config.getUmDirectory() + idFile)) {
					Console.WriteLine("UUID exists");
					instance.setUniqueID(File.ReadAllText(config.getUmDirectory() + idFile));

				// ID file does not exist - generate a new UUID.
				} else {
					Guid id = Guid.NewGuid();
					StreamWriter file = new StreamWriter(config.getUmDirectory() + idFile);
					file.WriteLine(id.ToString());
					instance.setUniqueID(idFile.ToString());
					file.Close();
				}
				
				instance.setLogDestination(config.getUmDirectory() + logFile);
				instance.startLog();
			}
		}

		public static Logger getLogger<T>() {
			if (instance != null) {
				return new Logger(typeof(T), instance);
			}

			return null;
		}

		public static void shutdown() {
			// Terminate the UserMetrix logging session.
			if (instance != null) {
				instance.finishLog();
				instance = null;
			}
		}

		private void setUniqueID(String id) {
			clientID = id;
		}

		private void setLogDestination(string logDestination) {
			log = logDestination;

			if (File.Exists(log)) {
				sendLog();
			}

			logWriter = new StreamWriter(logDestination);
		}

		private void startLog() {
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

		public void error(string message, Type source) {
			if (logWriter != null) {
				logWriter.Write("  - type: error" + Environment.NewLine);
				writeMessageDetails(message, source);
			}
		}

		public void error(Exception error, Type source) {
			if (logWriter != null) {
				this.error(error.Message, source);
				writeStackDetails(error);
			}
		}

		public void frustration(string tag, Type source) {
			if (logWriter != null) {
				logWriter.Write("  - type: frustration" + Environment.NewLine);
				writeMessageDetails(tag, source);
			}
		}

		private void writeMessageDetails(string message, Type source) {
			if (logWriter != null) {
				logWriter.Write("    time: " + instance.getElapsedMilliseconds() + Environment.NewLine);
				logWriter.Write("    source: " + source.ToString() + Environment.NewLine);
				logWriter.Write("    message: " + message + Environment.NewLine);
				logWriter.Flush();
			}
		}

		private void writeStackDetails(Exception error) {
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

		private void finishLog() {
			if (logWriter != null) {
				logWriter.Write("duration: " + instance.getElapsedMilliseconds() + Environment.NewLine);
				timer.Stop();
				logWriter.Close();

				if (config.getProjectID() != 0) {
					sendLog();
				}
			}
		}

		private long getElapsedMilliseconds() {
			return timer.ElapsedMilliseconds;
		}

		private void sendLog() {
		}
	}
}