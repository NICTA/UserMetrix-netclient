/*
 * Logger.cs
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
	public class Logger
	{
		/** The source class for messages from this logger. */
		private Type logSource;

		/** The UserMetrix manager responsible for dispatching messages. */
		private UserMetrix manager;

		public Logger(Type source, UserMetrix logManager) {
			logSource = source;
			manager = logManager;
		}

		public void frustration(string message) {
			manager.frustration(message, logSource);
		}

		public void error(string message) {
			manager.error(message, logSource);
		}

		public void error(string message, Exception exception) {
			manager.error(message, exception, logSource);
		}

		public void error(Exception exception) {
			manager.error(exception, logSource);
		}
	}
}

