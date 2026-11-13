# Easy Lyric

Windows x64 desktop application for silent monitoring of internet radio streams, with synchronized lyrics, guitar chords, and sheet music for DJs/CJs.

## Initial development target
- Windows 10 x64 as the primary development and test environment.
- Windows 11 compatibility.
- Windows 7 SP1 x64 compatibility will be tested; it is not guaranteed until verified.
- Portable ZIP distribution (no installer).
- The application must never play the monitored stream through the PC speakers. Audio is received internally for analysis only.
- The only main input is the internet radio stream URL. Previously used URLs are selected from the URL input's own history dropdown; do not add a separate history panel.

## Song identification and unavailable metadata
- First use broadcast metadata (for example, stream title information) to identify the current song.
- If metadata is missing or cannot identify the song, attempt audio-based song identification where technically and legally available.
- AI-generated songs and other unidentified tracks may not have searchable public song information. Do not leave the UI blank or crash in this case.
- Show a clear, dismissible message dialog, for example:
  > 곡 정보를 확인할 수 없습니다. AI 생성곡이거나 방송에서 곡 정보가 제공되지 않는 곡일 수 있습니다. 다음 곡 정보가 확인되면 자동으로 다시 검색합니다.
- Do not repeatedly show the same dialog for the same track. Resume searching when new track metadata or a new track is detected.
- Keep the main input limited to the broadcast URL; the notification must not introduce a separate song-title input field.

## Current milestone
This repository starts with the desktop UI and automated Windows build pipeline. Radio stream reception, song identification, lyric retrieval, score/chord sources, and timing synchronization will be implemented and tested in later milestones.

## Build
Open `EasyLyric.sln` in Visual Studio with the .NET Framework 4.8 targeting pack, or run the GitHub Actions workflow. The workflow packages the build output as a ZIP artifact.
