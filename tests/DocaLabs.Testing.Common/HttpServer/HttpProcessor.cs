// offered to the public domain for any use with no restriction
// and also with no warranty of any kind, please enjoy. - David Jeske. 
// http://www.codeproject.com/Articles/137979/Simple-HTTP-Server-in-C
// simple HTTP explanation
// http://www.jmarshall.com/easy/http/

using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Machine.Specifications.Annotations;
using NLog;

namespace DocaLabs.Testing.Common.HttpServer
{
    public class HttpProcessor : IDisposable
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        const int BufferSize = 16 * 1024;

        TcpClient Socket { get; set; }
        IHttpRequestHandler RequestHandler { get; set; }
        Stream InputStream { get; set; }

        public StreamWriter OutputWriter { get; private set; }
        public string HttpMethod { get; private set; }
        public string HttpUrl { get; private set; }
        public string HttpProtocolVersionstring { get; private set; }
        public Hashtable HttpHeaders { get; private set; }

        public HttpProcessor(TcpClient socket, IHttpRequestHandler requestHandler)
        {
            Socket = socket;
            RequestHandler = requestHandler;
            HttpHeaders = new Hashtable();

            InputStream = new BufferedStream(Socket.GetStream());
            OutputWriter = new StreamWriter(new BufferedStream(Socket.GetStream()));
        }

        [NotNull]
        static string StreamReadLine(Stream inputStream)
        {
            var data = new StringBuilder();

            while (true)
            {
                var nextChar = inputStream.ReadByte();

                if (nextChar == '\n')
                    break;

                if (nextChar == '\r')
                    continue;

                if (nextChar == -1)
                {
                    Thread.Sleep(100);
                    continue;
                }

                data.Append(Convert.ToChar(nextChar));
            }

            return data.ToString();
        }

        public void Process()
        {
            try
            {
                ParseRequest();

                ReadHeaders();

                if (HttpMethod.Equals("GET"))
                {
                    HandleGetRequest();
                }
                else if (HttpMethod.Equals("POST"))
                {
                    HandlePostRequest();
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception in processing the request", e);
                WriteResourceNotFound();
            }

            OutputWriter.Flush();
        }

        public void ParseRequest()
        {
            var request = StreamReadLine(InputStream);

            var tokens = request.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("invalid http request line");

            HttpMethod = tokens[0].ToUpper();
            HttpUrl = tokens[1];
            HttpProtocolVersionstring = tokens[2];

            Log.Debug("starting: " + request);
        }

        public void ReadHeaders()
        {
            Log.Debug(@"readHeaders()");

            while (true)
            {
                var line = StreamReadLine(InputStream);
                if (line.Equals(""))
                {
                    Log.Debug(@"got headers");
                    return;
                }

                var separator = line.IndexOf(':');
                if (separator == -1)
                    throw new Exception("invalid http header line: " + line);

                var name = line.Substring(0, separator);
                var pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                var value = line.Substring(pos, line.Length - pos);

                Log.Debug(@"header: {0}:{1}", name, value);

                HttpHeaders[name] = value;
            }
        }

        public void HandleGetRequest()
        {
            RequestHandler.HandleGetRequest(this);
        }

        public void HandlePostRequest()
        {
            Log.Debug(@"get post data start");

            using (var ms = new MemoryStream())
            {
                if (!HttpHeaders.ContainsKey("Content-Length"))
                    throw new Exception("SimpleHttpServer supports only requests where content length is set.");

                var contentLen = Convert.ToInt32(HttpHeaders["Content-Length"]);

                CopyInputTo(ms, contentLen);

                if (contentLen != ms.Length)
                    throw new Exception(string.Format("POST Content-Length({0}) header is not matching the content length.", contentLen));

                ms.Seek(0, SeekOrigin.Begin);

                Log.Debug(@"get post data end");

                RequestHandler.HandlePostRequest(this, new StreamReader(ms));
            }
        }

        void CopyInputTo(Stream dest, int toRead)
        {
            var buf = new byte[Math.Min(BufferSize, toRead)];

            while (toRead > 0)
            {
                Log.Debug(@"starting Read, to_read={0}", toRead);

                var numread = InputStream.Read(buf, 0, Math.Min(BufferSize, toRead));

                Log.Debug(@"read finished, numread={0}", numread);

                if (numread == 0)
                {
                    if (toRead == 0)
                        break;

                    throw new Exception("client disconnected during post");
                }

                toRead -= numread;

                dest.Write(buf, 0, numread);
            }
        } 

        public void WriteResourceNotFound()
        {
            OutputWriter.WriteLine("HTTP/1.0 404 Resource not found");
            OutputWriter.WriteLine("Connection: close");
            OutputWriter.WriteLine("");
        }

        public void Dispose()
        {
            if (InputStream != null)
                InputStream.Dispose();

            if(OutputWriter != null)
                OutputWriter.Dispose();
        }
    }
}