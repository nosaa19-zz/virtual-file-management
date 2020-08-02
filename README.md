# virtual-file-management
Programming Language: C# 8.0
Development Tool	: Visual Studio 2019 Community Edition

## Requirement:
Please download and Install the latest (recommended) .Net Core
https://dotnet.microsoft.com/download/dotnet-core

## How to Run:
open cmd on in Folder 'virtual-file-management'
cd to ./UsrControl 
type 'dotnet run'

## Available Command:
* register {username}
* create_folder {username} {folder_name} {description}
* delete_folder {username} {folder_id}
* get_folders {username} {sort_name | sort_time} {asc|dsc}
* rename_folders {username} {folder_id} {new_folder_name}
* upload_file {username} {folder_id} {file_name} {description}
* delete_file {username} {folder_id} {file_name}
* get_files {username} {folder_id} {sort_name|sort_time|sort_extension} {asc|dsc}

## +Addition Command
* add_label {username} {label_name} {color}
* get_labels {username}
* delete_labels {username} {label_name}
* add_folder_label {username} {folder_id} {label_name}
* delete_folder_label {username} {folder_id} {label_name}
* get_folders {username} {label_name} {sort_name | sort_time} {asc|dsc}


### Example command
- register user1
- create_folder user1 ‘Work’ ‘The working files and necessary files are here’
- get_folders user1
- rename_folder user1 1001 ‘Temp’
- delete_folder user1 1001
- create_folder user1 ‘Testing’ ‘The testing folders’
- upload_file user1 1002 ‘1.tc’ ‘first test case for a company’
- upload_file user1 1002 ‘1.png’ ‘the picture for first test case’
- get_files user1 1002 sort_extension acs
- delete_file user1 1002 1.png

### +Addition Example command
- add_label user1 ‘Work’ ‘blue’
- get_labels user1
- delete_label user1 ‘Temp’
- add_folder_label user1 1001 ‘Work’
- delete_folder_label user1 1001 ‘Work’
- get_folders user1 ‘Temp’ sort_name asc

## GOD COMMAND
* show_all_users
* show_all_folders
* show_all_labels
* show_all_folder_labels