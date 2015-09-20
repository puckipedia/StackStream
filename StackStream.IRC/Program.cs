using Newtonsoft.Json;
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

        private class Config
        {
            public string host { get; set; }
            public int port { get; set; }
            public List<string> connect { get; set; }
        }

        static void Main(string[] args)
        {
            Config config = null;
            try {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Could not find config! Rename config.example.json to config.json and edit it.");
                return;
            }

            TcpClient client = new TcpClient(config.host, config.port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);
            writer.AutoFlush = true;
            foreach (var line in config.connect)
            {
                writer.WriteLine(line);
            }

            Executor e = new Executor();
            var stdio = new IRCStdIo();
            e.Methods["stdinout"] = new Executor.NativeFunction(delegate (Executor exec)
            {
                exec.DataStack.Push(stdio);
            });

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
                        stdio.Builder.Clear();
                        e.CodeStack.Clear();
                        e.DataStack.Clear();

                        e.Methods["reset"] = new Executor.NativeFunction(delegate (Executor exec)
                        {
                            e = new Executor();
                            e.Methods["stdinout"] = new Executor.NativeFunction(delegate (Executor execu)
                            {
                                execu.DataStack.Push(stdio);
                            });
                        });

                        e.CodeStack.PushRange(Lexer.Parse(message.Substring(2)).Value);
                        string result;
                        int cyclecount = 0;
                        try {
                            for (cyclecount = 0; cyclecount < 12000 && e.CodeStack.Count > 0; cyclecount++)
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
                            writer.WriteLine("PRIVMSG {0} :\u200B({2}) {1}", msg[2], result, cyclecount);
                        if (e.DataStack.Count > 0)
                            writer.WriteLine("PRIVMSG {0} :\u200B({2}) Stack: {1}", msg[2], e.DataStack.ToString(), cyclecount);
                        if (result.Length == 0 && e.DataStack.Count == 0)
                            writer.WriteLine("PRIVMSG {0} :\u200B({1}) [NO RESULT]", msg[2], cyclecount);
                    }
                }
            }
        }
    }
}
