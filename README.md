![Invata Linux de la zero](https://github.com/BTF2021/Zero2Linux/blob/unstable/githubassets/Banner.gif)
Zero2Linux este un proiect personal, creat pentru a-i invata pe cei mai putini familiarizati cu ecosistemul Linux sa foloseasca un desktop Linux.

Dezvoltat in Godot folosind C#

# Platforme
## Windows

  Windows 10 si 11, pe arhitectura x86_64 (64 biti)
## Linux

  Arhitectura x86_64 (64 biti)

  Serverul de display recomandat: X11

## Android

  Android 10 sau mai nou, pe arhitectura arm64_v8a (arm 64 biti)

# FAQ
> **In pagina de Release sunt 2 versiuni. Care este diferenta dintre Full si Lite?**

  In Lite este doar executabilul si folderul pentru .NET. In Full sunt toate fisierele din Lite plus fisiere statice FFmpeg (cu decodoarele VP8 si Vorbis)
  pentru redarea videoclipurilor
> **De ce nu se pot reda videourile**

  1. Redarea videourilor nu este disponibila pe Android
  2. In cele mai multe cazuri, nu ai fisierele statice FFmpeg pentru redarea videourilor.
  In zipurile Full sunt incluse fisiere statice FFmpeg cu decodoarele VP8 si Vorbis disponibile

# Cum se deschide proiectul
- Descarca codul sursa
- Descarca Godot 4.3 **Mono** (Godot Engine - .NET) ([Link catre site](https://godotengine.org/download))
- Descarca SDKul .NET (daca esti pe Linux, trebuie instalat si Mono SDK)
- Deschide proiectul in Godot

**In cazul in care vrei sa generezi executabil**
- In cazul in care nu ai Export Templates, Apasa pe Editor > Manage Export Templates... si dupa aceea pe Download & Install
- Apasa pe Project > Export...
- Adauga un Preset pentru platforma dorita (se poate configura acel preset)
- Export Project...

Pentru Android necesita mult prea multi pasi pentru acest README, asa ca o sa las link catre documentatia oficiala Godot [aici](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html)
