# Build per iOS

Guida per compilare Nofun e installarlo su iPhone/iPad da un Mac.

## Requisiti

- Mac con **Xcode 16** (o più recente) e un account Apple Developer collegato in Xcode (Settings → Accounts).
- **Unity Hub** con **Unity 6 LTS (6000.0.x)** e il modulo **iOS Build Support**.
  Il progetto era stato creato con un'alpha (2023.3.0a18) che non si scarica più: Unity 6 è la stessa linea, già rilasciata.
- iPhone con iOS 13 o successivo, con la *Modalità sviluppatore* attiva (Impostazioni → Privacy e sicurezza).

## 1. Clona la repo con i submodule

```sh
git clone --recursive https://github.com/palla89/nofun-ios.git
cd nofun-ios
git checkout claude/practical-ride-gpustl
git submodule update --init --recursive
```

## 2. Apri il progetto con Unity 6

1. Unity Hub → **Add** → **Add project from disk** e scegli la cartella.
2. Hub ti segnala che la versione 2023.3.0a18 manca: scegli Unity 6000.0.x e conferma l'aggiornamento del progetto.
3. Al primo import Unity scarica i pacchetti (anche da GitHub: NuGetForUnity, VContainer, NaughtyAttributes, NativeFilePicker).
   Se qualche pacchetto in pre-release (`com.unity.inputsystem`, `com.unity.localization`) dà errori, aggiornalo dal Package Manager alla versione "Recommended" per Unity 6.

## 3. Imposta iOS

1. **File → Build Profiles** → seleziona **iOS** → **Switch Platform**.
2. **Player Settings → Other Settings**:
   - **Bundle Identifier**: è impostato `com.palla89.nofun`; cambialo se vuoi un ID tuo.
   - **Signing Team ID**: inserisci il tuo Team ID, oppure lascialo vuoto e sceglilo poi in Xcode.
   - Il resto è già configurato nella repo: iOS minimo 13.0, IL2CPP in Release, firma automatica, simbolo `NOFUN_PRODUCTION`.

## 4. Builda e installa

1. In Build Profiles premi **Build** e scegli una cartella di output (per esempio `Builds/iOS`).
2. Apri `Builds/iOS/Unity-iPhone.xcodeproj` in Xcode.
3. Target **Unity-iPhone** → **Signing & Capabilities** → scegli il tuo **Team**.
4. **Product → Scheme → Edit Scheme… → Run → Build Configuration: Release**.
   È importante: l'emulatore su iOS gira solo con l'interprete, e in Debug è molto più lento.
5. Collega l'iPhone, selezionalo come destinazione e premi **Run**.

Lo script `Assets/Editor/IOSPostProcessBuild.cs` collega automaticamente `libsqlite3` e abilita la condivisione file nell'Info.plist.

## Usare l'emulatore

- **Installare un gioco**: nella lista giochi premi il pulsante di installazione e scegli il file `.mpn` dall'app File o da iCloud Drive.
  Il gioco viene copiato dentro l'app.
- **Giochi Sony Ericsson**: al primo avvio si apre la configurazione.
  - Scegli il modello di telefono giusto (T300/T310 per lo schermo 101×80, T610 per il 128×160), altrimenti alcuni giochi danno "Terminal not found".
  - Scegli **System version 1.30**.
- **Ingranaggio** durante il gioco: riapre la configurazione o esce dal gioco.
- **Controlli**: touch a schermo; i controller MFi/Xbox/PlayStation collegati via Bluetooth funzionano tramite l'Input System.
- **Dati dell'app**: nell'app File, in *Sul mio iPhone → nofun*, trovi giochi installati e salvataggi.

## Limiti noti su iOS

- **Niente ricompilatore LLVM**: iOS non permette la compilazione al volo del codice (JIT), quindi si usa solo l'interprete.
  Va bene per i giochi 2D dell'epoca, mentre i titoli 3D possono risultare lenti.
- **Istruzioni mancanti nell'interprete**: alcune istruzioni PIP2 rare (`SYSCALL0-4`, load/store con offset immediato `STBi…LDHUi`) non sono implementate.
  Se un gioco le usa, si interrompe e il log mostra `Unimplemented opcode`.
- **MIDI nell'editor su Mac**: nell'editor Unity su Mac la musica MIDI non funziona, perché non c'è una libreria nativa per macOS.
  Sul dispositivo iOS funziona: il sintetizzatore viene compilato dai sorgenti in `Assets/Plugins/TSFMidiRenderer/iOS`.
  Per averla anche nell'editor puoi compilarla tu:

  ```sh
  cmake -S Native/TSFMidiRender -B Native/TSFMidiRender/build -DCMAKE_OSX_ARCHITECTURES="arm64;x86_64"
  cmake --build Native/TSFMidiRender/build --config Release
  ```

  Poi copia `libTSFMidiRenderer.dylib` in `Assets/Plugins/TSFMidiRenderer/macOS/` e, nell'Inspector, abilitalo per Editor e macOS.
