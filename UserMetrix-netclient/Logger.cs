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
		
		/**
		 * Don't call this constructor directly, instead use UserMetrix.GetLogger instead.
		 */
		public Logger(Type source, UserMetrix logManager) {
			logSource = source;
			manager = logManager;
		}
		
		/**
		 * Use this to log 'events' -- or actions triggered by a user. 'clicked debug',
		 * 'created new tab', etc. 
		 *
		 * \param tag The unique tag to identify for this event, i.e. 'clicked debug'.
		 */
		public void Event(string tag) {
			manager.Event(tag, logSource);
		}

		/**
		 * Use this to log 'views' -- or unique screens viewed by the user, i.e.
		 * 'save dialog', 'main editor', etc.
		 * 
		 * \param tag The unique tag to identify this view, i.e. 'save dialog'.
		 */
		public void View(string tag) {
			manager.View(tag, logSource);
		}

		/**
		 * Use this to log 'frustration' -- or when a user wants to provide negative
		 * feedback. This could be implemented by a 'panic' button or some other gesture.
		 * It allows people to 'spank' your exception and provide a little snippet of text
		 * about their experiences at that point in time.
		 * 
		 * \param message The message or snippet from the user.
		 */
		public void Frustration(string message) {
			manager.Frustration(message, logSource);
		}

		/**
		 * Use this to log traditional software exceptions.
		 *
		 * \param message A description of the error encountered.
		 */
		public void Error(string message) {
			manager.Error(message, logSource);
		}

		/**
		 * Use this to log traditional software exceptions.
		 * 
		 * \param message A description of the error encountered.
		 * \param exception The exception that caused the error.
		 */
		public void Error(string message, Exception exception) {
			manager.Error(message, exception, logSource);
		}

		/**
		 * Use this to log traditional software exceptions.
		 *
		 * \param exception The exception that caused the error.
		 */
		public void Error(Exception exception) {
			manager.Error(exception, logSource);
		}
	}
}
