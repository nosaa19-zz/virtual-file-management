using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using UsrControl.Domain;
using UsrControl.View;

namespace UsrControl.Service
{
    class MainController
    {
        private List<User> MasterUsers = new List<User>();
        private List<Folder> MasterFolders = new List<Folder>();
        private List<File> MasterFiles = new List<File>();
        private List<Label> MasterLabels = new List<Label>();
        private List<FolderLabel> Folder_Labels = new List<FolderLabel> ();
        private int InitFolderId = 1000;

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
                case "add_label":
                    AddLabel(command);
                    break;
                case "get_labels":
                    GetLabels(command);
                    break;
                case "delete_label":
                    DeleteLabel(command);
                    break;
                case "add_folder_label":
                    AddFolderLabel(command);
                    break;
                case "delete_folder_label":
                    DeleteFolderLabel(command);
                    break;
                case "show_all_users":
                    for (int i = 0; i < MasterUsers.Count; i++)
                    {
                        Console.WriteLine("| " + MasterUsers[i].Id + " | " + MasterUsers[i].Username + " |");
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
                    break;
                case "show_all_labels":
                    for (int i = 0; i < MasterLabels.Count; i++)
                    {
                        Console.WriteLine("| " + MasterLabels[i].Id + " | " + MasterLabels[i].UserId + " | " + MasterLabels[i].Name + " | " + MasterLabels[i].Color + " |");
                    }
                    break;
                case "show_all_folder_labels":
                    for (int i = 0; i < Folder_Labels.Count; i++)
                    {
                        Console.WriteLine("| " + Folder_Labels[i].Id + " | " + Folder_Labels[i].FolderId + " | " + Folder_Labels[i].LabelId + " |");
                    }
                    break;
                default:
                    Console.WriteLine("Error - undefined command");
                    break;
            }
        }

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
            var regularExpression = new Regex(@"(Get_Folders)\s(\w+)\s?(['`‘].+['`’])?\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String labelName = match.Groups[3].Value;
            String sortSettings = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");
            Label label = null;

            User user = MasterUsers.Find(user => user.Username.ToLower().Equals(username.ToLower()));
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            if (!(labelName.Equals(String.Empty))) 
            {
                label = MasterLabels.Find(label => label.Name.ToLower().Equals(labelName.ToLower()));

                if (label == null)
                {
                    Console.WriteLine("Error - label is not exists");
                    return;
                }
            }

            List<FolderLabelView> flViews = (from fl in Folder_Labels
                                             join l in MasterLabels on fl.LabelId equals l.Id
                                             select new FolderLabelView
                                             {
                                                 Id = fl.Id,
                                                 FolderId = fl.FolderId,
                                                 LabelId = fl.LabelId,
                                                 LabelName = l.Name,
                                                 Color = l.Color
                                             }).ToList<FolderLabelView>();

            List<Folder> folders = MasterFolders.FindAll(folder => folder.UserId.Equals(user.Id));

            List<UserFolderLabelView> uflViews = new List<UserFolderLabelView>();

            if (folders.Count() > 0)
            {
                uflViews = (from f in folders
                            join flv in flViews on f.Id equals flv.FolderId into temp
                            from flv in temp.DefaultIfEmpty()
                            select new UserFolderLabelView
                            {
                                Id = f.Id,
                                Username = user.Username,
                                Name = f.Name,
                                Label = flv == null ? String.Empty : flv.LabelName,
                                Description = f.Description,
                                CreatedAt = f.CreatedAt
                            }).ToList<UserFolderLabelView>();
            }

            if (uflViews.Count() > 0)
            {
                List<UserFolderLabelView> SortedUserFolderLabelView = uflViews;

                if(label != null)
                {
                    SortedUserFolderLabelView = uflViews.Where(p => p.Label.Equals(label.Name)).ToList();
                }
                String[] varSet = sortSettings.Split(" ");

                if (varSet.Count() == 2)
                {
                    String sortProp = varSet[0];
                    String sortOrder = varSet[1];

                    if (sortProp.ToLower().Contains("sort_name"))
                    {
                        if (sortOrder.ToLower().Contains("desc"))
                        {
                            SortedUserFolderLabelView = SortedUserFolderLabelView.OrderByDescending(o => o.Name).ToList();
                        }
                        else
                        {
                            SortedUserFolderLabelView = SortedUserFolderLabelView.OrderBy(o => o.Name).ToList();
                        }
                    }
                    else if (sortProp.ToLower().Contains("sort_time"))
                    {
                        if (sortOrder.ToLower().Contains("desc"))
                        {
                            SortedUserFolderLabelView = SortedUserFolderLabelView.OrderByDescending(o => o.CreatedAt).ToList();
                        }
                        else
                        {
                            SortedUserFolderLabelView = SortedUserFolderLabelView.OrderBy(o => o.CreatedAt).ToList();
                        }
                    }
                }

                Console.WriteLine("------------------------------------------------------------------------");
                for (int i = 0; i < SortedUserFolderLabelView.Count; i++)
                {
                    Console.WriteLine("| " + SortedUserFolderLabelView[i].Id + " | " + SortedUserFolderLabelView[i].Label + " | " + SortedUserFolderLabelView[i].Name + " | " + SortedUserFolderLabelView[i].Description + " | " + SortedUserFolderLabelView[i].CreatedAt.ToString("MM-dd-yyyy HH:mm:ss") + " | " + user.Username + " |");
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

                //Remove Related Data
                List<File> files = MasterFiles.FindAll(file => file.FolderId.Equals(folder.Id));
                if (files.Count() > 0)
                { 
                    foreach(File item in files)
                    {
                        MasterFiles.Remove(item);
                    }
                }

                List<FolderLabel> folderLabels = Folder_Labels.FindAll(o => o.FolderId == folder.Id);

                if (folderLabels.Count() > 0)
                {
                    foreach (FolderLabel item in folderLabels)
                    {
                        Folder_Labels.Remove(item);
                    }
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

        private void AddLabel(String command) {
            var regularExpression = new Regex(@"(Add_Label)\s(\w+)\s(['`‘].+['`’])\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String labelName = match.Groups[3].Value;
            String labelColor = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");
            labelColor = Regex.Replace(labelColor, "^['`‘]|['`‘]$", "");

            if (labelColor.Equals(String.Empty))
            {
                Console.WriteLine("Error - Please defined color!");
                return;
            }

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Label newLabel = MasterLabels.Find(newLabel => newLabel.UserId.Equals(user.Id) && newLabel.Name.ToLower().Equals(labelName.ToLower()));
            if (newLabel != null)
            {
                Console.WriteLine("Error - label with user " + user.Username + " already existing");
                return;
            }
            else
            {
                newLabel = new Label();
                newLabel.Id = Guid.NewGuid().ToString();
                newLabel.UserId = user.Id;
                newLabel.Name = labelName;
                newLabel.Color = labelColor;
                newLabel.CreatedAt = DateTime.Now;

                MasterLabels.Add(newLabel);
                Console.WriteLine("Success");
            }
        }

        private void DeleteLabel(String command) {
            var regularExpression = new Regex(@"(Delete_Label)\s(\w+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String labelName = match.Groups[3].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Label label = MasterLabels.Find(label => label.Name.ToLower() == labelName.ToLower());
            if (label == null)
            {
                Console.WriteLine("Error - label name doesn’t exist");
                return;
            }
            else
            {
                if (label.UserId != user.Id)
                {
                    Console.WriteLine("Error - label owner not match");
                    return;
                }

                //Remove Related Data
                List<FolderLabel> folderLabels = Folder_Labels.FindAll(o => o.LabelId == label.Id);

                if (folderLabels.Count() > 0) {
                    foreach (FolderLabel item in folderLabels) 
                    {
                        Folder_Labels.Remove(item);
                    }
                }

                MasterLabels.Remove(label);

                Console.WriteLine("Success");
            }
        }

        private void GetLabels(String command) {
            var regularExpression = new Regex(@"(Get_Labels)\s(\w+)\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String sortSettings = match.Groups[3].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            List<Label> labels = MasterLabels.FindAll(label => label.UserId.Equals(user.Id));
            if (labels.Count() > 0)
            {
                List<Label> SortedLabel = labels;
                String[] varSet = sortSettings.Split(" ");

                if (varSet.Count() == 2)
                {
                    String sortProp = varSet[0];
                    String sortOrder = varSet[1];

                    if (sortProp.ToLower().Contains("sort_name"))
                    {
                        if (sortOrder.ToLower().Contains("desc"))
                        {
                            SortedLabel = labels.OrderByDescending(o => o.Name).ToList();
                        }
                        else
                        {
                            SortedLabel = labels.OrderBy(o => o.Name).ToList();
                        }
                    }
                    else if (sortProp.ToLower().Contains("sort_time"))
                    {
                        if (sortOrder.ToLower().Contains("desc"))
                        {
                            SortedLabel = labels.OrderByDescending(o => o.CreatedAt).ToList();
                        }
                        else
                        {
                            SortedLabel = labels.OrderBy(o => o.CreatedAt).ToList();
                        }
                    }
                }

                Console.WriteLine("------------------------------------------------------------------------");
                for (int i = 0; i < SortedLabel.Count; i++)
                {
                    Console.WriteLine("| " + SortedLabel[i].Id + " | " + SortedLabel[i].Name + " | " + SortedLabel[i].Color + " | " + SortedLabel[i].CreatedAt.ToString("MM-dd-yyyy HH:mm:ss") + " | " + user.Username + " |");
                }
                Console.WriteLine("------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Warning - empty labels");
                return;
            }

        }

        private void AddFolderLabel(String command) {
            var regularExpression = new Regex(@"(Add_Folder_Label)\s(\w+)\s([0-9]+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String labelName = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            
            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Label label = MasterLabels.Find(label => label.UserId.Equals(user.Id) && label.Name.ToLower().Equals(labelName.ToLower()));
            if (label == null)
            {
                Console.WriteLine("Error - label name not exist");
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

                FolderLabel newFolderLabel = Folder_Labels.Find(newFolderLabel => newFolderLabel.FolderId.Equals(folder.Id) && newFolderLabel.LabelId.Equals(label.Id));
                if (newFolderLabel != null)
                {
                    Console.WriteLine("Error - Folder " + folder.Name + " with User " + user.Username + " already labeled with Label "+label.Name);
                    return;
                }
                else
                {
                    newFolderLabel = new FolderLabel();
                    newFolderLabel.Id = Guid.NewGuid().ToString();
                    newFolderLabel.FolderId = folder.Id;
                    newFolderLabel.LabelId = label.Id;

                    Folder_Labels.Add(newFolderLabel);
                    Console.WriteLine("Success");
                }

            }

        }

        private void DeleteFolderLabel(String command) {
            var regularExpression = new Regex(@"(Delete_Folder_Label)\s(\w+)\s([0-9]+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String labelName = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                Console.WriteLine("Error - unknown user");
                return;
            }

            Label label = MasterLabels.Find(label => label.UserId.Equals(user.Id) && label.Name.ToLower().Equals(labelName.ToLower()));
            if (label == null)
            {
                Console.WriteLine("Error - label name not exist");
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

                FolderLabel folderLabel = Folder_Labels.Find(folderLabel => folderLabel.FolderId.Equals(folder.Id) && folderLabel.LabelId.Equals(label.Id));
                if (folderLabel == null)
                {
                    Console.WriteLine("Error - folder-label doesn’t exist");
                    return;
                }
                else
                {
                    Folder_Labels.Remove(folderLabel);
                    Console.WriteLine("Success");
                }

            }
        }

    }
}
