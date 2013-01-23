﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2013 Michael Winsor
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Created: Monday, January 21, 2013 9:19:55 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GorgonLibrary.IO
{
    /// <summary>
    /// Reads/writes Gorgon chunked formatted data.
    /// </summary>
    /// <remarks>This object will take data and turn it into chunks of data.  This is similar to the old IFF format in that 
    /// it allows Gorgon's file formats to be future proof.  That is, if a later version of Gorgon has support for a feature
    /// that does not exist in a previous version, then the older version will be able to read the file and skip the 
    /// unnecessary parts.</remarks>
    public class GorgonChunkedFormat
        : IDisposable, IEnumerable<KeyValuePair<string, GorgonDataStream>>
    {
        #region Constants.
        /// <summary>
        /// The header for chunked data.
        /// </summary>
        public const string FileHeader = "GORCHUNK1.0";
        #endregion

        #region Variables.
        private byte[] _headerData = null;                                              // Header data.
        private bool _disposed = false;                                                 // Flag to indicate that the object was disposed.
        private IDictionary<string, GorgonDataStream> _chunks = null;                   // A list of chunks.
        #endregion

        #region Properties.
        /// <summary>
        /// Property to return the chunk count.
        /// </summary>
        public int ChunkCount
        {
            get
            {
                return _chunks.Count;
            }
        }

        /// <summary>
        /// Property to return a chunk stream by its name.
        /// </summary>
        public GorgonDataStream this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Parameter must not be empty.", "name");
                }

                string keyName = name.ToUpper();

                if (!_chunks.ContainsKey(keyName))
                {
                    throw new KeyNotFoundException("The chunk '" + name + "' does not exist.");
                }

                return _chunks[keyName];
            }
            set
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Parameter must not be empty.", "name");
                }

                string keyName = name.ToUpper();
                bool containsItem = _chunks.ContainsKey(keyName);

                if (containsItem)
                {
                    if (_chunks[keyName] != null)
                    {
                        _chunks[keyName].Dispose();
                        _chunks[keyName] = null;
                    }
                }
                else
                {
                    throw new KeyNotFoundException("The chunk '" + name + "' was not found.");
                }

                if (value == null)
                {
                    if (containsItem)
                    {
                        _chunks.Remove(keyName);
                    }

                    return;
                }

                _chunks[keyName] = value;
            }
        }
        #endregion

        #region Methods.
        /// <summary>
        /// Function to clear all chunks.
        /// </summary>
        public void ClearChunks()
        {
            foreach (var chunk in _chunks)
            {
                if (chunk.Value != null)
                {
                    chunk.Value.Dispose();
                }
            }

            _chunks.Clear();
        }

        /// <summary>
        /// Function to return whether a chunk exists.
        /// </summary>
        /// <param name="name">Name of the chunk.</param>
        /// <returns>TRUE if found, FALSE if not.</returns>
        public bool HasChunk(string name)
        {
            return _chunks.ContainsKey(name.ToUpper());
        }

        /// <summary>
        /// Function to destroy an existing chunk.
        /// </summary>
        /// <param name="name">Name of the chunk to destroy.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="name"/> parameter is NULL (Nothing in VB.Net).</exception>
        /// <exception cref="System.ArgumentException">Thrown when the name is empty.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the name was not found.</exception>
        public void DestroyChunk(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter must not be empty.", "name");
            }

            string keyName = name.ToUpper();

            if (!_chunks.ContainsKey(keyName))
            {                
                throw new KeyNotFoundException("The chunk '" + name + "' does not exist.");
            }
                        
            if (_chunks[keyName] != null)
            {
                _chunks[keyName].Dispose();
            }
            _chunks.Remove(keyName);
        }

        /// <summary>
        /// Function to create a new chunk block.
        /// </summary>
        /// <param name="name">Name of the chunk.</param>
        /// <param name="size">Size of the data in the chunk, in bytes.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="name"/> parameter is NULL (Nothing in VB.Net).</exception>
        /// <exception cref="System.ArgumentException">Thrown when the name is empty.</exception>
        /// <remarks>This creates a new chunk block to start writing into.  If the chunk exists, then the data will be appended to that chunk.</remarks>
        /// <returns>A data stream for the chunk information.</returns>
        public GorgonDataStream CreateChunk(string name, int size)
        {
            GorgonDataStream stream = new GorgonDataStream(size);

            CreateChunk(name, stream);

            return stream;
        }

        /// <summary>
        /// Function to create a new chunk block with an existing data stream.
        /// </summary>
        /// <param name="name">Name of the chunk.</param>
        /// <param name="stream">Existing data stream.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="name"/> parameter is NULL (Nothing in VB.Net).
        /// <para>-or-</para>
        /// <para>Thrown when the <paramref name="stream"/> parameter is NULL.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">Thrown when the name is empty.</exception>
        /// <exception cref="System.IO.IOException">Thrown when the stream is read-only.</exception>
        /// <remarks>This creates a new chunk block to start writing into.  If the chunk exists, then the data will be appended to that chunk.</remarks>
        /// <returns>A data stream for the chunk information.</returns>
        public void CreateChunk(string name, GorgonDataStream stream)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter must not be empty.", "name");
            }

            string keyName = name.ToUpper();

            if (!stream.CanWrite)
            {
                throw new IOException("The stream must be writable.");
            }

            if (_chunks.ContainsKey(keyName))
            {
                throw new ArgumentException("The chunk '" + name + "' already exists.");
            }

            _chunks[keyName] = stream;
        }

        /// <summary>
        /// Function to load a file containing chunked data.
        /// </summary>
        /// <param name="filePath">Path of the file to load.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="filePath"/> parameter is NULL (Nothing in VB.Net).</exception>
        /// <exception cref="System.ArgumentException">Thrown when the filePath is empty.</exception>
        public void Load(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Parameter must not be empty.", "filePath");
            }

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] byteData = new byte[stream.Length];

                stream.Read(byteData, 0, byteData.Length);
                Load(byteData);
            }
        }

        /// <summary>
        /// Function to load a stream of chunked data.
        /// </summary>
        /// <param name="stream">Stream to load.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="stream"/> parameter is NULL or empty.</exception>
        public void Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

			// Destroy current chunk data.
			ClearChunks();
						
			// Read from the stream.
			using (GorgonBinaryReader reader = new GorgonBinaryReader(stream, true))
			{
				// Get the header.
				reader.Read(_headerData, 0, _headerData.Length);

				if (string.Compare(Encoding.UTF8.GetString(_headerData, 0, _headerData.Length), FileHeader, true) != 0)
				{
					throw new IOException("The data is not a chunked format, or is a later version.");
				}

				// Get the number of chunks.
				int chunkCount = reader.ReadInt32();
				long tablePosition = stream.Position;
				long dataPosition = tablePosition + (chunkCount * sizeof(long));

				for (int i = 0; i < chunkCount; i++)
				{
					string chunkName = string.Empty;					// Chunk name.
					int chunkLength = 0;								// Chunk size, in bytes.
					GorgonDataStream chunkStream = null;				// Chunk stream.

					// Position to the next table index.
					stream.Position = tablePosition + (i * sizeof(long));

					// Move to the data.
					stream.Position = reader.ReadInt64() + tablePosition;

					chunkName = reader.ReadString();
					chunkLength = reader.ReadInt32();
					chunkStream = new GorgonDataStream(chunkLength);

					// Copy the data.
					reader.BaseStream.CopyTo(chunkStream);

					chunkStream.Position = 0;
					_chunks[chunkName] = chunkStream;
				}
			}
        }

        /// <summary>
        /// Function to load data a byte array in memory.
        /// </summary>
        /// <param name="data">Data to load.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="data"/> parameter is NULL.</exception>
        /// <exception cref="System.IO.IOException">Thrown if data is not Gorgon chunk data, or if the version is newer than this version can read.</exception>
        public void Load(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // Destroy current chunks.
            ClearChunks();

            if (data.Length == 0)
            {
                return;
            }

            using (GorgonDataStream stream = new GorgonDataStream(data))
            {
				Load(stream);
            }
        }

        /// <summary>
        /// Function to save the chunked data to a file.
        /// </summary>
        /// <param name="filePath">Path of the file to load.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="filePath"/> parameter is NULL (Nothing in VB.Net).</exception>
        /// <exception cref="System.ArgumentException">Thrown when the filePath is empty.</exception>
        public void Save(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Parameter must not be empty.", "filePath");
            }

            using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] data = Save();
                stream.Write(data, 0, data.Length);
            }
        }

        /*/// <summary>
        /// Function to save the chunked data to a stream.
        /// </summary>
        /// <param name="stream">Stream to write into.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="stream"/> parameter is NULL (Nothing in VB.Net).</exception>
        public void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] data = Save();

            if (data.Length > 0)
            {
                stream.Write(data, 0, data.Length);
            }
        }*/

		/// <summary>
		/// Function to save the chunked data to a stream.
		/// </summary>
		/// <param name="stream">Stream to write into.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="stream"/> parameter is NULL (Nothing in VB.Net).</exception>
		public void Save(Stream stream)
		{
			long chunkOffset = 0;

			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			if (_chunks.Count == 0)
			{
				return;
			}

			using (GorgonBinaryWriter writer = new GorgonBinaryWriter(stream, true))
			{				
				long streamStart = stream.Position;		// Capture the starting position of the stream.
				long tableOffset = 0;					// Offset into the stream for the table.
				int chunkCounter = 0;					// Chunk index counter.

				// Don't write empty chunks.
				var chunks = _chunks.Where(item => item.Value != null && item.Value.Length > 0);
				int chunkCount = chunks.Count();

				// Calculate how big the table will be.
				chunkOffset = (chunkCount * sizeof(long));

				// Write the header for the chunked data.
				writer.Write(_headerData, 0, _headerData.Length);

				// Write out the number of chunks.
				writer.Write(chunkCount);

				// Remember our table offset.
				tableOffset = stream.Position;

				// Pre fill table.
				for (int i = 0; i < chunkCount; i++)
				{
					writer.Write(0L);
				}				

				// Enumerate each chunk and send it to the stream.
				foreach (var chunk in chunks)
				{
					long chunkStreamPosition = 0;					// Chunk stream position.
					long chunkWriteSize = stream.Position;			// Used to calculate the actual chunk size, including header.

					// Reset the chunk stream to the beginning.
					chunk.Value.Position = 0;

					// Write the chunk header.
					writer.Write(chunk.Key.ToUpper());

					// Write the length of the chunk.
					writer.Write((int)chunk.Value.Length);

					// Copy the stream data into the destination stream.
					chunk.Value.CopyTo(stream);

					// Remember this stream position.
					chunkStreamPosition = stream.Position;

					// Calculate the size of the written data.
					chunkWriteSize = stream.Position - chunkWriteSize;

					// Put the entry back into the table.
					stream.Position = tableOffset + (chunkCounter * sizeof(long));
					writer.Write(chunkOffset);
					stream.Position = chunkStreamPosition;

					chunkOffset += chunkWriteSize;
					chunkCounter++;
				}
			}
		}

        /// <summary>
        /// Function to retrieve the chunked data as an array.
        /// </summary>
        /// <returns>An array of bytes containing all the chunked data.</returns>
        public byte[] Save()
        {            
            if (_chunks.Count == 0)
            {
                return new byte[] { };
            }

			using (MemoryStream stream = new MemoryStream())
			{
				Save(stream);

				return stream.ToArray();
			}
        }
        #endregion

        #region Constructor/Destructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonChunkedFormat" /> class.
        /// </summary>
        public GorgonChunkedFormat()
        {
            _chunks = new Dictionary<string, GorgonDataStream>();
            _headerData = Encoding.UTF8.GetBytes(FileHeader);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearChunks();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IEnumerable<KeyValuePair<string,GorgonDataStream>> Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, GorgonDataStream>> GetEnumerator()
        {
            return _chunks.GetEnumerator();   
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_chunks).GetEnumerator();        
        }
        #endregion
    }
}
