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
        private UserService userService = new UserService();
        private FolderService folderService = new FolderService();
        private FileService fileService = new FileService();
        private LabelService labelService = new LabelService();
        private FolderLabelService folderLabelService = new FolderLabelService();

        public String CommandProcess(String command)
        {
            String result = String.Empty;
            String action = GetFirstWord(command).ToLower();

            switch (action)
            {
                case "register":
                    result = userService.RegisterUser(command);
                    break;
                case "create_folder":
                    result = folderService.CreateFolder(command, userService.GetUsers());
                    break;
                case "delete_folder":
                    result = folderService.DeleteFolder(command, userService.GetUsers(), fileService.GetFiles(), folderLabelService.GetFolderLabels());
                    break;
                case "get_folders":
                    result = folderService.GetFolders(command, userService.GetUsers(), folderLabelService.GetFolderLabels(), labelService.GetLabels());
                    break;
                case "rename_folder":
                    result = folderService.RenameFolder(command, userService.GetUsers());
                    break;
                case "upload_file":
                    result = fileService.UploadFile(command, userService.GetUsers(), folderService.GetFolders());
                    break;
                case "delete_file":
                    result = fileService.DeleteFile(command, userService.GetUsers(), folderService.GetFolders());
                    break;
                case "get_files":
                    result = fileService.GetFiles(command, userService.GetUsers(), folderService.GetFolders());
                    break;
                case "add_label":
                    result = labelService.AddLabel(command, userService.GetUsers());
                    break;
                case "get_labels":
                    result = labelService.GetLabels(command, userService.GetUsers());
                    break;
                case "delete_label":
                    result = labelService.DeleteLabel(command, userService.GetUsers(), folderLabelService.GetFolderLabels());
                    break;
                case "add_folder_label":
                    result = folderLabelService.AddFolderLabel(command, userService.GetUsers(), labelService.GetLabels(), folderService.GetFolders());
                    break;
                case "delete_folder_label":
                    result = folderLabelService.DeleteFolderLabel(command, userService.GetUsers(), labelService.GetLabels(), folderService.GetFolders());
                    break;
                case "show_all_users":
                    Console.WriteLine("------------------------------------------------------------------------");
                    for (int i = 0; i < userService.GetUsers().Count; i++)
                    {
                        Console.WriteLine("| " + userService.GetUsers()[i].Id + " | " + userService.GetUsers()[i].Username + " |");
                    }
                    Console.Write("------------------------------------------------------------------------");
                    break;
                case "show_all_folders":
                    Console.WriteLine("------------------------------------------------------------------------");
                    for (int i = 0; i < folderService.GetFolders().Count; i++)
                    {
                        Console.WriteLine("| " + folderService.GetFolders()[i].Id + " | " + folderService.GetFolders()[i].UserId + " | " + folderService.GetFolders()[i].Name + " | " + folderService.GetFolders()[i].Description + " |");
                    }
                    Console.Write("------------------------------------------------------------------------");
                    break;
                case "show_all_files":
                    Console.WriteLine("------------------------------------------------------------------------");
                    for (int i = 0; i < fileService.GetFiles().Count; i++)
                    {
                        Console.WriteLine("| " + fileService.GetFiles()[i].Id + " | " + fileService.GetFiles()[i].FolderId + " | " + fileService.GetFiles()[i].Name + " | " + fileService.GetFiles()[i].Extension + " | " + fileService.GetFiles()[i].Description + " |");
                    }
                    Console.Write("------------------------------------------------------------------------");
                    break;
                case "show_all_labels":
                    Console.WriteLine("------------------------------------------------------------------------");
                    for (int i = 0; i < labelService.GetLabels().Count; i++)
                    {
                        Console.WriteLine("| " + labelService.GetLabels()[i].Id + " | " + labelService.GetLabels()[i].UserId + " | " + labelService.GetLabels()[i].Name + " | " + labelService.GetLabels()[i].Color + " |");
                    }
                    Console.Write("------------------------------------------------------------------------");
                    break;
                case "show_all_folder_labels":
                    Console.WriteLine("------------------------------------------------------------------------");
                    for (int i = 0; i < folderLabelService.GetFolderLabels().Count; i++)
                    {
                        Console.WriteLine("| " + folderLabelService.GetFolderLabels()[i].Id + " | " + folderLabelService.GetFolderLabels()[i].FolderId + " | " + folderLabelService.GetFolderLabels()[i].LabelId + " |");
                    }
                    Console.Write("------------------------------------------------------------------------");
                    break;
                default:
                    result = "Error - undefined command";
                    break;
            }

            return result;
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
    }
}
