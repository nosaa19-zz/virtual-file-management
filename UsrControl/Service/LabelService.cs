using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UsrControl.Domain;

namespace UsrControl.Service
{
    class LabelService
    {
        private List<Label> MasterLabels = new List<Label>();

        public List<Label> GetLabels() {
            return MasterLabels;
        }

        public String AddLabel(String command, List<User> MasterUsers)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Add_Label)\s(\w+)\s(['`‘].+['`’])\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String labelName = match.Groups[3].Value;
            String labelColor = match.Groups[4].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");
            labelColor = Regex.Replace(labelColor, "^['`‘]|['`‘]$", "");

            if (labelColor.Equals(String.Empty))
            {
                result = "Error - Please defined color!";
            }
            else {
                User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
                if (user == null)
                {
                    result = "Error - unknown user";
                }
                else 
                {
                    Label newLabel = MasterLabels.Find(newLabel => newLabel.UserId.Equals(user.Id) && newLabel.Name.ToLower().Equals(labelName.ToLower()));
                    if (newLabel != null)
                    {
                        result = "Error - label with user " + user.Username + " already existing";
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
                        result = "Success";
                    }

                }
            }
            return result;
        }

        public String DeleteLabel(String command, List<User> MasterUsers, List<FolderLabel> Folder_Labels)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Delete_Label)\s(\w+)\s(['`‘].+['`’])", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String labelName = match.Groups[3].Value;

            labelName = Regex.Replace(labelName, "^['`‘]|['`‘]$", "");

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                result = "Error - unknown user";
            }
            else 
            {
                Label label = MasterLabels.Find(label => label.Name.ToLower() == labelName.ToLower());
                if (label == null)
                {
                    result = "Error - label name doesn’t exist";
                }
                else
                {
                    if (label.UserId != user.Id)
                    {
                        result = "Error - label owner not match";
                    }
                    else 
                    {
                        //Remove Related Data
                        List<FolderLabel> folderLabels = Folder_Labels.FindAll(o => o.LabelId == label.Id);

                        if (folderLabels.Count() > 0)
                        {
                            foreach (FolderLabel item in folderLabels)
                            {
                                Folder_Labels.Remove(item);
                            }
                        }

                        MasterLabels.Remove(label);
                        result = "Success";
                    }
                }
            }
            return result;
        }

        public String GetLabels(String command, List<User> MasterUsers)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Get_Labels)\s(\w+)\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String sortSettings = match.Groups[3].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                result = "Error - unknown user";
            }
            else 
            {
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
                    Console.Write("------------------------------------------------------------------------");
                }
                else
                {
                    result = "Warning - empty labels";
                }
            }
            return result;
        }
    }
}
