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
using System.Threading;

namespace UserMetrix
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine("UserMetrix C# Example");

			// Call this once when your application starts.
			Configuration c = new Configuration(1);
			UserMetrix.Initalise(c);
			UserMetrix.SetCanSendLogs(true);

			// Throughout your application - use this to fetch a logger
			// for a specific source file.
			Logger l = UserMetrix.GetLogger<MainClass>();

			// Use this method to log 'views' - when single screens / dialogs
			// are displayed within your application.
			l.View("main view");

			// Use this method to log 'events' - when a user interacts with your
			// application, clicks a button, drags a slider, etc.
			l.Event("triggered button.");

			// Use this method to implement a 'panic' button - People can be frustrated
			// at anytime - you can also gather text feedback and send it to UserMetrix.
			l.Frustration("Unable to configure logger");

			try {
				testMethod();
			} catch (Exception e) {
				// Use this method to log when exceptions occur.
				l.Error("testMethod Failed", e);
			}

			// Call this when your application closes.
			UserMetrix.Shutdown();
		}

		public static void testMethod() {
			throw new Exception("OH-NOE!");
		}
	}
}
