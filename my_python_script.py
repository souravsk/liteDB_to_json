import os
import json
import shutil

current_json_dir = 'current'
tmp_dir = 'previous'

os.makedirs(tmp_dir, exist_ok=True)
previous_json_dir = tmp_dir

def load_json_file(file_path):
    with open(file_path, 'r') as f:
        return json.load(f)

def save_json_file(data, file_path):
    with open(file_path, 'w') as f:
        json.dump(data, f, indent=2)

def dict_to_tuple(d):
    return tuple(sorted(d.items()))

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
                print(f"Found difference: {item}")
                prev_json.append(item)

        save_json_file(prev_json, previous_file_path)