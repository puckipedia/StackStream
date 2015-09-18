using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.IRC
{
    class Program
    {
        private class IRCStdIo : Tokens.Stream
        {
            public IRCStdIo()
            {
                Builder = new StringBuilder();
            }

            public override bool IsEOF
            {
                get
                {
                    return true;
                }
            }

            public StringBuilder Builder
            {
                get;
                private set;
            }

            public override byte Read()
            {
                return 0;
            }

            public override bool Seek(int location)
            {
                return false;
            }

            public override int Tell()
            {
                return 0;
            }

            public override void Write(byte val)
            {
                Builder.Append((char) val);
            }
        }
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("ENTER_ME", 6667);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);
            writer.AutoFlush = true;
            writer.WriteLine("NICK ENTER_ME");
            writer.WriteLine("USER ENTER_ME 8 * :ENTER_ME");
            writer.WriteLine("JOIN #ENTER_ME");

            while (true)
            {
                var msg = reader.ReadLine().Split(' ');
                if (msg[0] == "PING")
                {
                    msg[0] = "PONG";
                    writer.WriteLine(string.Join(" ", msg));
                }
                else if (msg[1] == "PRIVMSG")
                {
                    string channel = msg[2];
                    string message = string.Join(" ", msg.Skip(3)).Substring(1);
                    if (message.StartsWith("~{"))
                    {
                        Executor e = new Executor();
                        var stdio = new IRCStdIo();
                        e.Methods["stdinout"] = delegate (Executor exec)
                        {
                            exec.DataStack.Push(stdio);
                        };

                        e.CodeStack.PushRange(Lexer.Parse(message.Substring(2)).Value);
                        string result;
                        try {
                            for (int i = 0; i < 6000 && e.CodeStack.Count > 0; i++)
                                e.Cycle();
                            if (e.CodeStack.Count > 0)
                                result = "[TIMED OUT]";
                            else
                                result = stdio.Builder.ToString();
                        }
                        catch (Exception ex)
                        {
                            result = ex.Message;
                        }

                        result = result.Split('\r', '\n')[0];
                        if (result.Length > 0)
                            writer.WriteLine("PRIVMSG {0} :\u200B{1}", msg[2], result);
                        if (e.DataStack.Count > 0)
                            writer.WriteLine("PRIVMSG {0} :\u200BStack: {1}", msg[2], e.DataStack.ToString());
                    }
                }
            }
        }
    }
}
