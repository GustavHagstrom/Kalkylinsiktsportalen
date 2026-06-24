# ADR — Architecture Decision Records

ADR:er används för att dokumentera viktiga tekniska beslut som är dyra att upptäcka i efterhand.

## När en ADR bör skrivas

Skriv en ADR när beslutet påverkar något av följande:
- arkitekturens riktning
- data- eller auth-modell
- val av integrationsmönster
- uppdelning mellan features och shared
- persistensstrategi
- val som är svåra att backa ur senare

## Enkel mall

Skapa en fil med namn enligt mönstret:
- `0001-kort-beskrivning.md`
- `0002-kort-beskrivning.md`

Använd gärna denna struktur:

### Titel
Kort och tydlig rubrik.

### Status
Exempel:
- Proposed
- Accepted
- Superseded

### Kontext
Vilket problem försöker vi lösa?

### Beslut
Vad bestämmer vi?

### Konsekvenser
Vad blir enklare, svårare eller viktigare framåt?

### Alternativ som övervägdes
Vilka andra vägar fanns?

## Förslag på första ADR:er i detta repo

- Varför huvudapplikationen är feature-baserad
- Hur `Shared` ska användas och begränsas
- Strategi för auth i development respektive production
- Strategi för hemlighetshantering och konfiguration
