
import re
import os

input_file = r"D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure\exported_data.sql"
output_file = r"D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure\import_postgres_data.sql"

def to_snake_case(name):
    # Remove brackets
    name = name.replace('[', '').replace(']', '')
    
    # Handle TabPrefix
    if name.startswith("Tab"):
        name = "tab_" + name[3:]
    
    # Convert PascalCase/CamelCase to snake_case
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
    return re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()

def process_file():
    # Detect encoding or try UTF-16
    try:
        with open(input_file, 'r', encoding='utf-16') as f:
            content = f.read()
    except UnicodeError:
        with open(input_file, 'r', encoding='utf-8') as f:
            content = f.read()

    lines = content.split('\n')
    converted = []

    for line in lines:
        line = line.strip()
        if not line:
            continue
            
        # Match INSERT [dbo].[TableName] ([ColList]) VALUES ([ValList])
        # Note: brackets might be missing if user manually edited, but standard SSMS uses them
        match = re.match(r"INSERT\s+(?:\[dbo\]\.)?\[?(\w+)\]?\s+\((.+)\)\s+VALUES\s+\((.+)\)", line, re.IGNORECASE)
        if match:
            table_raw = match.group(1)
            cols_raw = match.group(2)
            vals_raw = match.group(3)
            
            table_name = to_snake_case(table_raw)
            
            # Process columns
            cols = [to_snake_case(c.strip()) for c in cols_raw.split(',')]
            col_list = ", ".join(cols) + ", company_id"
            
            # Process values
            # 1. Remove N prefix: N'...' -> '...'
            vals = re.sub(r"N'(.*?)'", r"'\1'", vals_raw)
            # 2. Append company_id = 1
            val_list = vals.strip() + ", 1"
            
            converted.append(f"INSERT INTO {table_name} ({col_list}) VALUES ({val_list});")
        else:
            # Handle inserts without column list
            match_no_cols = re.match(r"INSERT\s+(?:\[dbo\]\.)?\[?(\w+)\]?\s+VALUES\s+\((.+)\)", line, re.IGNORECASE)
            if match_no_cols:
                table_raw = match_no_cols.group(1)
                vals_raw = match_no_cols.group(2)
                table_name = to_snake_case(table_raw)
                vals = re.sub(r"N'(.*?)'", r"'\1'", vals_raw)
                val_list = vals.strip() + ", 1"
                converted.append(f"INSERT INTO {table_name} VALUES ({val_list});")

    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("\n".join(converted))
    print(f"Success! Converted {len(converted)} lines to {output_file}")

if __name__ == "__main__":
    process_file()
