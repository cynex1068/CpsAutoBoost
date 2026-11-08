# CPS AUTO BOOST

[English](README-EN.md) | **Deutsch**

**CPS AUTO BOOST** ist ein Windows-Desktop-Tool, das die Mausklickausgabe basierend auf echten physischen Klicks pro Sekunde automatisch erhöht.

Es überwacht global die physischen Klicks der linken und rechten Maustaste, berechnet die reale CPS für jede Taste und sendet zusätzliche Mauseingaben, sobald die konfigurierte Aktivierungsbedingung erreicht ist.

## Funktionen

- Globale Überwachung der linken und rechten Maustaste
- Automatische Klickverstärkung über die Windows-Eingabe-API
- Die reale CPS wird ausschließlich aus physischen Klicks berechnet
- Simulierte Klicks werden bei der CPS-Berechnung ausgeschlossen
- Simulierte Klicks können keinen weiteren Verstärkungszyklus auslösen
- Konfigurierbare Aktivierungsbedingungen für linke und rechte Taste
- Konfigurierbare Multiplikatoren für linke und rechte Taste
- Benutzeroberfläche in Englisch, Französisch, Deutsch, Russisch, Ukrainisch, Chinesisch und Japanisch
- Standardmäßige Erkennung der Systemsprache
- Nicht unterstützte Systemsprachen fallen auf Englisch zurück
- Erstellung einer eigenständigen, einteiligen Windows-Executable

## Standardeinstellungen

| Taste | Aktivierungsbedingung | Standard-Multiplikator | Standardergebnis |
| --- | ---: | ---: | ---: |
| Links | `x >= 5` | `2x` | `y = 2x` |
| Rechts | `x >= 4` | `3x` | `y = 3x` |

Die Aktivierungsbedingungen und Multiplikatoren können direkt in der Anwendung geändert werden.

## Berechnung

`x` steht für die reale CPS, die aus physischen Mausklicks gemessen wird.

Für die linke Taste:

```text
if x >= left activation condition:
    y = x * left multiplier
else:
    y = x
```

Für die rechte Taste:

```text
if x >= right activation condition:
    y = x * right multiplier
else:
    y = x
```

Die Anwendung sendet nur die zusätzlichen Klicks, die zur Erreichung der Zielrate erforderlich sind. Sie fügt nicht für jeden physischen Klick eine vollständige zusätzliche Sekunde an Klicks hinzu.

## Sicherheit der CPS-Messung

Automatisch erzeugte Klicks werden markiert und durch den globalen Maus-Hook herausgefiltert. Während automatische Eingaben gesendet werden, unterdrückt die Anwendung diese Ereignisse für die realen CPS-Zähler. Dadurch wird verhindert, dass simulierte Klicks den Wert `x` erhöhen oder eine rekursive Beschleunigung auslösen.

## Voraussetzungen

- Windows 10 oder neuer
- .NET 8 SDK zum Erstellen aus dem Quellcode
- x64-Windows-System

Die veröffentlichte Executable ist eigenständig und erfordert keine separat installierte .NET-Runtime.

## Aus dem Quellcode erstellen

Installieren Sie das .NET 8 SDK, öffnen Sie PowerShell im Projektverzeichnis und führen Sie aus:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\build.ps1
```

Die erzeugte Executable steht anschließend unter folgendem Pfad zur Verfügung:

```text
publish\WindowsCpsTool.exe
```

## Verwendung

1. Starten Sie `CPS AUTO BOOST`.
2. Wählen Sie eine Sprache aus oder behalten Sie `System` bei, um der Windows-Sprache zu folgen.
3. Legen Sie die Aktivierungsbedingungen für links und rechts fest.
4. Legen Sie die Multiplikatoren für links und rechts fest.
5. Aktivieren Sie die automatische Verstärkung.
6. Deaktivieren Sie die automatische Verstärkung, bevor Sie zu Anwendungen wechseln, in denen keine automatische Mauseingabe gewünscht ist.

CPS AUTO BOOST sendet Mauseingaben erst, nachdem die Funktion aktiviert wurde. Beim Schließen der Anwendung wird der globale Maus-Hook entfernt und die automatische Eingabe gestoppt.

## Lizenz

Dieses Projekt ist unter der **MIT-Lizenz** lizenziert.
