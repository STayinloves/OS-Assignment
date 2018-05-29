using System;
using System.Collections.Generic;

namespace FileSystem
{
    [Flags]
    internal enum PermissionType
    {
        Read = 4,
        Write = 2,
        Excute = 1
    }

    internal class FileNode
    {
        public FileNode(FileNode father, string name, PermissionType permission)
        {
            Father = father;
            Name = name;
            Permission = permission;
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public PermissionType Permission { get; set; }
        public Dictionary<string, FileNode> ChildrenFiles { get; set; }
        public FileNode Father { get; }
    }

    internal class Program
    {
        private FileNode root =
            new FileNode(null, "/", PermissionType.Excute | PermissionType.Read | PermissionType.Write);

        private static void Main(string[] args)
        {
            while (true)
            {
                var input = Console.ReadLine()?.Trim().Split(' ');
                if (input == null)
                {
                    continue;
                }

                var cmd = input[0];
                try
                {
                    switch (cmd)
                    {
                        case "cat":
                            CatenateFile(input);
                            break;
                        case "ls":
                        case "dir":
                            ListDirectory(input);
                            break;
                        case "chmod":
                            ChangePermission(input);
                            break;
                        case "mkdir":
                            CreateDirectory(input);
                            break;
                        case "touch":
                            CreateFile(input);
                            break;
                        case "rm":
                            RemoveFileOrDirecotry(input);
                            break;
                        case "mv":
                            MoveFileOrDirecotry(input);
                            break;
                        case "exit":
                            return;
                        default:
                            Console.WriteLine($"The term \'{cmd}\' is not recognized as the name of a function");
                            break;
                    }
                }
                catch (Exception e)
                {
                    if (e is NotImplementedException)
                    {
                        Console.WriteLine($"The function \'{cmd}\' hasn't been implemented. O.o");
                    }
                }
            }
        }

        private static void CatenateFile(string[] param)
        {
            throw new NotImplementedException();
        }

        private static void ListDirectory(string[] param)
        {
            throw new NotImplementedException();
        }

        private static void ChangePermission(string[] param)
        {
            throw new NotImplementedException();
        }

        private static void CreateDirectory(string[] param)
        {
            throw new NotImplementedException();
        }

        private static void CreateFile(string[] param)
        {
            throw new NotImplementedException();
        }

        private static void MoveFileOrDirecotry(string[] param)
        {
            throw new NotImplementedException();
        }

        private static void RemoveFileOrDirecotry(string[] param)
        {
            throw new NotImplementedException();
        }
    }
}