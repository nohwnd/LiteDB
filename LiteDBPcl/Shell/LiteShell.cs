using System;
using System.Collections.Generic;
using LiteDB.Shell.Commands;

namespace LiteDB.Shell
{
    internal class LiteShell
    {
        private static readonly List<IShellCommand> _commands = new List<IShellCommand>();

        static LiteShell()
        {
            //todo: sorry no dynamic lookup
            //var type = typeof(IShellCommand);
            //var types = AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(s => s.GetTypes())
            //    .Where(p => type.IsAssignableFrom(p) && p.IsClass);

            //todo: list all
            var types = new List<Type> {typeof (FileFind)};

            foreach (var t in types)
            {
                var cmd = (IShellCommand) Activator.CreateInstance(t);
                _commands.Add(cmd);
            }
        }

        public BsonValue Run(DbEngine engine, string command)
        {
            if (string.IsNullOrEmpty(command)) return BsonValue.Null;

            var s = new StringScanner(command);

            foreach (var cmd in _commands)
            {
                if (cmd.IsCommand(s))
                {
                    return cmd.Execute(engine, s);
                }
            }

            throw LiteException.InvalidCommand(command);
        }
    }
}