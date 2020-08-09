using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UsrControl.Domain;

namespace UsrControl.Service
{
    class FolderLabelService
    {
        private List<FolderLabel> Folder_Labels = new List<FolderLabel>();

        public List<FolderLabel> GetFolderLabels() {
            return Folder_Labels;
        }

        public String AddFolderLabel(String command, List<User> MasterUsers, List<Label> MasterLabels, List<Folder> MasterFolders)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Add_Folder_Label)\s(\w+)\s([0-9]+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String labelName = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                result = "Error - unknown user";
            }
            else 
            {
                Label label = MasterLabels.Find(label => label.UserId.Equals(user.Id) && label.Name.ToLower().Equals(labelName.ToLower()));
                if (label == null)
                {
                    result = "Error - label name not exist";
                }
                else 
                {
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
                            FolderLabel newFolderLabel = Folder_Labels.Find(newFolderLabel => newFolderLabel.FolderId.Equals(folder.Id) && newFolderLabel.LabelId.Equals(label.Id));
                            if (newFolderLabel != null)
                            {
                                result = "Error - Folder " + folder.Name + " with User " + user.Username + " already labeled with Label " + label.Name;
                            }
                            else
                            {
                                newFolderLabel = new FolderLabel();
                                newFolderLabel.Id = Guid.NewGuid().ToString();
                                newFolderLabel.FolderId = folder.Id;
                                newFolderLabel.LabelId = label.Id;

                                Folder_Labels.Add(newFolderLabel);
                                result = "Success";
                            }
                        }
                    }
                }
            }
            return result;
        }

        public String DeleteFolderLabel(String command, List<User> MasterUsers, List<Label> MasterLabels, List<Folder> MasterFolders)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Delete_Folder_Label)\s(\w+)\s([0-9]+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String labelName = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                result = "Error - unknown user";
            }
            else 
            {
                Label label = MasterLabels.Find(label => label.UserId.Equals(user.Id) && label.Name.ToLower().Equals(labelName.ToLower()));
                if (label == null)
                {
                    result = "Error - label name not exist";
                }
                else 
                {
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
                            FolderLabel folderLabel = Folder_Labels.Find(folderLabel => folderLabel.FolderId.Equals(folder.Id) && folderLabel.LabelId.Equals(label.Id));
                            if (folderLabel == null)
                            {
                                result = "Error - folder-label doesn’t exist";
                            }
                            else
                            {
                                Folder_Labels.Remove(folderLabel);
                                result = "Success";
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
