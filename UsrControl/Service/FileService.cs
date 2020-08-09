using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UsrControl.Domain;

namespace UsrControl.Service
{
    class FileService
    {
        private List<File> MasterFiles = new List<File>();

        public List<File> GetFiles() {
            return MasterFiles;
        }

        public String UploadFile(String command, List<User> MasterUsers, List<Folder> MasterFolders)
        {
            String result = String.Empty;
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
                result = "Error - unknown user";
            }
            else {
                Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
                if (folder == null)
                {
                    result = "Error - folder_id not found";
                }
                else
                {
                    if (folder.UserId != user.Id)
                    {
                        result = "Error - folder owner not match";
                    }
                    else 
                    {
                        File newFile = MasterFiles.Find(newFile => newFile.FolderId.Equals(folder.Id) && newFile.Name.ToLower().Equals(fileName.ToLower()));
                        if (newFile != null)
                        {
                            result = "Error - file in Folder " + folder.Name + " with User " + user.Username + " already existing";
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
                            result ="Success";
                        }
                    }
                }
            }

            return result;
        }

        public String DeleteFile(String command, List<User> MasterUsers, List<Folder> MasterFolders)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Delete_File)\s(\w+)\s([0-9]+)\s(\w+\.\w+)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String fileName = match.Groups[4].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                result = "Error - unknown user";
            }
            else {
                Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
                if (folder == null)
                {
                    result = "Error - folder_id not found";
                }
                else
                {
                    if (folder.UserId != user.Id)
                    {
                        result = "Error - folder owner not match";
                    }
                    else 
                    {
                        File file = MasterFiles.Find(file => file.FolderId.Equals(folder.Id) && file.Name.ToLower().Equals(fileName.ToLower()));
                        if (file == null)
                        {
                            result = "Error - file doesn’t exist";
                        }
                        else
                        {
                            MasterFiles.Remove(file);
                            result = "Success";
                        }
                    }
                }
            }
            return result;
        }

        public String GetFiles(String command, List<User> MasterUsers, List<Folder> MasterFolders)
        {
            String result = String.Empty;
            var regularExpression = new Regex(@"(Get_Files)\s(\w+)\s([0-9]+)\s?(.*)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;
            String folderId = match.Groups[3].Value;
            String sortSettings = match.Groups[4].Value;

            User user = MasterUsers.Find(user => user.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                result = "Error - unknown user";
            }
            else 
            {
                Folder folder = MasterFolders.Find(folder => folder.Id == folderId);
                if (folder == null)
                {
                    result = "Error - folder_id not found";
                }
                else
                {
                    if (folder.UserId != user.Id)
                    {
                        result = "Error - folder owner not match";
                    }
                    else 
                    {
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
                            Console.WriteLine("------------------------------------------------------------------------");
                            for (int i = 0; i < SortedFiles.Count; i++)
                            {
                                Console.WriteLine("| " + SortedFiles[i].Id + " | " + SortedFiles[i].Name + " | " + SortedFiles[i].Extension + " | " + SortedFiles[i].Description + " | " + SortedFiles[i].CreatedAt.ToString("MM-dd-yyyy HH:mm:ss") + " | " + user.Username + " |");
                            }
                            Console.Write("------------------------------------------------------------------------");
                        }
                        else
                        {
                            result = "Warning - empty files";
                        }
                    }
                }
            }
            return result;
        }
    }
}
