
# 🪄 Ultimate File Renamer (V2.7)

Ein leistungsstarkes, konsolenbasiertes Tool zum automatischen Umbenennen von Dateien.  
Unterstützt Platzhalter, Zahlenfolgen, Buchstaben-Konvertierungen und zeigt eine Vorschau vor der Ausführung.

---

## 🚀 Hauptfunktionen

✅ **Wildcard-Unterstützung (`*`)**
- Beispiel: `bild*.jpg` → `foto*.jpg`  
- Das `*` ersetzt beliebige Zeichenketten.

✅ **Automatische Nummerierung**
- Wenn das neue Muster keine Platzhalter enthält, werden Dateien automatisch durchnummeriert.  
  Beispiel:  
  - Altes Muster: `bild*`  
  - Neues Muster: `foto1`  
  - Ergebnis: `foto1`, `foto2`, `foto3`, …

✅ **Zahlen → Buchstaben-Konvertierung**
- `[a]` oder `[A]` ersetzt Zahlen (1–26) durch Buchstaben.  
  Beispiel:  
  - `bild1` → `foto[a]` → `fotoa`  
  - `bild2` → `foto[A]` → `fotoB`

✅ **Vorschau vor der Ausführung**
- Das Programm zeigt alle geplanten Änderungen an und fragt, ob sie angewendet werden sollen.

✅ **Konfliktfreie Umbenennung**
- Dateien werden zuerst temporär umbenannt (`.tmp`), um Namenskonflikte zu vermeiden.

✅ **Wiederverwendung des Ordners**
- Nach dem Umbenennen kannst du wählen:
  - `R` → gleiches Verzeichnis behalten  
  - `C` → neues Verzeichnis auswählen  
  - `E` → Programm beenden  

---

## 🧱 Projektstruktur

```
renamerIdee/
│
├── Program.cs          # Einstiegspunkt – steuert Hauptlogik & Menü
├── FileRenamer.cs      # Kernlogik für das Umbenennen (Regex, Nummerierung, Buchstaben)
├── UserInterface.cs    # Konsolen-Ein-/Ausgaben (Fragen, Anzeigen, Optionen)
└── README.md           # Diese Dokumentation
```

---

## 📘 Klassenübersicht

### **Program.cs**
- Hauptprogramm – steuert die gesamte Anwendung.  
- Verantwortlich für:
  - Begrüßung und Versionsanzeige  
  - Hauptschleife (R/C/E-Menü)  
  - Übergabe an `UserInterface` und `FileRenamer`
- Hält den aktuellen Ordnerpfad (`currentFolder`) gespeichert, bis der Nutzer ihn ändert.

### **FileRenamer.cs**
- Führt den eigentlichen Umbenennungsprozess durch:
  1. Konvertiert das alte Muster in eine Regex.
  2. Sucht alle passenden Dateien.
  3. Erstellt neue Dateinamen gemäß Muster.
  4. Handhabt Nummerierungen und `[a]/[A]`-Buchstabenfolgen.
  5. Zeigt eine Vorschau an und fragt nach Bestätigung.
  6. Führt die Umbenennung sicher durch.

- Unterstützt:
  - Mehrfache Platzhalter `*`
  - Flexible Muster (Start/Ende nicht zwingend)
  - Sequenzielle Zählung oder Buchstaben-Serien

### **UserInterface.cs**
- Zuständig für alle Interaktionen mit dem Benutzer:
  - `AskFolder()` – fragt den Ordnerpfad ab.  
  - `ShowFiles()` – zeigt alle Dateien im ausgewählten Ordner.  
  - `AskOldPattern()` – fragt das alte Muster ab.  
  - `AskNewPattern()` – fragt das neue Muster ab.  
  - `AskOption()` – fragt nach dem nächsten Schritt (`R`, `C`, `E`).

- Kümmert sich um klare, gut lesbare Konsolen-Ausgaben.

---

## 💡 Beispielanwendung

**Beispielordner:**
```
bild1.jpg
bild2.jpg
bild3.jpg
```

**Eingabe:**
```
🔤 Altes Muster:
bild*

🆕 Neues Muster:
foto*
```

**Ergebnis:**
```
bild1.jpg → foto1.jpg
bild2.jpg → foto2.jpg
bild3.jpg → foto3.jpg
```

---

## 🔁 Steuerung

| Taste | Bedeutung |
|:------|:-----------|
| **R** | Erneut umbenennen (gleicher Ordner bleibt) |
| **C** | Anderen Ordner auswählen |
| **E** | Programm beenden |

---

## ⚙️ Installation & Ausführung

### Voraussetzungen
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) oder höher  
- Windows, macOS oder Linux

### Starten:
```bash
dotnet run
```

Oder kompilieren:
```bash
dotnet build
```
Dann die EXE im `bin/Debug/net6.0/`-Ordner ausführen.

---

## 🧪 Erweiterungsmöglichkeiten

- Unterstützung für mehr Platzhalter (z. B. `?` für ein einzelnes Zeichen)  
- Option für Vorschau ohne Nachfrage  
- Undo-Funktion  
- GUI-Version (WPF oder MAUI)  

---

## 🧑‍💻 Autor

**Ultimate File Renamer V2.7**  
Entwickelt mit ❤️ in C#  
Erstellt von [Deinem Namen hier eintragen]

---

## 📄 Lizenz

Dieses Projekt ist frei nutzbar und kann nach Belieben angepasst oder erweitert werden.  
Keine Haftung für versehentliches Umbenennen oder Datenverlust.
