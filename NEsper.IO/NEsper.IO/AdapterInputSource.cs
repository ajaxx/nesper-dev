// ---------------------------------------------------------------------------------- /
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;
using System.IO;
using System.Net;

using com.espertech.esper.common.client;
using com.espertech.esper.container;

namespace com.espertech.esperio
{
	/// <summary>
	/// An input source for adapters.
	/// </summary>
	public class AdapterInputSource
	{
		private readonly IContainer _container;
		private readonly Uri _url;
		private readonly string _resource;
		private readonly FileInfo _file;
		private readonly Stream _inputStream;
		private readonly TextReader _reader;
		
		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="container">the service container</param>
		/// <param name="resource">the name of the resource on the classpath to use as the source for an adapter</param>
		
		public AdapterInputSource(IContainer container, string resource)
		{
			_container = container;
		    _resource = resource ?? throw new ArgumentException("Cannot create AdapterInputStream from a null resource");
			_url = null;
			_file = null;
			_inputStream = null;
			_reader = null;
		}
		
		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="container">the service container</param>
		/// <param name="url">the URL for the resource to use as source for an adapter</param>
		public AdapterInputSource(IContainer container, Uri url)
		{
			_container = container;
			_url = url ?? throw new ArgumentException("Cannot create AdapterInputStream from a null URL");
			_resource = null;
			_file = null;
			_inputStream = null;
			_reader = null;
		}
		
		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="container">the service container</param>
		/// <param name="file">the file to use as a source</param>
		public AdapterInputSource(IContainer container, FileInfo file)
		{
			_container = container;
		    _file = file ?? throw new ArgumentException("file cannot be null");
			_url = null;
			_resource = null;
			_inputStream = null;
			_reader = null;
		}
		
		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="container">the service container</param>
		/// <param name="inputStream">the stream to use as a source</param>
		public AdapterInputSource(IContainer container, Stream inputStream)
		{
			_container = container;
		    _inputStream = inputStream ?? throw new ArgumentException("stream cannot be null");
			_file = null;
			_url = null;
			_resource = null;
			_reader = null;
		}

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="container">the service container</param>
		/// <param name="reader">reader is any reader for reading a file or string</param>
		public AdapterInputSource(IContainer container, TextReader reader)
		{
			_container = container;
		    _reader = reader ?? throw new ArgumentException("reader cannot be null");
			_url = null;
			_resource = null;
			_file = null;
			_inputStream  = null;
		}
		
		/// <summary>
		/// Get the resource as an input stream. If this resource was specified as an InputStream, 
		/// return that InputStream, otherwise, create and return a new InputStream from the 
		/// resource. If the source cannot be converted to a stream, return null.
		/// </summary>
		/// <returns>a stream from the resource</returns>
		public Stream GetAsStream()
		{
			if(_reader != null)
			{
				return null;
			}
			if(_inputStream != null)
			{
				return _inputStream;
			}
			if(_file != null)
			{
				try
				{
					return _file.OpenRead() ;
				} 
				catch (IOException e)
				{
					throw new EPException(e);
				}
			}
			if(_url != null)
			{
				try
				{
					WebClient webClient = new WebClient() ;
					return webClient.OpenRead(_url) ;
				} 
				catch (IOException e)
				{
					throw new EPException(e);
				}
			}
			else 
			{
				return ResolvePathAsStream(_resource);
			}
		}
		
		/// <summary>
		/// Return the reader if it was set, null otherwise.
		/// </summary>
		/// <returns>the Reader</returns>
		public TextReader GetAsReader()
		{
			return _reader;
		}
		
		/// <summary>
		/// Return true if calling getStream() will return a new InputStream created from the
		/// resource, which, assuming that the resource hasn't been changed, will have the same
		/// information as all the previous InputStreams returned by getStream() before they were
		/// manipulated; return false if the call will return the same instance of InputStream that 
		/// has already been obtained.
		/// </summary>
		/// <returns>true if each call to getStream() will create a new InputStream from the
		/// resource, false if each call will get the same instance of the InputStream
		/// </returns>
		public bool IsResettable
		{
			get { return _inputStream == null && _reader == null; }
		}
		
		private Stream ResolvePathAsStream(string path)
	    {
            var stream = _container.ResourceManager().GetResourceAsStream(path ) ;
            if (stream == null)
            {
                throw new EPException(path + " not found");
            }

		    return stream;
	    }
	}
}
