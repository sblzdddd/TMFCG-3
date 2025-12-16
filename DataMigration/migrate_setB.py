import json

with open('Set B.json', 'r', encoding='utf-8') as file:
    data = json.load(file)


for card in data['cards']:
    cn = card.get('character', {}).get('name', '')
    print(cn)

