using System;
using System.Collections.Generic;
using System.Linq;

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
            ChildrenFiles = new Dictionary<string, FileNode>();
            Content = "";
        }


        public string Name { get; set; }
        public string Content { get; set; }
        public PermissionType Permission { get; set; }
        public Dictionary<string, FileNode> ChildrenFiles { get; set; }
        public FileNode Father { get; }

        public bool AddChildrenFile(string name)
        {
            var child = new FileNode(this, name,
                PermissionType.Read | PermissionType.Write);
            if (ChildrenFiles.ContainsKey(name))
            {
                Console.WriteLine("File with the same filename exists!");
                return false;
            }

            ChildrenFiles.Add(name, child);
            return true;
        }

        public bool AddChildrenDir(string name)
        {
            var child = new FileNode(this, name,
                PermissionType.Read | PermissionType.Write | PermissionType.Excute);
            if (ChildrenFiles.ContainsKey(name))
            {
                Console.WriteLine("Dir with the same pathname exists!");
                return false;
            }

            ChildrenFiles.Add(name, child);
            return true;
        }

        public bool FindChildDirectory(string name, out FileNode fn)
        {
            fn = this;
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            var paths = name.Split('/');
            foreach (var path in paths)
            {
                var can = false;
                switch (path)
                {
                    case "..":
                        if (fn.Father != null)
                        {
                            fn = fn.Father;
                            can = true;
                        }

                        break;
                    case ".":
                        if (fn.Father != null)
                        {
                            can = true;
                        }

                        break;
                    default:
                        if (fn.ChildrenFiles.ContainsKey(path) &&
                            (fn.ChildrenFiles[path].Permission & PermissionType.Excute) != 0)
                        {
                            fn = fn.ChildrenFiles[path];
                            can = true;
                        }

                        break;
                }

                if (!can)
                {
                    Console.WriteLine("Path donesn't exist!");
                    return false;
                }
            }

            return true;
        }

        public void RemoveChild(string name)
        {
            if (!ChildrenFiles.ContainsKey(name))
            {
                Console.WriteLine($"File '{name}' doesn't exist!");
            }
            else
            {
                ChildrenFiles.Remove(name);
            }
        }

        public void MoveChild(string name, string newName)
        {
            if (!ChildrenFiles.ContainsKey(name))
            {
                Console.WriteLine($"File '{name}' doesn't exist!");
            }
            else if (name != newName)
            {
                var node = ChildrenFiles[name];
                if ((node.Permission & PermissionType.Excute) != 0)
                {
                    if (AddChildrenDir(newName))
                    {
                        ChildrenFiles[newName] = node;
                    }
                }
                else
                {
                    if (AddChildrenFile(newName))
                    {
                        ChildrenFiles[newName] = node;
                    }
                }

                ChildrenFiles.Remove(name);
            }
        }

        public void ChangePermission(string name, string op, string mod)
        {
            if (!ChildrenFiles.ContainsKey(name))
            {
                Console.WriteLine($"File '{name}' doesn't exist!");
            }
            else
            {
                var node = ChildrenFiles[name];
                switch (op)
                {
                    case "-":
                        if (mod.Contains("w"))
                        {
                            node.Permission &= ~PermissionType.Write;
                        }

                        if (mod.Contains("r"))
                        {
                            node.Permission &= ~PermissionType.Read;
                        }

                        break;
                    case "+":
                        if (mod.Contains("w"))
                        {
                            node.Permission |= PermissionType.Write;
                        }

                        if (mod.Contains("r"))
                        {
                            node.Permission |= PermissionType.Read;
                        }

                        break;
                    case "=":
                        if (mod.Contains("w"))
                        {
                            node.Permission &= ~PermissionType.Read;
                            node.Permission = PermissionType.Write;
                        }

                        if (mod.Contains("r"))
                        {
                            node.Permission &= ~PermissionType.Write;
                            node.Permission = PermissionType.Read;
                        }

                        break;
                }
            }
        }

        public void ShowContent()
        {
            Console.WriteLine(Content);
        }

        public void ShowChildContent(string name)
        {
            if (!ChildrenFiles.ContainsKey(name))
            {
                Console.WriteLine($"File '{name}' doesn't exist!");
            }
            else
            {
                ChildrenFiles[name].ShowContent();
            }
        }

        public string PathInfo()
        {
            var it = this;
            var path = it.Name;

            while (it.Father != null)
            {
                it = it.Father;
                path = it.Name + "/" + path;
            }

            return "~/" + path;
        }

        public void ShowBaseInfo()
        {
            var path = PathInfo();
            Console.Write("[" + path + "]$");
        }

        public void ShowPathInfo()
        {
            Console.WriteLine(PathInfo());
        }

        public void ShowChildrenFiles()
        {
            Console.WriteLine($"There is {ChildrenFiles.Count} file in " + PathInfo());
            foreach (var childrenFile in ChildrenFiles.Values)
            {
                var str = "";
                if ((childrenFile.Permission & PermissionType.Excute) != 0)
                {
                    str += "d";
                }
                else
                {
                    str += "-";
                }

                if ((childrenFile.Permission & PermissionType.Read) != 0)
                {
                    str += "r";
                }
                else
                {
                    str += "-";
                }

                if ((childrenFile.Permission & PermissionType.Write) != 0)
                {
                    str += "w";
                }
                else
                {
                    str += "-";
                }

                if ((childrenFile.Permission & PermissionType.Excute) != 0)
                {
                    str += "x";
                }
                else
                {
                    str += "-";
                }

                Console.WriteLine($"{str} {childrenFile.ChildrenFiles.Count} {childrenFile.Name}");
            }
        }
    }

    internal class Program
    {
        private static readonly FileNode root =
            new FileNode(null, "",
                PermissionType.Excute | PermissionType.Read |
                PermissionType.Write);

        private static FileNode _current = root;

        private static void Main(string[] args)
        {
            while (true)
            {
                _current.ShowBaseInfo();
                var input = Console.ReadLine()?.Trim().Split(' ').Select(o => o.Trim()).ToArray();
                if (input == null)
                {
                    continue;
                }

                var cmd = input[0];
                try
                {
                    switch (cmd)
                    {
                        case "cd":
                            ChangeDirectory(input);
                            break;
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
                            // Exit Program
                            return;
                        default:
                            Console.WriteLine(
                                $"The term \'{cmd}\' is not recognized as the name of a function");
                            break;
                    }
                }
                catch (Exception e)
                {
                    if (e is NotImplementedException)
                    {
                        Console.WriteLine(
                            $"The function \'{cmd}\' hasn't been implemented. O.o");
                    }
                }
            }
        }

        private static void ChangeDirectory(string[] param)
        {
            if (param[1].StartsWith("/"))
            {
                if (root.FindChildDirectory(param[1].Substring(1), out var fn))
                {
                    _current = fn;
                }
            }
            else if (_current.FindChildDirectory(param[1], out var fn))
            {
                _current = fn;
            }
        }

        private static void CatenateFile(string[] param)
        {
            _current.ShowChildContent(param[1]);
        }

        private static void ListDirectory(string[] param)
        {
            _current.ShowChildrenFiles();
        }

        private static void ChangePermission(string[] param)
        {
            _current.ChangePermission(param[1], param[2], param[3]);
        }

        private static void CreateDirectory(string[] param)
        {
            _current.AddChildrenDir(param[1]);
        }

        private static void CreateFile(string[] param)
        {
            _current.AddChildrenFile(param[1]);
        }

        private static void MoveFileOrDirecotry(string[] param)
        {
            _current.MoveChild(param[1], param[2]);
        }

        private static void RemoveFileOrDirecotry(string[] param)
        {
            _current.RemoveChild(param[1]);
        }
    }
}