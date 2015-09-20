using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream
{
    class Program
    {
        static void Main(string[] args)
        {
            Executor exec = new Executor();

            while (true) // loop
            {
                Console.Write(" {0} | ", exec.DataStack.ToString()); // print
                var line = Console.ReadLine();

                exec.CodeStack.PushRange(Lexer.Parse(line).Value); // read
                exec.Methods["new-tcpstream"] = new Executor.NativeFunction(Builtins.FileTcpIo.NewTcpStream);
                exec.Methods["read-file"] = new Executor.NativeFunction(Builtins.FileTcpIo.ReadFile);
                exec.Methods["write-file"] = new Executor.NativeFunction(Builtins.FileTcpIo.WriteFile);

                try {
                    while (exec.CodeStack.Count > 0)
                        exec.Cycle(); // eval
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                }
            }
        }
    }
}
