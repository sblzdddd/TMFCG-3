import json
from pathlib import Path
from typing import List, Dict
import lxml.etree as ET
import json

matched = 0
names = []

with open("characters_old.json", "r", encoding="utf-8") as f:
    old_characters = json.load(f)


def _get_text(elem) -> str:
	if elem is None:
		return ""
	# Join all nested text nodes
	try:
		return "".join(elem.itertext()).strip()
	except Exception:
		return (elem.text or "").strip()


def parse_character_item(item, index) -> Dict[str, str]:
	# Name shown in the link box (usually Chinese display name)
	name_el = item.xpath(".//div[contains(@class,'chara-iconbox')]//div[contains(@class,'chara-linkbox')]/a")
	name = _get_text(name_el[0]) if name_el else ""

	if name in names:
		return None
	names.append(name)

	# Japanese/English names inside the info box
	jp_el = item.xpath(".//div[contains(@class,'chara-infobox')]//div[contains(@class,'chara-jpname')]")
	en_el = item.xpath(".//div[contains(@class,'chara-infobox')]//div[contains(@class,'chara-enname')]")
	nick_el = item.xpath(".//div[contains(@class,'chara-infobox')]//div[contains(@class,'chara-nickname')]")

	japanese_name = _get_text(jp_el[0]) if jp_el else ""
	english_name = _get_text(en_el[0]) if en_el else ""
	nickname = _get_text(nick_el[0]) if nick_el else ""

	# First appearance series
	first_el = item.xpath(".//div[contains(@class,'chara-first')]//a")
	first = _get_text(first_el[0]) if first_el else ""

	# Description
	desc_el = item.xpath(".//div[contains(@class,'chara-desc')]")
	description = _get_text(desc_el[0]) if desc_el else ""

	global matched, old_characters
	oldCharacter = next((chara for chara in old_characters if chara["name"] == name), None)
	if oldCharacter is not None:
		matched += 1

	data = {
		"characterId": f'th-{index}',
		"name": name,
		"enName": english_name.split("，"),
		"jpName": japanese_name,
		"nickname": nickname.split("，"),
		"description": description,
		"first": first,
		"variants": []
	}

	if oldCharacter and oldCharacter["image_prefix"] and oldCharacter["images"]:
		for img in oldCharacter["images"]:
			data["variants"].append({
				"image": oldCharacter["image_prefix"] + " " + img,
				"transform": [0.0, 0.0, 1.0]
			})

	return data


def parse_characters_from_html(html_path: Path) -> List[Dict[str, str]]:
	# Force UTF-8 decoding to avoid mojibake from implicit encodings
	text = html_path.read_text(encoding="utf-8")
	root = ET.HTML(text)
	items = root.xpath("//div[contains(@class,'chara-item') and contains(@class,'chara-sub')]")

	characters: List[Dict[str, str]] = []
	for i, item in enumerate(items):
		data = parse_character_item(item, i)
		# Skip empty entries if no name
		if data and any(data.values()) and (data.get("name") or data.get("chinese_name")):
			characters.append(data)
	global old_characters
	fallback_character = old_characters[0]
	fallback_character["characterId"] = "FALLBACK"
	fallback_character["enName"] = fallback_character.get("en_name", "").split("，")
	fallback_character["nickname"] = [fallback_character.get("nickname", "")]
	fallback_character["jpName"] = fallback_character.get("jp_name", "")
	fallback_character["variants"] = [{"image": fallback_character.get("image_prefix", ""), "transform": [0.0, 0.0, 1.0]}]
	del fallback_character["en_name"]
	del fallback_character["jp_name"]
	del fallback_character["image_prefix"]
	del fallback_character["images"]
	characters.insert(0, fallback_character)
	return characters


def main():
	base = Path(__file__).parent
	html_path = base / "character.html"
	# Write to a separate file to avoid overwriting existing schema
	out_path = base / "Characters.json"

	characters = parse_characters_from_html(html_path)
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


if __name__ == "__main__":
	main()




