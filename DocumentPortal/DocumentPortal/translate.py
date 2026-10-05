import os
import re
import xml.etree.ElementTree as ET

terms = {
    "Manage Documents": "Gestionare Documente",
    "Upload Document": "Adăugare Document",
    "Section": "Secțiune",
    "Description": "Descriere",
    "Order Number": "Număr de Ordine",
    "Document Type": "Tip Document",
    "File Upload": "Încărcare Fișier",
    "Text Content": "Conținut Text",
    "File (Word, Excel, PPT, Image)": "Fișier (Word, Excel, Jpeg etc.)",
    "Upload": "Încarcă",
    "Uploaded Documents": "Documente Încărcate",
    "-- All Sections --": "-- Toate Secțiunile --",
    "Filter": "Filtrează",
    "Clear": "Ștergeți Filtrul",
    "File Name / Content Previz.": "Nume Fișier / Previzualizare",
    "File Name": "Nume Fișier",
    "Order": "Ordine",
    "Type": "Tip",
    "Date": "Data",
    "Actions": "Acțiuni",
    "Text Document": "Document Text",
    "No Content": "Fără Conținut",
    "Edit": "Editează",
    "Delete": "Șterge",
    "Delete this document?": "Ștergeți acest document?",
    "Manage Menus": "Gestionare Meniuri",
    "Add New Menu": "Adăugare Meniu Nou",
    "Name": "Nume",
    "Add Menu": "Adaugă Meniu",
    "Existing Menus": "Meniuri Existente",
    "Delete this menu?": "Ștergeți acest meniu?",
    "Manage Sections": "Gestionare Secțiuni",
    "Add New Section": "Adăugare Secțiune Nouă",
    "Menu": "Meniu",
    "Title": "Titlu",
    "Is Private (Requires Login)": "Privat (Membri)",
    "Add Section": "Adaugă Secțiune",
    "Existing Sections": "Secțiuni Existente",
    "Private?": "Privat?",
    "Private": "Privat",
    "Public": "Public",
    "Delete this section?": "Ștergeți această secțiune?",
    "Admin Dashboard": "Panou de Administrare",
    "Manage the main navigation structure.": "Gestionează structura meniului.",
    "Manage": "Gestionează",
    "Manage content sections under menus.": "Gestionează secțiunile de conținut.",
    "Upload and classify documents.": "Upload și clasificare documente.",
    "Users": "Utilizatori",
    "Manage front-end users.": "Gestionează utilizatorii.",
    "Cancel": "Anulați",
    "Save Changes": "Salvați",
    "Edit Document": "Editare Document",
    "Edit Document Metadata": "Modificare Metadate Document",
    "Edit Menu": "Editare Meniu",
    "Edit Section": "Editare Secțiune",
    "Manage Users": "Gestionare Utilizatori",
    "Add New User": "Adaugă Utilizator",
    "Username": "Utilizator",
    "Password": "Parola",
    "Role": "Rol",
    "Add User": "Adaugă Utilizator",
    "Existing Users": "Utilizatori Existenți",
    "Delete this user?": "Ștergeți acest utilizator?",
    "Edit User": "Modificare Utilizator"
}

views_dir = r"c:/Users/belea/.gemini/antigravity/scratch/DocumentPortal/DocumentPortal/Views/Admin"

for filename in os.listdir(views_dir):
    if filename.endswith(".cshtml"):
        filepath = os.path.join(views_dir, filename)
        with open(filepath, "r", encoding="utf-8") as f:
            content = f.read()
        
        for en, ro in terms.items():
            content = content.replace(f"return confirm('{en}');", f"return confirm('@Localizer[\"{en}\"]');")
            content = content.replace(f'ViewData["Title"] = "{en}";', f'ViewData["Title"] = Localizer["{en}"];')
            content = content.replace(f'>{en}<', f'>@Localizer["{en}"]<')
            content = content.replace(f'"{en}"', f'"@Localizer["{en}"]"')

        with open(filepath, "w", encoding="utf-8") as f:
            f.write(content)

def update_resx(file_path, dictionary, is_ro):
    tree = ET.parse(file_path)
    root = tree.getroot()
    existing_keys = {data.attrib['name'] for data in root.findall('data')}
    
    for en, ro in dictionary.items():
        if en not in existing_keys:
            data = ET.SubElement(root, 'data')
            data.set('name', en)
            data.set('xml:space', 'preserve')
            value = ET.SubElement(data, 'value')
            value.text = ro if is_ro else en
            existing_keys.add(en)
    
    tree.write(file_path, encoding='utf-8', xml_declaration=True)

update_resx(r"c:/Users/belea/.gemini/antigravity/scratch/DocumentPortal/DocumentPortal/Resources/SharedResource.en.resx", terms, False)
update_resx(r"c:/Users/belea/.gemini/antigravity/scratch/DocumentPortal/DocumentPortal/Resources/SharedResource.ro.resx", terms, True)

print("Done")
