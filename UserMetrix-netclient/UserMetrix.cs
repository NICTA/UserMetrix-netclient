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
using System.Net;

namespace UserMetrix
{
	public class UserMetrix
	{
		/** The current instance of the UserMetrix logging tool. */
		private static UserMetrix instance = null;

		/** The current version of the log file this client generates. */
		const int LOG_VERSION = 1;

		/** The name of the temporary log file we write to disk. */
		const string LOGFILE = "usermetrix.log";

		/** The name of the persistant file that we store the client ID within. */
		const string IDFILE = "usermetrix.id";

		/** The UUID for this client. */
		private string clientID;

		/** The path to the to the log file that we are writing too. */
		private string log;

		/** The log writer that we use to write log messages. */
		private StreamWriter logWriter = null;

		/** The timer we use to determine when log events occur. */
		private Stopwatch timer;

		/** The configuration object for this UserMetrix client. */
		private Configuration config;

		/** Are we permitted to transmit usage logs to the UserMetrix server? */
		private Boolean canSendLogs;

		/**
		 * Initalise the UserMetrix log - call this when you start your
		 * application.
		 * 
		 * \param config The configuration to use for this UserMetrix client.
		 */
		public static void Initalise(Configuration config) {
			if (instance == null) { 
				instance = new UserMetrix(config);

				// Determine UUID for this client - ID file exists on disk read UUID from file.
				if (File.Exists(config.GetUmDirectory() + IDFILE)){
					Console.WriteLine("UUID exists");
					instance.SetUniqueID(File.ReadAllText(config.GetUmDirectory() + IDFILE));

				// ID file does not exist - generate a new UUID.
				} else {
					Guid id = Guid.NewGuid();
					StreamWriter file = new StreamWriter(config.GetUmDirectory() + IDFILE);
					file.WriteLine(id.ToString());
					instance.SetUniqueID(IDFILE.ToString());
					file.Close();
				}
				
				instance.SetLogDestination(config.GetUmDirectory() + LOGFILE);
				instance.StartLog();
			}
		}

	    /**
      	 * Are we permitted by the user to send usage logs to the UserMetrix server?
      	 *
      	 * \param canSend True if we are able to send logs to the UserMetrix server
      	 * false otherwise.
      	 */
    	public static void SetCanSendLogs(Boolean canSend) {
        	instance.canSendLogs = canSend;
    	}

		/**
		 * \return The logger for the supplied class (T).
		 */
		public static Logger GetLogger<T>() {
			if (instance != null) {
				return new Logger(typeof(T), instance);
			}

			return null;
		}

		/**
		 * Shutdown the UserMetrix log - call this when your application gracefully
		 * exits.
		 */
		public static void Shutdown() {
			// Terminate the UserMetrix logging session.
			if (instance != null) {
				instance.FinishLog();
				instance = null;
			}
		}


		/**********************************************************************
		 * Private UserMetrix methods below here.
		 **********************************************************************/
		/**
		 * Private constructor, implemented as singleton - use UserMetrix.Initalise
		 * to start logging with UserMetrix.
		 */
		private UserMetrix(Configuration configuration) {
			canSendLogs = true;
			config = configuration;
		}
		
		
		private void SetUniqueID(String id) {
			clientID = id;
		}

		private void SetLogDestination(string logDestination) {
			log = logDestination;

			// If we have a log already on disk - the application must have aborted
			// before it had a chance to complete and send the log. Transmit the
			// Partial log to the UserMetrix server.
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
				logWriter.Write("  start: " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzz00") + Environment.NewLine);
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
				logWriter.Write("    message: \"" + message + "\"" + Environment.NewLine);
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

		private void CleanLogFromDisk() {
			File.Delete(config.GetUmDirectory() + LOGFILE);
		}

		private void SendLog() {
			if (!canSendLogs) {
				// Not permitted to send logs - clean log and leave.
				CleanLogFromDisk();
				return;
			}

        	string boundary = "***" + DateTime.Now.Ticks.ToString("x");
	        byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

	        HttpWebRequest wr = (HttpWebRequest) WebRequest.Create("http://usermetrix.com/projects/" + config.GetProjectID() + "/log");
	        wr.ContentType = "multipart/form-data;boundary=" + boundary;
	        wr.Method = "POST";
	        wr.KeepAlive = true;
	        wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
	        Stream rs = wr.GetRequestStream();

			// Write the header of the multipart HTTP POST request.
			rs.Write(boundarybytes, 0, boundarybytes.Length);
	        string header = "Content-Disposition: form-data; name=\"upload\"; filename=\"usermetrix.log\"\r\n\r\n";
	        byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
	        rs.Write(headerbytes, 0, headerbytes.Length);

			// Read the log and append it as an attachment to the POST request.
	        FileStream fileStream = new FileStream(config.GetUmDirectory() + LOGFILE, FileMode.Open, FileAccess.Read);
	        byte[] buffer = new byte[4096];
	        int bytesRead = 0;
	        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
	            rs.Write(buffer, 0, bytesRead);
	        }
	        fileStream.Close();

			// Write the footer of the multipart HTTP POST request.
			byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        	rs.Write(trailer, 0, trailer.Length);
	        rs.Close();

			// Delete the log file after succesfully sending logs
			CleanLogFromDisk();
		}
	}
}