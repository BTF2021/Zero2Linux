![Invata Linux de la zero](https://github.com/BTF2021/Zero2Linux/blob/unstable/githubassets/Banner.gif)

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

  In Lite este doar executabilul si folderul pentru .NET. In Full sunt toate fisierele din Lite plus fisiere statice FFmpeg (cu codecurile VP8, VP9 si Vorbis disponibile)
  pentru redarea videoclipurilor cum ar fi fundalul in meniul principal si videourile propriu zise
> **De ce nu se pot reda videourile**

  1. Redarea videourilor nu este disponibila pe Android
  2. In 90% din cazuri, nu ai fisierele statice FFmpeg pentru redarea videourilor.
  In zipurile Full sunt incluse fisiere statice FFmpeg cu codecurile VP8, VP9 si Vorbis disponibile

# Cum se compileaza
- Descarca codul sursa
- Descarca Godot 4.2.2 **Mono** (Godot Engine - .NET) ([Link catre site](https://godotengine.org/download))
- Descarca SDKul .NET (daca esti pe Linux, trebuie instalat si Mono SDK)
- Deschide proiectul in Godot
- Project > Export...
