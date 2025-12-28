import json

with open('Set B.json', 'r', encoding='utf-8') as file:
    data = json.load(file)

with open("Characters.json", "r", encoding="utf-8-sig") as f:
    characters = json.load(f)

cards = []

for card in data['cards']:
    cn = card.get('character', {}).get('name', '')
    var = card.get('character', {}).get('variant', '')
    if cn:
        ch = next((c for c in characters if c['name'] == cn), None)
        if ch:
            v = next((v for v in ch['variants'] if v.split(" ")[-1] == var), None)
            print(ch["characterId"], v)
            cards.append({
                "id": card['id'],
                "suit": card['suit'],
                "value": card['number'],
                "characterId": ch["characterId"],
                "characterVariantIndex": ch['variants'].index(v)
            })
            continue
    print("MISSING:", cn, var)

with open('default_decks.json', 'w', encoding='utf-8') as file:
    json.dump([{
        "name": "Official Deck B",
        "author": "SulphurDXD & sblzdddd & Prontological",
        "version": "1.0.0",
        "apiVersion": "1.0.0",
        "description": "官方B卡组的默认玩法",
        "cards": cards
    }], file, ensure_ascii=False, indent=2)
