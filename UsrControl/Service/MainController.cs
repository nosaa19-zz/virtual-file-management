using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using UsrControl.Domain;

namespace UsrControl.Service
{
    class MainController
    {
        private List<User> MasterUsers = new List<User>();
        private List<Folder> MasterFolders = new List<Folder>();
        private List<File> MasterFiles = new List<File>();
        private int InitFolderId = 1000;

        private string GetFirstWord(string text)
        {
            string firstWord = String.Empty;

            // Check for empty string.
            if (String.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Get first word from passed string
            firstWord = text.Split(' ').FirstOrDefault();
            if (String.IsNullOrEmpty(firstWord))
            {
                return string.Empty;
            }

            return firstWord;
        }

        private void RegisterUser(String command) {

            var regularExpression = new Regex(@"(Register)\s(.+)",RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;

            if (username.Split(" ").Count() > 1)
            {
                Console.WriteLine("Warning - username contains whitespace, please consider another name");
                return;
            }
            else 
            {
                if (username.Equals(String.Empty))
                {
                    Console.WriteLine("Error - wrong format!");
                    return;
                }
            }


            User newUser = MasterUsers.Find(newUser => newUser.Username.ToLower() == username.ToLower());
            if (newUser != null)
            {
                Console.WriteLine("Error - user already existing");
                return;
            }
            else
            {
                newUser = new User();
                newUser.Id = Guid.NewGuid().ToString();
                newUser.Username = username;

                MasterUsers.Add(newUser);
                Console.WriteLine("Success");
            }
        }

        private void CreateFolder(String command) {
            var regularExpression = new Regex(@"(Create_Folder)\s(\w+)\s(['`‘].+['`’])\s(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderName = match.Groups[3].Value;
            String folderDescription = match.Groups[4].Value;

            folderName = Regex.Replace(folderName, "^['`‘]|['`‘]$", "");
            folderDescription = Regex.Replace(folderDescription, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Folder newFolder = MasterFolders.Find(newFolder => newFolder.UserId.Equals(user.Id) && newFolder.Name.ToLower().Equals(folderName.ToLower()));
            if (newFolder != null)
            {
                Console.WriteLine("Error - folder with user "+ user.Username +" already existing");
                return;
            }
            else
            {
                newFolder = new Folder();
                newFolder.Id = (++InitFolderId).ToString();
                newFolder.UserId = user.Id;
                newFolder.Name = folderName;
                newFolder.Description = folderDescription;
                newFolder.CreatedAt = DateTime.Now;

                MasterFolders.Add(newFolder);
                Console.WriteLine(newFolder.Id);
            }

        }

        private void GetFolders(String command) {
            var regularExpression = new Regex(@"(Get_Folders)\s(\w+)\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String sortSettings = match.Groups[3].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            List<Folder> folders = MasterFolders.FindAll(folder => folder.UserId.Equals(user.Id));
            if (folders.Count() > 0)
            {
                List<Folder> SortedFolder = folders;
                String[] varSet = sortSettings.Split(" ");

                if (varSet.Count() == 2)
                {
                    String sortProp = varSet[0];
                    String sortOrder = varSet[1];

                    if (sortProp.ToLower().Contains("sort_name"))
                    {
                        if (sortOrder.ToLower().Contains("desc"))
                        {
                            SortedFolder = folders.OrderByDescending(o => o.Name).ToList();
                        }
                        else
                        {
                            SortedFolder = folders.OrderBy(o => o.Name).ToList();
                        }
                    }
                    else if (sortProp.ToLower().Contains("sort_time"))
                    {
                        if (sortOrder.ToLower().Contains("desc"))
                        {
                            SortedFolder = folders.OrderByDescending(o => o.CreatedAt).ToList();
                        }
                        else
                        {
                            SortedFolder = folders.OrderBy(o => o.CreatedAt).ToList();
                        }
                    }
                }

                Console.WriteLine("------------------------------------------------------------------------");
                for (int i = 0; i < SortedFolder.Count; i++)
                {
                    Console.WriteLine("| " + SortedFolder[i].Id + " | " + SortedFolder[i].Name + " | " + SortedFolder[i].Description + " | " + SortedFolder[i].CreatedAt.ToString("MM-dd-yyyy HH:mm:ss") + " | " + user.Username + " |");
                }
                Console.WriteLine("------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Warning - empty folders");
                return;
            }

        }

        private void RenameFolder(String command) {
            var regularExpression = new Regex(@"(Rename_Folder)\s(\w+)\s(.+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String newFolderName = match.Groups[4].Value;

            newFolderName = Regex.Replace(newFolderName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
            if (folder == null)
            {
                Console.WriteLine("Error - folder doesn’t exist");
                return;
            }
            else
            {
                if (folder.UserId != user.Id)
                {
                    Console.WriteLine("Error - folder owner not match");
                    return;
                }


                Folder findFolder = MasterFolders.Find(findFolder => findFolder.UserId.Equals(user.Id) && findFolder.Name.ToLower().Equals(newFolderName.ToLower()));

                if (findFolder != null)
                {
                    Console.WriteLine("Error - folder with user " + user.Username + " already existing, please consider another name");
                    return;
                }

                folder.Name = newFolderName;
                Console.WriteLine("Success");
            }
        }

        private void DeleteFolder(String command) {
            var regularExpression = new Regex(@"(Delete_Folder)\s(\w+)\s(.+)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
            if (folder == null)
            {
                Console.WriteLine("Error - folder doesn’t exist");
                return;
            }
            else 
            {
                if (folder.UserId != user.Id) {
                    Console.WriteLine("Error - folder owner not match");
                    return;
                }

                MasterFolders.Remove(folder);
                Console.WriteLine("Success");
            }
        }

        private void UploadFile(String command)
        {
            var regularExpression = new Regex(@"(Upload_File)\s(\w+)\s([0-9]+)\s(['`‘]\w+\.\w+['`’])\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String fileName = match.Groups[4].Value;
            String fileDescription = match.Groups[5].Value;

            fileName = Regex.Replace(fileName, "^['`‘]|['`‘]$", "");
            fileDescription = Regex.Replace(fileDescription, "^['`‘]|['`‘]$", "");
            String fileExtension = fileName.Substring(fileName.IndexOf('.') + 1);

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
            if (folder == null)
            {
                Console.WriteLine("Error - folder_id not found");
                return;
            }
            else
            {
                if (folder.UserId != user.Id)
                {
                    Console.WriteLine("Error - folder owner not match");
                    return;
                }

                File newFile = MasterFiles.Find(newFile => newFile.FolderId.Equals(folder.Id) && newFile.Name.ToLower().Equals(fileName.ToLower()));
                if (newFile != null)
                {
                    Console.WriteLine("Error - file in Folder " + folder.Name + " with User " + user.Username + " already existing");
                    return;
                }
                else
                {
                    newFile = new File();
                    newFile.Id = Guid.NewGuid().ToString();
                    newFile.FolderId = folder.Id;
                    newFile.Name = fileName;
                    newFile.Extension = fileExtension;
                    newFile.Description = fileDescription;
                    newFile.CreatedAt = DateTime.Now;

                    MasterFiles.Add(newFile);
                    Console.WriteLine("Success");
                }

            }
        }

        private void DeleteFile(String command) 
        {
            var regularExpression = new Regex(@"(Delete_File)\s(\w+)\s([0-9]+)\s(\w+\.\w+)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String fileName = match.Groups[4].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
            if (folder == null)
            {
                Console.WriteLine("Error - folder_id not found");
                return;
            }
            else
            {
                if (folder.UserId != user.Id)
                {
                    Console.WriteLine("Error - folder owner not match");
                    return;
                }

                File file = MasterFiles.Find(file => file.FolderId.Equals(folder.Id) && file.Name.ToLower().Equals(fileName.ToLower()));
                if (file == null)
                {
                    Console.WriteLine("Error - file doesn’t exist");
                    return;
                }
                else
                {
                    MasterFiles.Remove(file);
                    Console.WriteLine("Success");
                }

            }
        }

        private void GetFiles(String command) {
            var regularExpression = new Regex(@"(Get_Files)\s(\w+)\s([0-9]+)\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String sortSettings = match.Groups[4].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
            if (folder == null)
            {
                Console.WriteLine("Error - folder_id not found");
                return;
            }
            else
            {
                if (folder.UserId != user.Id)
                {
                    Console.WriteLine("Error - folder owner not match");
                    return;
                }

                List<File> files = MasterFiles.FindAll(file => file.FolderId.Equals(folder.Id));
                if (files.Count() > 0)
                {
                    List<File> SortedFiles = files;
                    String[] varSet = sortSettings.Split(" ");

                    if (varSet.Count() == 2)
                    {
                        String sortProp = varSet[0];
                        String sortOrder = varSet[1];

                        if (sortProp.ToLower().Contains("sort_name"))
                        {
                            if (sortOrder.ToLower().Contains("desc"))
                            {
                                SortedFiles = files.OrderByDescending(o => o.Name).ToList();
                            }
                            else
                            {
                                SortedFiles = files.OrderBy(o => o.Name).ToList();
                            }
                        }
                        else if (sortProp.ToLower().Contains("sort_time"))
                        {
                            if (sortOrder.ToLower().Contains("desc"))
                            {
                                SortedFiles = files.OrderByDescending(o => o.CreatedAt).ToList();
                            }
                            else
                            {
                                SortedFiles = files.OrderBy(o => o.CreatedAt).ToList();
                            }
                        }
                        else if (sortProp.ToLower().Contains("sort_extension"))
                        {
                            if (sortOrder.ToLower().Contains("desc"))
                            {
                                SortedFiles = files.OrderByDescending(o => o.Extension).ToList();
                            }
                            else
                            {
                                SortedFiles = files.OrderBy(o => o.Extension).ToList();
                            }
                        }
                    }

                    for (int i = 0; i < SortedFiles.Count; i++)
                    {
                        Console.WriteLine("| " + SortedFiles[i].Id + " | " + SortedFiles[i].Name + " | " + SortedFiles[i].Extension + " | " + SortedFiles[i].Description + " | " + SortedFiles[i].CreatedAt.ToString("MM-dd-yyyy HH:mm:ss") + " | " + user.Username + " |");
                    }
                }
                else
                {
                    Console.WriteLine("Warning - empty files");
                    return;
                }

            }
        }

        public void CommandProcess(String command)
        {
            String action = GetFirstWord(command).ToLower();

            switch (action)
            {
                case "register":
                    RegisterUser(command);
                    break;
                case "create_folder":
                    CreateFolder(command);
                    break;
                case "delete_folder":
                    DeleteFolder(command);
                    break;
                case "get_folders":
                    GetFolders(command);
                    break;
                case "rename_folder":
                    RenameFolder(command);
                    break;
                case "upload_file":
                    UploadFile(command);
                    break;
                case "delete_file":
                    DeleteFile(command);
                    break;
                case "get_files":
                    GetFiles(command);
                    break;
                case "show_all_users":
                    for (int i = 0; i < MasterUsers.Count; i++) {
                        Console.WriteLine("| " + MasterUsers[i].Id + " | " +MasterUsers[i].Username +" |");
                    }
                    break;
                case "show_all_folders":
                    for (int i = 0; i < MasterFolders.Count; i++)
                    {
                        Console.WriteLine("| " + MasterFolders[i].Id + " | " + MasterFolders[i].UserId + " | " + MasterFolders[i].Name + " | " + MasterFolders[i].Description + " |");
                    }
                    break;
                case "show_all_files":
                    for (int i = 0; i < MasterFiles.Count; i++)
                    {
                        Console.WriteLine("| " + MasterFiles[i].Id + " | " + MasterFiles[i].FolderId + " | " + MasterFiles[i].Name + " | " + MasterFiles[i].Extension + " | " + MasterFiles[i].Description + " |");
                    }
                    //Console.WriteLine("------------------------------------------------------------------------");
                    break;
                default:
                    Console.WriteLine("Error - undefined command");
                    break;
            }
        }
    }
}
