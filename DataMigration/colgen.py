from pathlib import Path
from colorthief import ColorThief
import json

def extract_primary_color(image_path):
    """
    Extracts the primary (dominant) color from an image using ColorThief.
    """
    try:
        color_thief = ColorThief(image_path)
        # get_color(quality=1) returns the dominant color. 
        # quality=1 is the highest quality (checks all pixels), higher numbers are faster but less accurate.
        dominant_color = color_thief.get_color(quality=1)
        return dominant_color
    except Exception as e:
        print(f"Error processing {image_path}: {e}")
        return (0, 0, 0)  # Default to black on error

with open("Characters.json", "r", encoding="utf-8-sig") as f:
    characters = json.load(f)

for i, character in enumerate(characters):
    if len(character['variants']) <= 0: continue
    path = f"../Assets/Database/Characters/{character['variants'][0]}.png"
    print(f"Processing {path}...", end="")
    p = extract_primary_color(path)
    print(p)
    characters[i]['primary'] = p
	
base = Path(__file__).parent
out_path = base / "Characters.json"

# Custom pretty-print: objects indented, arrays of primitives inline
def _is_primitive(x):
    return isinstance(x, (str, int, float, bool)) or x is None

def _to_json(value, indent=0, indent_step=2, parent_key: str | None = None):
    space = " " * indent
    if isinstance(value, dict):
        if not value:
            return "{}"
        parts = []
        for i, (k, v) in enumerate(value.items()):
            key = json.dumps(k, ensure_ascii=False)
            rendered = _to_json(v, indent + indent_step, indent_step, parent_key=k)
            parts.append(f"{' ' * (indent + indent_step)}{key}: {rendered}")
        return "{\n" + ",\n".join(parts) + f"\n{space}}}"
    elif isinstance(value, list):
        if not value:
            return "[]"
        # Inline primitive arrays
        if all(_is_primitive(el) for el in value):
            inline = ", ".join(json.dumps(el, ensure_ascii=False) for el in value)
            return f"[{inline}]"
        # Mixed or object lists: multiline with indentation
        parts = []
        for el in value:
            parts.append(f"{' ' * (indent + indent_step)}{_to_json(el, indent + indent_step, indent_step, parent_key=None)}")
        return "[\n" + ",\n".join(parts) + f"\n{space}]"
    else:
        return json.dumps(value, ensure_ascii=False)

output_text = _to_json(characters, indent=0, indent_step=2)
# Write with BOM for better Windows compatibility in some viewers
out_path.write_text(output_text, encoding="utf-8-sig")
