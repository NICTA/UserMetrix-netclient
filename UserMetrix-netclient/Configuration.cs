/*
 * Configuration.cs
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

namespace UserMetrix
{
	public class Configuration
	{
		/** The directory to store UserMetrix working files. */
		private string umDirectory = "";

		/** The ID of the project on the UserMetrix server. */
		private int projectID = 0;	

		/**
		 * Private constructor.
		 */
		private Configuration() {
		}

		/**
		 * Constructor
		 * 
		 * \param newProjectID The UserMetrix ID for your project.
		 */
		public Configuration(int newProjectID) {
			projectID = newProjectID;
		}

		/**
		 * \return The UserMetrix ID for your project.
		 */
		public int GetProjectID() {
			return projectID;
		}

		/**
		 * \return The directory UserMetrix can use to store persistant files.
		 */
		public string GetUmDirectory() {
			return umDirectory;
		}

		/**
		 * Sets the directory UserMetrix can use to store persistant files.
		 *
		 * \param directory The directory UserMetrix can use to store persistant files.
		 */
		public void SetUmDirectory(string directory) {
			umDirectory = directory;
		}
	}
}

