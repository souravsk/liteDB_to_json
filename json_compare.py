import os
import json
import shutil
import datetime

current_json_dir = 'current'
tmp_dir = 'previous'
output_zip = f'zip_file'

os.makedirs(tmp_dir, exist_ok=True)
os.makedirs(output_zip,exist_ok=True)
previous_json_dir = tmp_dir

def load_json_file(file_path):
    with open(file_path, 'r') as f:
        return json.load(f)

def save_json_file(data, file_path):
    with open(file_path, 'w') as f:
        json.dump(data, f, indent=2)

import zipfile

def zip_folder(folder_path, output_path):
    if not os.path.exists(folder_path):
        print(f"Folder '{folder_path}' does not exist.")
        return
    with zipfile.ZipFile(output_path, 'w') as zipf:
        for root, _, files in os.walk(folder_path):
            for file in files:
                file_path = os.path.join(root, file)
                zipf.write(file_path, os.path.relpath(file_path, folder_path))

    print(f"Folder '{folder_path}' has been zipped to '{output_path}'.")

custom_zip_name = f'{output_zip}/gameData_{datetime.datetime.now().date()}.zip'

for filename in os.listdir(current_json_dir):
    current_file_path = os.path.join(current_json_dir, filename)
    previous_file_path = os.path.join(previous_json_dir, filename)
    if not os.path.exists(previous_file_path):
        print(f"Adding {filename} to previous directory")
        shutil.copy(current_file_path, previous_file_path)
    else:
        print(f"Loading {filename} from previous directory")
        prev_json = load_json_file(previous_file_path)
        try:
            curr_json = load_json_file(current_file_path)
        except:
            continue
        for item in curr_json:
            if item not in prev_json:
                shutil.copy(current_file_path, previous_file_path)
                break

        save_json_file(prev_json, previous_file_path)

zip_folder(previous_json_dir, custom_zip_name)