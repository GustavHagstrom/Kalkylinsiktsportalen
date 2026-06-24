# Öppna frågor och uppföljningspunkter

Det här dokumentet samlar frågor som dök upp under den första repo-analysen.

## Produkt och domän

- Vad är den officiella produktbeskrivningen för Blazor-appen?
- Vilka användartyper finns?
- Vad betyder centrala domänbegrepp som:
  - Bidcon
  - Estimation
  - View
  - Access claim
  - Organization

## Arkitektur

- Var ska gränsen gå mellan `Features/*` och `Shared/*`?
- Finns det en målbild för hur feature-baserad arkitekturen ska bli?

## Säkerhet och konfiguration

- Hur ska secrets hanteras lokalt och i repo?
- Finns det policy för User Secrets, miljövariabler eller annan hemlighetshantering?
- Hur ska credentials för Bidcon lagras och roteras över tid?

## Auth och access

- Vilka claims och roller är officiellt definierade?
- Hur ska development-auth förhålla sig till production-auth långsiktigt?
- Ska alla features ha tydligt dokumenterade policies och accessregler?

## Kvalitet och testning

- Vilken teststrategi är önskad för repoet?
- Ska testtäckningen utökas för stores, auth, features och komponentlogik?
- Vilka delar räknas som mest affärskritiska och bör testas först?

## Dokumentation

- Vem äger dokumentationen?
- När ska docs uppdateras som del av vanligt utvecklingsarbete?
- Behöver repoet en begreppslista eller domänkarta?
