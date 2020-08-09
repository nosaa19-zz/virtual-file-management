using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UsrControl.Domain;
using UsrControl.View;

namespace UsrControl.Service
{
    class FolderService
    {
        private List<Folder> MasterFolders = new List<Folder>();

        private int InitFolderId = 1000;

        public List<Folder> GetFolders() {
            return MasterFolders;
        }

        public String CreateFolder(String command, List<User> MasterUsers)
        {
            String result = String.Empty;

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
                result = "Error - unknown user";
            }
            else 
            {
                Folder newFolder = MasterFolders.Find(newFolder => newFolder.UserId.Equals(user.Id) && newFolder.Name.ToLower().Equals(folderName.ToLower()));
                if (newFolder != null)
                {
                    result = "Error - folder with user " + user.Username + " already existing";
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
                    result = newFolder.Id;
                }
            }
            return result;

        }

        public String RenameFolder(String command, List<User> MasterUsers)
        {
            String result = String.Empty;

            var regularExpression = new Regex(@"(Rename_Folder)\s(\w+)\s(.+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String newFolderName = match.Groups[4].Value;

            newFolderName = Regex.Replace(newFolderName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                result = "Error - unknown user";
            }
            else {

                Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
                if (folder == null)
                {
                    result = "Error - folder doesn’t exist";
                }
                else
                {
                    if (folder.UserId != user.Id)
                    {
                        result = "Error - folder owner not match";
                    }
                    else 
                    {
                        Folder findFolder = MasterFolders.Find(findFolder => findFolder.UserId.Equals(user.Id) && findFolder.Name.ToLower().Equals(newFolderName.ToLower()));

                        if (findFolder != null)
                        {
                            result = "Error - folder with user " + user.Username + " already existing, please consider another name";
                        }
                        else {
                            folder.Name = newFolderName;
                            result = "Success";
                        }
                    }
                }
            }
            return result;
        }

        public String DeleteFolder(String command, List<User> MasterUsers, List<File> MasterFiles, List<FolderLabel> Folder_Labels)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Delete_Folder)\s(\w+)\s(.+)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                result = "Error - unknown user";
            }
            else {
                Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
                if (folder == null)
                {
                    result = "Error - folder doesn’t exist";
                }
                else
                {
                    if (folder.UserId != user.Id)
                    {
                        result = "Error - folder owner not match";
                    }
                    else 
                    {
                        //Remove Related Data
                        List<File> files = MasterFiles.FindAll(file => file.FolderId.Equals(folder.Id));
                        if (files.Count() > 0)
                        {
                            foreach (File item in files)
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
                        result = "Success";
                    }
                }
            }
            return result;
        }

        public String GetFolders(String command, List<User> MasterUsers, List<FolderLabel> Folder_Labels, List<Label> MasterLabels)
        {
            String result = String.Empty;
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
                result = "Error - unknown user";
            }
            else 
            {
                if (!(labelName.Equals(String.Empty)))
                {
                    label = MasterLabels.Find(label => label.Name.ToLower().Equals(labelName.ToLower()));

                    if (label == null)
                    {
                        return "Error - label is not exists";
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

                    if (label != null)
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
                    Console.Write("------------------------------------------------------------------------");
                }
                else
                {
                    result = "Warning - empty folders";
                }
            }
            return result;
        }


    }
}
